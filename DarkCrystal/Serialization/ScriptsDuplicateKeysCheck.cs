
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DarkCrystal.Serialization
{
    /*public class ScriptsDuplicateKeysCheck
    {
        const BindingFlags SearchFlags = BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        enum KeysLoaded
        {
            None = 0,
            Static = 1 << 0,
            Runtime = 1 << 1,
            Save = 1 << 2,
            Load = 1 << 3
        }

#if UNITY_EDITOR

        [UnityEditor.MenuItem("Tools/Check Scripts Keys Duplicates")]
        public static void EditorCheck()
        {
            var types = Utils.GetAssemblyTypes();

            float counter = 0;
            foreach (var type in Routine(types))
            {
                UnityEditor.EditorUtility.DisplayProgressBar("Check Scripts Keys Duplicates", string.Format("{0}/{1}: {2}", counter, types.Length, type.Name), counter / types.Length);
                counter++;
            }

            UnityEditor.EditorUtility.ClearProgressBar();
        }

#endif

        public static void SilentCheck()
        {
            foreach (var type in Routine(Utils.GetAssemblyTypes()))
            {
            }
        }

        static private IEnumerable<Type> Routine(IEnumerable<Type> types)
        {
            var scriptKeyHelper = new Core.ScriptKeyHelper();
            foreach (var type in types)
            {
                try
                {
                    var attribute = type.GetCustomAttribute<DarkContractAttribute>(false);
                
                    if (attribute != null)
                    {
                        Dictionary<int, KeysLoaded> keys = new Dictionary<int, KeysLoaded>();
                        foreach (var field in type.GetFields(SearchFlags))
                        {
                            ProcessMember(field, scriptKeyHelper, keys, type);
                        }

                        foreach (var property in type.GetProperties(SearchFlags))
                        {
                            ProcessMember(property, scriptKeyHelper, keys, type);
                        }

                        if (type.IsSubclassOf(typeof(Script)))
                        {
                            var onlyOnceEntries = new HashSet<int>();
                            foreach (var method in type.GetMethods(SearchFlags))
                            {
                                var index = method.GetCustomAttribute<OnlyOnceAttribute>()?.Index;
                                if (index.HasValue)
                                {
                                    if (onlyOnceEntries.Contains(index.Value))
                                    {
                                        var str = string.Format("Type {0} has OnlyOnce intersection for index {1}", type.Name, index);
                                        UnityEngine.Debug.LogError(str);
                                    }
                                    else
                                    {
                                        onlyOnceEntries.Add(index.Value);
                                    }
                                }
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError("Error when processing type " + type.Name);
                    UnityEngine.Debug.LogException(ex);
                }

                yield return type;
            }
        }
        
        private static void ProcessMember(MemberInfo memberInfo, ScriptKeyHelper scriptKeyHelper, Dictionary<int, KeysLoaded> keys, Type type)
        {
            var keyAttributes = memberInfo.GetCustomAttributes<KeyAttribute>();
            foreach (var keyAttribute in keyAttributes)
            {
                var value = keyAttribute.Value;
                if (value == -1)
                {
                    if (!scriptKeyHelper.TryGetNewValue(memberInfo, out value))
                    {
                        var str = string.Format("Type {0} has unsetted script key!", type.Name);
                        UnityEngine.Debug.LogError(str);
                    }
                }

                if (!keys.TryGetValue(value, out var filledFlags))
                {
                    filledFlags = KeysLoaded.None;
                }

                var attributeKeys = GetAttributeKeys(keyAttribute);
                if ((attributeKeys & filledFlags) != KeysLoaded.None)
                { 
                    var str = string.Format("Type {0} has keys intersection for index {1}", type.Name, value);
                    UnityEngine.Debug.LogError(str);
                }

                filledFlags |= attributeKeys;
                keys[value] = filledFlags;
            }
        }

        private static KeysLoaded GetAttributeKeys(KeyAttribute keyAttribute)
        {
            var flags = keyAttribute.Flags;
            var value = KeysLoaded.None;

            if (flags == SerializationFlags.Default)
            {
                return KeysLoaded.Load | KeysLoaded.Runtime | KeysLoaded.Save | KeysLoaded.Static;
            }

            if (flags.Contains(SerializationFlags.Runtime))
            {
                value |= KeysLoaded.Runtime;
            }

            if (flags.Contains(SerializationFlags.Static))
            {
                value |= KeysLoaded.Static;
            }

            if (flags.Contains(SerializationFlags.Save))
            {
                value |= KeysLoaded.Save;
            }

            if (flags.Contains(SerializationFlags.Load))
            {
                value |= KeysLoaded.Load;
            }

            return value;
        }
    }*/
}