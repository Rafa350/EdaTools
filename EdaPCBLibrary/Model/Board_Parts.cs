namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Pcb.Model.Collections;
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Clase que representa una placa de circuit impres.
    /// </summary>
    /// 
    public sealed partial class Board {

        // Parts
        private ParentChildKeyCollection<Board, Part, String> parts;


        /// <summary>
        /// Afegeix un component.
        /// </summary>
        /// <param name="part">El component a afeigir.</param>
        /// 
        public void AddPart(Part part) {

            if (part == null)
                throw new ArgumentNullException("part");

            if (part.Board != null)
                throw new InvalidOperationException(
                    String.Format("El componente '{0}' ya pertenece a una placa.", part.Name));

            if (parts == null)
                parts = new ParentChildKeyCollection<Board, Part, String>(this);
            parts.Add(part);
        }

        /// <summary>
        /// Afegeix una coleccio de components.
        /// </summary>
        /// <param name="parts">Els components a afeigir</param>
        /// 
        public void AddParts(IEnumerable<Part> parts) {

            if (parts == null)
                throw new ArgumentException("parts");

            foreach (var part in parts)
                AddPart(part);
        }

        /// <summary>
        /// Elimina una component de la placa.
        /// </summary>
        /// <param name="part">La peça a eliminar.</param>
        /// 
        public void RemovePart(Part part) {

            if (part == null)
                throw new ArgumentNullException("part");

            if ((parts == null) || !parts.Contains(part))
                throw new InvalidOperationException("El componente no pertenece a la placa.");

            parts.Remove(part);
            if (parts.IsEmpty)
                parts = null;
        }

        /// <summary>
        /// Obte un component pel seu nom.
        /// </summary>
        /// <param name="name">El nom del component a buscar.</param>
        /// <param name="throwOnError">True si cal generar una exceptio si no el troba.</param>
        /// <returns>El component, o null si no el troba.</returns>
        /// 
        public Part GetPart(string name, bool throwOnError = false) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            foreach (var part in parts)
                if (part.Name == name)
                    return part;

            if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("El componente '{0}', no se encontro en esta placa.", name));

            return null;
        }

        /// <summary>
        /// Obte la placa a la que pertany un element
        /// </summary>
        /// <param name="element">L'element.</param>
        /// <returns>La placa. Null si no pertany a cap.</returns>
        /// 
        internal static Board GetBoard(Part part) {

            return ParentChildCollection<Board, Part>.GetParent(part);
        }

        /// <summary>
        /// Indica si conte parts.
        /// </summary>
        /// 
        public bool HasParts {
            get {
                return parts != null;
            }
        }

        /// <summary>
        /// Obte un enumerador pels components.
        /// </summary>
        /// 
        public IEnumerable<Part> Parts {
            get {
                return parts;
            }
        }
    }
}
