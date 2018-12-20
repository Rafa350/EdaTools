namespace MikroPic.EdaTools.v1.BoardEditor.Render.WPF.Visuals {

    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Infrastructure;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
    using System.Windows.Media;
    using System.Collections.Generic;

    using Color = MikroPic.EdaTools.v1.Base.Geometry.Color;

    public sealed class TextVisual: ElementVisual {

        private static readonly TextDrawer td;
        private readonly Color color;
        private readonly Part part;

        static TextVisual() {

            FontFactory ff = FontFactory.Instance;
            Font font = ff.GetFont("Standard");
            td = new TextDrawer(font);
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="parent">La visual pare.</param>
        /// <param name="text">L'element.</param>
        /// <param name="color">El color amb el que es representa.</param>
        /// 
        public TextVisual(DrawingVisual parent, TextElement text, Part part, Color color):
            base(parent, text) {

            this.part = part;
            this.color = color;
        }

        /// <summary>
        /// Dibuixa la representacio de l'element.
        /// </summary>
        /// <param name="dc">El context de representacio.</param>
        /// 
        protected override void Draw(DrawVisualContext dc) {

            PartAttributeAdapter paa = new PartAttributeAdapter(part, Element);
            IEnumerable<GlyphTrace> glyphTraces = td.Draw(paa.Value, new Point(0, 0), paa.HorizontalAlign, paa.VerticalAlign, paa.Height);

            Transformation t = new Transformation();
            t.Translate(Element.Position);
            t.Rotate(Element.Position, Element.Rotation);
            dc.PushTransform(t);

            Pen pen = dc.GetPen(color, Element.Thickness, PenLineCap.Round);
            dc.DrawGlyphs(null, pen, glyphTraces);
            dc.DrawEllipse(Brushes.YellowGreen, null, new Point(0, 0), 150000, 150000);

            dc.Pop();
        }

        /// <summary>
        /// Obte l'element associat al visual.
        /// </summary>
        public new TextElement Element {
            get {
                return base.Element as TextElement;
            }
        }
    }
}
