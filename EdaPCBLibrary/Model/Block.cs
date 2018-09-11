namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Pcb.Model.Collections;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Clase que representa un bloc predefinit.
    /// </summary>
    /// 
    public sealed partial class Block : IVisitable, IName, IKey<String>, IDisposable {

        private string name;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">Identificador del bloc.</param>
        /// <param name="elements">Llista d'elements.</param>
        /// <param name="attributes">Llista d'atributs.</param>
        /// 
        public Block(string name, IEnumerable<Element> elements = null, IEnumerable<BlockAttribute> attributes = null) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;

            if (elements != null)
                AddElements(elements);

            if (attributes != null)
                AddAttributes(attributes);
        }

        /// <summary>
        /// Clona l'objecte en profunditat.
        /// </summary>
        /// <returns>El clon de l'objecte.</returns>
        /// 
        public Block Clone() {

            Block block = new Block(name);

            if (elements != null)
                foreach (var element in elements)
                    block.AddElement(element.Clone());

            if (attributes != null)
                foreach (var attribute in attributes)
                    block.AddAttribute(attribute.Clone());

            return block;
        }

        /// <summary>
        /// Accepta un visitador.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte la clau.
        /// </summary>
        /// <returns>La clau.</returns>
        /// <remarks>Implementa IKey.GetKey()</remarks>
        /// 
        public string GetKey() {

            return name;
        }

        /// <summary>
        /// Obte o asigna el identificador del component.
        /// </summary>
        /// 
        public string Name {
            get {
                return name;
            }
            set {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("Block.Name");

                name = value;
            }
        }

        /// <summary>
        /// Obte la placa a la que pertany.
        /// </summary>
        /// 
        public Board Board {
            get {
                return Board.GetBoard(this); 
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // Para detectar llamadas redundantes

        void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    // TODO: elimine el estado administrado (objetos administrados).
                    RemoveAllAttributes();
                    RemoveAllElements();
                }

                // TODO: libere los recursos no administrados (objetos no administrados) y reemplace el siguiente finalizador.
                // TODO: configure los campos grandes en nulos.

                disposedValue = true;
            }
        }

        // TODO: reemplace un finalizador solo si el anterior Dispose(bool disposing) tiene código para liberar los recursos no administrados.
        // ~Block() {
        //   // No cambie este código. Coloque el código de limpieza en el anterior Dispose(colocación de bool).
        //   Dispose(false);
        // }

        // Este código se agrega para implementar correctamente el patrón descartable.
        public void Dispose() {
            // No cambie este código. Coloque el código de limpieza en el anterior Dispose(colocación de bool).
            Dispose(true);
            // TODO: quite la marca de comentario de la siguiente línea si el finalizador se ha reemplazado antes.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
