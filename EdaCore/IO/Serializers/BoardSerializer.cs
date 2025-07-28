using System;
using System.Linq;
using MikroPic.EdaTools.v1.Core.Model.Board;
using NetSerializer.V6;
using NetSerializer.V6.TypeDescriptors;
using NetSerializer.V6.TypeSerializers;
using NetSerializer.V6.TypeSerializers.Serializers;

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

            switch (name) {
                case "Layers":
                    EdaLayer[] layers = board.HasLayers ? board.Layers.ToArray() : null;
                    context.WriteArray(name, layers);
                    break;

                case "Devices":
                    EdaDevice[] devices = board.HasDevices ? board.Devices.ToArray() : null;
                    context.WriteArray(name, devices);
                    break;

                case "Components":
                    EdaComponent[] components = board.HasComponents ? board.Components.ToArray() : null;
                    context.WriteArray(name, components);
                    break;

                case "Elements":
                    EdaElementBase[] elements = board.HasElements ? board.Elements.ToArray() : null;
                    context.WriteArray(name, elements);
                    break;

                case "Parts":
                    EdaPart[] parts = board.HasParts ? board.Parts.ToArray() : null;
                    context.WriteArray(name, parts);
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

            switch (name) {
                case "Components":
                    var components = (EdaComponent[]) context.ReadArray(name, typeof(EdaComponent[]));
                    if (components != null)
                        Array.ForEach(components, item => board.AddComponent(item));
                    break;

                default:
                    base.DeserializeProperty(context, obj, propertyDescriptor);
                    break;
            }
        }
    }
}
