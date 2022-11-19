using System;
using System.Linq;
using MikroPic.EdaTools.v1.Core.Model.Board;
using NetSerializer.V5.Descriptors;
using NetSerializer.V5.Formatters.Xml;
using NetSerializer.V5.Formatters;
using NetSerializer.V5;
using NetSerializer.V5.TypeSerializers.Serializers;

namespace MikroPic.EdaTools.v1.Core.IO.Serializers {

    /// <summary>
    /// Serialitzador per la clase 'LblLabel'
    /// </summary>
    /// 
    internal sealed class ComponentSerializer: ClassSerializer {

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
                (name == "Attributes") ||
                (name == "Name"))
                return true;
            else
                return base.CanProcessProperty(propertyDescriptor);
        }

        /// <inheritdoc/>
        /// 
        protected override void SerializeProperty(SerializationContext context, object obj, PropertyDescriptor propertyDescriptor) {

            var component = (EdaComponent)obj;
            var name = propertyDescriptor.Name;

            if (name == "Name") {
                var serializer = context.GetTypeSerializer(typeof(string));
                serializer.Serialize(context, name, typeof(string), propertyDescriptor.GetValue(obj));
            }

            else if (name == "Elements") {
                EdaElement[] elements = component.HasElements ? component.Elements.ToArray() : null;
                var serializer = context.GetTypeSerializer(typeof(EdaElement[]));
                serializer.Serialize(context, name, typeof(EdaElement[]), elements);
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
                var serializer = context.GetTypeSerializer(typeof(EdaElement[]));
                serializer.Deserialize(context, name, typeof(EdaElement[]), out object elements);
                if (elements != null)
                    Array.ForEach((EdaElement[])elements, item => component.AddElement(item));
            }

            else
                base.DeserializeProperty(context, obj, propertyDescriptor);
        }
    }
}
