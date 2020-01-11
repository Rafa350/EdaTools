namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using System;
    using System.Collections.Generic;

    public sealed partial class Library {

        private Dictionary<string, Component> components;

        /// <summary>
        /// Afegeix un component.
        /// </summary>
        /// <param name="component">El component a afeigir.</param>
        /// 
        public void AddComponent(Component component) {

            if (component == null)
                throw new ArgumentNullException("component");

            if ((components != null) && components.ContainsKey(component.Name))
                throw new InvalidOperationException(
                    String.Format("El componente '{0}', ya pertenece a la biblioteca.", component.Name));

            if (components == null)
                components = new Dictionary<string, Component>();
            components.Add(component.Name, component);
        }

        /// <summary>
        /// Afegeix diversos components.
        /// </summary>
        /// <param name="components">Els components a afeigir.</param>
        /// 
        public void AddComponents(IEnumerable<Component> components) {

            if (components == null)
                throw new ArgumentNullException("components");

            foreach (var component in components)
                AddComponent(component);
        }

        /// <summary>
        /// Elimina un component.
        /// </summary>
        /// <param name="component">El component a eliminar.</param>
        /// 
        public void RemoveComponent(Component component) {

            if (component == null)
                throw new ArgumentNullException("component");

            if ((components == null) || components.ContainsKey(component.Name))
                throw new InvalidOperationException(
                    String.Format("El componente '{0}' no esta asignado a esta biblioteca.", component.Name));

            components.Remove(component.Name);
            if (components.Count == 0)
                components = null;
        }

        /// <summary>
        /// Indica si la biblioteca conte components.
        /// </summary>
        /// 
        public bool HasComponents {
            get {
                return components != null;
            }
        }

        /// <summary>
        /// Enumera els noms dels components.
        /// </summary>
        /// 
        public IEnumerable<string> ComponentNames {
            get {
                return components.Keys;
            }
        }

        /// <summary>
        /// Enumera els components.
        /// </summary>
        /// 
        public IEnumerable<Component> Components {
            get {
                return components.Values;
            }
        }
    }
}
