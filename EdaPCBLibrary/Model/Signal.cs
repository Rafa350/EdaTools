namespace MikroPic.EdaTools.v1.Pcb.Model {
    
    using System;

    public sealed class Signal: IName, IVisitable {

        private string name;
        private int clearance;

        public Signal() {

        }

        public Signal(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
        }

        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Obte o asigna el nom de la senyal.
        /// </summary>
        /// 
        public string Name {
            get {
                return name;
            }
            set {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("Name");

                name = value;
            }
        }

        public int Clearance {
            get {
                return clearance;
            }
            set {
                if (clearance < 0)
                    throw new ArgumentOutOfRangeException("Clearance");
                clearance = value;
            }
        }
    }
}
