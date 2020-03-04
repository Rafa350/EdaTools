namespace EdaBoardViewer.Views.Controls {

    using Avalonia.Controls;
    using Avalonia.Media;

    public interface ISceneRenderer {

        void Render(DrawingContext context);
    }

    public sealed partial class SceneControl : Control {

        static SceneControl() {

            AffectsRender<SceneControl>(
                ValueMatrixProperty,
                SceneRendererProperty);
        }

        public SceneControl() {

            ClipToBounds = true;
        }

        public override void Render(DrawingContext context) {

            if (SceneRenderer != null)
                using (context.PushPreTransform(ValueMatrix))
                    SceneRenderer.Render(context);
        }
    }
}
