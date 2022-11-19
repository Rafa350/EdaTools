using System;
using System.ComponentModel;
using System.Globalization;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    /// <summary>
    /// Clase per la gestio de la conversio de tipus.
    /// </summary>
    /// 
    public sealed class EdaArcPointConverter: TypeConverter {

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {

            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {

            var ap = (EdaArcPoint)value;
            return String.Format("{0}, {1}, {2}", ap.Position.X, ap.Position.Y, ap.Arc.Value);
        }
    }
}
