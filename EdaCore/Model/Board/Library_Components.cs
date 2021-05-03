using System;
using System.Collections.Generic;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    public sealed partial class Library {

        private Dictionary<string, Component> _components;

        /// <summary>
        /// Afegeix un component.
        /// </summary>
        /// <param name="component">El component a afeigir.</param>
        /// 
        public void AddComponent(Component component) {

            if (component == null)
                throw new ArgumentNullException(nameof(component));

            if ((_components != null) && _components.ContainsKey(component.Name))
                throw new InvalidOperationException(
                    String.Format("El componente '{0}', ya pertenece a la biblioteca.", component.Name));

            if (_components == null)
                _components = new Dictionary<string, Component>();
            _components.Add(component.Name, component);
        }

        /// <summary>
        /// Afegeix diversos components.
        /// </summary>
        /// <param name="components">Els components a afeigir.</param>
        /// 
        public void AddComponents(IEnumerable<Component> components) {

            if (components == null)
                throw new ArgumentNullException(nameof(components));

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
                throw new ArgumentNullException(nameof(component));

            if ((_components == null) || _components.ContainsKey(component.Name))
                throw new InvalidOperationException(
                    String.Format("El componente '{0}' no esta asignado a esta biblioteca.", component.Name));

            _components.Remove(component.Name);
            if (_components.Count == 0)
                _components = null;
        }

        /// <summary>
        /// Indica si la biblioteca conte components.
        /// </summary>
        /// 
        public bool HasComponents {
            get {
                return _components != null;
            }
        }

        /// <summary>
        /// Enumera els noms dels components.
        /// </summary>
        /// 
        public IEnumerable<string> ComponentNames =>
            _components.Keys;

        /// <summary>
        /// Enumera els components.
        /// </summary>
        /// 
        public IEnumerable<Component> Components =>
            _components.Values;
    }
}
