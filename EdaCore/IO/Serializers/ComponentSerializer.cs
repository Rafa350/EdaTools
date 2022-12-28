using System;
using System.Linq;
using MikroPic.EdaTools.v1.Core.Model.Board;
using NetSerializer.V5;
using NetSerializer.V5.Descriptors;
using NetSerializer.V5.TypeSerializers.Serializers;

namespace MikroPic.EdaTools.v1.Core.IO.Serializers {

    /// <summary>
    /// Serialitzador per la clase 'LblLabel'
    /// </summary>
    /// 
    internal sealed class ComponentSerializer: CustomClassSerializer {

        /// <inheritdoc/>
        /// 
        public ComponentSerializer() :
            base() {

        }

        /// <inheritdoc/>
        /// 
        public override bool CanProcess(Type type) {

            return type == typeof(EdaComponent);
        }

        /// <inheritdoc/>
        /// 
        protected override bool CanProcessProperty(PropertyDescriptor propertyDescriptor) {

            var name = propertyDescriptor.Name;

            if ((name == "Elements") ||
                (name == "Attributes"))
                return true;
            else
                return base.CanProcessProperty(propertyDescriptor);
        }

        /// <inheritdoc/>
        /// 
        protected override void SerializeProperty(SerializationContext context, object obj, PropertyDescriptor propertyDescriptor) {

            var component = (EdaComponent)obj;
            var name = propertyDescriptor.Name;

            if (name == "Elements") {
                EdaElementBase[] elements = component.HasElements ? component.Elements.ToArray() : null;
                var serializer = context.GetTypeSerializer(typeof(EdaElementBase[]));
                serializer.Serialize(context, name, typeof(EdaElementBase[]), elements);
            }

            else if (name == "Attributes") {
                EdaComponentAttribute[] attributes = component.HasAttributes ? component.Attributes.ToArray() : null;
                var serializer = context.GetTypeSerializer(typeof(EdaComponentAttribute[]));
                serializer.Serialize(context, name, typeof(EdaComponentAttribute[]), attributes);
            }

            else
                base.SerializeProperty(context, obj, propertyDescriptor);
        }

        /// <inheritdoc/>
        /// 
        protected override void DeserializeProperty(DeserializationContext context, object obj, PropertyDescriptor propertyDescriptor) {

            var component = (EdaComponent)obj;
            var name = propertyDescriptor.Name;

            if (name == "Elements") {
                var serializer = context.GetTypeSerializer(typeof(EdaElementBase[]));
                serializer.Deserialize(context, name, typeof(EdaElementBase[]), out object elements);
                if (elements != null)
                    Array.ForEach((EdaElementBase[])elements, item => component.AddElement(item));
            }

            else
                base.DeserializeProperty(context, obj, propertyDescriptor);
        }
    }
}
