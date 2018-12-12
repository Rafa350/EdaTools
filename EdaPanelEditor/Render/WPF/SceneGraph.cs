namespace MikroPic.EdaTools.v1.PanelEditor.Render.WPF {

    using System.Windows.Media;

    public sealed class SceneGraph: ISceneGraph {

        private readonly DrawingVisual visual;

        public SceneGraph(DrawingVisual visual) {

            this.visual = visual;
        }

        public DrawingVisual Visual {
            get {
                return visual;
            }
        }
    }
}
