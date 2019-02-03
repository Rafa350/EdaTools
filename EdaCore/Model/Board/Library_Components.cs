namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using System;
    using System.Collections.Generic;

    public sealed partial class Library {

        private Dictionary<string, Component> components;
        
        public void AddComponent(Component component) {
            
            if (component == null)
                throw new ArgumentNullException("component");
        }

        /// <summary>
        /// Indica si la bibliotecxa conte components.
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
            get{
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
