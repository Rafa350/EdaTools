namespace MikroPic.EdaTools.v1.Pcb.Model {

    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    public sealed class Part: IPosition, IRotation, IName {

        private string name;
        private Point position;
        private double rotation;
        private bool isFlipped;
        private readonly Block block;
        private readonly List<Pad> pads = new List<Pad>();
        private Dictionary<string, Parameter> parameters;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="block">El bloc associat.</param>
        /// 
        public Part(Block block) {

            if (block == null)
                throw new ArgumentNullException("block");

            this.block = block;

            UpdateItemList();
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="name">El nom.</param>
        /// <param name="position">Posicio.</param>
        /// <param name="rotation">Angle de rotacio</param>
        /// <param name="isFlipped">Indica si va girat.</param>
        /// <param name="block">El component associat.</param>
        /// 
        public Part(string name, Point position, double rotation, bool isFlipped, Block block) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (block == null)
                throw new ArgumentNullException("block");

            this.name = name;
            this.position = position;
            this.rotation = rotation;
            this.isFlipped = isFlipped;
            this.block = block;

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

        public Pad GetPad(string name, bool throwOnError = false) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            foreach (Pad pad in pads)
                if (pad.Name == name)
                    return pad;

            if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("El pad '{0}', no se encontro en el componente '{1}'.", name, this.name));

            return null;
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

            foreach (Element element in block.Elements) {
                if (element is SmdPadElement) {
                    SmdPadElement item = (SmdPadElement)element;
                    pads.Add(new Pad(item.Name, item));
                }
                else if (element is ThPadElement) {
                    ThPadElement item = (ThPadElement)element;
                    pads.Add(new Pad(item.Name, item));
                }
            }
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
        /// Obte el block associat.
        /// </summary>
        /// 
        public Block Block {
            get {
                return block;
            }
        }

        /// <summary>
        /// Obte la llista de pads.
        /// </summary>
        /// 
        public IEnumerable<Pad> Pads {
            get {
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
