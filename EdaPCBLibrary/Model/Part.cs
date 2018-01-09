namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Pcb.Geometry;
    using System;
    using System.Linq;
    using System.Windows;
    using System.Collections.Generic;
    using System.Windows.Media;

    public sealed class Part: IPosition, IRotation, IName {

        private string name;
        private Point position;
        private Angle rotation;
        private bool isFlipped;
        private Component component;
        private Dictionary<string, Parameter> parameters;

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
        public Angle Rotation {
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
                m.RotateAt(rotation.Degrees, position.X, position.Y);
                return m;
            }
        }

        /// <summary>
        /// Obte o asigna el component.
        /// </summary>
        /// 
        public Component Component {
            get {
                return component;
            }
            set {
                component = value;
            }
        }

        /// <summary>
        /// Obte la llista de pads.
        /// </summary>
        /// 
        public IEnumerable<Pad> Pads {
            get {
                List<Pad> pads = new List<Pad>();
                foreach (IConected element in component.Elements.OfType<IConected>())
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
