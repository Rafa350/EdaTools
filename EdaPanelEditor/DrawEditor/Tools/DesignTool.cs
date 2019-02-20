namespace MikroPic.EdaTools.v1.PanelEditor.DrawEditor.Tools {

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

        public event DesignToolActivationEventHandler Active;
        public event DesignToolActivationEventHandler Deactive;
        public event DesignToolMouseEventHandler MouseUp;
        public event DesignToolMouseEventHandler MouseDown;
        public event DesignToolMouseEventHandler MouseMove;

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
        protected virtual void OnActivate() {

            Active?.Invoke(this);
        }

        /// <summary>
        /// Executa l'accio de desactivacio de l'eina.
        /// </summary>
        /// 
        protected virtual void OnDeactivate() {

            Deactive?.Invoke(this);
        }

        /// <summary>
        /// Executa l'accio corresponent al deixar anar el boto del mouse.
        /// </summary>
        /// 
        protected virtual void OnMouseUp() {

            MouseUp?.Invoke(this);
        }

        /// <summary>
        /// Executa l'accio corresponent a prema el boto del mouse.
        /// </summary>
        /// 
        protected virtual void OnMouseDown() {

            MouseDown?.Invoke(this);
        }

        /// <summary>
        /// Executa l'accio corresponent a mouse el mouse.
        /// </summary>
        /// 
        protected virtual void OnMouseMove() {

            MouseMove?.Invoke(this);
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

            if (state == State.Active) {
                OnDeactivate();
                state = State.Idle;
            }
        }

        /// <summary>
        /// Notifica a l'eina que s'ha mogut el mouse
        /// </summary>
        /// <param name="position">Posicio del mouse.</param>
        /// 
        public void NotifyMouseMove(Point position) {

            if (state == State.Dragging) {
               if (InLimits(position)) {
                    endPosition = Snap(position);
                    OnMouseMove();
                }
            }
        }

        /// <summary>
        /// Notifica a l'eina que s'ha premut un boto del mouse.
        /// </summary>
        /// <param name="position">Posicio del mouse.</param>
        /// <returns>True si s'ha realitzat l'operacio.</returns>
        /// 
        public bool NotifyMouseDown(Point position) {

            if (InLimits(position)) {
                if (state == State.Active) {
                    startPosition = Snap(position);
                    endPosition = startPosition;
                    state = State.Dragging;

                    OnMouseDown();
                }

                return state == State.Dragging;
            }
            else
                return false;
        }

        /// <summary>
        /// Notifica a l'eina que s'ha deixat anar el boto de l'eina.
        /// </summary>
        /// <param name="position">Posicio del mouse.</param>
        /// 
        public void NotifyMouseUp(Point position) {

            if (state == State.Dragging) {

                OnMouseUp();

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
        /// Obte o asigna la autodesactivacio de l'eina.
        /// </summary>
        /// 
        public bool AutoDeactivate {
            get {
                return autoDeactivate;
            }
            set {
                autoDeactivate = value;
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
