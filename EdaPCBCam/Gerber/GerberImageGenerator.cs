﻿namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using MikroPic.EdaTools.v1.Cam.Gerber.Builder;
    using MikroPic.EdaTools.v1.Cam.Gerber.Builder.Apertures;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Windows;

    public class GerberImageGenerator: GerberGenerator { 

        public enum ImageType {
            Top,
            Bottom,
            TopSolderMask,
            BottomSolderMask,
            Profile,
            TopLegend,
            BottomLegend
        }

        public GerberImageGenerator() {
        }

        /// <summary>
        /// Genera un fitxer gerber.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// <param name="layers">Llista de capes a procesar.</param>
        /// <param name="fileName">Nom del fitxer de sortida.</param>
        /// 
        public virtual void Generate(Board board, IList<Layer> layers, ImageType imageType, string fileName) {

            if (board == null)
                throw new ArgumentNullException("board");

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            // Escriu el fitxer de sortida
            //
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {

                    ApertureDictionary apertures = new ApertureDictionary();
                    board.AcceptVisitor(new DefineAperturesVisitor(layers, apertures));

                    GerberBuilder gb = new GerberBuilder(writer);

                    // Definicio de capcelera, unitats, format, etc.
                    //
                    gb.Comment("EdaTools v1.0.");
                    gb.Comment("EdaTools CAM processor. Gerber generator.");
                    gb.Comment("BEGIN HEADER");
                    switch (imageType) {
                        case ImageType.Top:
                            gb.Attribute(".FileFunction,Copper,L1,Top,Signal");
                            gb.Attribute(".FilePolarity,Positive");
                            break;

                        case ImageType.Bottom:
                            gb.Attribute(".FileFunction,Copper,L2,Bottom,Signal");
                            gb.Attribute(".FilePolarity,Positive");
                            break;

                        case ImageType.TopSolderMask:
                            gb.Attribute(".FileFunction,Soldermask,Top");
                            gb.Attribute(".FilePolarity,Negative");
                            break;

                        case ImageType.BottomSolderMask:
                            gb.Attribute(".FileFunction,Soldermask,Bot");
                            gb.Attribute(".FilePolarity,Negative");
                            break;

                        case ImageType.Profile:
                            gb.Attribute(".FileFunction,Profile,NP");
                            gb.Attribute(".FilePolarity,Positive");
                            break;
                    }
                    gb.SetUnits(Units.Milimeters);
                    gb.SetCoordinateFormat(8, 5);
                    gb.SetOffset(0, 0);
                    gb.SetPolarity(true);
                    gb.SetAperturePolarity(AperturePolarity.Dark);
                    gb.SetApertureRotation(0);
                    gb.Comment("END HEADER");

                    // Definicio de macros per les apertures
                    //
                    gb.Comment("BEGIN MACROS");
                    gb.DefineMacros(apertures.Macros);
                    gb.Comment("END MACROS");

                    // Definicio de les apertures
                    //
                    gb.Comment("BEGIN APERTURES");
                    gb.DefineApertures(apertures.Apertures);
                    gb.Comment("END APERTURES");

                    // Definicio de la imatge
                    //
                    gb.Comment("BEGIN IMAGE");
                    board.AcceptVisitor(new ImageGeneratorVisitor(gb, layers, apertures));
                    gb.Comment("END IMAGE");

                    // Final
                    //
                    gb.EndFile();
                }
            }
        }

        private sealed class DefineAperturesVisitor : DefaultVisitor {

            private readonly IList<Layer> layers;
            private readonly ApertureDictionary apertures;

            public DefineAperturesVisitor(IList<Layer> layers, ApertureDictionary apertures) {

                this.layers = layers;
                this.apertures = apertures;
            }

            public override void Visit(LineElement line) {

                if (line.InAnyLayer(layers)) 
                    apertures.AddCircle(Math.Max(line.Thickness, 0.01));
            }

            public override void Visit(ArcElement arc) {

                if (arc.InAnyLayer(layers))
                    apertures.AddCircle(Math.Max(arc.Thickness, 0.01));
            }

            public override void Visit(RectangleElement rectangle) {

                if (rectangle.InAnyLayer(layers)) {
                    if (rectangle.Thickness == 0) {
                        double rotate = rectangle.Rotate + (VisitingPart != null ? VisitingPart.Rotate : 0);
                        apertures.AddRectangle(rectangle.Size.Width, rectangle.Size.Height, rotate);
                    }
                }
            }

            public override void Visit(CircleElement circle) {

                if (circle.InAnyLayer(layers)) {
                    if (circle.Thickness == 0)
                        apertures.AddCircle(circle.Diameter);
                }
            }

            public override void Visit(ViaElement via) {

                if (via.InAnyLayer(layers)) {
                    switch (via.Shape) {
                        case ViaElement.ViaShape.Circular:
                            apertures.AddCircle(via.Size);
                            break;

                        case ViaElement.ViaShape.Square:
                            apertures.AddRectangle(via.Size, via.Size, 0);
                            break;

                        case ViaElement.ViaShape.Octogonal:
                            apertures.AddOctagon(via.Size, 0);
                            break;
                    }
                }
            }

            public override void Visit(ThPadElement pad) {

                if (pad.InAnyLayer(layers)) {
                    double rotate = pad.Rotate + (VisitingPart != null ? VisitingPart.Rotate : 0);
                    switch (pad.Shape) {
                        case ThPadElement.ThPadShape.Circular:
                            apertures.AddCircle(pad.Size);
                            break;

                        case ThPadElement.ThPadShape.Square:
                            apertures.AddRectangle(pad.Size, pad.Size, rotate);
                            break;

                        case ThPadElement.ThPadShape.Octogonal:
                            apertures.AddOctagon(pad.Size, rotate);
                                break;

                        case ThPadElement.ThPadShape.Oval:
                            apertures.AddOval(pad.Size * 2, pad.Size, rotate);
                            break;
                    }
                }
            }

            public override void Visit(SmdPadElement pad) {

                if (pad.InAnyLayer(layers)) {
                    double rotate = pad.Rotate + (VisitingPart != null ? VisitingPart.Rotate : 0);
                    double radius = (pad.Roundnes - 0.01) * Math.Min(pad.Size.Width, pad.Size.Height) / 2;
                    apertures.AddRoundRectangle(pad.Size.Width, pad.Size.Height, radius, rotate);
                }
            }
        }

        private sealed class ImageGeneratorVisitor : DefaultVisitor {

            private readonly GerberBuilder gb;
            private readonly IList<Layer> layers;
            private readonly ApertureDictionary apertures;

            public ImageGeneratorVisitor(GerberBuilder gb, IList<Layer> layers, ApertureDictionary apertures) {

                this.gb = gb;
                this.layers = layers;
                this.apertures = apertures;
            }

            public override void Visit(LineElement line) {

                if (line.InAnyLayer(layers)) {
                    Aperture ap = apertures.GetCircleAperture(Math.Max(line.Thickness, 0.01));
                    gb.SelectAperture(ap);
                    Point p1 = line.GetPosition(VisitingPart);
                    gb.Operation(p1.X, p1.Y, OperationCode.Move);
                    Point p2 = line.GetEndPosition(VisitingPart);
                    gb.SetLinealInterpolationMode();
                    gb.Operation(p2.X, p2.Y, OperationCode.Interpolate);
                }
            }

            public override void Visit(ArcElement arc) {

                if (arc.InAnyLayer(layers)) {
                    Aperture ap = apertures.GetCircleAperture(Math.Max(arc.Thickness, 0.01));
                    gb.SelectAperture(ap);
                    Point p1 = arc.GetPosition(VisitingPart);
                    gb.Operation(p1.X, p1.Y, OperationCode.Move);
                    Point p2 = arc.GetEndPosition(VisitingPart);
                    Point c = arc.GetCenter(VisitingPart);
                    if (arc.Angle <= 90) {
                        gb.SetCircularInterpolationMode(CircularInterpolationQuadrant.Single, CircularInterpolationDirection.CW);
                        gb.Operation(p2.X, p2.Y, Math.Abs(c.X - p1.X), Math.Abs(c.Y - p1.Y), OperationCode.Interpolate);
                    }
                }
            }

            public override void Visit(RectangleElement rectangle) {

                if (rectangle.InAnyLayer(layers)) {
                    if (rectangle.Thickness == 0) {
                        double rotate = rectangle.Rotate + (VisitingPart != null ? VisitingPart.Rotate : 0);
                        Aperture ap = apertures.GetRectangleAperture(rectangle.Size.Width, rectangle.Size.Height, rotate);
                        gb.SelectAperture(ap);
                        Point p = rectangle.GetPosition(VisitingPart);
                        gb.Operation(p.X, p.Y, OperationCode.Flash);
                    }
                }
            }

            public override void Visit(CircleElement circle) {

                if (circle.InAnyLayer(layers)) {
                    if (circle.Thickness == 0) {
                        Aperture ap = apertures.GetCircleAperture(circle.Diameter);
                        gb.SelectAperture(ap);
                        Point p = circle.GetPosition(VisitingPart);
                        gb.Operation(p.X, p.Y, OperationCode.Flash);
                    }
                }
            }

            public override void Visit(ViaElement via) {

                if (via.InAnyLayer(layers)) {
                    Aperture ap = null;
                    switch (via.Shape) {
                        default:
                        case ViaElement.ViaShape.Circular:
                            ap = apertures.GetCircleAperture(via.Size);
                            break;

                        case ViaElement.ViaShape.Square:
                            ap = apertures.GetRectangleAperture(via.Size, via.Size, 0);
                            break;

                        case ViaElement.ViaShape.Octogonal:
                            ap = apertures.GetOctagonAperture(via.Size, 0);
                            break;
                    }
                    gb.SelectAperture(ap);
                    gb.Operation(via.Position.X, via.Position.Y, OperationCode.Flash);
                }
            }

            /// <summary>
            /// Visita un element de tipus ThPadElement
            /// </summary>
            /// <param name="pad">El element a visitar.</param>
            /// 
            public override void Visit(ThPadElement pad) {

                if (pad.InAnyLayer(layers)) {
                    double rotate = pad.Rotate + (VisitingPart != null ? VisitingPart.Rotate : 0);
                    Aperture ap = null;
                    switch (pad.Shape) {
                        case ThPadElement.ThPadShape.Circular:
                            ap = apertures.GetCircleAperture(pad.Size);
                            break;

                        case ThPadElement.ThPadShape.Square:
                            ap = apertures.GetRectangleAperture(pad.Size, pad.Size, rotate);
                            break;

                        case ThPadElement.ThPadShape.Octogonal:
                            ap = apertures.GetOctagonAperture(pad.Size, rotate);
                            break;

                        case ThPadElement.ThPadShape.Oval:
                            ap = apertures.GetOvalAperture(pad.Size * 2, pad.Size, rotate);
                            break;
                    }
                    gb.SelectAperture(ap);
                    Point p = pad.GetPosition(VisitingPart);
                    gb.Operation(p.X, p.Y, OperationCode.Flash);
                }
            }

            /// <summary>
            /// Visita un element de tipus SmdPad.
            /// </summary>
            /// <param name="pad">El element a visitar.</param>
            /// 
            public override void Visit(SmdPadElement pad) {

                if (pad.InAnyLayer(layers)) {
                    double rotate = pad.Rotate + (VisitingPart != null ? VisitingPart.Rotate : 0);
                    double radius = (pad.Roundnes - 0.01) * Math.Min(pad.Size.Width, pad.Size.Height) / 2;
                    Aperture ap = apertures.GetRoundRectangleAperture(pad.Size.Width, pad.Size.Height, radius, rotate);
                    gb.SelectAperture(ap);
                    Point p = pad.GetPosition(VisitingPart);
                    gb.Operation(p.X, p.Y, OperationCode.Flash);
                }
            }
        }
    }
}
