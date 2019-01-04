namespace MikroPic.EdaTools.v1.BoardEditor.Render.WPF.Visuals {

    using MikroPic.EdaTools.v1.Base.WPF;
    using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using System.Windows.Media;

    using Color = MikroPic.EdaTools.v1.Base.Geometry.Color;

    public sealed class ThPadVisual : ElementVisual {

        private readonly Layer layer;
        private readonly Color color;

        /// <summary>
        /// Constructor de l'objecte
        /// </summary>
        /// <param name="parent">La visual pare.</param>
        /// <param name="element">El element</param>
        /// <param name="layer">La capa en la que es representa.</param>
        /// <param name="color">El color amb el que es representa.</param>
        /// 
        public ThPadVisual(DrawingVisual parent, ThPadElement element, Layer layer, Color color) :
            base(parent, element) {

            this.layer = layer;
            this.color = color;
        }

        /// <summary>
        /// Dibuixa la representacio del element.
        /// </summary>
        /// <param name="dc">El contexte de representacio.</param>
        /// 
        protected override void Draw(DrawVisualContext dc) {

            DrawShape(dc);
            DrawOutline(dc);
        }

        /// <summary>
        /// Dibuixa la representacio de la forma del element.
        /// </summary>
        /// <param name="dc">El context de representacio.</param>
        /// 
        private void DrawShape(DrawVisualContext dc) {

            if (Element.Shape == ThPadElement.ThPadShape.Circle) {

                int size =
                    layer.Id.Side == BoardSide.Top ? Element.TopSize :
                    layer.Id.Side == BoardSide.Bottom ? Element.BottomSize :
                    Element.InnerSize;

                int thickness = (size - Element.Drill) / 2;
                int radius = (size - thickness / 2) / 2;

                Pen pen = dc.GetPen(color, thickness, PenLineCap.Flat);
                dc.DrawEllipse(Brushes.Black, pen, Element.Position, radius, radius);
            }

            else {

                Polygon polygon = Element.GetPolygon(layer.Id.Side);

                Brush brush = dc.GetBrush(color);
                dc.DrawPolygon(brush, null, polygon);
                dc.DrawEllipse(Brushes.Black, null, Element.Position, Element.Drill / 2, Element.Drill / 2);
            }

            /*dc.PushTransform(new ScaleTransform(1, -1, pad.Position.X, pad.Position.Y));

            Brush textBrush = GetBrush(Colors.Yellow);
            FormattedText formattedText = new FormattedText(
                pad.Name, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
                new Typeface("Arial"), 50, textBrush);
            formattedText.TextAlignment = TextAlignment.Center;

            WinPoint textPosition = new WinPoint(pad.Position.X, pad.Position.Y);
            dc.DrawText(formattedText, new WinPoint(textPosition.X, textPosition.Y - formattedText.Height / 2));

            dc.Pop();
            */
        }

        /// <summary>
        /// Dibuixa la representacio del perfil exterior del element.
        /// </summary>
        /// <param name="dc">El context de representacio.</param>
        /// 
        private void DrawOutline(DrawVisualContext dc) {

            Polygon p = Element.GetOutlinePolygon(layer.Side, 150000);
            Pen pen = dc.GetPen(new Color(0, 255, 255), 10000, PenLineCap.Round);
            dc.DrawPolygon(null, pen, p);
        }

        /// <summary>
        /// Obte l'element associat al visual.
        /// </summary>
        /// 
        public new ThPadElement Element {
            get {
                return base.Element as ThPadElement;
            }
        }
    }
}