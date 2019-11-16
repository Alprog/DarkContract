
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;

namespace DarkCrystal.Serialization
{
    public partial class Serializer
    {
        public class SerializerState : RefCountedObject
        {
            public static ConcurrentPool<byte[]> BufferPool = new ConcurrentPool<byte[]>(() => new byte[65535]);

            public readonly SerializationSettings Settings;
            public readonly bool IsText; // cached shortcut
            public readonly bool WriteOnlyChanged; // cached shortcut

            public byte[] Buffer;
            public Dictionary<object, int> ObjectReferences;
            public List<object> ObjectInstances;
            public int LocalOffset;

            public SerializerState PreviousState;

            public SerializerState(SerializationSettings settings, SerializerState previousState)
            {
                this.PreviousState = previousState;

                this.Settings = settings;
                this.Buffer = BufferPool.Retain();
                this.ObjectReferences = new Dictionary<object, int>();
                this.ObjectInstances = new List<object>();

                this.IsText = Settings.Flags.HasFlag(SerializationFlags.Text);
                this.WriteOnlyChanged = settings.Flags.HasFlag(SerializationFlags.WriteOnlyChanged);
                this.LocalOffset = Int32.MaxValue;
                Retain();
            }

            public override void Dispose()
            {
                BufferPool.Release(Buffer);
            }

            //                 LocalOffset
            //                      V
            // global:  0   1   2   3   4   5   6
            // local:                  -1  -2  -3    

            public bool RegObjectReference(object instance, out int reference)
            {
                if (ObjectReferences.TryGetValue(instance, out reference))
                {
                    if (reference > LocalOffset) // is local
                    {
                        reference = LocalOffset - reference; // to local form
                    }
                    return false;
                }
                else
                {
                    RegNewObjectReference(instance);
                    return true;
                }
            }

            public void RegNewObjectReference(object instance)
            {
                ObjectReferences[instance] = ObjectInstances.Count;
                ObjectInstances.Add(instance);
            }

            public object GetInstance(int reference)
            {
                if (reference < 0) // is local form
                {
                    reference = LocalOffset - reference; // to global
                }
                return ObjectInstances[reference];
            }

            public void StartLocalGroup()
            {
                this.LocalOffset = ObjectInstances.Count - 1;
            }

            public void StartLocalGroup(out int previousOffset)
            {
                previousOffset = this.LocalOffset;
                this.LocalOffset = ObjectInstances.Count - 1;
            }

            public void RestoreLocalGroup(int localOffset)
            {
                this.LocalOffset = localOffset;
            }

            public void UnrollLocalGroup()
            {
                for (int i = ObjectInstances.Count - 1; i > LocalOffset; i--)
                {
                    ObjectReferences.Remove(ObjectInstances[i]);
                    ObjectInstances.RemoveAt(i);
                }
            }
        }
    }
}