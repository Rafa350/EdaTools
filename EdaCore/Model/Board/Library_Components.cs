namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using System.Collections.Generic;

    public sealed partial class Library {

        private List<Component> components;

        public bool HasComponents {
            get {
                return components != null;
            }
        }

        public IEnumerable<Component> Components {
            get {
                return components;
            }
        }
    }
}
