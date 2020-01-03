namespace MikroPic.EdaTools.v1.BoardEditor.Render.WPF {

    using MikroPic.EdaTools.v1.Base.WPF;
    using MikroPic.EdaTools.v1.BoardEditor.Render.WPF.Visuals;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using System.Windows.Media;

    public sealed class SceneGraph {

        private readonly DrawingVisual rootVisual;

        public SceneGraph(DrawingVisual visual) {

            this.rootVisual = visual;
        }

        public VisualBase GetVisual(int x, int y) {

            HitTestResult result = VisualTreeHelper.HitTest(rootVisual, new System.Windows.Point(x, y));
            if (result != null)
                return result.VisualHit as VisualBase;
            else
                return null;
        }

        public ElementVisual GetVisual(Element element) {

            return GetVisual(rootVisual, element);
        }

        private ElementVisual GetVisual(DrawingVisual visual, Element element) {

            if (visual is ElementVisual ev)
                return ev.Element == element ? ev : null;

            else {
                foreach (DrawingVisual child in visual.Children) {
                    ElementVisual v = GetVisual(child, element);
                    if (v != null)
                        return v;
                }
                return null;
            }
        }

        public DrawingVisual Visual {
            get {
                return rootVisual;
            }
        }
    }
}
