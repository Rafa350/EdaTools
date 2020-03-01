namespace EdaBoardViewer.Views.Controls {

    using System.IO;
    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Media;
    using EdaBoardViewer.Render;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.IO;

    class BoardControl : Control {

        public static readonly StyledProperty<Matrix> ValueMatrixProperty = AvaloniaProperty.Register<RulerControl, Matrix>(nameof(ValueMatrix), Matrix.Identity);

        private Board board = null;

        static BoardControl() {

            AffectsRender<BoardControl>(
                ValueMatrixProperty);
        }

        public BoardControl() {

            //ClipToBounds = true;
        }

        protected override void OnInitialized() {

            using (Stream stream = new FileStream("board3.xbrd", FileMode.Open, FileAccess.Read, FileShare.None)) {
                BoardStreamReader reader = new BoardStreamReader(stream);
                board = reader.Read();
            }
        }

        public override void Render(DrawingContext context) {

            using (context.PushPreTransform(ValueMatrix)) {
                var boardRenderer = new BoardRenderer(context);
                boardRenderer.Render(board);
            }
        }

        /// <summary>
        /// Obte o asigna la matriu de transformacio del valor.
        /// </summary>
        /// 
        public Matrix ValueMatrix {
            get { return GetValue(ValueMatrixProperty); }
            set { SetValue(ValueMatrixProperty, value); }
        }
    }
}
