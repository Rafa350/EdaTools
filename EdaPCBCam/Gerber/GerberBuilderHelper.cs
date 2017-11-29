namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Cam.Gerber.Infrastructure;

    public static class GerberBuilderHelper {

       
        private sealed class DefineElementApertures: DefaultVisitor {

            private readonly GerberBuilder gb;
            private readonly IList<Layer> layers;
            private readonly Dictionary<string, int> apertures = new Dictionary<string, int>();
            private Part currentPart = null;

            public DefineElementApertures(GerberBuilder gb, IList<Layer> layers) {

                this.gb = gb;
                this.layers = layers;
            }

            public override void Visit(Part part) {

                currentPart = part;
                base.Visit(part);
                currentPart = null;
            }

            public override void Visit(LineElement line) {

                if (layers.Contains(line.Layer)) {
                    string key = ApertureKeyGenerator.GenerateKey(line);
                    if (!apertures.ContainsKey(key))
                        apertures.Add(key, gb.DefineCircleAperture(-1, line.Thickness));
                }
            }

            public override void Visit(ViaElement via) {

                if (layers.Contains(via.Layer)) {
                    string key = ApertureKeyGenerator.GenerateKey(via);
                    if (!apertures.ContainsKey(key)) {
                        switch (via.Shape) {
                            case ViaElement.ViaShape.Circular:
                                apertures.Add(key, gb.DefineCircleAperture(-1, via.Size, via.Drill));
                                break;

                            case ViaElement.ViaShape.Square:
                                apertures.Add(key, gb.DefineRectangleAperture(-1, via.Size, via.Size, via.Drill));
                                break;

                            case ViaElement.ViaShape.Octogonal:
                                apertures.Add(key, gb.DefinePoligonAperture(8, -1, via.Size, 0, via.Drill));
                                break;
                        }
                    }
                }
            }

            public override void Visit(ThPadElement pad) {

                if (layers.Contains(pad.Layer)) {
                    string key = ApertureKeyGenerator.GenerateKey(pad);
                    if (!apertures.ContainsKey(key)) {
                        double rotate = pad.Rotate + (currentPart != null ? currentPart.Rotate : 0);
                        switch (pad.Shape) {
                            case ThPadElement.ThPadShape.Circular:
                                apertures.Add(key, gb.DefineCircleAperture(-1, pad.Size, pad.Drill));
                                break;

                            case ThPadElement.ThPadShape.Square:
                                apertures.Add(key, gb.DefineMacroAperture(-1, 1, pad.Size, pad.Size, pad.Drill, pad.Rotate));
                                break;

                            case ThPadElement.ThPadShape.Octogonal:
                                apertures.Add(key, gb.DefinePoligonAperture(-1, 8, pad.Size, rotate, pad.Drill));
                                break;

                            case ThPadElement.ThPadShape.Oval:
                                apertures.Add(key, gb.DefineObroundAperture(-1, pad.Size * 2, pad.Size, pad.Drill));
                                break;
                        }
                    }
                }
            }

            public override void Visit(SmdPadElement pad) {

                if (layers.Contains(pad.Layer)) {
                    string key = ApertureKeyGenerator.GenerateKey(pad);
                    if (!apertures.ContainsKey(key)) {
                        double rotate = pad.Rotate + (currentPart != null ? currentPart.Rotate : 0);
                        if (pad.Roundnes > 0) {
                            double radius = pad.Roundnes * Math.Min(pad.Size.Width, pad.Size.Height) / 2;
                            apertures.Add(key, gb.DefineMacroAperture(-1, 0, pad.Size.Width, pad.Size.Height, radius, rotate));
                        }
                        else
                            apertures.Add(key, gb.DefineRectangleAperture(-1, pad.Size.Width, pad.Size.Height, 0));
                    }
                }
            }
        }

        private sealed class FlashElementApertures : DefaultVisitor {

            private readonly GerberBuilder gb;
            private readonly IList<Layer> layers;
            private readonly Dictionary<string, int> apertures = new Dictionary<string, int>();
            private int apertureIndex = 10;
            private Part currentPart;

            public FlashElementApertures(GerberBuilder gb, IList<Layer> layers) {

                this.gb = gb;
                this.layers = layers;
            }

            public override void Visit(Part part) {

                currentPart = part;
                base.Visit(part);
                currentPart = null;
            }

            public override void Visit(LineElement line) {

                if (layers.Contains(line.Layer)) {

                    string key = ApertureKeyGenerator.GenerateKey(line);
                    if (!apertures.ContainsKey(key))
                        apertures.Add(key, apertureIndex++);

                    gb.SelectAperture(apertures[key]);
                    gb.Operation(line.StartPosition.X, line.StartPosition.Y, OperationCode.Move);
                    gb.Operation(line.EndPosition.X, line.EndPosition.Y, OperationCode.Interpolate);
                }
            }

            public override void Visit(ViaElement via) {

                if (layers.Contains(via.Layer)) {

                    string key = ApertureKeyGenerator.GenerateKey(via);
                    if (!apertures.ContainsKey(key))
                        apertures.Add(key, apertureIndex++);

                    gb.SelectAperture(apertures[key]);
                    gb.Operation(via.Position.X, via.Position.Y, OperationCode.Flash);
                }
            }

            /// <summary>
            /// Visita un element de tipus ThPadElement
            /// </summary>
            /// <param name="pad">El element a visitar.</param>
            /// 
            public override void Visit(ThPadElement pad) {

                if (layers.Contains(pad.Layer)) {

                    string key = ApertureKeyGenerator.GenerateKey(pad);
                    if (!apertures.ContainsKey(key))
                        apertures.Add(key, apertureIndex++);

                    Point p = pad.Position;
                    if (currentPart != null) 
                        p = RotateTransform(p, currentPart.Position, currentPart.Rotate);

                    gb.SelectAperture(apertures[key]);
                    gb.Operation(p.X, p.Y, OperationCode.Flash);
                }
            }

            /// <summary>
            /// Visita un element de tipus SmdPad.
            /// </summary>
            /// <param name="pad">El element a visitar.</param>
            /// 
            public override void Visit(SmdPadElement pad) {

                if (layers.Contains(pad.Layer)) {

                    string key = ApertureKeyGenerator.GenerateKey(pad);
                    if (!apertures.ContainsKey(key))
                        apertures.Add(key, apertureIndex++);

                    Point p = pad.Position;
                    if (currentPart != null) 
                        p = RotateTransform(p, currentPart.Position, currentPart.Rotate);

                    gb.SelectAperture(apertures[key]);
                    gb.Operation(p.X, p.Y, OperationCode.Flash);
                }
            }

            /// <summary>
            /// Aplica una rotacio a un punt.
            /// </summary>
            /// <param name="p">El punt a transformar.</param>
            /// <param name="center">Punt del centre de rotacio.</param>
            /// <param name="rotate">Angle de rotacio.</param>
            /// <returns></returns>
            private Point RotateTransform(Point p, Point center, double rotate) {

                Matrix m = new Matrix();
                m.Translate(center.X, center.Y);
                m.RotateAt(rotate, center.X, center.Y);

                return m.Transform(p);
            }
        }

        public static void DefineApertures(this GerberBuilder gb, Board board, IList<Layer> layers) {

            DefineElementApertures visitor = new DefineElementApertures(gb, layers);
            board.AcceptVisitor(visitor);
        }

        public static void FlasApertures(this GerberBuilder gb, Board board, IList<Layer> layers) {

            FlashElementApertures visitor = new FlashElementApertures(gb, layers);
            board.AcceptVisitor(visitor);
        }
    }
}
