namespace MikroPic.EdaTools.v1.PanelEditor.DrawEditor.Tools {

    using System;
    using System.Windows;
    using System.Windows.Media;

    public sealed class SelectionTool : VisualTool {

        private readonly Brush brush;
        private readonly Pen pen;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="visualContainer">El contenidor visuals.</param>
        /// 
        public SelectionTool(IVisualContainer visualContainer) :
            base(visualContainer) {

            pen = new Pen {
                Brush = Brushes.Black,
                Thickness = 0.5,
                DashStyle = new DashStyle(new double[] { 8, 8 }, 0)
            };

            brush = new SolidColorBrush {
                Color = Colors.Azure,
                Opacity = 0.5
            };
        }

        /// <summary>
        /// Actualitza el pen per l'animacio del rectangle de seleccio
        /// </summary>
        /// 
        protected override void OnTick() {

            double offset = pen.DashStyle.Offset;

            offset += 2;
            if (offset == 16)
                offset = 0;

            pen.DashStyle.Offset = offset;

            base.OnTick();
        }

        /// <summary>
        /// Dibuixa el rectangle de seleccio.
        /// </summary>
        /// <param name="dc">Context de dibuix.</param>
        /// <param name="startPosition">Punt inicial.</param>
        /// <param name="endPosition">Punt final.</param>
        /// 
        protected override void OnDrawVisual(DrawingContext dc, Point startPosition, Point endPosition) {

            dc.DrawRectangle(
                brush,
                pen,
                new Rect(
                    new Point(startPosition.X + 0.5, startPosition.Y + 0.5),
                    new Point(endPosition.X + 0.5, endPosition.Y + 0.5)));

            base.OnDrawVisual(dc, startPosition, endPosition);
        }

        /// <summary>
        /// Obte el rectangle de selccio.
        /// </summary>
        /// 
        public Rect Selection {
            get {
                return new Rect(
                    Math.Min(StartPosition.X, EndPosition.X),
                    Math.Min(StartPosition.Y, EndPosition.Y),
                    Math.Abs(EndPosition.X - StartPosition.X),
                    Math.Abs(EndPosition.Y - StartPosition.Y));
            }
        }
    }
}
