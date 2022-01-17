namespace EdaBoardViewer.Views.Controls {

    using Avalonia;
    using System;

    public delegate void ViewPointChangedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Clase que controla el punt de vista de l'escena.
    /// </summary>
    /// 
    public sealed class ViewPoint {

        private Matrix m = Matrix.Identity;
        private Matrix im = Matrix.Identity;
        private bool notifyEnabled = true;
        private bool notifyPending = false;

        public event ViewPointChangedEventHandler Changed;

        /// <summary>
        /// Notifica els canvis
        /// </summary>
        /// 
        private void NotifyChange() {

            if (notifyEnabled) {
                Changed?.Invoke(this, new EventArgs());
                notifyPending = false;
            }
            else
                notifyPending = true;
        }

        /// <summary>
        /// Inicialitza el punt de vista.
        /// </summary>
        /// <param name="viewport">Tamany fisic del area de visualitzacio.</param>
        /// 
        public void Reset(Size viewport) {

            Reset(viewport, new Rect(0, 0, viewport.Width, viewport.Height));
        }

        /// <summary>
        /// Inicialitza el punt de vista.
        /// </summary>
        /// <param name="viewport">Tamany fisic del area de visualitzacio.</param>
        /// <param name="window">Rectangle a representar.</param>
        /// 
        public void Reset(Size viewport, Rect window) {

            Reset(viewport, window, Matrix.Identity);
        }

        /// <summary>
        /// Inicialitza el punt de vista.
        /// </summary>
        /// <param name="viewport">Tamany fisic del area de visualitzacio.</param>
        /// <param name="window">Rectangle a representar.</param>
        /// <param name="matrix">Matriu de transformacio.</param>
        /// 
        public void Reset(Size viewport, Rect window, Matrix matrix) {

            // Ajusta la relacio d'aspecte del area de representacio
            // perque s'ajusti a l'area a representar.
            //
            double viewportAspect = viewport.Width / viewport.Height;
            double windowAspect = window.Width / window.Height;

            double width = window.Width;
            double height = window.Height;
            if (viewportAspect < windowAspect)
                height = window.Width / viewportAspect;
            else if (viewportAspect > windowAspect)
                width = window.Height * viewportAspect;

            double offsetX = (width - window.Width) / 2;
            double offsetY = (height - window.Height) / 2;

            double scaleX = viewport.Width / width;
            double scaleY = viewport.Height / height;

            // Inicialitza la matriu.
            //
            m = matrix *
                Matrix.CreateTranslation(offsetX, offsetY) *
                Matrix.CreateScale(scaleX, scaleY);
            im = m.Invert();

            // Notifica els canvis.
            //
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

            if ((deltaX != 0) || (deltaY != 0)) {

                m = Matrix.CreateTranslation(deltaX, deltaY) * m;
                im = m.Invert();

                NotifyChange();
            }
        }

        /// <summary>
        /// Desplaçament panoramic segons un vector.
        /// </summary>
        /// <param name="delta">El vector de desplaçament.</param>
        /// 
        public void Pan(Vector delta) {

            Pan(delta.X, delta.Y);
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

                double tx = centerX - (factor * centerX);
                double ty = centerY - (factor * centerY);
                m = new Matrix(factor, 0, 0, factor, tx, ty) * m;
                im = m.Invert();

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
        /// Zoom per acostar
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
        /// Rotacio
        /// </summary>
        /// <param name="angle">L'angle de rotacio.</param>
        /// 
        public void Rotate(double angle) {

            if (angle != 0) {

                m = Matrix.CreateRotation(Matrix.ToRadians(angle)) * m;
                im = m.Invert();

                NotifyChange();
            }
        }

        /// <summary>
        /// Rotacio
        /// </summary>
        /// <param name="angle">L'angle de rotacio en graus.</param>
        /// <param name="centerX">Coordinada X del center de rotacio.</param>
        /// <param name="centerY">Coordinada Y del centre de rotacio.</param>
        /// 
        public void Rotate(double angle, double centerX, double centerY) {

            if (angle != 0) {

                double r = angle * Math.PI / 180;
                double sin = Math.Sin(r);
                double cos = Math.Cos(r);
                double tx = centerX - (centerX * cos) + (centerY * sin);
                double ty = centerY - (centerX * sin) - (centerY * cos);
                m = new Matrix(cos, sin, -sin, cos, tx, ty) * m;
                im = m.Invert();

                NotifyChange();
            }
        }

        /// <summary>
        /// Rotacio.
        /// </summary>
        /// <param name="angle">Angle de rotacio.</param>
        /// <param name="center">Centre de rotacio.</param>
        /// 
        public void Rotate(double angle, Point center) {

            Rotate(angle, center.X, center.Y);
        }

        /// <summary>
        /// Transforma un punt a coordinades fisiques.
        /// </summary>
        /// <param name="p">El punt a transformar.</param>
        /// <returns>El resultat.</returns>
        /// 
        public Point TransformToView(Point p) {

            return new Point(
                (p.X * m.M11) + (p.Y * m.M21) + m.M31,
                (p.X * m.M12) + (p.Y * m.M22) + m.M32);
        }

        /// <summary>
        /// Transforma un punt a coordinades mundials.
        /// </summary>
        /// <param name="p">El punt a convertir.</param>
        /// <returns>El resultat.</returns>
        /// 
        public Point TransformToWorld(Point p) {

            return new Point(
                (p.X * im.M11) + (p.Y * im.M21) + im.M31,
                (p.X * im.M12) + (p.Y * im.M22) + im.M32);
        }

        /// <summary>
        /// Obte la matriu de transformacio.
        /// </summary>
        /// 
        public Matrix Matrix => m;

        /// <summary>
        /// Autoritza o desautoritza la notificacio de canvis.
        /// </summary>
        /// <remarks>Un cop autoritzat, si hi han canvis pendents, es notifiquen.</remarks>
        /// 
        public bool NotifyEnabled {
            get {
                return notifyEnabled;
            }
            set {
                notifyEnabled = value;
                if (notifyEnabled && notifyPending)
                    NotifyChange();
            }
        }
    }
}