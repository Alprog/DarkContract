
using System;
using System.Collections.Generic;
using System.Reflection;

namespace DarkCrystal
{
    public static class Enum<T>
    {
        public static T[] Values { get; } = (T[])Enum.GetValues(typeof(T));
        public static string[] Names => Array.ConvertAll(Values, v => v.ToString());
        public static int Count => Values.Length;

        public static T FromInt(int value, T defaultValue)
        {
            foreach (T enumValue in Values)
            {
                if (value == Convert.ToInt32(enumValue))
                {
                    return enumValue;
                }
            }
            return defaultValue;
        }

        public static TEnum FromString<TEnum>(string value, TEnum defaultValue) where TEnum : struct, Enum, T
        {
            if (Enum.TryParse(value, out TEnum res))
            {
                return res;
            }
            return defaultValue;
        }

        public static IEnumerable<Tuple<T, MemberInfo>> IterateMembers()
        {
            var type = typeof(T);
            foreach (var val in Values)
            {
                yield return new Tuple<T, MemberInfo>(val, type.GetMember(val.ToString())[0]);
            }
        }
    }
}
