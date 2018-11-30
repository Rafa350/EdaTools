namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using MikroPic.EdaTools.v1.Collections;
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Clase que representa una placa de circuit impres.
    /// </summary>
    /// 
    public sealed partial class Board {

        // Components
        private ParentChildKeyCollection<Board, Component, String> components;

        /// <summary>
        /// Afeigeix un component.
        /// </summary>
        /// <param name="component">El component a afeigir.</param>
        /// 
        public void AddComponent(Component component) {

            if (component == null)
                throw new ArgumentNullException("component");

            if (component.Board != null)
                throw new InvalidOperationException(
                    String.Format("El bloque '{0}', ya pertenece a una placa.", component.Name));

            if (components == null)
                components = new ParentChildKeyCollection<Board, Component, String>(this);
            components.Add(component);
        }

        public void AddComponents(IEnumerable<Component> components) {

            if (components == null)
                throw new ArgumentNullException("components");

            foreach (var component in components)
                AddComponent(component);
        }

        /// <summary>
        /// Elimina un block.
        /// </summary>
        /// <param name="component">El bloc a eliminar.</param>
        /// 
        public void RemoveComponent(Component component) {

            if (component == null)
                throw new ArgumentNullException("component");

            if (component.Board != this)
                throw new InvalidOperationException(
                    String.Format("El componente '{0}' no esta asignado a esta placa.", component.Name));

            components.Remove(component);
            if (components.IsEmpty)
                components = null;
        }

        /// <summary>
        /// Obte un bloc pel seu nom.
        /// </summary>
        /// <param name="name">El nom del bloc.</param>
        /// <param name="throwOnError">True si cal generar una excepcio si no el troba.</param>
        /// <returns>El bloc, o null si no el troba.</returns>
        /// 
        public Component GetComponent(string name, bool throwOnError = true) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Component component = components?.Get(name);
            if (component != null)
                return component;

            else if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("El componente '{0}', no esta asignado a esta placa.", name));

            else
                return null;
        }

        /// <summary>
        /// Indica si conte components
        /// </summary>
        /// 
        public bool HasComponents {
            get {
                return components != null;
            }
        }

        /// <summary>
        /// Enumera els noms dels components
        /// </summary>
        /// 
        public IEnumerable<string> ComponentNames {
            get {
                return components?.Keys;
            }
        }

        /// <summary>
        /// Obte un enumerador pels blocs.
        /// </summary>
        /// 
        public IEnumerable<Component> Components {
            get {
                return components;
            }
        }
    }
}
