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
    public sealed class BoardSerializer: ClassSerializer {

        /// <inheritdoc/>
        /// 
        public BoardSerializer(TypeManager typeManager) :
            base(typeManager) {

        }
        /// <inheritdoc/>
        /// 
        public override bool CanSerialize(Type type) {

            return type == typeof(Board);
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

            var board = (Board) obj;
            var name = propertyDescriptor.Name;

            if (name == "Layers") {
                Layer[] layers = board.HasLayers ? board.Layers.ToArray() : null;
                var serializer = GetSerializer(typeof(LayerId[]));
                serializer.Serialize(writer, name, typeof(LayerId[]), layers);
            }

            else if (name == "Components") {
                Component[] components = board.HasComponents ? board.Components.ToArray() : null;
                var serializer = GetSerializer(typeof(Component[]));
                serializer.Serialize(writer, name, typeof(Component[]), components);
            }

            else if (name == "Elements") {
                Element[] elements = board.HasElements ? board.Elements.ToArray() : null;
                var serializer = GetSerializer(typeof(Element[]));
                serializer.Serialize(writer, name, typeof(Element[]), elements);
            }

            else if (name == "Parts") {
                Part[] parts = board.HasParts ? board.Parts.ToArray() : null;
                var serializer = GetSerializer(typeof(Part[]));
                serializer.Serialize(writer, name, typeof(Part[]), parts);
            }

            else
                base.SerializeProperty(writer, obj, propertyDescriptor);
        }

        /// <inheritdoc/>
        /// 
        protected override void DeserializeProperty(StorageReader reader, object obj, PropertyDescriptor propertyDescriptor) {

            var board = (Board)obj;
            var name = propertyDescriptor.Name;

            if (name == "Components") {
                var serializer = GetSerializer(typeof(Component[]));
                serializer.Deserialize(reader, name, typeof(Component[]), out object components);
                if (components != null)
                    Array.ForEach((Component[])components, item => board.AddComponent(item));
            }

            else
                base.DeserializeProperty(reader, obj, propertyDescriptor);
        }
    }
}
