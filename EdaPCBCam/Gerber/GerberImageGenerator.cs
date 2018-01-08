﻿namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using MikroPic.EdaTools.v1.Cam.Gerber.Builder;
    using MikroPic.EdaTools.v1.Cam.Gerber.Infrastructure;
    using MikroPic.EdaTools.v1.Pcb.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Windows;

    public sealed class GerberImageGenerator : GerberGenerator {

        public enum ImageType {
            Top,
            Bottom,
            TopSolderMask,
            BottomSolderMask,
            TopCream,
            BottomCream,
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
        public void Generate(Board board, IList<Layer> layers, ImageType imageType, string fileName) {

            if (board == null)
                throw new ArgumentNullException("board");

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            // Escriu el fitxer de sortida
            //
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {

                    // Crea el diccionari d'apertures
                    //
                    ApertureDictionary apertureDict = ApertureDictionaryBuilder.Build(board, layers);

                    GerberBuilder gb = new GerberBuilder(writer);

                    // Definicio de capcelera, unitats, format, etc.
                    //
                    gb.Comment("EdaTools v1.0.");
                    gb.Comment("EdaTools CAM processor. Gerber generator.");
                    gb.Comment("BEGIN HEADER");
                    switch (imageType) {
                        case ImageType.Top:
                            gb.Attribute(String.Format(".FileFunction,Copper,L{0},{1},Signal", 1, "Top"));
                            gb.Attribute(".FilePolarity,Positive");
                            break;

                        case ImageType.Bottom:
                            gb.Attribute(String.Format(".FileFunction,Copper,L{0},{1},Signal", 2, "Bot"));
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

                        case ImageType.TopCream:
                            gb.Attribute(".FileFunction,Paste,Top");
                            gb.Attribute(".FilePolarity,Negative");
                            break;

                        case ImageType.BottomCream:
                            gb.Attribute(".FileFunction,Paste,Bot");
                            gb.Attribute(".FilePolarity,Negative");
                            break;

                        case ImageType.Profile:
                            gb.Attribute(".FileFunction,Profile,NP");
                            gb.Attribute(".FilePolarity,Positive");
                            break;
                    }
                    gb.Attribute(".Part,Single");
                    gb.SetUnits(Units.Milimeters);
                    gb.SetCoordinateFormat(8, 5);
                    gb.LoadPolarity(Polarity.Dark);
                    gb.Comment("END HEADER");

                    // Definicio de macros per les apertures
                    //
                    gb.Comment("BEGIN MACROS");
                    gb.DefineMacros(apertureDict.Macros);
                    gb.Comment("END MACROS");

                    // Definicio de les apertures
                    //
                    gb.Comment("BEGIN APERTURES");
                    gb.DefineApertures(apertureDict.Apertures);
                    gb.Comment("END APERTURES");

                    // Definicio de les regions
                    //
                    gb.Comment("BEGIN POLYGONS");
                    board.AcceptVisitor(new RegionGeneratorVisitor(gb, layers, apertureDict));
                    gb.Comment("END POLYGONS");

                    // Definicio de la imatge
                    //
                    gb.Comment("BEGIN IMAGE");
                    board.AcceptVisitor(new ImageGeneratorVisitor(gb, layers, apertureDict));
                    gb.Comment("END IMAGE");

                    // Final
                    //
                    gb.EndFile();
                }
            }
        }

        /// <summary>
        /// Clase utilitzada per la generacio de la imatge.
        /// </summary>
        /// 
        private sealed class ImageGeneratorVisitor : BoardVisitor {

            private readonly GerberBuilder gb;
            private readonly IList<Layer> layers;
            private readonly ApertureDictionary apertureDict;

            public ImageGeneratorVisitor(GerberBuilder gb, IList<Layer> layers, ApertureDictionary apertureDict) {

                this.gb = gb;
                this.layers = layers;
                this.apertureDict = apertureDict;
            }

            public override void Visit(LineElement line) {

                if (line.IsOnAnyLayer(layers)) {
                    Aperture ap = apertureDict.GetCircleAperture(Math.Max(line.Thickness, 0.01));
                    gb.SelectAperture(ap);
                    System.Windows.Point p1 = line.StartPosition;
                    if (VisitingPart != null)
                        p1 = VisitingPart.Transform(p1);
                    gb.MoveTo(p1);
                    System.Windows.Point p2 = line.EndPosition;
                    if (VisitingPart != null)
                        p2 = VisitingPart.Transform(p2);
                    gb.LineTo(p2);
                }
            }

            public override void Visit(ArcElement arc) {

                if (arc.IsOnAnyLayer(layers)) {
                    Aperture ap = apertureDict.GetCircleAperture(Math.Max(arc.Thickness, 0.01));
                    gb.SelectAperture(ap);
                    System.Windows.Point p1 = VisitingPart.Transform(arc.StartPosition);
                    gb.MoveTo(p1);
                    System.Windows.Point p2 = VisitingPart.Transform(arc.EndPosition);
                    System.Windows.Point c = VisitingPart.Transform(arc.Center);
                    gb.ArcTo(
                        p2.X, p2.Y,
                        c.X - p1.X, c.Y - p1.Y,
                        arc.Angle < 0 ? ArcDirection.CW : ArcDirection.CCW);
                }
            }

            public override void Visit(RectangleElement rectangle) {

                if (rectangle.IsOnAnyLayer(layers)) {
                    if (rectangle.Thickness == 0) {
                        Angle rotate = rectangle.Rotation + (VisitingPart != null ? VisitingPart.Rotation : Angle.FromDegrees(0));
                        Aperture ap = apertureDict.GetRectangleAperture((double)rectangle.Size.Width, (double)rectangle.Size.Height, rotate.Degrees);
                        gb.SelectAperture(ap);
                        System.Windows.Point p = VisitingPart.Transform((System.Windows.Point)rectangle.Position);
                        gb.FlashAt(p);
                    }
                }
            }

            public override void Visit(CircleElement circle) {

                if (circle.IsOnAnyLayer(layers)) {
                    if (circle.Thickness == 0) {
                        Aperture ap = apertureDict.GetCircleAperture(circle.Diameter);
                        gb.SelectAperture(ap);
                        System.Windows.Point p = VisitingPart.Transform(circle.Position);
                        gb.FlashAt(p);
                    }
                }
            }

            public override void Visit(ViaElement via) {

                if (via.IsOnAnyLayer(layers)) {
                    Aperture ap = null;
                    switch (via.Shape) {
                        default:
                        case ViaElement.ViaShape.Circular:
                            ap = apertureDict.GetCircleAperture(via.OuterSize);
                            break;

                        case ViaElement.ViaShape.Square:
                            ap = apertureDict.GetRectangleAperture(via.OuterSize, via.OuterSize, 0);
                            break;

                        case ViaElement.ViaShape.Octogonal:
                            ap = apertureDict.GetOctagonAperture(via.OuterSize, 0);
                            break;
                    }
                    gb.SelectAperture(ap);
                    gb.FlashAt(via.Position);
                }
            }

            /// <summary>
            /// Visita un element de tipus ThPadElement
            /// </summary>
            /// <param name="pad">El element a visitar.</param>
            /// 
            public override void Visit(ThPadElement pad) {

                if (pad.IsOnAnyLayer(layers)) {
                    Angle rotate = pad.Rotation + (VisitingPart != null ? VisitingPart.Rotation : Angle.FromDegrees(0));
                    Aperture ap = null;
                    switch (pad.Shape) {
                        case ThPadElement.ThPadShape.Circular:
                            ap = apertureDict.GetCircleAperture(pad.Size);
                            break;

                        case ThPadElement.ThPadShape.Square:
                            ap = apertureDict.GetRectangleAperture(pad.Size, pad.Size, rotate.Degrees);
                            break;

                        case ThPadElement.ThPadShape.Octogonal:
                            ap = apertureDict.GetOctagonAperture(pad.Size, rotate.Degrees);
                            break;

                        case ThPadElement.ThPadShape.Oval:
                            ap = apertureDict.GetOvalAperture(pad.Size * 2, pad.Size, rotate.Degrees);
                            break;
                    }
                    gb.SelectAperture(ap);
                    System.Windows.Point p = VisitingPart.Transform((System.Windows.Point)pad.Position);
                    gb.FlashAt(p);
                }
            }

            /// <summary>
            /// Visita un element de tipus SmdPad.
            /// </summary>
            /// <param name="pad">El element a visitar.</param>
            /// 
            public override void Visit(SmdPadElement pad) {

                if (pad.IsOnAnyLayer(layers)) {
                    Angle rotation = pad.Rotation + (VisitingPart != null ? VisitingPart.Rotation : Angle.FromDegrees(0));
                    double radius = pad.Roundnes * Math.Min((double)pad.Size.Width, (double)pad.Size.Height) / 2;
                    Aperture ap = radius == 0 ?
                        apertureDict.GetRectangleAperture((double)pad.Size.Width, (double)pad.Size.Height, rotation.Degrees) :
                        apertureDict.GetRoundRectangleAperture((double)pad.Size.Width, (double)pad.Size.Height, radius, rotation.Degrees);
                    gb.SelectAperture(ap);
                    System.Windows.Point p = VisitingPart.Transform((System.Windows.Point)pad.Position);
                    gb.FlashAt(p);
                }
            }
        }

        private sealed class RegionGeneratorVisitor : BoardVisitor {

            private readonly GerberBuilder gb;
            private readonly IList<Layer> layers;
            private readonly ApertureDictionary apertureDict;

            public RegionGeneratorVisitor(GerberBuilder gb, IList<Layer> layers, ApertureDictionary apertureDict) {

                this.gb = gb;
                this.layers = layers;
                this.apertureDict = apertureDict;
            }

            public override void Visit(RegionElement region) {

                if (region.IsOnAnyLayer(layers)) {

                    if (VisitingSignal != null) {

                        double clearance = 0.15;

                        Polygon regionPolygon = PolygonBuilder.Build(region);
                        IEnumerable<Polygon> holePolygons = PolygonListBuilder.Build(VisitingBoard, layers[0], regionPolygon, clearance + (region.Thickness / 2));

                        PolygonNode polygonTree = PolygonProcessor.ClipExtended(regionPolygon, holePolygons, PolygonProcessor.ClipOperation.Diference);
                        ProcessPolygonTree(polygonTree, region.Thickness);
                    }
                }
            }

            private void ProcessPolygonTree(PolygonNode polygonTree, double thickness) {

                ProcessPolygonTree(polygonTree, 0, thickness);
            }

            private void ProcessPolygonTree(PolygonNode polygonTree, int level, double thickness) {

                // Procesa el poligon
                //
                if (polygonTree.Polygon != null) {

                    // Dibuixa el contingut de la regio
                    //
                    gb.LoadPolarity((level % 2) == 0 ? Polarity.Clear : Polarity.Dark);
                    gb.BeginRegion();
                    gb.Region(polygonTree.Polygon, true);
                    gb.EndRegion();

                    // Dibuixa el perfil de la regio
                    //
                    Aperture ap = apertureDict.GetCircleAperture(thickness);
                    gb.SelectAperture(ap);
                    gb.LoadPolarity(Polarity.Dark);
                    gb.Polygon(polygonTree.Polygon);
                }

                // Processa els fills. Amb level < 2 evitem els poligons orfres
                //
                if (polygonTree.HasChilds && (level < 2))
                    foreach (PolygonNode polygonTreeChild in polygonTree.Childs)
                        ProcessPolygonTree(polygonTreeChild, level + 1, thickness);
            }
        }
    }
}