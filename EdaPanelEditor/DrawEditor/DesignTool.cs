namespace MikroPic.EdaTools.v1.PanelEditor.DrawEditor {

    using System;
    using System.Windows;
    using System.Windows.Input;

    public delegate void DesignToolActivationEventHandler(object sender);
    public delegate void DesignToolMouseEventHandler(object sender);

    public abstract class DesignTool {

        private enum State {
            Idle,
            Active,
            Dragging
        }

        private State state = State.Idle;
        private bool autoDeactivate = false;
        private double xSnap = 1;
        private double ySnap = 1;
        private Rect limits = new Rect(0, 0, Double.MaxValue, Double.MaxValue);
        private Point startPosition;
        private Point endPosition;

        public event DesignToolActivationEventHandler OnActivate;
        public event DesignToolActivationEventHandler OnDeactivate;
        public event DesignToolMouseEventHandler OnMouseUp;
        public event DesignToolMouseEventHandler OnMouseDown;
        public event DesignToolMouseEventHandler OnMouseMove;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// 
        public DesignTool() {

        }

        /// <summary>
        /// Comprova si el punt esta d'ins dels limita de l'eina.
        /// </summary>
        /// <param name="point">El punt a comprovar.</param>
        /// <returns>True si el punt es dins dels limits.</returns>
        /// 
        private bool InLimits(Point point) {

            return limits.Contains(point);
        }

        /// <summary>
        /// Ajusta un punt a la cuadricula.
        /// </summary>
        /// <param name="point">El punt a ajustar.</param>
        /// <returns>El punt ajustat a la cuadricula.</returns>
        /// 
        private Point Snap(Point point) {

            return new Point(
                Math.Round(point.X / xSnap) * xSnap,
                Math.Round(point.Y / ySnap) * ySnap);
        }

        /// <summary>
        /// Executa l'accio d'activacio de l'eina.
        /// </summary>
        /// 
        protected virtual void DoActivate() {

            OnActivate?.Invoke(this);
        }

        /// <summary>
        /// Executa l'accio de desactivacio de l'eina.
        /// </summary>
        /// 
        protected virtual void DoDeactivate() {

            OnDeactivate?.Invoke(this);
        }

        /// <summary>
        /// Executa l'accio corresponent al deixar anar el boto del mouse.
        /// </summary>
        /// 
        protected virtual void DoMouseUp() {

            OnMouseUp?.Invoke(this);
        }

        /// <summary>
        /// Executa l'accio corresponent a prema el boto del mouse.
        /// </summary>
        /// 
        protected virtual void DoMouseDown() {

            OnMouseDown?.Invoke(this);
        }

        /// <summary>
        /// Executa l'accio corresponent a mouse rl mouse.
        /// </summary>
        /// 
        protected virtual void DoMouseMove() {

            OnMouseMove?.Invoke(this);
        }

        /// <summary>
        /// Obte el cursor.
        /// </summary>
        /// <param name="position">La posicio del mouse.</param>
        /// <returns>El cursor corresponent.</returns>
        /// 
        protected virtual Cursor GetCursor(Point position) {

            return Cursors.Cross;
        }

        /// <summary>
        /// Activa l'eina.
        /// </summary>
        /// <returns>True si l'eina ha estat activada.</returns>
        /// 
        public bool Activate() {

            if (state == State.Idle) {
                state = State.Active;
                DoActivate();
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

            if (state == State.Active) {
                DoDeactivate();
                state = State.Idle;
            }
        }

        /// <summary>
        /// Procesa el moviment del mouse
        /// </summary>
        /// <param name="position">Posicio del mouse.</param>
        /// 
        public void MouseMove(Point position) {

            if (state == State.Dragging) {
               if (InLimits(position)) {
                    endPosition = Snap(position);
                    DoMouseMove();
                }
            }
        }

        /// <summary>
        /// Procesa accio de prema el boto del mouse.
        /// </summary>
        /// <param name="position">Posicio del mouse.</param>
        /// <returns>True si s'ha realitzat l'operacio.</returns>
        /// 
        public bool MouseButtonDown(Point position) {

            if (InLimits(position)) {
                if (state == State.Active) {
                    angle = 0;
                    startPosition = Snap(position);
                    endPosition = startPosition;
                    state = State.Dragging;

                    DoMouseDown();
                }

                return state == State.Dragging;
            }
            else
                return false;
        }

        /// <summary>
        /// Procesa l'accio de deixar anar el boto del mouse.
        /// </summary>
        /// <param name="position">Posicio del mouse.</param>
        /// 
        public void MouseButtonUp(Point position) {

            if (state == State.Dragging) {

                DoMouseUp();

                state = State.Active;

                if (autoDeactivate)
                    Deactivate();
            }
        }

        /// <summary>
        /// Indica si l'eina esta activa.
        /// </summary>
        /// 
        protected bool IsActive {
            get {
                return state != State.Idle;
            }
        }

        /// <summary>
        /// Obte o asigna els limits de l'eina.
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
        /// Obte o asigna el tamany de cuadricula en el eix X
        /// </summary>
        /// 
        public double XSnap {
            get {
                return xSnap;
            }
            set {
                xSnap = value;
            }
        }

        /// <summary>
        /// Obte o asigna el tamany de cuadricula en el eix Y
        /// </summary>
        /// 
        public double YSnap {
            get {
                return ySnap;
            }
            set {
                ySnap = value;
            }
        }

        /// <summary>
        /// Obte la posicio inicial.
        /// </summary>
        /// 
        public Point StartPosition {
            get {
                return startPosition;
            }
        }

        /// <summary>
        /// Obte la posicio final.
        /// </summary>
        /// 
        public Point EndPosition {
            get {
                return endPosition;
            }
        }
    }
}
