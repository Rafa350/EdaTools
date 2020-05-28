namespace EdaBoardViewer.Views.Controls {

    using Avalonia;
    using Avalonia.Controls;

    public sealed partial class SceneControl : Control {

        public static readonly StyledProperty<Matrix> ValueMatrixProperty = AvaloniaProperty.Register<SceneControl, Matrix>(nameof(ValueMatrix), Matrix.Identity);
        public static readonly StyledProperty<ISceneRenderer> SceneRendererProperty = AvaloniaProperty.Register<SceneControl, ISceneRenderer>(nameof(SceneRenderer), null);

        /// <summary>
        /// Obte o asigna la matriu de transformacio del valor.
        /// </summary>
        /// 
        public Matrix ValueMatrix {
            get { return GetValue(ValueMatrixProperty); }
            set { SetValue(ValueMatrixProperty, value); }
        }

        /// <summary>
        /// Obte o asigna el renderitzador de l'escena.
        /// </summary>
        /// 
        public ISceneRenderer SceneRenderer {
            get { return GetValue(SceneRendererProperty); }
            set { SetValue(SceneRendererProperty, value); }
        }
    }
}
