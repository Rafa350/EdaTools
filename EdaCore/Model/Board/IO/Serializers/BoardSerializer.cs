using System;
using System.Linq;

using NetSerializer.Descriptors;
using NetSerializer.Storage;
using NetSerializer.TypeSerializers;
using NetSerializer.TypeSerializers.Serializers;

namespace MikroPic.EdaTools.v1.Core.Model.Board.IO.Serializers {

    /// <summary>
    /// Serialitzador per la clase 'LblLabel'
    /// </summary>
    /// 
    public sealed class BoardSerializer : ClassSerializer {

        /// <inheritdoc/>
        /// 
        public BoardSerializer(TypeManager typeManager) :
            base(typeManager) {

        }
        /// <inheritdoc/>
        /// 
        public override bool CanSerialize(Type type) {

            return type == typeof(EdaBoard);
        }

        /// <inheritdoc/>
        /// 
        public override bool CanSerializeProperty(PropertyDescriptor propertyDescriptor) {

            var name = propertyDescriptor.Name;

            if ((name == "Components") ||
                (name == "Elements") ||
                (name == "Parts") ||
                (name == "Layers"))
                return true;
            else
                return base.CanSerializeProperty(propertyDescriptor);
        }

        /// <inheritdoc/>
        /// 
        protected override void SerializeProperty(StorageWriter writer, object obj, PropertyDescriptor propertyDescriptor) {

            var board = (EdaBoard)obj;
            var name = propertyDescriptor.Name;

            if (name == "Layers") {
                EdaLayer[] layers = board.HasLayers ? board.Layers.ToArray() : null;
                var serializer = GetSerializer(typeof(EdaLayerId[]));
                serializer.Serialize(writer, name, typeof(EdaLayerId[]), layers);
            }

            else if (name == "Components") {
                EdaComponent[] components = board.HasComponents ? board.Components.ToArray() : null;
                var serializer = GetSerializer(typeof(EdaComponent[]));
                serializer.Serialize(writer, name, typeof(EdaComponent[]), components);
            }

            else if (name == "Elements") {
                EdaElement[] elements = board.HasElements ? board.Elements.ToArray() : null;
                var serializer = GetSerializer(typeof(EdaElement[]));
                serializer.Serialize(writer, name, typeof(EdaElement[]), elements);
            }

            else if (name == "Parts") {
                EdaPart[] parts = board.HasParts ? board.Parts.ToArray() : null;
                var serializer = GetSerializer(typeof(EdaPart[]));
                serializer.Serialize(writer, name, typeof(EdaPart[]), parts);
            }

            else
                base.SerializeProperty(writer, obj, propertyDescriptor);
        }

        /// <inheritdoc/>
        /// 
        protected override void DeserializeProperty(StorageReader reader, object obj, PropertyDescriptor propertyDescriptor) {

            var board = (EdaBoard)obj;
            var name = propertyDescriptor.Name;

            if (name == "Components") {
                var serializer = GetSerializer(typeof(EdaComponent[]));
                serializer.Deserialize(reader, name, typeof(EdaComponent[]), out object components);
                if (components != null)
                    Array.ForEach((EdaComponent[])components, item => board.AddComponent(item));
            }

            else
                base.DeserializeProperty(reader, obj, propertyDescriptor);
        }
    }
}
