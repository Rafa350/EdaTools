using System;
using System.Collections.Generic;
using System.Linq;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    public sealed partial class Library {

        private Dictionary<string, EdaComponent> _components;

        /// <summary>
        /// Afegeix un component.
        /// </summary>
        /// <param name="component">El component a afeigir.</param>
        /// 
        public void AddComponent(EdaComponent component) {

            if (component == null)
                throw new ArgumentNullException(nameof(component));

            if ((_components != null) && _components.ContainsKey(component.Name))
                throw new InvalidOperationException(
                    String.Format("El componente '{0}', ya pertenece a la biblioteca.", component.Name));

            if (_components == null)
                _components = new Dictionary<string, EdaComponent>();
            _components.Add(component.Name, component);
        }

        /// <summary>
        /// Afegeix diversos components.
        /// </summary>
        /// <param name="components">Els components a afeigir.</param>
        /// 
        public void AddComponents(IEnumerable<EdaComponent> components) {

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
        public void RemoveComponent(EdaComponent component) {

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
        public bool HasComponents =>
            _components != null;

        /// <summary>
        /// Enumera els noms dels components.
        /// </summary>
        /// 
        public IEnumerable<string> ComponentNames =>
            _components == null ? Enumerable.Empty<string>() : _components.Keys;

        /// <summary>
        /// Enumera els components.
        /// </summary>
        /// 
        public IEnumerable<EdaComponent> Components =>
            _components == null ? Enumerable.Empty<EdaComponent>() : _components.Values;
    }
}
