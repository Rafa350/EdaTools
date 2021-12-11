using System;
using System.Collections.Generic;
using System.Linq;

using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Extensions per la clase Component
    /// </summary>
    /// 
    public static class EdaComponentExtensions {

        /// <summary>
        /// Obte un pad pel seu nom.
        /// </summary>
        /// <param name="component">El component.</param>
        /// <param name="name">El nom del pad.</param>
        /// <param name="throwOnError">True si dispara una excepcio si no el troba.</param>
        /// <returns>El pad. Null si no el troba.</returns>
        /// 
        public static PadElement GetPad(this EdaComponent component, string name, bool throwOnError = true) {

            var pad = component.Pads().First(pad => pad.Name == name);
            if (pad != null)
                return pad;

            else if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("No se encontro el pad '{0}' en el componente '{1}'.", name, component.Name));

            else
                return null;
        }

        /// <summary>
        /// Indica si el component conte pads.
        /// </summary>
        /// <param name="component">El component.</param>
        /// <returns>True si conte pads.</returns>
        /// 
        public static bool HasPads(this EdaComponent component) =>
            component.Elements.OfType<PadElement>().Any();

        /// <summary>
        /// Enumera el nom dels pads.
        /// </summary>
        /// <param name="component">El component.</param>
        /// <returns>El resultat.</returns>
        /// 
        public static IEnumerable<string> PadNames(this EdaComponent component) =>
            component.Elements.OfType<PadElement>().Select(pad => pad.Name);

        /// <summary>
        /// Enumera els pads.
        /// </summary>
        /// <param name="component">El component.</param>
        /// <returns>El resultat.</returns>
        /// 
        public static IEnumerable<PadElement> Pads(this EdaComponent component) =>
            component.Elements.OfType<PadElement>();
    }
}
