

using DarkCrystal.Serialization;
using System;
using UnityEngine;

namespace DarkCrystal.Example
{
    [DarkContract(TypeIndex.BaseNode)]
    public class BaseNode : GuidObject
    {
        protected override bool Reg(bool state) => base.Reg(state) && Registrator.SetObject(this, state);

        [Key(0)] public override string ID { get; set; }
        [Key(1)] public Vector3 Position;
        [Key(2)] public BaseNode Link;

        public BaseNode(string id, Vector3 position): 
            base(Guid.NewGuid())
        {
            this.ID = id;
            this.Position = position;
        }

        public BaseNode(Guid guid) : base(guid) {}
    }

    [DarkContract(TypeIndex.DerivedNode)]
    public class DerivedNode : BaseNode
    {
        protected override bool Reg(bool state) => base.Reg(state) && Registrator.SetObject(this, state);

        [Key(0)] public string DerivedNotes;

        public DerivedNode(string id, Vector3 position, string derivedNotes):
            base(Guid.NewGuid())
        {
            this.ID = id;
            this.Position = position;
            this.DerivedNotes = derivedNotes;
        }

        public DerivedNode(Guid guid) : base(guid) {}
    }
}
