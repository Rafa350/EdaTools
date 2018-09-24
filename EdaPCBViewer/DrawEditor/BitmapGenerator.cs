namespace MikroPic.EdaTools.v1.Designer.DrawEditor {

    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;

    using Color = MikroPic.EdaTools.v1.Geometry.Color;
    using SysPoint = System.Windows.Point;
    using SysRect = System.Windows.Rect;
    using SysSize = System.Windows.Size;
    using SysColor = System.Windows.Media.Color;

    public sealed class BitmapGenerator {

        private readonly SysSize size;
        private readonly Matrix matrix;
        private readonly Board board;

        private sealed class RenderVisitor: ElementVisitor {

            private readonly WriteableBitmap bitmap;
            private readonly SysColor color;
            private readonly Matrix matrix;

            public RenderVisitor(Board board, Layer layer, WriteableBitmap bitmap, Matrix matrix):
                base(board, layer) {

                this.bitmap = bitmap;
                this.matrix = matrix;

                color = Colors.Aquamarine;
            }

            public override void Visit(LineElement line) {

                SysPoint p1 = Transform(line.StartPosition);
                SysPoint p2 = Transform(line.EndPosition);

                bitmap.DrawLine((int)p1.X, (int)p1.Y, (int)p2.X, (int)p2.Y, color);
            }

            private SysPoint Transform(Point point) {

                SysPoint p = new SysPoint(point.X, point.Y);
                return matrix.Transform(p);
            }

        }

        public BitmapGenerator(Board board, SysSize size, Matrix matrix) {

            this.board = board;
            this.size = size;
            this.matrix = matrix;
        }

        public DrawingVisual CreateVisual() {

            //WriteableBitmap bitmap = BitmapFactory.New((int)size.Width, (int)size.Height);
            WriteableBitmap bitmap = BitmapFactory.New(100, 100);
            bitmap.Clear(Colors.SlateGray);

/*            using (bitmap.GetBitmapContext()) {
                foreach (var layer in board.Layers) {
                    if (layer.IsVisible) {
                        RenderVisitor visitor = new RenderVisitor(board, layer, bitmap, matrix);
                        visitor.Run();
                    }
                }
            }*/

            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext context = visual.RenderOpen()) {
                context.DrawEllipse(Brushes.Yellow, null, new SysPoint(100, 100), 200, 200);
                //context.DrawImage(bitmap, new SysRect(0, 0, size.Width, size.Height));
            }

            return visual;
        }
    }
}

