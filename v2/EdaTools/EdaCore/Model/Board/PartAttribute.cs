namespace MikroPic.EdaTools.v1.Core.Model.Board {

    using System;
    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Base.Geometry.Fonts;

    /// <summary>
    /// Clase que representa un atribut d'un component
    /// </summary>
    public sealed class PartAttribute : IBoardVisitable {

        private readonly string name;
        private string value;
        private Point position;
        private Angle rotation;
        private int height;
        private HorizontalTextAlign horizontalAlign;
        private VerticalTextAlign verticalAlign;
        private bool usePosition;
        private bool useRotation;
        private bool useHeight;
        private bool useAlign;
        private bool isVisible;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="name">Nom del atribut</param>
        /// <param name="value">Valor del atribut.</param>
        /// <param name="isVisible">Indica si es visible.</param>
        /// 
        public PartAttribute(string name, string value, bool isVisible = false) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
            this.value = value;
            IsVisible = isVisible;
            horizontalAlign = HorizontalTextAlign.Left;
            verticalAlign = VerticalTextAlign.Bottom;
            usePosition = false;
            useRotation = false;
            useHeight = false;
            useAlign = false;
        }

        public PartAttribute(string name, Point position, Angle rotation, int height,
            HorizontalTextAlign horizontalAlign, VerticalTextAlign verticalAlign, string value) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
            this.value = value;
            isVisible = true;

            this.position = position;
            usePosition = true;

            this.rotation = rotation;
            useRotation = true;

            this.height = height;
            useHeight = true;

            this.horizontalAlign = horizontalAlign;
            this.verticalAlign = verticalAlign;
            useAlign = true;
        }

        public PartAttribute Clone() {

            PartAttribute attribute = new PartAttribute(name, value, isVisible);
            attribute.position = position;
            attribute.usePosition = usePosition;
            attribute.rotation = rotation;
            attribute.useRotation = useRotation;
            attribute.height = height;
            attribute.useHeight = useHeight;
            attribute.horizontalAlign = horizontalAlign;
            attribute.verticalAlign = verticalAlign;
            attribute.useAlign = useAlign;

            return attribute;
        }

        public void AcceptVisitor(IBoardVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Indica si l'atribut es visible o no.
        /// </summary>
        /// 
        public bool IsVisible {
            get {
                return isVisible;
            }
            set {
                isVisible = value;
            }
        }

        /// <summary>
        /// Obte el nom del atribut.
        /// </summary>
        /// 
        public string Name {
            get {
                return name;
            }
        }

        /// <summary>
        /// Obte la posicio del atribut.
        /// </summary>
        /// 
        public Point Position {
            get {
                return position;
            }
            set {
                position = value;
                usePosition = true;
            }
        }

        /// <summary>
        /// Obte l'angle de rotacio de l'atribut.
        /// </summary>
        /// 
        public Angle Rotation {
            get {
                return rotation;
            }
            set {
                rotation = value;
                useRotation = true;
            }
        }

        /// <summary>
        /// Obte l'aliniacio del atribut.
        /// </summary>
        /// 
        public HorizontalTextAlign HorizontalAlign {
            get {
                return horizontalAlign;
            }
            set {
                horizontalAlign = value;
                useAlign = true;
            }
        }

        /// <summary>
        /// Obte l'aliniacio del atribut.
        /// </summary>
        /// 
        public VerticalTextAlign VerticalAlign {
            get {
                return verticalAlign;
            }
            set {
                verticalAlign = value;
                useAlign = true;
            }
        }

        /// <summary>
        /// Obte l'alçada del atribut.
        /// </summary>
        /// 
        public int Height {
            get {
                return height;
            }
            set {
                height = value;
                useHeight = true;
            }
        }

        /// <summary>
        /// Obte el valor de l'atribut.
        /// </summary>
        /// 
        public string Value {
            get {
                return value;
            }
            set {
                this.value = value;
            }
        }

        public bool UsePosition {
            get {
                return usePosition;
            }
        }

        public bool UseRotation {
            get {
                return useRotation;
            }
        }

        public bool UseHeight {
            get {
                return useHeight;
            }
        }

        public bool UseAlign {
            get {
                return useAlign;
            }
        }
    }
}