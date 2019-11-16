
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

namespace DarkCrystal
{
    public static class GuidStorage
    {
        public static GuidObject Get(Guid guid) => GuidStorage<GuidObject>.Get(guid);

        public static T Get<T>(Guid guid) => GuidStorage<T>.Get(guid);
        public static T Get<T>(string guidString) => GuidStorage<T>.Get(guidString);
    }

    public static class GuidStorage<T>
    {
        private static Dictionary<Guid, T> Map = new Dictionary<Guid, T>();
        
        public static IEnumerable<T> Items => Map.Values;
        public static int Count => Map.Count;

        public static T Get(Guid guid)
        {
            T result = default(T);
            Map.TryGetValue(guid, out result);
            return result;
        }

        public static T Get(string guidString)
        {
            if (string.IsNullOrEmpty(guidString))
            {
                return default(T);
            }
            else
            {
                return Get(new Guid(guidString));
            }
        }

        public class Internal
        {
            public static void Register(Guid guid, T data) => Map.Add(guid, data);
            public static void Unregister(Guid guid) => Map.Remove(guid);
        }
    }
}