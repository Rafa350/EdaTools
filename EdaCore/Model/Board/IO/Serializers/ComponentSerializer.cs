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
    public sealed class ComponentSerializer: ClassSerializer {

        /// <inheritdoc/>
        /// 
        public ComponentSerializer(TypeManager typeManager) :
            base(typeManager) {

        }
        /// <inheritdoc/>
        /// 
        public override bool CanSerialize(Type type) {

            return type == typeof(Component);
        }

        /// <inheritdoc/>
        /// 
        public override bool CanSerializeProperty(PropertyDescriptor propertyDescriptor) {

            var name = propertyDescriptor.Name;

            if ((name == "Elements") ||
                (name == "Attributes") ||
                (name == "Name"))
                return true;
            else
                return base.CanSerializeProperty(propertyDescriptor);
        }

        /// <inheritdoc/>
        /// 
        protected override void SerializeProperty(StorageWriter writer, object obj, PropertyDescriptor propertyDescriptor) {

            var component = (Component) obj;
            var name = propertyDescriptor.Name;

            if (name == "Name") {
                var serializer = GetSerializer(typeof(string));
                serializer.Serialize(writer, name, typeof(string), propertyDescriptor.GetValue(obj));
            }

            else if (name == "Elements") {
                Element[] elements = component.HasElements ? component.Elements.ToArray() : null;
                var serializer = GetSerializer(typeof(Element[]));
                serializer.Serialize(writer, name, typeof(Element[]), elements);
            }

            else if (name == "Attributes") {
                ComponentAttribute[] attributes = component.HasAttributes ? component.Attributes.ToArray() : null;
                var serializer = GetSerializer(typeof(ComponentAttribute[]));
                serializer.Serialize(writer, name, typeof(ComponentAttribute[]), attributes);
            }

            else
                base.SerializeProperty(writer, obj, propertyDescriptor);
        }

        /// <inheritdoc/>
        /// 
        protected override void DeserializeProperty(StorageReader reader, object obj, PropertyDescriptor propertyDescriptor) {

            var component = (Component)obj;
            var name = propertyDescriptor.Name;

            if (name == "Elements") {
                var serializer = GetSerializer(typeof(Element[]));
                serializer.Deserialize(reader, name, typeof(Element[]), out object elements);
                if (elements != null)
                    Array.ForEach((Element[])elements, item => component.AddElement(item));
            }

            else
                base.DeserializeProperty(reader, obj, propertyDescriptor);
        }
    }
}
