
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;

namespace DarkCrystal.Serialization
{
    public enum DarkFlags
    {
        None = 0,

        Static = 1 << 0,
        Abstract = 1 << 1,
        Serializable = 1 << 2,
        Sealed = 1 << 3,
        Inline = 1 << 4,
        SkipReading = 1 << 5,
        SkipWriting = 1 << 6,
        IgnoreBaseModule = 1 << 7
    }

    public static class DarkFlagsExtensions
    {
        public static bool IsStatic(this DarkFlags flags) => flags.HasFlag(DarkFlags.Static);
        public static bool IsAbstract(this DarkFlags flags) => flags.HasFlag(DarkFlags.Abstract);
        public static bool IsSerializable(this DarkFlags flags) => flags.HasFlag(DarkFlags.Serializable);
        public static bool IsIgnoreBaseModule(this DarkFlags flags) => flags.HasFlag(DarkFlags.IgnoreBaseModule);
        public static bool IsSealed(this DarkFlags flags) => flags.HasFlag(DarkFlags.Sealed);
        public static bool IsInline(this DarkFlags flags) => flags.HasFlag(DarkFlags.Inline);
        public static bool IsSkipReading(this DarkFlags flags) => flags.HasFlag(DarkFlags.SkipReading);
        public static bool IsSkipWriting(this DarkFlags flags) => flags.HasFlag(DarkFlags.SkipWriting);
    }
}