namespace Eda.PCBViewer.DrawEditor {

    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Media;

    public delegate void DesignToolActivationEventHandler(object sender);
    public delegate void DesignToolMouseEventHandler(object sender);

    public abstract class DesignTool {

        private enum State {
            Idle,
            Active,
            Dragging
        }

        private readonly DesignSurface surface;
        private State state = State.Idle;
        private bool autoDeactivate = false;
        private DrawingVisual visual;
        private double xSnap = 1;
        private double ySnap = 1;
        private double aSnap = 0;
        private Cursor oldCursor = Cursors.Arrow;
        private Rect limits;
        private Point startPosition;
        private Point endPosition;
        private double angle;

        public event DesignToolActivationEventHandler OnActivate;
        public event DesignToolActivationEventHandler OnDeactivate;
        public event DesignToolMouseEventHandler OnMouseUp;
        public event DesignToolMouseEventHandler OnMouseDown;
        public event DesignToolMouseEventHandler OnMouseMove;

        public DesignTool(DesignSurface surface) {

            if (surface == null)
                throw new ArgumentNullException("surface");

            this.surface = surface;
            this.limits = new Rect(0, 0, Double.MaxValue, Double.MaxValue);
        }

        private bool InLimits(Point point) {

            return limits.Contains(point);
        }

        private Point Snap(Point point) {

            return new Point(
                Math.Round(point.X / xSnap) * xSnap,
                Math.Round(point.Y / ySnap) * ySnap);
        }

        private void DoActivate() {

            if (OnActivate != null)
                OnActivate(this);
        }

        private void DoDeactivate() {

            if (OnActivate != null)
                OnDeactivate(this);
        }

        private void DoMouseUp() {

            if (OnMouseUp != null)
                OnMouseUp(this);
        }

        private void DoMouseDown() {

            if (OnMouseDown != null)
                OnMouseDown(this);
        }

        private void DoMouseMove() {

            if (OnMouseMove != null)
                OnMouseMove(this);
        }

        protected virtual Cursor GetCursor(Point position) {

            return Cursors.Cross;
        }

        protected abstract void RenderBox(DrawingContext dc, Point startPosition, Point endPosition);

        private void ShowObject() {

            visual = new DrawingVisual();
            surface.AddVisual(visual);
        }

        private void HideObject() {

            surface.RemoveVisual(visual);
        }

        private void RenderObject() {

            using (DrawingContext dc = visual.RenderOpen())
                RenderBox(dc, startPosition, endPosition);
        }

        public virtual bool Activate() {

            if (state == State.Idle) {
                oldCursor = surface.Cursor;
                state = State.Active;
                DoActivate();
                return true;
            }
            else
                return false;
        }

        public virtual void Deactivate() {

            if (state == State.Active) {
                DoDeactivate();
                surface.Cursor = oldCursor;
                state = State.Idle;
            }
        }

        public virtual void MouseMove(Point position) {

            if (state == State.Dragging) {
               if (InLimits(position)) {
                    endPosition = Snap(position);
                    RenderObject();
                    DoMouseMove();
                }
            }

            if (state != State.Idle) {
                if (InLimits(position))
                    surface.Cursor = GetCursor(position);
                else
                    surface.Cursor = Cursors.No;
            }
        }

        public virtual bool MouseButtonDown(Point position) {

            if (InLimits(position)) {
                if (state == State.Active) {
                    angle = 0;
                    startPosition = Snap(position);
                    endPosition = startPosition;
                    ShowObject();
                    RenderObject();
                    surface.CaptureMouse();
                    state = State.Dragging;

                    DoMouseDown();
                }

                return state == State.Dragging;
            }
            else
                return false;
        }

        public virtual void MouseButtonUp(Point position) {

            if (state == State.Dragging) {

                DoMouseUp();

                HideObject();
                surface.ReleaseMouseCapture();
                state = State.Active;

                if (autoDeactivate)
                    Deactivate();
            }
        }

        protected DesignSurface Surface {
            get {
                return surface;
            }
        }

        protected bool IsActive {
            get {
                return state != State.Idle;
            }
        }

        public Rect Limits {
            get {
                return limits;
            }
            set {
                limits = value;
            }
        }

        public double XSnap {
            get {
                return xSnap;
            }
            set {
                xSnap = value;
            }
        }

        public double YSnap {
            get {
                return ySnap;
            }
            set {
                ySnap = value;
            }
        }

        public double ASnap {
            get {
                return aSnap;
            }
            set {
                aSnap = value;
            }
        }

        public Point StartPosition {
            get {
                return startPosition;
            }
        }

        public Point EndPosition {
            get {
                return endPosition;
            }
        }

        public double Angle {
            get {
                return angle;
            }
        }
    }
}
