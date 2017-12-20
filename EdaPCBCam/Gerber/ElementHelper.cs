namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Collections;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    internal static class ElementHelper {

        /// <summary>
        /// Comprova si el element pertany a alguna capa.
        /// </summary>
        /// <param name="element">El element.</param>
        /// <param name="layers">Conjunt de capes a comprovar.</param>
        /// <returns>True si pertany a alguna capa.</returns>
        /// 
        public static bool IsOnAnyLayer(this Element element, IEnumerable<Layer> layers) {

            foreach (Layer layer in layers)
                if (element.IsOnLayer(layer))
                    return true;
            return false;
        }

        public static Point GetPosition(this IPosition element, Part part) {

            Point p = element.Position;
            if (part != null) {
                Matrix m = new Matrix();
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotate, part.Position.X, part.Position.Y);
                return m.Transform(p);
            }
            else
                return p;
        }

        public static Point GetEndPosition(this LineElement element, Part part) {

            Point p = element.EndPosition;
            if (part != null) {
                Matrix m = new Matrix();
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotate, part.Position.X, part.Position.Y);
                return m.Transform(p);
            }
            else
                return p;
        }

        public static Point GetCenter(this ArcElement element, Part part) {

            Point p = element.Center;
            if (part != null) {
                Matrix m = new Matrix();
                m.Translate(part.Position.X, part.Position.Y);
                m.RotateAt(part.Rotate, part.Position.X, part.Position.Y);
                return m.Transform(p);
            }
            else
                return p;
        }
    }
}

