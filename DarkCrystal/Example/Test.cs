
// Copyright (c) Dark Crystal Games. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using DarkCrystal.FieldSystem;
using DarkCrystal.Serialization;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace DarkCrystal.Example
{
    public static class Test
    {
        [MenuItem("DarkContractExample/NodeTest")]
        public static void RunNodeTest()
        {
            var baseNode = new BaseNode("BaseNode", Vector3.right);
            var derivedNode = new DerivedNode("DerivedNode", Vector3.up, "this is notes");
            baseNode.Link = derivedNode;
            derivedNode.Link = baseNode;

            var settings = new SerializationSettings(SerializationFlags.Text | SerializationFlags.Annotations, derivedNode);
            var bytes = Serializer.Instance.Serialize(derivedNode, settings);

            var json = Encoding.UTF8.GetString(bytes);
            Logger.Instance.Print(json);

            baseNode.Release();
            derivedNode.Release();

            var deserializedNode = Serializer.Instance.Deserialize<Entity>(bytes, settings);
            settings = new SerializationSettings();
            Logger.Instance.Print("Deserialized");
        }

        [MenuItem("DarkContractExample/EntityTest")]
        public static void RunEntityTest()
        {
            var entityA = new Entity();
            entityA.ID = "EntityA";
            entityA.Fields.SetValue(StringField.Country, "Russia");
            entityA.Fields.SetValue(StringField.City, "Kaliningrad");
            entityA.Fields.SetValue(IntField.AppleCount, 4);
            entityA.Fields.SetValue(IntField.OrangeCount, 3);

            var entityB = new Entity();
            entityB.Fields.SetValue(StringField.Country, "China");
            entityB.Fields.SetValue(IntField.BananaCount, 77);
            entityB.Fields.SetValue(StringField.Notes, "this is notes");

            entityA.Fields.SetValue(EntityField.Link, entityB);
            entityB.Fields.SetValue(EntityField.Link, entityA);
            
            var settings = new SerializationSettings(SerializationFlags.Binary);
            var bytes = Serializer.Instance.Serialize(entityA, settings);

            var json = Serializer.Instance.ToJson(bytes);
            Logger.Instance.Print(json);

            entityA.Release();
            entityB.Release();

            var deserializedEntity = Serializer.Instance.Deserialize<Entity>(bytes, settings);
            Logger.Instance.Print("Deserialized");
        }
    }
}