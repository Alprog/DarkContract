
using System;
using System.Collections.Generic;
using System.Reflection;
using MessagePack;
using Module = DarkCrystal.Encased.Core.ModuleSystem.Module;

namespace DarkCrystal.Serialization
{
    public partial class DarkMeta
    {
        private static Dictionary<Type, DarkMeta> MetasByType;
        private static Dictionary<TypeIndex, DarkMeta> MetasByIndex;

        public Type Type { get; private set; }

        public DarkMeta BaseMeta { get; private set; }
        public int DeepLevel { get; private set; }
        public ConstructorInfo ConstructorInfo { get; private set; }
        private MemberFormatterGroup[] MemberFormatterGroups;
        private MethodInfo OnDeserializedHandler;
        public TypeIndex TypeIndex { get; private set; }
        public DefaultFieldInfo DefaultFieldInfo { get; private set; }
        public DarkFlags Flags { get; private set; }

        static DarkMeta()
        {
            MetasByType = new Dictionary<Type, DarkMeta>();
            MetasByIndex = new Dictionary<TypeIndex, DarkMeta>();
            ScriptsDuplicateKeysCheck.SilentCheck();
        }

        private DarkMeta(Type type)
        {
            this.Type = type;
            InitFlags();
            InitBaseMeta();
            InitConstructor();
            InitMemberFormatters();
            InitCallbacks();
            InitTypeIndex();
        }

        public object CreateInstance()
        {
            if (ConstructorInfo == null)
            {
                Logger.Logger.Instance.Error(String.Format("No constructor at {0} darkmeta", Type.Name));
            }
            return ConstructorInfo.Invoke(EmptyArray<object>.Value);
        }

        public object CreateInstance(object arg)
        {
            if (ConstructorInfo == null)
            {
                Logger.Logger.Instance.Error(String.Format("No constructor at {0} darkmeta", Type.Name));
            }
            return ConstructorInfo.Invoke(new object[] { arg });
        }

        public void OnDeserialized(object instance)
        {
            if (OnDeserializedHandler != null)
            {
                OnDeserializedHandler.Invoke(instance, EmptyArray<object>.Value);
            }
        }
    
        public static DarkMeta Get(Type type)
        {
            DarkMeta meta;
            if (!MetasByType.TryGetValue(type, out meta))
            {
                meta = new DarkMeta(type);
                MetasByType[type] = meta;
                if (meta.TypeIndex != TypeIndex.Invalid)
                {
                    MetasByIndex[meta.TypeIndex] = meta;
                }
            }
            return meta;
        }

        public static DarkMeta Get(TypeIndex typeIndex)
        {
            if (typeIndex != TypeIndex.Invalid)
            {
                DarkMeta meta;
                if (MetasByIndex.TryGetValue(typeIndex, out meta))
                {
                    return meta;
                }
                else
                {
                    Type type;
                    if (TypeRegistry.TryGetType(typeIndex, out type))
                    {
                        return Get(type);
                    }
                }
            }
            return null;
        }
        
        public bool HasFormatters => MemberFormatterGroups.Length > 0;
        
        private void InitFlags()
        {
            var attribute = Type.GetCustomAttribute<DarkContractAttribute>(false);
            this.Flags = attribute != null ? attribute.Flags : DarkFlags.None;
        }

        private void InitBaseMeta()
        {
            var baseType = Type.BaseType;
            while (baseType != null)
            {
                var attribute = baseType.GetCustomAttribute<DarkContractAttribute>(false);
                if (attribute != null && attribute.Flags.IsSerializable())
                {
                    BaseMeta = DarkMeta.Get(baseType);
                    DeepLevel = !Flags.IsSkipWriting() ? BaseMeta.DeepLevel + 1 : BaseMeta.DeepLevel;
                    return;
                }
                baseType = baseType.BaseType;
            }
            DeepLevel = !Flags.IsSkipWriting() ? 1 : 0;
        }

        private void InitConstructor()
        {
            if (Type.IsSubclassOf(typeof(GuidObject)))
            {
                ConstructorInfo = Type.GetConstructor(new Type[] { typeof(Guid) });
            }
            else
            {
                ConstructorInfo = Type.GetDefaultConstructor();
            }
        }

        private LastKeyAttribute LastKeyAttribute;
        private void InitMemberFormatters()
        {
            var count = 0;
            var formatters = new List<KeyValuePair<int, MemberFormatter>>();
            LastKeyAttribute = Type.GetCustomAttribute<LastKeyAttribute>(false);

            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            foreach (var fieldInfo in Type.GetFields(flags))
            {
                ProcessMember(fieldInfo, formatters, ref count);
            }
            foreach (var propertyInfo in Type.GetProperties(flags))
            {
                ProcessMember(propertyInfo, formatters, ref count);
            }

            MemberFormatterGroups = new MemberFormatterGroup[count];
            foreach (var pair in formatters)
            {
                var group = MemberFormatterGroups[pair.Key];
                if (group == null)
                {
                    group = new MemberFormatterGroup();
                    MemberFormatterGroups[pair.Key] = group;
                }
                group.Add(pair.Value);
            }
        }

        private void ProcessMember(MemberInfo memberInfo, List<KeyValuePair<int, MemberFormatter>> formatters, ref int count)
        {
            var keys = memberInfo.GetCustomAttributes<KeyAttribute>();
            foreach (var key in keys)
            {
                if (key.Value == -1)
                {
                    if (LastKeyAttribute == null)
                    {
                        LastKeyAttribute = new LastKeyAttribute();
                        Logger.Logger.Instance.Error("Type {0} contains unsetted keys, but it has no one [LastKey] attribute. Saves won't work properly!", Type.FullName);
                    }
                    key.Value = LastKeyAttribute.Value++;
                }

                if (key.DistributedSubPath != null)
                {
                    var flags = key.Flags | SerializationFlags.Distributed;
                    var externalFormatter = MemberFormatter.CreateDistributed(memberInfo, flags, key.DistributedSubPath);
                    formatters.Add(new KeyValuePair<int, MemberFormatter>(key.Value, externalFormatter));
                }
                var formatter = MemberFormatter.Create(memberInfo, key.Flags);
                formatters.Add(new KeyValuePair<int, MemberFormatter>(key.Value, formatter));
                count = Math.Max(count, key.Value + 1);
            }
        }

        private void InitCallbacks()
        {
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            OnDeserializedHandler = Type.GetMethod("OnDeserialized", flags);
        }

        private void InitTypeIndex()
        {
            TypeIndex = TypeRegistry.GetIndex(Type);

            if (TypeIndex != TypeIndex.Invalid && Type.IsSubclassOf(typeof(Module)))
            {
                if (BaseMeta?.DefaultFieldInfo == null || Flags.IsIgnoreBaseModule())
                {
                    DefaultFieldInfo = new DefaultFieldInfo(Type, TypeIndex);
                }
                else
                {
                    DefaultFieldInfo = BaseMeta.DefaultFieldInfo;
                }
            }
        }

        public int WriteMembers(SerializationFlags flags, object instance, ref byte[] bytes, int offset, IFormatterResolver formatterResolver, bool recursive = true)
        {
            var startOffset = offset;

            if (BaseMeta != null && recursive)
            {
                offset += BaseMeta.WriteMembers(flags, instance, ref bytes, offset, formatterResolver);
            }

            if (Flags.IsSerializable() && !Flags.IsSkipWriting())
            {
                if (MemberFormatterGroups == null)
                {
                    offset += MessagePackBinary.WriteNil(ref bytes, offset);
                }
                else
                {
                    var notNilCount = 0;
                    var formatters = new MemberFormatter[MemberFormatterGroups.Length];
                    for (int i = 0; i < MemberFormatterGroups.Length; i++)
                    {
                        var formatter = MemberFormatterGroups[i]?.GetFormatter(flags);
                        if (formatter != null)
                        {
                            formatters[i] = formatter;
                            notNilCount = i + 1;
                        }
                    }

                    bool hasAnnotations = Serializer.Instance.State.Settings.Flags.HasFlag(SerializationFlags.Annotations);
                    offset += MessagePackBinary.WriteArrayHeader(ref bytes, offset, notNilCount);
                    for (int i = 0; i < notNilCount; i++)
                    {
                        var formatter = formatters[i];
                        if (formatter == null)
                        {
                            offset += MessagePackBinary.WriteNil(ref bytes, offset);
                        }
                        else
                        {
                            if (hasAnnotations)
                            {
                                offset += MessagePackBinaryExtension.WriteComment(ref bytes, offset, formatter.MemberName + ":");
                            }
                              
                            offset += formatter.Write(instance, ref bytes, offset, formatterResolver);
                        }
                    }
                }
            }

            return offset - startOffset;
        }

        public void ReadMembers(SerializationFlags flags, ref object instance, byte[] bytes, int offset, IFormatterResolver formatterResolver, out int readSize, bool recursive = true)
        {
            var startOffset = offset;

            if (BaseMeta != null && recursive)
            {
                BaseMeta.ReadMembers(flags, ref instance, bytes, offset, formatterResolver, out readSize);
                offset += readSize;
            }

            if (Flags.IsSerializable() && !Flags.IsSkipReading())
            {
                if (MessagePackBinary.IsNil(bytes, offset))
                {
                    offset += 1;
                }
                else
                {
                    var memberCount = MessagePackBinary.ReadArrayHeader(bytes, offset, out readSize);
                    offset += readSize;

                    for (int i = 0; i < memberCount; i++)
                    {
                        var formatter = GetFormatter(i, flags);
                        if (formatter != null)
                        {
                            formatter.Read(ref instance, bytes, offset, formatterResolver, out readSize);
                            offset += readSize;
                        }
                        else
                        {
                            if (MessagePackBinary.IsNil(bytes, offset))
                            {
                                MessagePackBinary.ReadNil(bytes, offset, out readSize);
                                offset += readSize;
                            }
                            else
                            {
                                var message = String.Format("unknown serializaion data: {0}:{1}", Type.Name, i);
                                Logger.Logger.Instance.Warning(message);
                                offset += MessagePackBinary.ReadNextBlock(bytes, offset);
                            }
                        }
                    }
                }
            }

            readSize = offset - startOffset;
        }

        public IEnumerable<MemberFormatter> GetAllFormatters()
        {
            var formattersList = new List<MemberFormatter>();
            foreach (var group in MemberFormatterGroups)
            {
                if (group == null)
                {
                    continue;
                }
                formattersList.AddRange(group);
            }
            return formattersList;
        }

        public IEnumerable<MemberFormatter> GetFormatters(SerializationFlags flags)
        {
            foreach (var group in MemberFormatterGroups)
            {
                var formatter = group.GetFormatter(flags);
                if (formatter != null)
                {
                    yield return formatter;
                }
            }
        }

        private MemberFormatter GetFormatter(int index, SerializationFlags flags)
        {
            if (index < MemberFormatterGroups.Length)
            {
                return MemberFormatterGroups[index]?.GetFormatter(flags);
            }
            else
            {
                return null;
            }
        }
    }
}