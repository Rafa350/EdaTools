﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Clase que representa una placa de circuit impres.
    /// </summary>
    /// 
    public sealed partial class EdaBoard {

        private Dictionary<string, EdaComponent> _components;

        /// <summary>
        /// Afeigeix un component.
        /// </summary>
        /// <param name="component">El component a afeigir.</param>
        /// 
        public void AddComponent(EdaComponent component) {

            if (component == null)
                throw new ArgumentNullException(nameof(component));

            if ((_components != null) && _components.ContainsKey(component.Name))
                throw new InvalidOperationException(
                    String.Format("El componente '{0}', ya pertenece a la placa.", component.Name));

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
                    String.Format("El componente '{0}' no esta asignado a esta placa.", component.Name));

            _components.Remove(component.Name);

            if (_components.Count == 0)
                _components = null;
        }

        /// <summary>
        /// Obte un component pel seu nom.
        /// </summary>
        /// <param name="name">El nom del component.</param>
        /// <param name="throwOnError">True si cal generar una excepcio si no el troba.</param>
        /// <returns>El component, o null si no el troba.</returns>
        /// 
        public EdaComponent GetComponent(string name, bool throwOnError = true) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if ((_components != null) && _components.TryGetValue(name, out var component))
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
        public bool HasComponents =>
            _components != null;

        /// <summary>
        /// Obte el nombre de components.
        /// </summary>
        /// 
        public int ComponentCount =>
            _components == null ? 0 : _components.Count;

        /// <summary>
        /// Enumera els noms dels components
        /// </summary>
        /// 
        public IEnumerable<string> ComponentNames =>
            _components == null ? Enumerable.Empty<string>() : _components.Keys;

        /// <summary>
        /// Obte un enumerador pels blocs.
        /// </summary>
        /// 
        public IEnumerable<EdaComponent> Components =>
            _components == null ? Enumerable.Empty<EdaComponent>() : _components.Values;
    }
}
