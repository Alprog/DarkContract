
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#if UNITY_EDITOR

using DarkCrystal.Example;
using DarkCrystal.FieldSystem;
using DarkCrystal.Serialization;
using UnityEditor;

namespace DarkCrystal.UnityIntegration
{
    public partial class DataExplorer
    {
        [CustomEditor(typeof(DataSelector))]
        public class DataSelectorEditor : ExtendedEditor<DataSelector>
        {
            public GuidObject GuidObject => Target.SelectedObject;

            protected override void OnGUI()
            {
                if (GuidObject == null)
                {
                    Layout.Label("No selection");
                    return;
                }

                Inspect(GuidObject);
            }

            public void Inspect(GuidObject guidObject)
            {
                Layout.ReadonlyTextField("Guid", guidObject.Guid.ToString());

                guidObject.ID = EditorGUILayout.TextField("ID", guidObject.ID);

                if (guidObject is BaseNode node)
                {
                    InspectNode(node);
                }

                if (guidObject is Entity entity)
                {
                    InspectEntity(entity);
                }
            }

            public void InspectNode(BaseNode node)
            {
                Layout.Vector3Field("Position", ref node.Position);
                Layout.Button("Link: " + Utils.ToString(node.Link), () => DataExplorer.Instance.Select(node.Link)); 

                if (node is DerivedNode derived)
                {
                    Layout.TextField("DerivedNotes", ref derived.DerivedNotes);
                }
            }

            public void InspectEntity(Entity entity)
            {
                Layout.Label("Fields:");
                foreach (var key in entity.Fields.OwnValues.Keys.ToList())
                {
                    var type = FieldMeta.Get(key).FieldType;

                    if (type == typeof(int))
                    {
                        var value = entity.Fields.GetValue<int>(key, entity);
                        if (Layout.IntField(key.GetName(), ref value))
                        {
                            entity.Fields.SetValue(key, value);
                        }
                    }
                    else if (type == typeof(string))
                    {
                        var value = entity.Fields.GetValue<string>(key, entity);
                        if (Layout.TextField(key.GetName(), ref value))
                        {
                            entity.Fields.SetValue(key, value);
                        }
                    }
                    else if (type == typeof(Entity))
                    {
                        var link = entity.Fields.GetValue<Entity>(key, entity);
                        Layout.Button("Link: " + Utils.ToString(link), () => DataExplorer.Instance.Select(link));
                    }
                }
            }
        }
    }
}

#endif