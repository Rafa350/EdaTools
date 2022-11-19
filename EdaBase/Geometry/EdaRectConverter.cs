using System;
using System.ComponentModel;
using System.Globalization;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    /// <summary>
    /// Clase per la gestio de la conversio de tipus.
    /// </summary>
    /// 
    public sealed class EdaRectConverter: TypeConverter {

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {

            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {

            var rect = (EdaRect)value;
            return String.Format("{0}, {1}, {2}, {3}", rect.X, rect.Y, rect.Width, rect.Height);
        }
    }

}
