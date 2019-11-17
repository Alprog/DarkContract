
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using DarkCrystal.Serialization;

namespace DarkCrystal.FieldSystem
{
    [DarkContract(TypeIndex.Entity)]
    public partial class Entity : GuidObject
    {
        protected override bool Reg(bool state) => base.Reg(state) && Registrator.SetObject(this, state);

        [Key(0)] public override string ID { get; set; }

        public FieldStorage Fields;

        [Key(1)]
        private FieldStorage FieldsSerialization
        {
            get => this.Fields;
            set => this.Fields.Merge(value);
        }

        public Entity() : base(Guid.NewGuid())
        {
            this.ID = "NewEntity";
            Fields.SetEntity(this);
        }

        public Entity(Guid guid) : base(guid)
        {
            Fields.SetEntity(this);
        }
        
        private void OnDeserialized()
        {
            Fields.SetEntity(this);
        }
    }
}