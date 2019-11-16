
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