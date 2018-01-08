namespace MikroPic.EdaTools.v1.Cam.Gerber.Infrastructure {

    using System;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Pcb.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using MikroPic.EdaTools.v1.Cam.Gerber;

    internal static class ApertureDictionaryBuilder {

        /// <summary>
        /// Clase utilitzada per la inicialitzacio del diccionari d'apertures.
        /// </summary>
        /// 
        private sealed class DefineAperturesVisitor : BoardVisitor {

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
                    apertureDict.DefineCircleAperture(Math.Max(line.Thickness, 0.01));
            }

            public override void Visit(ArcElement arc) {

                if (arc.IsOnAnyLayer(layers))
                    apertureDict.DefineCircleAperture(Math.Max(arc.Thickness, 0.01));
            }

            public override void Visit(RectangleElement rectangle) {

                if (rectangle.IsOnAnyLayer(layers)) {
                    if (rectangle.Thickness == 0) {
                        Angle rotate = rectangle.Rotation + (VisitingPart != null ? VisitingPart.Rotation : Angle.FromDegrees(0));
                        apertureDict.DefineRectangleAperture((double)rectangle.Size.Width, (double)rectangle.Size.Height, rotate.Degrees);
                    }
                }
            }

            public override void Visit(CircleElement circle) {

                if (circle.IsOnAnyLayer(layers)) {
                    if (circle.Thickness == 0)
                        apertureDict.DefineCircleAperture(circle.Diameter);
                }
            }

            public override void Visit(ViaElement via) {

                if (via.IsOnAnyLayer(layers)) {
                    switch (via.Shape) {
                        case ViaElement.ViaShape.Circular:
                            apertureDict.DefineCircleAperture(via.OuterSize);
                            break;

                        case ViaElement.ViaShape.Square:
                            apertureDict.DefineRectangleAperture(via.OuterSize, via.OuterSize, 0);
                            break;

                        case ViaElement.ViaShape.Octogonal:
                            apertureDict.DefineOctagonAperture(via.OuterSize, 0);
                            break;
                    }
                }
            }

            public override void Visit(ThPadElement pad) {

                if (pad.IsOnAnyLayer(layers)) {
                    Angle rotate = pad.Rotation + (VisitingPart != null ? VisitingPart.Rotation : Angle.FromDegrees(0));
                    switch (pad.Shape) {
                        case ThPadElement.ThPadShape.Circular:
                            apertureDict.DefineCircleAperture(pad.Size);
                            break;

                        case ThPadElement.ThPadShape.Square:
                            apertureDict.DefineRectangleAperture(pad.Size, pad.Size, rotate.Degrees);
                            break;

                        case ThPadElement.ThPadShape.Octogonal:
                            apertureDict.DefineOctagonAperture(pad.Size, rotate.Degrees);
                            break;

                        case ThPadElement.ThPadShape.Oval:
                            apertureDict.DefineOvalAperture(pad.Size * 2, pad.Size, rotate.Degrees);
                            break;
                    }
                }
            }

            public override void Visit(SmdPadElement pad) {

                if (pad.IsOnAnyLayer(layers)) {
                    Angle rotation = pad.Rotation + (VisitingPart != null ? VisitingPart.Rotation : Angle.FromDegrees(0));
                    double radius = pad.Roundnes * Math.Min((double)pad.Size.Width, (double)pad.Size.Height) / 2;
                    if (radius == 0)
                        apertureDict.DefineRectangleAperture((double)pad.Size.Width, (double)pad.Size.Height, rotation.Degrees);
                    else
                        apertureDict.DefineRoundRectangleAperture((double)pad.Size.Width, (double)pad.Size.Height, radius, rotation.Degrees);
                }
            }

            public override void Visit(RegionElement region) {

                if (region.IsOnAnyLayer(layers))
                    apertureDict.DefineCircleAperture(region.Thickness);
            }
        }

        public static ApertureDictionary Build(Board board, IList<Layer> layers) {

            ApertureDictionary apertureDict = new ApertureDictionary();
            board.AcceptVisitor(new DefineAperturesVisitor(layers, apertureDict));

            return apertureDict;
        }
    }
}
