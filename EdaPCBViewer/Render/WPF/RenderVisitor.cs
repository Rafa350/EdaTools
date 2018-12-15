namespace MikroPic.EdaTools.v1.PanelEditor.Render.WPF {

    using MikroPic.EdaTools.v1.PanelEditor.Render.WPF.Infrastructure;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
    using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    using Color = MikroPic.EdaTools.v1.Base.Geometry.Color;
    using WinColor = System.Windows.Media.Color;

    /// <summary>
    /// Clase per visitar la placa i generar les visuals.
    /// </summary>
    /// 
    internal sealed class RenderVisitor : ElementVisitor {

        private readonly HashSet<Element> visibleElements;
        private readonly Layer layer;
        private readonly Color color;
        private readonly VisualDrawer drawer;
        private DrawingVisual parentVisual;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="layer">La capa on aplicar el proces.</param>
        /// <param name="rootVisual">El visual arrel.</param>
        /// 
        public RenderVisitor(HashSet<Element> visibleElements, Layer layer, DrawingVisual rootVisual, Color color, VisualDrawer drawer) { 

            this.visibleElements = visibleElements;
            this.layer = layer;
            this.drawer = drawer;
            this.color = color;
            parentVisual = rootVisual;
        }

        /// <summary>
        /// Visita un objecte 'LiniaElement'
        /// </summary>
        /// <param name="line">L'objecte a visitar.</param>
        /// 
        public override void Visit(LineElement line) {

            if (IsVisible(line)) {
                DrawingVisual visual = new DrawingVisual();
                AddVisual(visual);
                drawer.DrawLineElement(visual, layer, line, color);
            }
        }

        /// <summary>
        /// Visita un objecte ArcElement
        /// </summary>
        /// <param name="arc">L'objecte a visitar.</param>
        /// 
        public override void Visit(ArcElement arc) {

            if (IsVisible(arc)) {
                DrawingVisual visual = new DrawingVisual();
                AddVisual(visual);
                drawer.DrawArcElement(visual, layer, arc, color);
            }
        }

        /// <summary>
        /// Visita un objecte 'RectangleElement'
        /// </summary>
        /// <param name="rectangle">L'objecte a visitar.</param>
        /// 
        public override void Visit(RectangleElement rectangle) {

            if (IsVisible(rectangle)) {
                DrawingVisual visual = new DrawingVisual();
                AddVisual(visual);
                drawer.DrawRectangleElement(visual, layer, rectangle, color);
            }
        }

        /// <summary>
        /// Visita un objecte 'CircleElement'.
        /// </summary>
        /// <param name="circle">L'objecte a visitar.</param>
        /// 
        public override void Visit(CircleElement circle) {

            if (IsVisible(circle)) {
                DrawingVisual visual = new DrawingVisual();
                AddVisual(visual);
                drawer.DrawCircleElement(visual, layer, circle, color);
            }
        }

        /// <summary>
        /// Visita un objecte 'RegionElement'.
        /// </summary>
        /// <param name="region">L'objecte a visitar.</param>
        /// 
        public override void Visit(RegionElement region) {

            if (IsVisible(region)) {
                DrawingVisual visual = new DrawingVisual();
                AddVisual(visual);
                drawer.DrawRegionElement(visual, layer, Board, region, color);
            }
        }

        /// <summary>
        /// Visita un objecte 'ViaElement'
        /// </summary>
        /// <param name="via">L'objecte a visitar.</param>
        /// 
        public override void Visit(ViaElement via) {

            if (IsVisible(via)) {
                DrawingVisual visual = new DrawingVisual();
                AddVisual(visual);
                drawer.DrawViaElement(visual, layer, via, color);
            }
        }

        /// <summary>
        /// Visita un objecte 'SmdPadElement'.
        /// </summary>
        /// <param name="pad">L'objecte a visitar.</param>
        /// 
        public override void Visit(SmdPadElement pad) {

            if (IsVisible(pad)) {
                DrawingVisual visual = new DrawingVisual();
                AddVisual(visual);
                drawer.DrawSmdPadElement(visual, layer, pad, color);
            }
        }

        /// <summary>
        /// Visita un objecte 'ThPadElement'.
        /// </summary>
        /// <param name="pad">L'objecte a visitar.</param>
        /// 
        public override void Visit(ThPadElement pad) {

            if (IsVisible(pad)) {
                DrawingVisual visual = new DrawingVisual();
                AddVisual(visual);
                drawer.DrawThPadElement(visual, layer, pad, color);
            }
        }

        /// <summary>
        /// Visita un objecte 'HoleElement'
        /// </summary>
        /// <param name="hole">L'objecte a visitar.</param>
        /// 
        public override void Visit(HoleElement hole) {

            if (IsVisible(hole)) {
                DrawingVisual visual = new DrawingVisual();
                AddVisual(visual);
                drawer.DrawHoleElement(visual, layer, hole, color);
            }
        }

        /// <summary>
        /// Visita un objecte de tipus 'TextElement'
        /// </summary>
        /// <param name="text">L'objwecte a visitar.</param>
        /// 
        public override void Visit(TextElement text) {

            if (IsVisible(text)) {
                DrawingVisual visual = new DrawingVisual();
                AddVisual(visual);
                drawer.DrawTextElement(visual, layer, Part, text, color);
            }
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

        private bool IsVisible(Element element) {

            return visibleElements.Contains(element) && element.LayerSet.Contains(layer.Id);
        }
    }
}