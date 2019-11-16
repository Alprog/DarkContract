
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace DarkCrystal.Serialization
{
    public partial class DarkMeta
    {
        public class MemberFormatterGroup : List<MemberFormatter>
        {
            public MemberFormatter GetFormatter(SerializationFlags flags)
            {
                foreach (var formatter in this)
                {
                    if (flags.HasFlag(formatter.Flags))
                    {
                        return formatter;
                    }
                }
                return null;
            }
        }
    }
}