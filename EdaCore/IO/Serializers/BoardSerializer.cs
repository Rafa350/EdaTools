using System;
using System.Linq;
using MikroPic.EdaTools.v1.Core.Model.Board;
using NetSerializer.V5;
using NetSerializer.V5.Descriptors;
using NetSerializer.V5.TypeSerializers;
using NetSerializer.V5.TypeSerializers.Serializers;

namespace MikroPic.EdaTools.v1.Core.IO.Serializers {

    /// <summary>
    /// Serialitzador per la clase 'Board'
    /// </summary>
    /// 
    internal sealed class BoardSerializer: CustomClassSerializer {

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
                (name == "Devices") ||
                (name == "Elements") ||
                (name == "Layers") ||
                (name == "Parts"))
                return true;
            else
                return base.CanProcessProperty(propertyDescriptor);
        }

        /// <inheritdoc/>
        /// 
        protected override void SerializeProperty(SerializationContext context, object obj, PropertyDescriptor propertyDescriptor) {

            var board = (EdaBoard)obj;
            var name = propertyDescriptor.Name;

            ITypeSerializer serializer;
            switch (name) {
                case "Layers":
                    EdaLayer[] layers = board.HasLayers ? board.Layers.ToArray() : null;
                    serializer = context.GetTypeSerializer(typeof(EdaLayer[]));
                    serializer.Serialize(context, name, typeof(EdaLayer[]), layers);
                    break;

                case "Devices":
                    EdaDevice[] devices = board.HasDevices ? board.Devices.ToArray() : null;
                    serializer = context.GetTypeSerializer(typeof(EdaDevice[]));
                    serializer.Serialize(context, name, typeof(EdaDevice[]), devices);
                    break;

                case "Components":
                    EdaComponent[] components = board.HasComponents ? board.Components.ToArray() : null;
                    serializer = context.GetTypeSerializer(typeof(EdaComponent[]));
                    serializer.Serialize(context, name, typeof(EdaComponent[]), components);
                    break;

                case "Elements":
                    EdaElementBase[] elements = board.HasElements ? board.Elements.ToArray() : null;
                    serializer = context.GetTypeSerializer(typeof(EdaElementBase[]));
                    serializer.Serialize(context, name, typeof(EdaElementBase[]), elements);
                    break;

                case "Parts":
                    EdaPart[] parts = board.HasParts ? board.Parts.ToArray() : null;
                    serializer = context.GetTypeSerializer(typeof(EdaPart[]));
                    serializer.Serialize(context, name, typeof(EdaPart[]), parts);
                    break;

                default:
                    base.SerializeProperty(context, obj, propertyDescriptor);
                    break;
            }
        }

        /// <inheritdoc/>
        /// 
        protected override void DeserializeProperty(DeserializationContext context, object obj, PropertyDescriptor propertyDescriptor) {

            var board = (EdaBoard)obj;
            var name = propertyDescriptor.Name;

            ITypeSerializer serializer;
            switch (name) {
                case "Components":
                    serializer = context.GetTypeSerializer(typeof(EdaComponent[]));
                    serializer.Deserialize(context, name, typeof(EdaComponent[]), out object components);
                    if (components != null)
                        Array.ForEach((EdaComponent[])components, item => board.AddComponent(item));
                    break;

                default:
                    base.DeserializeProperty(context, obj, propertyDescriptor);
                    break;
            }
        }
    }
}
