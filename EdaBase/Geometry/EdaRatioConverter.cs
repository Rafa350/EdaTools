using System;
using System.ComponentModel;
using System.Globalization;

namespace MikroPic.EdaTools.v1.Base.Geometry {

    /// <summary>
    /// Clase per la gestio de la conversio de tipus.
    /// </summary>
    /// 
    public sealed class EdaRatioConverter: TypeConverter {

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {

            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {

            var ratio = (EdaRatio)value;
            return String.Format("{0}", ratio.Value);
        }
    }

}
