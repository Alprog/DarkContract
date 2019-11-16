
using System;
using DarkCrystal.Serialization;

namespace DarkCrystal.FieldSystem
{
    [StaticDarkContract] [Serializable]
    public struct FieldKey : IComparable<FieldKey>
    {
        [Key(0)] public readonly TypeIndex TypeId;
        [Key(1)] public readonly int MemberId;

        public FieldKey(TypeIndex typeId, int memberId)
        {
            this.TypeId = typeId;
            this.MemberId = memberId;
        }

        public int CompareTo(FieldKey other)
        {
            if (other.TypeId != this.TypeId)
            {
                return this.TypeId.CompareTo(other.TypeId);
            }
            else
            {
                return this.MemberId.CompareTo(other.MemberId);
            }
        }

        public override string ToString()
        {
            return String.Format("FieldKey[{0} ({1}), {2} ({3})]", TypeId, TypeId.ToInt(), GetName(), MemberId);
        }

        public string GetFullName()
        {
            return TypeId.ToString() + "." + GetName();
        }

        public string GetName()
        {
            if (TypeRegistry.TryGetType(TypeId, out Type type))
            {
                return Enum.GetName(type, MemberId);
            }
            else
            {
                return MemberId.ToString();
            }
        }

        public Enum GetEnum()
        {
            var type = TypeRegistry.GetType(TypeId);
            return (Enum)Enum.ToObject(type, MemberId);
        }
    }
}