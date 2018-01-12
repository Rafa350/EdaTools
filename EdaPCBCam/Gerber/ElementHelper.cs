namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System.Collections.Generic;

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
                if (element.IsOnLayer(layer.LayerId))
                    return true;
            return false;
        }
    }
}

