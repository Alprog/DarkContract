
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR

// uncomment to enable "Check Fragmentation" Tools option
//#define CHECK_FRAGMENTATION

using DarkCrystal.Encased;
using DarkCrystal.Encased.Core;
using DarkCrystal.Encased.Core.Tools;
using DarkCrystal.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace DarkCrystal.Serialization
{ 
    public partial class SerializationKeyDefragmentator
    {
        enum MatchResult
        {
            Invalid,
            Namespace,
            Type,
            Member
        }

        struct MemberData
        {
            public string Replacement;
            public string MemberName;
            public Group Attributes;
        }
       

        /*
        *  Regex usage:
        *  - Group 1: namespace
        *  - Group 2: class type name
        *  - Group 3: isGeneric
        *  - Group 4: Something inside attribute
        *  - Group 5: Member name
        * */
        private const string NamespaceSearcher = @"(?:namespace ([\w.]+))";
        private const string ClassTypeSearcher = @"(?:class|struct)\s(\w+)(?:<([\w\d, ]+)>)?";
        private const string MemberSearcher = @"((?:\[.+\]\s*)+)(?=(?:[\t ,.<>]+(\w+))+\s*[=;{])";

        private const string AttributeSearcher = @"(\w*Key)\((\d+)(.*?)\)";
        private const string AttributeReplacer = @"SaveKey({0}$3), LoadKey($2$3) /*$1($2$3)*/";
        private const string RuntimeKeyReplacer = @"$1({0}$3)";

        private const string ReverceAttributeSearcher = @"SaveKey\((\d+).*?\), LoadKey\(\d+.*?\) \/\*(\w*Key)\(\d+(.*?)\)\*\/";
        private const string ReverceAttributeReplacer = @"$2($1$3)";

        static readonly Regex Searcher = new Regex(string.Format("{0}|{1}|{2}", NamespaceSearcher, ClassTypeSearcher, MemberSearcher));
        static readonly Regex Replacer = new Regex(AttributeSearcher);
        static readonly Regex ReverceReplacer = new Regex(ReverceAttributeSearcher);

        private delegate void ProcessFilestringDelegate(ref string str);

#if CHECK_FRAGMENTATION

        [MenuItem("Tools/Check Fragmentation")]
        public static void Check()
        {
            foreach (var type in Utils.GetAssemblyTypes())
            {
                var contractAttributes = type.GetCustomAttributes<DarkContractAttribute>(false).ToList();
                if (contractAttributes.IsNullOrEmpty())
                {
                    continue;
                }
                else if (contractAttributes.Count > 1)
                {
                    Debug.LogFormat("Oo: {0}", type);
                }
                else
                {
                    SortedSet<int> runtimeSet = new SortedSet<int>();
                    SortedSet<int> staticSet = new SortedSet<int>();
                    foreach (var member in ScriptKeyHelper.IterateClass(type))
                    {
                        var keyAttribute = member.GetCustomAttribute<KeyAttribute>();
                        if (keyAttribute == null)
                        {
                            continue;
                        }

                        if (keyAttribute.Flags.Contains(SerializationFlags.Static))
                        {
                            staticSet.Add(keyAttribute.Value);
                        }
                        else if (keyAttribute.Flags.Contains(SerializationFlags.Runtime))
                        {
                            runtimeSet.Add(keyAttribute.Value);
                        }
                        else
                        {
                            staticSet.Add(keyAttribute.Value);
                            runtimeSet.Add(keyAttribute.Value);
                        }
                    }

                    int lastKey = -1;
                    foreach (var key in runtimeSet)
                    {
                        if (key != ++lastKey)
                        {
                            The.Logger.Error("Key Fragmetation in type {0}", type.Name);
                        }
                    }

                    lastKey = -1;
                    foreach (var key in staticSet)
                    {
                        if (key != ++lastKey)
                        {
                            The.Logger.Error("Key Fragmetation in type {0}", type.Name);
                        }
                    }
                }
            }
        }

#endif

        [MenuItem("Tools/Serialization Defragmentation")]
        public static void Defragment()
        {
            var processor = new SerializationKeyDefragmentator();
            var process = new ProgressProcess();
            foreach (var file in Directory.GetFiles(Utils.PathCombine(Application.dataPath, "Sources/"), "*.cs", SearchOption.AllDirectories))
            {
                process.AddStep(() => processor.ProcessFile(file, true));
            }

            // workaround with assembly reload. If refresh triggers syncronius update - it will be reverce step, but other way it won't
            process.AddStep(() => AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport));
            process.AddStep(() =>
            {
                if (EditorApplication.isCompiling)
                {
                    EditorPrefs.SetBool("DefragmentatorWorks", true);
                }
            });

            process.Run();
        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void ScheduleSecondStep()
        {
            if (EditorPrefs.GetBool("DefragmentatorWorks", false))
            {
                EditorPrefs.SetBool("DefragmentatorWorks", false);
                ReloadDatabaseStep();
                ReverceOperationStep();
            }
        }
        
        private static void ReloadDatabaseStep()
        {
            The.GameState.LoadTempEditorState();
            The.GameState.SaveDatabase();
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            AssemblyReloadEvents.afterAssemblyReload -= ReloadDatabaseStep;
        }
        
        private static void ReverceOperationStep()
        {
            AssemblyReloadEvents.afterAssemblyReload -= ReverceOperationStep;
            var processor = new SerializationKeyDefragmentator();
            var process = new ProgressProcess();

            foreach (var file in Directory.GetFiles(Utils.PathCombine(Application.dataPath, "Sources/"), "*.cs", SearchOption.AllDirectories))
            {
                process.AddStep(() => processor.ProcessFile(file, false));
            }

            process.Run();
        }

        private MatchResult GetMatchResult(Match match, Stack<TypeData> typeResult, ref MemberData memberResult)
        {
            if (match.Groups[1].Success) // namespace
            {
                typeResult.Clear();
                typeResult.Push(new TypeData() { Namespace = match.Groups[1].Value });
                return MatchResult.Namespace;
            }

            if (match.Groups[2].Success) // typeName
            {
                TypeData current;
                if (typeResult.Count == 0)
                {
                    current = default;
                }
                else
                {
                    current = typeResult.Pop();
                }

                var genericsCount = match.Groups[3].Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
                var typeName = match.Groups[2].Value + (match.Groups[3].Success ? "`" + genericsCount : "");

                if (string.IsNullOrEmpty(current.TypeName))
                {
                    current.TypeName = typeName;
                    typeResult.Push(current);
                }
                else
                {
                    typeResult.Push(current);
                    current = new TypeData()
                    {
                        TypeName = typeName,
                        UpperLevel = current.ToType()
                    };

                    if (current.ToType() == null)
                    {
                        var copy = new Stack<TypeData>(typeResult);
                        while (typeResult.Count > 0)
                        {
                            current = typeResult.Peek();
                            current = new TypeData()
                            {
                                TypeName = typeName,
                                UpperLevel = current.ToType()
                            };

                            if (current.ToType() == null)
                            {
                                typeResult.Pop();
                                continue;
                            }

                            typeResult.Push(current);
                            break;
                        }
                         
                        if (typeResult.Count == 0)
                        {
                            current = new TypeData()
                            {
                                TypeName = typeName,
                                Namespace = current.Namespace ?? current.UpperLevel?.Namespace
                            };

                            if (current.ToType() == null)
                            {
                                while (copy.Count > 0)
                                {
                                    typeResult.Push(copy.Pop());
                                }
                            }
                            else
                            {
                                typeResult.Push(current);
                            }
                        }
                    }
                    else
                    {
                        typeResult.Push(current);
                    }
                }

                return MatchResult.Type;
            }

            if (!(match.Groups[4].Success && match.Groups[5].Success)) // member name
            {
                The.Logger.Warning("Invalid Match Result: {0}", match.Value);
                return MatchResult.Invalid;
            }

            memberResult.Attributes = match.Groups[4];
            memberResult.MemberName = match.Groups[5].Value;
            return MatchResult.Member;
        }

        private void ProcessFile(string filePath, bool forward)
        {
            var file = File.OpenText(filePath);
            Func<MemberData, ProcessFilestringDelegate> actionGetter = GetFunc(forward);
            string defaultReplacement = forward ? AttributeReplacer : ReverceAttributeReplacer;

            try
            {
                if (filePath.Contains("DialogGraph.cs"))
                {
                }
                Stack<ProcessFilestringDelegate> delegates = new Stack<ProcessFilestringDelegate>();
                var typeDatas = new Stack<TypeData>();

                var fileContent = file.ReadToEnd();
                var collection = Searcher.Matches(fileContent);
                foreach (Match match in collection)
                {
                    var memberData = new MemberData();
                    var result = GetMatchResult(match, typeDatas, ref memberData);
                    if (result == MatchResult.Member)
                    {
                        if (typeDatas.Count == 0)
                        {
                            continue;
                        }
                        var typeData = typeDatas.Peek();
                        if (string.IsNullOrEmpty(typeData.TypeName))
                        {
                            continue;
                        }

                        var members = typeData.ToType(true)?.GetMember(memberData.MemberName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                        if (members.IsNullOrEmpty())
                        {
                            continue;
                        }

                        var attributes = members[0].GetCustomAttributes<KeyAttribute>().ToList();
                        memberData.Replacement = defaultReplacement;

                        if (attributes.IsNullOrEmpty())
                        {
                            continue;
                        }

                        switch (attributes[0])
                        {
                            case ScriptKeyAttribute sKey:
                                continue;

                            case RuntimeKeyAttribute rKey:
                                if (forward) memberData.Replacement = RuntimeKeyReplacer;
                                else continue;

                                break;

                            case SaveKeyAttribute sKey:
                            case LoadKeyAttribute lKey:
                                if (forward) continue;
                                break;

                            default:
                                if (!forward) continue;
                                break;
                        }

                        if (forward)
                        {
                            if (!KeyDefragmentationData.TryGetKey(members[0], out var newKey))
                            {
                                continue;
                            }

                            if (newKey == attributes[0].Value)
                            {
                                continue;
                            }
                            memberData.Replacement = string.Format(memberData.Replacement, newKey);
                        }

                        delegates.Push(actionGetter(memberData));
                    }
                }

                file.Close();
                if (delegates.Count > 0)
                {
                    Debug.LogFormat("---------------------- {0} ---------------------", Path.GetFileName(filePath));
                    while (delegates.Count > 0)
                    {
                        delegates.Pop()(ref fileContent);
                    }

                    File.WriteAllText(filePath, fileContent);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                file.Close();
            }
        }

        private Func<MemberData, ProcessFilestringDelegate> GetFunc(bool forward)
        {
            var replacer = forward ? Replacer : ReverceReplacer;
            return (memberData) => GetAction(memberData, replacer);
        }

        private ProcessFilestringDelegate GetAction(MemberData memberData, Regex replacer)
        {
            return (ref string str) =>
            {
                str = replacer.Replace(str, (match) =>
                {
                    if (match.Index < memberData.Attributes.Index + memberData.Attributes.Length)
                    {
                        var res = match.Result(memberData.Replacement);

                        Debug.LogFormat("Process Match {0} (start index {1}, length {2}). Found replace at index {3}({4} -> {5})",
                            memberData.Attributes.Value, memberData.Attributes.Index, memberData.Attributes.Length,
                            match.Index, match.Value, res);

                        return res;
                    }
                    else
                    {
                        Debug.LogErrorFormat("Process Match {0} (start index {1}, length {2}). Found replace outside bounds, at index {3}({4})",
                            memberData.Attributes.Value, memberData.Attributes.Index, memberData.Attributes.Length,
                            match.Index, match.Value);
                        return match.Value;
                    }
                }, 1, memberData.Attributes.Index);
            };
        }
    }
}

#endif