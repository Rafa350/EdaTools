﻿namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Clase que representa una placa de circuit impres.
    /// </summary>
    /// 
    public sealed partial class Board {

        private Dictionary<string, Component> components;

        /// <summary>
        /// Afeigeix un component.
        /// </summary>
        /// <param name="component">El component a afeigir.</param>
        /// 
        public void AddComponent(Component component) {

            if (component == null)
                throw new ArgumentNullException(nameof(component));

            if ((components != null) && components.ContainsKey(component.Name))
                throw new InvalidOperationException(
                    String.Format("El componente '{0}', ya pertenece a la placa.", component.Name));

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

            if ((components == null) || components.ContainsKey(component.Name))
                throw new InvalidOperationException(
                    String.Format("El componente '{0}' no esta asignado a esta placa.", component.Name));

            components.Remove(component.Name);
            if (components.Count == 0)
                components = null;
        }

        /// <summary>
        /// Obte un component pel seu nom.
        /// </summary>
        /// <param name="name">El nom del component.</param>
        /// <param name="throwOnError">True si cal generar una excepcio si no el troba.</param>
        /// <returns>El component, o null si no el troba.</returns>
        /// 
        public Component GetComponent(string name, bool throwOnError = true) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            if ((components != null) && components.TryGetValue(name, out var component))
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
                return components?.Values;
            }
        }
    }
}
