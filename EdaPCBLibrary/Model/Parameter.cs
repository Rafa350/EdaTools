namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;
    using System.Windows;

    public sealed class Parameter: IVisitable {

        private string name;
        private string value;
        private Point position;
        private double rotate;
        private bool isVisible;
        // private bool usePosition

        public Parameter() {
        }

        public Parameter(string name, string value) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
            this.value = value;
        }

        public Parameter(string name, Point position, double rotate, bool isVisible, string value) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
            this.position = position;
            this.rotate = rotate;
            this.isVisible = isVisible;
            this.value = value;
        }

        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        public Point Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }

        public double Rotate {
            get {
                return rotate;
            }
            set {
                rotate = value;
            }
        }

        public bool IsVisible {
            get {
                return isVisible;
            }
            set {
                isVisible = value;
            }
        }

        public string Value {
            get {
                return value;
            }
            set {
                this.value = value;
            }
        }
    }
}
