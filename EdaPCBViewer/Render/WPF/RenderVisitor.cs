namespace MikroPic.EdaTools.v1.Designer.Render.WPF {

    using MikroPic.EdaTools.v1.Designer.Render.WPF.Infrastructure;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Clase per visitar la placa i generar les visuals.
    /// </summary>
    /// 
    internal sealed class RenderVisitor : ElementVisitor {

        private readonly VisualDrawer drawer;
        private DrawingVisual parentVisual;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="board">La placa a procesar.</param>
        /// <param name="layer">La capa on aplicar el proces.</param>
        /// <param name="rootVisual">El visual arrel.</param>
        /// 
        public RenderVisitor(Board board, Layer layer, DrawingVisual rootVisual, VisualDrawer drawer) :
            base(board, layer) {

            this.drawer = drawer;
            parentVisual = rootVisual;
        }

        /// <summary>
        /// Visita un objecte 'LiniaElement'
        /// </summary>
        /// <param name="line">L'objecte a visitar.</param>
        /// 
        public override void Visit(LineElement line) {

            DrawingVisual visual = new DrawingVisual();
            AddVisual(visual);
            drawer.DrawLineElement(visual, Layer, line);
        }

        /// <summary>
        /// Visita un objecte ArcElement
        /// </summary>
        /// <param name="arc">L'objecte a visitar.</param>
        /// 
        public override void Visit(ArcElement arc) {

            DrawingVisual visual = new DrawingVisual();
            AddVisual(visual);
            drawer.DrawArcElement(visual, Layer, arc);
        }

        /// <summary>
        /// Visita un objecte 'RectangleElement'
        /// </summary>
        /// <param name="rectangle">L'objecte a visitar.</param>
        /// 
        public override void Visit(RectangleElement rectangle) {

            DrawingVisual visual = new DrawingVisual();
            AddVisual(visual);
            drawer.DrawRectangleElement(visual, Layer, rectangle);
        }

        /// <summary>
        /// Visita un objecte 'CircleElement'.
        /// </summary>
        /// <param name="circle">L'objecte a visitar.</param>
        /// 
        public override void Visit(CircleElement circle) {

            DrawingVisual visual = new DrawingVisual();
            AddVisual(visual);
            drawer.DrawCircleElement(visual, Layer, circle);
        }

        /// <summary>
        /// Visita un objecte 'RegionElement'.
        /// </summary>
        /// <param name="region">L'objecte a visitar.</param>
        /// 
        public override void Visit(RegionElement region) {

            DrawingVisual visual = new DrawingVisual();
            AddVisual(visual);
            drawer.DrawRegionElement(visual, Layer, Board, region);
        }

        /// <summary>
        /// Visita un objecte 'ViaElement'
        /// </summary>
        /// <param name="via">L'objecte a visitar.</param>
        /// 
        public override void Visit(ViaElement via) {

            DrawingVisual visual = new DrawingVisual();
            AddVisual(visual);
            drawer.DrawViaElement(visual, Layer, via);
        }

        /// <summary>
        /// Visita un objecte 'SmdPadElement'.
        /// </summary>
        /// <param name="pad">L'objecte a visitar.</param>
        /// 
        public override void Visit(SmdPadElement pad) {

            DrawingVisual visual = new DrawingVisual();
            AddVisual(visual);
            drawer.DrawSmdPadElement(visual, Layer, pad);
        }

        /// <summary>
        /// Visita un objecte 'ThPadElement'.
        /// </summary>
        /// <param name="pad">L'objecte a visitar.</param>
        /// 
        public override void Visit(ThPadElement pad) {

            DrawingVisual visual = new DrawingVisual();
            AddVisual(visual);
            drawer.DrawThPadElement(visual, Layer, pad);
        }

        /// <summary>
        /// Visita un objecte 'HoleElement'
        /// </summary>
        /// <param name="hole">L'objecte a visitar.</param>
        /// 
        public override void Visit(HoleElement hole) {

            DrawingVisual visual = new DrawingVisual();
            AddVisual(visual);
            drawer.DrawHoleElement(visual, Layer, hole);
        }

        /// <summary>
        /// Visita un objecte de tipus 'TextElement'
        /// </summary>
        /// <param name="text">L'objwecte a visitar.</param>
        /// 
        public override void Visit(TextElement text) {

            DrawingVisual visual = new DrawingVisual();
            AddVisual(visual);
            drawer.DrawTextElement(visual, Layer, Part, text);
        }

        /// <summary>
        /// Visuta un objecte de tipus 'Part'
        /// </summary>
        /// <param name="part">L'objecte a visitar.</param>
        /// 
        public override void Visit(Part part) {

            DrawingVisual visual = new DrawingVisual();
            AddVisual(visual);

            visual.Transform = GetTransform(part);

            DrawingVisual saveVisual = parentVisual;
            parentVisual = visual;
            try {
                base.Visit(part);
            }
            finally {
                parentVisual = saveVisual;
            }
        }

        /// <summary>
        /// Afegeix la visual al seu pare
        /// </summary>
        /// <param name="visual">La visual a afeigir</param>
        /// 
        private void AddVisual(DrawingVisual visual) {

            parentVisual.Children.Add(visual);
        }

        /// <summary>
        /// Obte la transformacio d'un component
        /// </summary>
        /// <param name="part">El component.</param>
        /// <returns>La transformacio.</returns>
        /// 
        private static Transform GetTransform(Part part) {

            Point position = new Point(part.Position.X, part.Position.Y);
            double angle = part.Rotation.Degrees / 100.0;

            Matrix m = new Matrix();
            m.Translate(position.X, position.Y);
            m.RotateAt(angle, position.X, position.Y);

            Transform transform = new MatrixTransform(m);
            transform.Freeze();

            return transform;
        }
    }
}