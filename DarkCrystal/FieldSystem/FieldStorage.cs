
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using DarkCrystal.Serialization;
using DarkCrystal.UnityIntegration;

namespace DarkCrystal.FieldSystem
{
    [StaticDarkContract]
    public struct FieldStorage
    {
        public Entity Entity { get; private set; }

        [Key(1)] public List<Entity> Parents;
        [Key(2)] public FieldList OwnValues;

        public bool HasParents => Parents.IsNullOrEmpty();
        public int ParentCount => HasParents ? Parents.Count : 0;

        public void SetEntity(Entity entity)
        {
            this.Entity = entity;
        }

        public void Merge(FieldStorage other)
        {
            this.Parents = other.Parents;
            if (this.OwnValues == null)
            {
                this.OwnValues = other.OwnValues;
            }
            else
            {
                if (other.OwnValues != null)
                {
                    this.OwnValues.Merge(other.OwnValues);
                }
            }
        }

        public bool InsertParent(Entity entity, int index = -1)
        {
            if (IsDerivedFrom(entity))
            {
                return false;
            }
            if (Parents.IsNullOrEmpty())
            {
                Parents = new List<Entity> { entity };
            }
            else
            {
                Parents.Insert(index, entity);
            }

            return true;
        }

        public bool RemoveParent(Entity entity)
        {
            if (!Parents.IsNullOrEmpty())
            {
                Parents.Remove(entity);
                return true;
            }
            return false;
        }

        public bool IsDerivedFrom(Entity entity, bool recursive = true)
        {
            if (!Parents.IsNullOrEmpty())
            {
                foreach (var parent in Parents)
                {
                    if (parent == entity)
                    {
                        return true;
                    }
                    else if (recursive && parent.Fields.IsDerivedFrom(entity, recursive))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public T GetValue<T>(FieldKey key, Entity context)
        {
            T result;
            if (TryGetValue<T>(key, out result, context))
            {
                return result;
            }
            else
            {
                return default(T);
            }
        }

        public T GetPureValue<T>(FieldKey key, Entity context)
        {
            T result;
            if (TryGetPureValue<T>(key, out result, context))
            {
                return result;
            }
            else
            {
                return default(T);
            }
        }

        public void SetValue(FieldKey key, object value)
        {
            var type = value.GetType();
            var fieldMeta = FieldMeta.Get(key);
            var expectedType = fieldMeta.FieldType;
            if (type != expectedType)
            {
                var producerInterace = typeof(IValueProducer<>).MakeGenericType(expectedType);
                if (!type.IsImplementInterface(producerInterace))
                {
                    throw new System.Exception("Field type doesn't match!");
                }
            }

            if (OwnValues == null)
            {
                OwnValues = new FieldList();
            }

            OwnValues[key] = value;
        }

        public bool HasValue(FieldKey key, Entity context)
        {
            object value;
            return TryGetValue(key, out value, context);
        }

        public bool HasOwnField(FieldKey key)
        {
            if (OwnValues != null)
            {
                return OwnValues.ContainsKey(key);
            }
            return false;
        }

        public FieldStatus GetFieldStatus(FieldKey key, out object value, Entity context)
        {
            object parentValue = null;
            bool hasParentValue = false;

            if (!Parents.IsNullOrEmpty())
            {
                foreach (var parent in Parents)
                {
                    if (parent.Fields.TryGetValue(key, out parentValue, context))
                    {
                        hasParentValue = true;
                        break;
                    }
                }
            }

            object ownValue;
            if (TryGetOwnValue<object>(key, context, out ownValue))
            {
                value = ownValue;
                return hasParentValue ? FieldStatus.Overriden : FieldStatus.Base;
            }
            else
            {
                if (hasParentValue)
                {
                    value = parentValue;
                    return FieldStatus.Inherited;
                }
                else
                {
                    value = null;
                    return FieldStatus.Missed;
                }
            }
        }

        public void EnsureOwnValue(FieldKey key)
        {
            if (!OwnValues.ContainsKey(key))
            {
                AddOwnValue(key);
            }
        }

        public object AddOwnValue(FieldKey key, Type type = null)
        {
            if (type == null)
            {
                type = FieldMeta.Get(key).FieldType;
            }
            object value = null;
            if (type.IsArray)
            {
                value = Activator.CreateInstance(type, 0);
            }
            else
            {
                if (type == typeof(String) || type == typeof(string))
                {
                    value = String.Empty;
                }
                else
                {
                    value = Activator.CreateInstance(type);
                }
            }
            SetValue(key, value);
            return value;
        }

        public void ClearAll()
        {
            if (OwnValues != null)
            {
                OwnValues.Clear();
                OwnValues = null;
            }
        }

        public bool ClearOwnValue(FieldKey key)
        {
            if (OwnValues != null)
            {
                var index = OwnValues.IndexOfKey(key);
                if (index >= 0)
                {
                    OwnValues.RemoveAt(index);
                    if (OwnValues.Count == 0)
                    {
                        OwnValues = null;
                    }
                    return true;
                }
            }
            return false;
        }

        public HashSet<FieldKey> GetStoredKeys()
        {
            var set = new HashSet<FieldKey>();
            GetStoredKeys(set);
            return set;
        }

        public object AddOwnValue(FieldKey key, FieldBytes fieldBytes, bool sort = false)
        {
            var slice = fieldBytes.ByteSlice;
            Serializer.Instance.State = GameState.Instance.OriginalState;
            Serializer.Instance.State.StartLocalGroup();
            var resolver = MessagePack.MessagePackSerializer.DefaultResolver;
            var value = FieldMeta.Get(key).Deserialize(slice.GetBytes(), slice.Offset, resolver, out int readSize);
            Serializer.Instance.State = null;
            SetValue(key, value);
            OwnValues.AddFieldBytes(fieldBytes);
            if (sort)
            {
                OwnValues.OriginalBytes.Sort();
            }
            return value;
        }

        //------------------------------------------------------------------------------------

        private void GetStoredKeys(HashSet<FieldKey> set)
        {
            if (OwnValues != null)
            {
                foreach (var key in OwnValues.Keys)
                {
                    set.Add(key);
                }
            }

            if (Parents != null)
            {
                foreach (var parent in Parents)
                {
                    parent.Fields.GetStoredKeys(set);
                }
            }
        }

        private bool TryGetValue<T>(FieldKey key, out T value, Entity context)
        {
            if (!TryGetPureValue(key, out value, context))
            {
                return false;
            }

            return true;
        }

        private bool TryGetPureValue<T>(FieldKey key, out T value, Entity context)
        {
            if (TryGetOwnValue<T>(key, context, out value))
            {
                return true;
            }

            if (!Parents.IsNullOrEmpty())
            {
                foreach (var parent in Parents)
                {
                    if (parent.Fields.TryGetValue(key, out value, context))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool TryGetOwnValue<T>(FieldKey key, Entity context, out T value)
        {
            if (OwnValues != null)
            {
                object @objectValue;
                if (OwnValues.TryGetValue(key, out @objectValue))
                {
                    if (objectValue is T tValue)
                    {
                        value = tValue;
                        return true;
                    }
                    else
                    {
                        var producer = objectValue as IValueProducer<T>;
                        if (producer != null)
                        {
                            value = producer.Produce(context);
                            return true;
                        }
                        else
                        {
                            try
                            {
                                value = (T)Convert.ChangeType(objectValue, typeof(T));
                                return true;
                            }
                            catch
                            {
                                var format = "Unexpected field type. Expect {0}, got {1}";
                                var message = String.Format(format, typeof(T).Name, objectValue.GetType().Name);
                                throw new Exception(message);
                            }
                        }
                    }
                }
            }
            value = default(T);
            return false;
        }
    }
}