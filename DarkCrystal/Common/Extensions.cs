
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Reflection;
using DarkCrystal.Serialization;
using DarkCrystal.FieldSystem;

namespace DarkCrystal
{
    public static class Extensions
    {
        public static object InvokeDefaultConstructor(this Type type)
        {
            return type.GetDefaultConstructor().Invoke(EmptyArray<object>.Value);
        }

        public static ConstructorInfo GetDefaultConstructor(this Type type)
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            return type.GetTypeInfo().GetConstructor(flags, null, EmptyArray<Type>.Value, null);
        }

        public static FieldKey ToFieldKey(this Enum Value)
        {
            return new FieldKey(Value.GetType().GetCustomAttribute<DarkContractAttribute>().TypeIndex, Value.ToInt());
        }

        public static FieldKey ToFieldKey<T>(this T @value) where T : Enum
        {
            return new FieldKey(TypeIndex<T>.Value, @value.ToInt());
        }

        public static int ToInt(this Enum @value)
        {
            return Convert.ToInt32(@value);
        }

        public static bool IsImplementInterface<T>(this Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }

        public static bool IsImplementInterface(this Type type, Type interfaceType)
        {
            return interfaceType.IsAssignableFrom(type);
        }
    }
}