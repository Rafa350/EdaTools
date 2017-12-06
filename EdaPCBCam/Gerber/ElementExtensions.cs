namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public static class ElementExtensions {

        /// <summary>
        /// Comprova si el element pertany a alguna capa.
        /// </summary>
        /// <param name="element">El element.</param>
        /// <param name="layers">Conjunt de capes a comprovar.</param>
        /// <returns>True si pertany a alguna capa.</returns>
        /// 
        public static bool InAnyLayer(this ElementBase element, IEnumerable<Layer> layers) {

            foreach (Layer layer in layers)
                if (element.InLayer(layer))
                    return true;
            return false;
        }
    }
}
