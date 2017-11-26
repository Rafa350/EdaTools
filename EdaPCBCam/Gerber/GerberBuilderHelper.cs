namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Model;
    using MikroPic.EdaTools.v1.Model.Elements;
    using MikroPic.EdaTools.v1.Cam.Gerber.Infrastructure;

    public static class GerberBuilderHelper {

        private sealed class DefineElementApertures: DefaultVisitor {

            private readonly GerberBuilder gb;
            private readonly Dictionary<string, int> apertures = new Dictionary<string, int>();

            public DefineElementApertures(GerberBuilder gb) {

                this.gb = gb;
            }

            public override void Visit(ViaElement via) {

                string key = ApertureKeyGenerator.GenerateKey(via);
                if (!apertures.ContainsKey(key)) {
                    switch (via.Shape) {
                        case ViaElement.ViaShape.Circular:
                            apertures.Add(key, gb.DefineCircleAperture(via.Size, via.Drill));
                            break;

                        case ViaElement.ViaShape.Square:
                            apertures.Add(key, gb.DefineRectangleAperture(via.Size, via.Size, via.Drill));
                            break;

                        case ViaElement.ViaShape.Octogonal:
                            apertures.Add(key, gb.DefinePoligonAperture(8, via.Size, 22.5, via.Drill));
                            break;
                    }
                }
            }

            public override void Visit(ThPadElement pad) {

                string key = ApertureKeyGenerator.GenerateKey(pad);
                if (!apertures.ContainsKey(key)) {
                    switch (pad.Shape) {
                        case ThPadElement.ThPadShape.Circular:
                            apertures.Add(key, gb.DefineCircleAperture(pad.Size, pad.Drill));
                            break;

                        case ThPadElement.ThPadShape.Square:
                            apertures.Add(key, gb.DefineRectangleAperture(pad.Size, pad.Size, pad.Drill));
                            break;

                        case ThPadElement.ThPadShape.Octogonal:
                            apertures.Add(key, gb.DefinePoligonAperture(8, pad.Size, 22.5, pad.Drill));
                            break;

                        case ThPadElement.ThPadShape.Oval:
                            apertures.Add(key, gb.DefineObroundAperture(pad.Size, pad.Size * 2, pad.Drill));
                            break;
                    }
                }
            }

            public override void Visit(SmdPadElement pad) {

                string key = ApertureKeyGenerator.GenerateKey(pad);
                if (!apertures.ContainsKey(key)) {
                    if (pad.Roundnes > 0) {
                        double radius = pad.Roundnes * Math.Min(pad.Size.Width, pad.Size.Height) / 2;
                        apertures.Add(key, gb.DefineMacroAperture(0, pad.Size.Width, pad.Size.Height, radius, 0));
                    }
                    else
                        apertures.Add(key, gb.DefineRectangleAperture(pad.Size.Width, pad.Size.Height, 0));
                }
            }
        }

        private sealed class FlashElementApertures : DefaultVisitor {

            private readonly GerberBuilder gb;
            private readonly Dictionary<string, int> apertures = new Dictionary<string, int>();
            private int apertureIndex = 10;
            private Part currentPart;

            public FlashElementApertures(GerberBuilder gb) {

                this.gb = gb;
            }

            public override void Visit(Part part) {

                currentPart = part;
                base.Visit(part);
                currentPart = null;
            }

            public override void Visit(ViaElement via) {

                string key = ApertureKeyGenerator.GenerateKey(via);
                if (!apertures.ContainsKey(key))
                    apertures.Add(key, apertureIndex++);

                gb.SelectAperture(apertures[key]);
                gb.Operation(via.Position.X, via.Position.Y, OperationCode.Flash);
            }

            public override void Visit(ThPadElement pad) {

                string key = ApertureKeyGenerator.GenerateKey(pad);
                if (!apertures.ContainsKey(key))
                    apertures.Add(key, apertureIndex++);

                Point p = pad.Position;
                if (currentPart != null)
                    p = Transform(p, currentPart.Position, currentPart.Rotate);

                gb.SelectAperture(apertures[key]);
                gb.Operation(p.X, p.Y, OperationCode.Flash);
            }

            public override void Visit(SmdPadElement pad) {

                string key = ApertureKeyGenerator.GenerateKey(pad);
                if (!apertures.ContainsKey(key))
                    apertures.Add(key, apertureIndex++);

                Point p = pad.Position;
                if (currentPart != null)
                    p = Transform(p, currentPart.Position, currentPart.Rotate);

                gb.SelectAperture(apertures[key]);
                gb.Operation(p.X, p.Y, OperationCode.Flash);
            }

            private Point Transform(Point p, Point origin, double rotate) {

                Matrix m = new Matrix();
                m.Translate(origin.X, origin.Y);
                m.RotateAt(rotate, origin.X, origin.Y);

                return m.Transform(p);
            }
        }

        public static void DefineApertures(this GerberBuilder gb, Board board) {

            DefineElementApertures visitor = new DefineElementApertures(gb);
            board.AcceptVisitor(visitor);
        }

        public static void FlasApertures(this GerberBuilder gb, Board board) {

            FlashElementApertures visitor = new FlashElementApertures(gb);
            board.AcceptVisitor(visitor);
        }
    }
}
