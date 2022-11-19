using System;
using System.Linq;
using MikroPic.EdaTools.v1.Core.Model.Board;
using NetSerializer.V5;
using NetSerializer.V5.Descriptors;
using NetSerializer.V5.TypeSerializers.Serializers;

namespace MikroPic.EdaTools.v1.Core.IO.Serializers {

    /// <summary>
    /// Serialitzador per la clase 'Board'
    /// </summary>
    /// 
    internal sealed class BoardSerializer: ClassSerializer {

        /// <inheritdoc/>
        /// 
        public BoardSerializer() :
            base() {

        }
        /// <inheritdoc/>
        /// 
        public override bool CanProcess(Type type) {

            return type == typeof(EdaBoard);
        }

        /// <inheritdoc/>
        /// 
        protected override bool CanProcessProperty(PropertyDescriptor propertyDescriptor) {

            var name = propertyDescriptor.Name;

            if ((name == "Components") ||
                (name == "Elements") ||
                (name == "Parts") ||
                (name == "Layers"))
                return true;
            else
                return base.CanProcessProperty(propertyDescriptor);
        }

        /// <inheritdoc/>
        /// 
        protected override void SerializeProperty(SerializationContext context, object obj, PropertyDescriptor propertyDescriptor) {

            var board = (EdaBoard)obj;
            var name = propertyDescriptor.Name;

            if (name == "Layers") {
                EdaLayer[] layers = board.HasLayers ? board.Layers.ToArray() : null;
                var serializer = context.GetTypeSerializer(typeof(EdaLayerId[]));
                serializer.Serialize(context, name, typeof(EdaLayerId[]), layers);
            }

            else if (name == "Components") {
                EdaComponent[] components = board.HasComponents ? board.Components.ToArray() : null;
                var serializer = context.GetTypeSerializer(typeof(EdaComponent[]));
                serializer.Serialize(context, name, typeof(EdaComponent[]), components);
            }

            else if (name == "Elements") {
                EdaElement[] elements = board.HasElements ? board.Elements.ToArray() : null;
                var serializer = context.GetTypeSerializer(typeof(EdaElement[]));
                serializer.Serialize(context, name, typeof(EdaElement[]), elements);
            }

            else if (name == "Parts") {
                EdaPart[] parts = board.HasParts ? board.Parts.ToArray() : null;
                var serializer = context.GetTypeSerializer(typeof(EdaPart[]));
                serializer.Serialize(context, name, typeof(EdaPart[]), parts);
            }

            else
                base.SerializeProperty(context, obj, propertyDescriptor);
        }

        /// <inheritdoc/>
        /// 
        protected override void DeserializeProperty(DeserializationContext context, object obj, PropertyDescriptor propertyDescriptor) {

            var board = (EdaBoard)obj;
            var name = propertyDescriptor.Name;

            if (name == "Components") {
                var serializer = context.GetTypeSerializer(typeof(EdaComponent[]));
                serializer.Deserialize(context, name, typeof(EdaComponent[]), out object components);
                if (components != null)
                    Array.ForEach((EdaComponent[])components, item => board.AddComponent(item));
            }

            else
                base.DeserializeProperty(context, obj, propertyDescriptor);
        }
    }
}
