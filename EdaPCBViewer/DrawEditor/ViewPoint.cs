namespace Eda.PCBViewer.DrawEditor {

    using System;
    using System.Windows;
    using System.Windows.Media;

    public delegate void ViewPointChangedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Clase que controla el punt de vista de la escena
    /// </summary>
    public sealed class ViewPoint {

        private Matrix m = new Matrix();

        public event ViewPointChangedEventHandler Changed;

        /// <summary>
        /// Notifica els canvis
        /// </summary>
        /// 
        private void NotifyChange() {

            if (Changed != null)
                Changed(this, new EventArgs());
        }

        /// <summary>
        /// Inicialitza el punt de vista.
        /// </summary>
        /// <param name="vSize">Tamany fisic del area de visualitzacio.</param>
        /// 
        public void Reset(Size vSize) {

            Reset(vSize, new Rect(0, 0, vSize.Width, vSize.Height));
        }

        /// <summary>
        /// Inicialitza el punt de vista.
        /// </summary>
        /// <param name="vSize">Tamany fisic del area de visualitzacio.</param>
        /// <param name="wRect">Rectangle a representar</param>
        /// 
        public void Reset(Size vSize, Rect wRect) {

            double vWidth = vSize.Width;
            double vHeight = vSize.Height;

            double wX = wRect.X;
            double wY = wRect.Y;
            double wWidth = wRect.Width;
            double wHeight = wRect.Height;

            // Ajusta la relacio d'aspecte del area de representacio
            // perque s'ajusti a l'area a representar.
            //
            double vAspect = vWidth / vHeight;
            double wAspect = wWidth / wHeight;

            if (vAspect < wAspect) 
                wHeight = wWidth / vAspect;

            else if (vAspect > wAspect) 
                wWidth = wHeight * vAspect;

            double scaleX = vWidth / wWidth;
            double scaleY = vHeight / wHeight;
            
            m = new Matrix();
            m.Translate(-wX, -wY);
            m.Scale(scaleX, scaleY);

            NotifyChange();
        }

        /// <summary>
        /// Desplaçament panoramic segons el eix X
        /// </summary>
        /// <param name="delta">Increment de la posicio X</param>
        /// 
        public void PanX(double delta) {

            Pan(delta, 0);
        }

        /// <summary>
        /// Desplaáment panoramic segins el eix Y.
        /// </summary>
        /// <param name="delta">Increment de la poisicio Y.</param>
        /// 
        public void PanY(double delta) {

            Pan(0, delta);
        }

        /// <summary>
        /// Desplaçamant panoramic de l'area de representacio
        /// </summary>
        /// <param name="deltaX">Increment de la posicio X</param>
        /// <param name="deltaY">Increment de la posicio Y</param>
        /// 
        public void Pan(double deltaX, double deltaY) {

            if (deltaX != 0 || deltaY != 0) {
                m.TranslatePrepend(deltaX, deltaY);
                NotifyChange();
            }
        }

        /// <summary>
        /// Amplia o redueix l'area de representacio.
        /// </summary>
        /// <param name="factor">Factor d'ampliacio o reduccio.</param>
        /// <param name="centerX">Centre d'ampliacio X.</param>
        /// <param name="centerY">Centre d'ampliacio Y</param>
        /// 
        public void Zoom(double factor, double centerX, double centerY) {

            if (factor != 0) {
                m.ScaleAtPrepend(factor, factor, centerX, centerY);
                NotifyChange();
            }
        }

        /// <summary>
        /// Zoom per acostar.
        /// </summary>
        /// <param name="z">Valor d'ampliacio.</param>
        /// <param name="centerX">Centre d'ampliacio X.</param>
        /// <param name="centerY">Centre d'ampliacio Y.</param>
        /// 
        public void ZoomIn(double z, double centerX, double centerY) {

            Zoom(1 + z, centerX, centerY);
        }

        /// <summary>
        /// Zoom per acostar.
        /// </summary>
        /// <param name="z">Valor d'ampliacio.</param>
        /// <param name="center">Centre d'ampliacio.</param>
        /// 
        public void ZoomIn(double z, Point center) {

            ZoomIn(z, center.X, center.Y);
        }

        /// <summary>
        /// Zoom per costar
        /// </summary>
        /// 
        public void ZoomIn(double z) {

            ZoomIn(z, 0, 0);
        }

        /// <summary>
        /// Zoom per allunyar.
        /// </summary>
        /// <param name="z">Valor de reduccio.</param>
        /// <param name="centerX">Centre de reduccio X.</param>
        /// <param name="centerY">Centre de reduccio Y.</param>
        /// 
        public void ZoomOut(double z, double centerX, double centerY) {

            Zoom(1 / (1 + z), centerX, centerY);
        }

        /// <summary>
        /// Zoom per allunyar.
        /// </summary>
        /// <param name="z">Valor de reduccio.</param>
        /// <param name="center">Centre de reduccio.</param>
        /// 
        public void ZoomOut(double z, Point center) {

            ZoomOut(z, center.X, center.Y);
        }

        /// <summary>
        /// Zoom per allunyar
        /// </summary>
        /// <param name="z">Valor de reduccio.</param>
        /// 
        public void ZoomOut(double z) {

            ZoomOut(z, 0, 0);
        }

        /// <summary>
        /// Transforma un punt a coordinades fisiques.
        /// </summary>
        /// <param name="p">El punt a transformar.</param>
        /// <returns>El resultat.</returns>
        /// 
        public Point TransformToView(Point p) {

            return m.Transform(p);
        }

        /// <summary>
        /// Transforma un rectangle a coordinades fisiques.
        /// </summary>
        /// <param name="r">El rectangle a convertir.</param>
        /// <returns>El resultat.</returns>
        /// 
        public Rect TransformToView(Rect r) {

            return new Rect(m.Transform(r.TopLeft), m.Transform(r.BottomRight));
        }

        /// <summary>
        /// Transforma un punt a coordinades mundials.
        /// </summary>
        /// <param name="p">El punt a convertir.</param>
        /// <returns>El resultat.</returns>
        /// 
        public Point TransformToWorld(Point p) {

            Matrix im = m;
            im.Invert();
            return im.Transform(p);
        }

        /// <summary>
        /// Transforma un rectangle a coordinades mundials.
        /// </summary>
        /// <param name="r">El rectangle a convertir.</param>
        /// <returns>El resultat.</returns>
        /// 
        public Rect TransformToWorld(Rect r) {

            Matrix im = m;
            im.Invert();
            return new Rect(im.Transform(r.TopLeft), im.Transform(r.BottomRight));
        }

        /// <summary>
        /// Obte la matriu de transformacio.
        /// </summary>
        /// 
        public Matrix Matrix {
            get {
                return m;
            }
        }
    }
}
