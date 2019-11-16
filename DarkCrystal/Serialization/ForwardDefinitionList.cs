
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;

namespace DarkCrystal.Serialization
{
    public class ForwardDefinitionList : HashSet<GuidObject>
    {
        public ForwardDefinitionList()
        {
        }

        public ForwardDefinitionList(IEnumerable<GuidObject> items) : base(items)
        {
        }
    }
}