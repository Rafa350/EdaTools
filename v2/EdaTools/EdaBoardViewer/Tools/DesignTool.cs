namespace EdaBoardViewer.Tools {

    using Avalonia;
    using System;

    /// <summary>
    /// Clase que representa un eina de dibuix
    /// </summary>
    /// 
    public class DesignTool {

        public class PointerEventArgs: EventArgs {

            private readonly Point position;

            public PointerEventArgs(Point popsition) {

                this.position = popsition;
            }

            public Point Position => position;
        }

        public delegate void ActivationEventHandler(object sender);
        public delegate void PointerEventHandler(object sender, PointerEventArgs e);

        public enum ToolState {
            Idle,    // Inactiva
            Active,  // Activa
            Dragging // Activa amb el boto premut
        }

        private ToolState state = ToolState.Idle;
        private bool autoDeactivate = false;
        private double xSnap = 1;
        private double ySnap = 1;
        private Rect limits;
        private Point currentPosition;
        private Point startPosition;
        private Point endPosition;

        public event ActivationEventHandler Activated;
        public event ActivationEventHandler Deactivated;
        public event PointerEventHandler PointerPressed;
        public event PointerEventHandler PointerReleased;
        public event PointerEventHandler PointerMoved;

        /// <summary>
        /// Constructor de l'objecte
        /// </summary>
        /// 
        public DesignTool() {

            limits = new Rect(new Point(Double.MinValue, Double.MinValue), new Point(Double.MaxValue, Double.MaxValue));
        }

        /// <summary>
        /// Comprova si un punt es d'ins dels limits establerts.
        /// </summary>
        /// <param name="p">El punt a verificar.</param>
        /// <returns>True si es d'ins dels limits.</returns>
        /// 
        private bool InLimits(Point p) {

            return limits.Contains(p);
        }

        /// <summary>
        /// Comprova el punt s'ha mogut respecte la posicio actual.
        /// </summary>
        /// <param name="p">La nova posicio.</param>
        /// <returns>True si s'ha mogut.</returns>
        /// 
        private bool HasMoved(Point p) {

            return currentPosition != p;
        }

        /// <summary>
        /// Ajusta les coordinades d'un punt.
        /// </summary>
        /// <param name="p">El punt.</param>
        /// <returns>El punt amb les coordinades ajustades.</returns>
        /// 
        private Point Snap(Point p) {

            return new Point (
                Math.Round(p.X / xSnap) * xSnap,
                Math.Round(p.Y / xSnap) * ySnap);
        }

        /// <summary>
        /// Procesa l'activacio de l'eina.
        /// </summary>
        /// 
        protected virtual void OnActivate() {

            Activated?.Invoke(this);
        }

        /// <summary>
        ///  Procesa la desactivacio de l'eina
        /// </summary>
        /// 
        protected virtual void OnDeactivate() {

            Deactivated?.Invoke(this);
        }

        /// <summary>
        /// Procesa quant es deixa anar el boto.
        /// </summary>
        /// 
        protected virtual void OnPointerReleased(Point position) {

            PointerReleased?.Invoke(this, new PointerEventArgs(position));
        }

        /// <summary>
        /// Procesa quant es prem el boto.
        /// </summary>
        /// 
        protected virtual void OnPointerPressed(Point position) {

            PointerPressed?.Invoke(this, new PointerEventArgs(position));
        }

        /// <summary>
        /// Procesa quant es mou el punter.
        /// </summary>
        /// 
        protected virtual void OnPointerMoved(Point position) {

            PointerMoved?.Invoke(this, new PointerEventArgs(position));
        }

        /// <summary>
        /// Activa l'eina.
        /// </summary>
        /// <returns>True si l'operacio de exit.</returns>
        /// 
        public bool Activate() {

            if (state == ToolState.Idle) {
                state = ToolState.Active;
                OnActivate();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Desactiva l'eina.
        /// </summary>
        /// 
        public void Deactivate() {

            if (state == ToolState.Active) {
                OnDeactivate();
                state = ToolState.Idle;
            }
        }

        /// <summary>
        /// Procesa els moviments del mouse.
        /// </summary>
        /// <param name="position">Posicio del mouse.</param>
        /// 
        public void Move(Point position) {

            position = Snap(position);

            if (InLimits(position) && HasMoved(position)) {

                currentPosition = position;

                if (state == ToolState.Dragging) {
                    endPosition = position;
                    OnPointerMoved(position);
                }
            }
        }

        /// <summary>
        /// Procesa el oment quant es prem el boto del mouse.
        /// </summary>
        /// <param name="position">Posicio del mouse.</param>
        /// <returns>True si s'ha activat l'eina correctament.</returns>
        /// 
        public bool Press(Point position) {

            position = Snap(position);

            if (InLimits(position)) {

                currentPosition = position;

                if (state == ToolState.Active) {
                    startPosition = position;
                    endPosition = position;
                    state = ToolState.Dragging;
                    OnPointerPressed(position);
                }

                return state == ToolState.Dragging;
            }
            else
                return false;
        }

        /// <summary>
        /// Procesa el moment en que es deixa anar el boto del mouse.
        /// </summary>
        /// <param name="position">Posicio del mouse.</param>
        /// 
        public virtual void Release(Point position) {

            if (state == ToolState.Dragging) {

                OnPointerReleased(position);

                state = ToolState.Active;

                if (autoDeactivate)
                    Deactivate();
            }
        }

        /// <summary>
        /// Comprova si l'eina es activa.
        /// </summary>
        /// 
        public bool IsActive =>
            state != ToolState.Idle;

        /// <summary>
        /// Obte l'estat de l'eina.
        /// </summary>
        /// 
        public ToolState State => 
            state;

        /// <summary>
        /// Obte o asigna el rectangle dels limits.
        /// </summary>
        /// 
        public Rect Limits {
            get {
                return limits;
            }
            set {
                limits = value;
            }
        }

        /// <summary>
        /// Obte el factor d'arrodonoment de la coordinada X.
        /// </summary>
        /// 
        public double XSnap {
            get {
                return xSnap;
            }
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("XSnap");
                xSnap = value;
            }
        }

        /// <summary>
        /// Obte el factor d'arrodoniment de la coordinada Y.
        /// </summary>
        /// 
        public double YSnap {
            get {
                return ySnap;
            }
            set {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("YSnap");
                ySnap = value;
            }
        }

        /// <summary>
        /// Obte la posicio actual.
        /// </summary>
        /// 
        public Point CurrentPosition => currentPosition;

        /// <summary>
        /// Obte la posicio inicial. La posicio del mouse quant es prem el boto.
        /// </summary>
        /// 
        public Point StartPosition => startPosition;

        /// <summary>
        /// Obte la posicio final. La posicio del mouse quant s'allibera el boto.
        /// </summary>
        /// 
        public Point EndPosition => endPosition;
    }
}
