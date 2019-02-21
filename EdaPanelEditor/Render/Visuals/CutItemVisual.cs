namespace MikroPic.EdaTools.v1.PanelEditor.Render.Visuals {

    using System;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Base.WPF;
    using MikroPic.EdaTools.v1.Panel.Model.Items;
    using MikroPic.EdaTools.v1.PanelEditor.DrawEditor;

    public sealed class CutItemVisual: PanelItemVisual {

        private readonly Color cutColor = Color.FromRgb(128, 128, 128);

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="parent">El visual pare,</param>
        /// <param name="item">El item CUT.</param>
        /// 
        public CutItemVisual(VisualItem parent, CutItem item):
            base(parent, item) {
        }

        protected override void OnRender(DrawingContext dc) {

            Draw(new DrawVisualContext(dc));
            base.OnRender(dc);
        }

        /// <summary>
        /// Renderitzat.
        /// </summary>
        /// <param name="dc">El context de renderitzat.</param>
        /// 
        private void Draw(DrawVisualContext dc) {

            // Obte els punts de referencia
            //
            int[] refPoints = Item.GetReferencePoints();
            int[] cutPoints = Item.GetCutReferencePoints(refPoints);

            // Obte la pendent 
            //
            double dx = Item.EndPosition.X - Item.StartPosition.X;
            double dy = Item.EndPosition.Y - Item.StartPosition.Y;
            double rad = Math.Atan2(dy, dx);

            // Calcula la transformacio
            //
            Transformation t = new Transformation();
            t.Translate(Item.StartPosition);
            t.Rotate(Item.StartPosition, Angle.FromRadiants(rad));

            Pen pen = dc.GetPen(cutColor, Item.Thickness, PenLineCap.Round);

            // Afegeix les linies de tall
            //
            for (int i = 0; i < cutPoints.Length; i += 2) {

                // Transforma els punts a la posicio real
                //
                Point q1 = t.ApplyTo(new Point(cutPoints[i], 0));
                Point q2 = t.ApplyTo(new Point(cutPoints[i + 1], 0));

                // Dibuixa les linies
                //
                dc.DrawLine(pen, q1, q2);
            }
        }

        /// <summary>
        /// Obte l'item associat.
        /// </summary>
        /// 
        public new CutItem Item {
            get {
                return base.Item as CutItem;
            }
        }
    }
}
