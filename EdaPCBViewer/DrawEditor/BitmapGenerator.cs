namespace MikroPic.EdaTools.v1.Designer.DrawEditor {

    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using MikroPic.EdaTools.v1.Pcb.Model.BoardElements;

    using Color = MikroPic.EdaTools.v1.Geometry.Color;
    using SysRect = System.Windows.Rect;
    using SysColor = System.Windows.Media.Color;

    public sealed class BitmapGenerator {

        private readonly Board board;

        private sealed class RenderVisitor: ElementVisitor {

            private readonly WriteableBitmap bitmap;
            private readonly SysColor color;

            public RenderVisitor(Board board, Layer layer, WriteableBitmap bitmap):
                base(board, layer) {

                this.bitmap = bitmap;

                Color layerColor = layer.Color;
                color = SysColor.FromRgb(color.R, color.G, color.B);
            }

            public override void Run() {

            }

            public override void Visit(LineElement line) {

                bitmap.DrawLine(
                    line.StartPosition.X,
                    line.StartPosition.Y,
                    line.EndPosition.X,
                    line.EndPosition.Y,
                    color);
            }

        }

        public BitmapGenerator(Board board) {

            this.board = board;
        }

        public DrawingVisual CreateVisual() {

            WriteableBitmap bitmap = BitmapFactory.New(512, 512);

            foreach (var layer in board.Layers) {
                if (layer.IsVisible) {
                    RenderVisitor visitor = new RenderVisitor(board, layer, bitmap);
                    visitor.Run();
                }
            }

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen()) {
                context.DrawImage(bitmap, new SysRect(0, 0, 512, 512));
            }

            return visual;
        }
    }
}

