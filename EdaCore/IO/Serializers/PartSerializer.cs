using System;
using System.Linq;
using MikroPic.EdaTools.v1.Core.Model.Board;
using NetSerializer.V6;
using NetSerializer.V6.TypeDescriptors;
using NetSerializer.V6.TypeSerializers.Serializers;

namespace MikroPic.EdaTools.v1.Core.IO.Serializers {

    internal sealed class PartSerializer: CustomClassSerializer {

        /// <inheritdoc/>
        /// 
        public PartSerializer() :
            base() {

        }

        /// <inheritdoc/>
        /// 
        public override bool CanProcess(Type type) {

            return type == typeof(EdaPart);
        }

        /// <inheritdoc/>
        /// 
        protected override bool CanProcessProperty(PropertyDescriptor propertyDescriptor) {

            var name = propertyDescriptor.Name;

            if (name == "Attributes")
                return true;
            else
                return base.CanProcessProperty(propertyDescriptor);
        }

        /// <inheritdoc/>
        /// 
        protected override void SerializeProperty(SerializationContext context, object obj, PropertyDescriptor propertyDescriptor) {

            var part = (EdaPart)obj;
            var name = propertyDescriptor.Name;

            if (name == "Attributes") {
                EdaPartAttribute[] attributes = part.HasAttributes ? part.Attributes.ToArray() : null;
                context.WriteArray(name, attributes);
            }

            else
                base.SerializeProperty(context, obj, propertyDescriptor);
        }

        /// <inheritdoc/>
        /// 
        protected override void DeserializeProperty(DeserializationContext context, object obj, PropertyDescriptor propertyDescriptor) {

            var part = (EdaPart)obj;
            var name = propertyDescriptor.Name;

            base.DeserializeProperty(context, obj, propertyDescriptor);
        }
    }
}
