namespace MikroPic.EdaTools.v1.Cam.Gerber.Infrastructure {

    using System;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Cam.Gerber;

    internal static class ApertureDictionaryBuilder {

        /// <summary>
        /// Clase utilitzada per la inicialitzacio del diccionari d'apertures.
        /// </summary>
        /// 
        private sealed class DefineAperturesVisitor : DefaultVisitor {

            private readonly IList<Layer> layers;
            private readonly ApertureDictionary apertureDict;

            /// <summary>
            /// Constructor de la clase.
            /// </summary>
            /// <param name="layers">La llista de capes.</param>
            /// <param name="apertureDict">El diccionari per inicialitzar.</param>
            /// 
            public DefineAperturesVisitor(IList<Layer> layers, ApertureDictionary apertureDict) {

                this.layers = layers;
                this.apertureDict = apertureDict;
            }

            public override void Visit(LineElement line) {

                if (line.IsOnAnyLayer(layers))
                    apertureDict.AddCircle(Math.Max(line.Thickness, 0.01));
            }

            public override void Visit(ArcElement arc) {

                if (arc.IsOnAnyLayer(layers))
                    apertureDict.AddCircle(Math.Max(arc.Thickness, 0.01));
            }

            public override void Visit(RectangleElement rectangle) {

                if (rectangle.IsOnAnyLayer(layers)) {
                    if (rectangle.Thickness == 0) {
                        double rotate = rectangle.Rotate + (VisitingPart != null ? VisitingPart.Rotate : 0);
                        apertureDict.AddRectangle(rectangle.Size.Width, rectangle.Size.Height, rotate);
                    }
                }
            }

            public override void Visit(CircleElement circle) {

                if (circle.IsOnAnyLayer(layers)) {
                    if (circle.Thickness == 0)
                        apertureDict.AddCircle(circle.Diameter);
                }
            }

            public override void Visit(ViaElement via) {

                if (via.IsOnAnyLayer(layers)) {
                    switch (via.Shape) {
                        case ViaElement.ViaShape.Circular:
                            apertureDict.AddCircle(via.OuterSize);
                            break;

                        case ViaElement.ViaShape.Square:
                            apertureDict.AddRectangle(via.OuterSize, via.OuterSize, 0);
                            break;

                        case ViaElement.ViaShape.Octogonal:
                            apertureDict.AddOctagon(via.OuterSize, 0);
                            break;
                    }
                }
            }

            public override void Visit(ThPadElement pad) {

                if (pad.IsOnAnyLayer(layers)) {
                    double rotate = pad.Rotate + (VisitingPart != null ? VisitingPart.Rotate : 0);
                    switch (pad.Shape) {
                        case ThPadElement.ThPadShape.Circular:
                            apertureDict.AddCircle(pad.Size);
                            break;

                        case ThPadElement.ThPadShape.Square:
                            apertureDict.AddRectangle(pad.Size, pad.Size, rotate);
                            break;

                        case ThPadElement.ThPadShape.Octogonal:
                            apertureDict.AddOctagon(pad.Size, rotate);
                            break;

                        case ThPadElement.ThPadShape.Oval:
                            apertureDict.AddOval(pad.Size * 2, pad.Size, rotate);
                            break;
                    }
                }
            }

            public override void Visit(SmdPadElement pad) {

                if (pad.IsOnAnyLayer(layers)) {
                    double rotate = pad.Rotate + (VisitingPart != null ? VisitingPart.Rotate : 0);
                    double radius = (pad.Roundnes - 0.01) * Math.Min(pad.Size.Width, pad.Size.Height) / 2;
                    apertureDict.AddRoundRectangle(pad.Size.Width, pad.Size.Height, radius, rotate);
                }
            }
        }

        public static ApertureDictionary Build(Board board, IList<Layer> layers) {

            ApertureDictionary apertureDict = new ApertureDictionary();
            board.AcceptVisitor(new DefineAperturesVisitor(layers, apertureDict));

            return apertureDict;
        }
    }
}
