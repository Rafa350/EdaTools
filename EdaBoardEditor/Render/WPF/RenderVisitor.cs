namespace MikroPic.EdaTools.v1.BoardEditor.Render.WPF {

    using MikroPic.EdaTools.v1.BoardEditor.Render.WPF.Visuals;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;
    using System.Windows.Media;

    /// <summary>
    /// Clase per visitar la placa i generar les visuals.
    /// </summary>
    /// 
    internal sealed class RenderVisitor : ElementVisitor {

        private readonly Layer layer;
        private readonly VisualLayer visualLayer;
        private DrawingVisual parentVisual;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="layer">La capa on aplicar el proces.</param>
        /// <param name="rootVisual">El visual arrel.</param>
        /// 
        public RenderVisitor(Layer layer, DrawingVisual rootVisual, VisualLayer visualLayer) { 

            this.layer = layer;
            this.visualLayer = visualLayer;
            parentVisual = rootVisual;
        }

        /// <summary>
        /// Visita un objecte 'LiniaElement'
        /// </summary>
        /// <param name="line">L'objecte a visitar.</param>
        /// 
        public override void Visit(LineElement line) {

            if (IsVisible(line)) {
                ElementVisual visual = new LineVisual(parentVisual, line, visualLayer.Color);
                visual.Draw();
            }
        }

        /// <summary>
        /// Visita un objecte ArcElement
        /// </summary>
        /// <param name="arc">L'objecte a visitar.</param>
        /// 
        public override void Visit(ArcElement arc) {

            if (IsVisible(arc)) {
                ElementVisual visual = new ArcVisual(parentVisual, arc, visualLayer.Color);
                visual.Draw();
            }
        }

        /// <summary>
        /// Visita un objecte 'RectangleElement'
        /// </summary>
        /// <param name="rectangle">L'objecte a visitar.</param>
        /// 
        public override void Visit(RectangleElement rectangle) {

            if (IsVisible(rectangle)) {
                ElementVisual visual = new RectangleVisual(parentVisual, rectangle, visualLayer.Color);
                visual.Draw();
            }
        }

        /// <summary>
        /// Visita un objecte 'CircleElement'.
        /// </summary>
        /// <param name="circle">L'objecte a visitar.</param>
        /// 
        public override void Visit(CircleElement circle) {

            if (IsVisible(circle)) {
                ElementVisual visual = new CircleVisual(parentVisual, circle, visualLayer.Color);
                visual.Draw();
            }
        }

        /// <summary>
        /// Visita un objecte 'RegionElement'.
        /// </summary>
        /// <param name="region">L'objecte a visitar.</param>
        /// 
        public override void Visit(RegionElement region) {

            if (IsVisible(region)) {
                RegionVisual visual = new RegionVisual(parentVisual, region, Board, layer, visualLayer.Color);
                visual.Draw();
            }
        }

        /// <summary>
        /// Visita un objecte 'ViaElement'
        /// </summary>
        /// <param name="via">L'objecte a visitar.</param>
        /// 
        public override void Visit(ViaElement via) {

            if (IsVisible(via)) {
                ElementVisual visual = new ViaVisual(parentVisual, via, layer, visualLayer.Color);
                visual.Draw();
            }
        }

        /// <summary>
        /// Visita un objecte 'SmdPadElement'.
        /// </summary>
        /// <param name="pad">L'objecte a visitar.</param>
        /// 
        public override void Visit(SmdPadElement pad) {

            if (IsVisible(pad)) {
                ElementVisual visual = new SmdPadVisual(parentVisual, pad, visualLayer.Color);
                visual.Draw();
            }
        }

        /// <summary>
        /// Visita un objecte 'ThPadElement'.
        /// </summary>
        /// <param name="pad">L'objecte a visitar.</param>
        /// 
        public override void Visit(ThPadElement pad) {

            if (IsVisible(pad)) {
                ElementVisual visual = new ThPadVisual(parentVisual, pad, layer, visualLayer.Color);
                visual.Draw();
            }
        }

        /// <summary>
        /// Visita un objecte 'HoleElement'
        /// </summary>
        /// <param name="hole">L'objecte a visitar.</param>
        /// 
        public override void Visit(HoleElement hole) {

            if (IsVisible(hole)) {
                ElementVisual visual = new HoleVisual(parentVisual, hole, visualLayer.Color);
                visual.Draw();
            }
        }

        /// <summary>
        /// Visita un objecte de tipus 'TextElement'
        /// </summary>
        /// <param name="text">L'objwecte a visitar.</param>
        /// 
        public override void Visit(TextElement text) {

            if (IsVisible(text)) {
                ElementVisual visual = new TextVisual(parentVisual, text, Part, visualLayer.Color);
                visual.Draw();
            }
        }

        /// <summary>
        /// Visuta un objecte de tipus 'Part'
        /// </summary>
        /// <param name="part">L'objecte a visitar.</param>
        /// 
        public override void Visit(Part part) {

            PartVisual visual = new PartVisual(parentVisual, part);
            //visual.Draw();

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

            double x = part.Position.X;
            double y = part.Position.Y;
            double angle = part.Rotation.Degrees / 100.0;

            Matrix m = new Matrix();
            m.Translate(x, y);
            m.RotateAt(angle, x, y);

            Transform transform = new MatrixTransform(m);
            transform.Freeze();

            return transform;
        }

        private bool IsVisible(Element element) {

            return visualLayer.IsVisible(element);
        }
    }
}