namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;
    using System.Linq;
    using System.Windows;
    using System.Collections.Generic;
    using System.Windows.Media;

    public sealed class Part: IPosition, IRotation, IName {

        private string name;
        private Point position;
        private double rotation;
        private bool isFlipped;
        private readonly Block component;
        private Dictionary<string, Parameter> parameters;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="component">El component associat.</param>
        /// 
        public Part(Block component) {

            this.component = component;

            UpdateItemList();
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">El nom.</param>
        /// <param name="position">Posicio.</param>
        /// <param name="rotation">Angle de rotacio</param>
        /// <param name="isFlipped">Indica si va girat.</param>
        /// <param name="component">El component associat.</param>
        /// 
        public Part(string name, Point position, double rotation, bool isFlipped, Block component) {

            this.name = name;
            this.position = position;
            this.rotation = rotation;
            this.isFlipped = isFlipped;
            this.component = component;

            UpdateItemList();
        }

        /// <summary>
        /// Accepta un visitador.
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        public void AddParameter(Parameter parameter) {

            if (parameter == null)
                throw new ArgumentNullException("parameter");

            if (parameters == null)
                parameters = new Dictionary<string, Parameter>();

            parameters.Add(parameter.Name, parameter);
        }

        public void RemoveParameter(Parameter parameter) {

            if (parameter == null)
                throw new ArgumentNullException("parameter");

            parameters.Remove(parameter.Name);
            if (parameters.Count == 0)
                parameters = null;
        }

        public Parameter GetParameter(string name) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if ((parameters != null) && (parameters.ContainsKey(name)))
                return parameters[name];
            else
                return null;
        }

        private void UpdateItemList() {

        }

        /// <summary>
        /// Obte o asigna el nom.
        /// </summary>
        /// 
        public string Name {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        /// <summary>
        /// Obte o asigna la posicio.
        /// </summary>
        /// 
        public Point Position {
            get {
                return position;
            }
            set {
                position = value;
            }
        }

        /// <summary>
        /// Obte o asigna l'angle de rotacio.
        /// </summary>
        /// 
        public double Rotation {
            get {
                return rotation;
            }
            set {
                rotation = value;
            }
        }

        public bool IsFlipped {
            get {
                return isFlipped;
            }
            set {
                isFlipped = value;
            }
        }

        /// <summary>
        /// Obte la matriu de transformacio per aplicar als elements.
        /// </summary>
        /// 
        public Matrix Transformation {
            get {
                Matrix m = new Matrix();
                m.Translate(position.X, position.Y);
                m.RotateAt(rotation, position.X, position.Y);
                return m;
            }
        }

        /// <summary>
        /// Obte o asigna el component.
        /// </summary>
        /// 
        public Block Component {
            get {
                return component;
            }
        }

        /// <summary>
        /// Obte la llista de pads.
        /// </summary>
        /// 
        public IEnumerable<Pad> Pads {
            get {
                List<Pad> pads = new List<Pad>();
                foreach (IConectable element in component.Elements.OfType<IConectable>())
                    pads.Add(new Pad(element));
                return pads;
            }

        }

        /// <summary>
        /// Obte la llista de parametres.
        /// </summary>
        /// 
        public IEnumerable<Parameter> Parameters {
            get {
                return parameters == null ? null : parameters.Values;
            }
        }
    }
}
