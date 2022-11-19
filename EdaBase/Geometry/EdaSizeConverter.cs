using System;
using System.ComponentModel;
using System.Globalization;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    /// <summary>
    /// Clase per la gestio de la conversio de tipus.
    /// </summary>
    /// 
    public sealed class EdaSizeConverter: TypeConverter {

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {

            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {

            var size = (EdaSize)value;
            return String.Format("{0}, {1}", size.Width, size.Height);
        }
    }
}
