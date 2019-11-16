
using System;
using System.IO;
using System.Text;
using MessagePack;

namespace DarkCrystal.Serialization
{
    public partial class Serializer : Singleton<Serializer>
    {
        public Serializer()
        {
            MessagePackSerializer.SetDefaultResolver(MainResolver.Instance);
        }

        [ThreadStatic] private static SerializerState ThreadStaticState;

        public SerializerState State
        {
            get => ThreadStaticState; 
            set
            {
                ThreadStaticState = value;
                if (ThreadStaticState != null)
                {
                    MessagePack.Internal.InternalMemoryPool.SetBuffer(ThreadStaticState.Buffer);
                }
            }
        }

        public byte[] Serialize<T>(T @object, SerializationSettings settings = default)
        {
            try
            {
                this.State = new SerializerState(settings, State);

                var bytes = MessagePackSerializer.Serialize<T>(@object);
                if (settings.Flags.HasFlag(SerializationFlags.Text))
                {
                    bytes = ToTextFormat(bytes);
                }
                return bytes;
            }
            finally
            {
                this.State.Release();
                this.State = State.PreviousState;
            }
        }

        public T Deserialize<T>(byte[] bytes, SerializationSettings settings = default, Action<SerializerState, byte[]> internalStateCallback = null)
        {
            try
            {
                this.State = new SerializerState(settings, State);

                if (settings.Flags.HasFlag(SerializationFlags.Text))
                {
                    bytes = FromTextFormat(bytes);
                }
                internalStateCallback?.Invoke(State, bytes);
                return MessagePackSerializer.Deserialize<T>(bytes);
            }
            finally
            {
                this.State.Release();
                this.State = State.PreviousState;
            }
        }

        public string ToJson(byte[] bytes)
        {
            return MessagePackSerializer.ToJson(bytes);
        }

        public byte[] FromJson(string jsonText)
        {
            return MessagePackSerializer.FromJson(jsonText);
        }

        public string ToJson<T>(T @object, SerializationSettings settings = default)
        {
            return ToJson(Serialize(@object, settings));
        }

        public T Clone<T>(T @object, SerializationSettings settings = default)
        {
            return Deserialize<T>(Serialize(@object, settings), settings);
        }

        public void SerializeToFile<T>(T @object, string path, SerializationSettings settings = default)
        {
            var bytes = Serialize(@object, settings);
            FileSystemCache.WriteAllBytes(path, bytes);
        }

        public T DeserializeFromFile<T>(string path, SerializationSettings settings = default)
        {
            var bytes = File.ReadAllBytes(path);
            return Deserialize<T>(bytes, settings);
        }
       
        public bool IsInternal(GuidObject guidObject)
        {
            return State.Settings.PartialSerialization ? State.Settings.InternalObjects.Contains(guidObject) : true;
        }

        public byte[] RemoveAnnotations(byte[] bytes)
        {
            var stream = new MemoryStream();

            int offset = 0;
            int count;
            while (offset < bytes.Length)
            {
                switch (MessagePackBinary.GetMessagePackType(bytes, offset))
                {
                    case MessagePackType.Extension:
                        var comment = MessagePackBinaryExtension.ReadComment(bytes, offset, out count);
                        break;

                    case MessagePackType.Array:
                        MessagePackBinary.ReadArrayHeader(bytes, offset, out count);
                        stream.Write(bytes, offset, count);
                        break;

                    case MessagePackType.Map:
                        MessagePackBinary.ReadMapHeader(bytes, offset, out count);
                        stream.Write(bytes, offset, count);
                        break;

                    default:
                        count = MessagePackBinary.ReadNext(bytes, offset);
                        stream.Write(bytes, offset, count);
                        break;
                }
                offset += count;
            }

            return stream.ToArray();
        }

        private void RecoverState(SerializerState state)
        {
            this.State = state;
            if (State?.Buffer != null)
            {
                MessagePack.Internal.InternalMemoryPool.SetBuffer(State.Buffer);
            }
        }

        private byte[] ToTextFormat(byte[] bytes)
        {
            var jsonText = MessagePackSerializer.ToJson(bytes);
            return Encoding.UTF8.GetBytes(jsonText);
        }

        private byte[] FromTextFormat(byte[] bytes)
        {
            var jsonText = Encoding.UTF8.GetString(bytes);
            return MessagePackSerializer.FromJson(jsonText);
        }
    }
}