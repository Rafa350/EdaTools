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

            return type == typeof(EdaComponent);
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

            var component = (EdaComponent)obj;
            var name = propertyDescriptor.Name;

            if (name == "Name") {
                var serializer = GetSerializer(typeof(string));
                serializer.Serialize(writer, name, typeof(string), propertyDescriptor.GetValue(obj));
            }

            else if (name == "Elements") {
                EdaElement[] elements = component.HasElements ? component.Elements.ToArray() : null;
                var serializer = GetSerializer(typeof(EdaElement[]));
                serializer.Serialize(writer, name, typeof(EdaElement[]), elements);
            }

            else if (name == "Attributes") {
                EdaComponentAttribute[] attributes = component.HasAttributes ? component.Attributes.ToArray() : null;
                var serializer = GetSerializer(typeof(EdaComponentAttribute[]));
                serializer.Serialize(writer, name, typeof(EdaComponentAttribute[]), attributes);
            }

            else
                base.SerializeProperty(writer, obj, propertyDescriptor);
        }

        /// <inheritdoc/>
        /// 
        protected override void DeserializeProperty(StorageReader reader, object obj, PropertyDescriptor propertyDescriptor) {

            var component = (EdaComponent)obj;
            var name = propertyDescriptor.Name;

            if (name == "Elements") {
                var serializer = GetSerializer(typeof(EdaElement[]));
                serializer.Deserialize(reader, name, typeof(EdaElement[]), out object elements);
                if (elements != null)
                    Array.ForEach((EdaElement[])elements, item => component.AddElement(item));
            }

            else
                base.DeserializeProperty(reader, obj, propertyDescriptor);
        }
    }
}
