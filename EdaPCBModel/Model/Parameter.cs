namespace MikroPic.EdaTools.v1.Model {

    using System;

    public sealed class Parameter: IVisitable {

        private string name;
        private double x;
        private double y;
        private double rotate;
        private string value;
        private bool isVisible;

        public Parameter() {
        }

        public Parameter(string name, string value) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
            this.value = value;
        }

        public Parameter(string name, double x, double y, double rotate, bool isVisible, string value) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
            this.x = x;
            this.y = y;
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

        public double X {
            get {
                return x;
            }
            set {
                x = value;
            }
        }

        public double Y {
            get {
                return y;
            }
            set {
                y = value;
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
