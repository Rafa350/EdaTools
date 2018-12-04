namespace MikroPic.EdaTools.v1.Core.Model.Net {

    using System;
    using System.Collections.Generic;

    public sealed partial class Net {

        private Dictionary<string, NetComponent> components;

        public void AddComponent(NetComponent component) {

            if (component == null)
                throw new ArgumentNullException("element");

            if ((components == null) || components.ContainsKey(component.Name))
                throw new InvalidOperationException(
                    String.Format("El componente '{0}' ya pertenece a la lista de componentes.", component.Name));

            if (components == null)
                components = new Dictionary<string, NetComponent>();
            components.Add(component.Name, component);
        }

        public void AddComponents(IEnumerable<NetComponent> components) {

            if (components == null)
                throw new ArgumentNullException("components");

            foreach (var component in components)
                AddComponent(component);
        }

        public void RemoveComponent(NetComponent component) {

            if (component == null)
                throw new ArgumentNullException("component");

            if ((components == null) || !components.ContainsKey(component.Name))
                throw new InvalidOperationException(
                    String.Format("El componente '{0}', no pertecece a la lista de componentes.", component.Name));

            components.Remove(component.Name);
            if (components.Count == 0)
                components = null;
        }

        public NetComponent GetComponent(string name, bool throwOnError = true) {

            if ((components != null) && components.TryGetValue(name, out var component))
                return component;

            else if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("No se encontro el componente '{0}'.", name));
            else
                return null;
        }

        public bool HasComponents {
            get {
                return components != null;
            }
        }

        public IEnumerable<string> ComponentNames {
            get {
                return components?.Keys;
            }
        }

        public IEnumerable<NetComponent> Components {
            get {
                return components?.Values;
            }
        }
    }
}
