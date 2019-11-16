
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Reflection;
using DarkCrystal.Encased;
using DarkCrystal.Encased.Core;

namespace DarkCrystal.Serialization
{
    public partial class SerializationKeyDefragmentator
    {
        static class KeyDefragmentationData
        {
            struct KeyData
            {
                public MemberInfo RuntimeHolder;
                public MemberInfo StaticHolder;
                public bool Filled => RuntimeHolder != null && StaticHolder != null;
                public bool SameHolder => RuntimeHolder == StaticHolder;
            }


            static Dictionary<Type, Dictionary<MemberInfo, int>> m_map = new Dictionary<Type, Dictionary<MemberInfo, int>>();
            static KeyDefragmentationData()
            {
                SortedList<int, KeyData> keyList = new SortedList<int, KeyData>();
                foreach (var type in Utils.GetAssemblyTypes())
                {
                    keyList.Clear();
                    foreach (var member in ScriptKeyHelper.IterateClass(type))
                    {
                        var attributes = member.GetCustomAttributes<KeyAttribute>().ToList();
                        if (attributes.IsNullOrEmpty() || attributes[0] is SaveKeyAttribute || attributes[0] is LoadKeyAttribute)
                        {
                            continue;
                        }

                        if (attributes.Count > 1)
                        {
                            The.Logger.Error("Don't know how to process this class!");
                            return;
                        }

                        keyList.TryGetValue(attributes[0].Value, out var data);

                        bool findFlag = false;
                        if (attributes[0].Flags.Contains(SerializationFlags.Static))
                        {
                            data.StaticHolder = member;
                            findFlag = true;
                        }

                        if (attributes[0].Flags.Contains(SerializationFlags.Runtime))
                        {
                            data.RuntimeHolder = member;
                            findFlag = true;
                        }

                        if (!findFlag)
                        {
                            data.RuntimeHolder = data.StaticHolder = member;
                        }

                        keyList[attributes[0].Value] = data;
                    }

                    if (keyList.Count > 0)
                    {
                        List<KeyData> filledAndSame = new List<KeyData>();
                        List<KeyData> filledNotSame = new List<KeyData>();
                        List<KeyData> staticOnly = new List<KeyData>();
                        List<KeyData> runtimeOnly = new List<KeyData>();

                        foreach (var pair in keyList)
                        {
                            if (pair.Value.Filled)
                            {
                                if (pair.Value.SameHolder)
                                {
                                    filledAndSame.Add(pair.Value);
                                }
                                else
                                {
                                    filledNotSame.Add(pair.Value);
                                }
                            }
                            else
                            {
                                if (pair.Value.RuntimeHolder != null)
                                {
                                    runtimeOnly.Add(pair.Value);
                                }
                                else if (pair.Value.StaticHolder != null)
                                {
                                    staticOnly.Add(pair.Value);
                                }
                            }
                        }

                        List<KeyData> resultList = new List<KeyData>();
                        resultList.AddRange(filledAndSame);
                        while (staticOnly.Count > 0 && runtimeOnly.Count > 0)
                        {
                            filledNotSame.Add(new KeyData
                            {
                                RuntimeHolder = runtimeOnly[0].RuntimeHolder,
                                StaticHolder = staticOnly[0].StaticHolder
                            });

                            runtimeOnly.RemoveAt(0);
                            staticOnly.RemoveAt(0);
                        }

                        resultList.AddRange(filledNotSame);
                        resultList.AddRange(runtimeOnly);
                        resultList.AddRange(staticOnly);

                        var map = new Dictionary<MemberInfo, int>();
                        int index = 0;
                        foreach (var data in resultList)
                        {
                            if (data.RuntimeHolder != null)
                            {
                                map[data.RuntimeHolder] = index;
                            }

                            if (data.StaticHolder != null)
                            {
                                map[data.StaticHolder] = index;
                            }

                            index++;
                        }

                        m_map[type] = map;
                    }
                }
            }

            public static bool TryGetKey(MemberInfo member, out int key)
            {
                key = -1;
                m_map.TryGetValue(member.ReflectedType, out var keysMap);
                return keysMap?.TryGetValue(member, out key) ?? false;
            }
        }
    }
}
