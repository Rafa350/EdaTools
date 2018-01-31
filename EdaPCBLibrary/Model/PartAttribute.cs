namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;
    using System.Windows;

    public sealed class PartAttribute: IVisitable {

        private readonly string name;
        private string value;
        private Point position;
        private double rotation;
        private bool isVisible;

        public PartAttribute(string name, string value) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
            this.value = value;
        }

        public PartAttribute(string name, Point position, double rotation, bool isVisible, string value) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
            this.position = position;
            this.rotation = rotation;
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
        }

        public Point Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }

        public double Rotation {
            get {
                return rotation;
            }
            set {
                rotation = value;
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
