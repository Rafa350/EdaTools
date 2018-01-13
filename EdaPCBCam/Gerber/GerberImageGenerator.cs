﻿namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using MikroPic.EdaTools.v1.Cam.Gerber.Builder;
    using MikroPic.EdaTools.v1.Cam.Gerber.Infrastructure;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Media;

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
                    board.AcceptVisitor(new RegionGeneratorVisitor(gb, board, layers, apertureDict));
                    gb.Comment("END POLYGONS");

                    // Definicio de la imatge
                    //
                    gb.Comment("BEGIN IMAGE");
                    board.AcceptVisitor(new ImageGeneratorVisitor(gb, board, layers, apertureDict));
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
            private readonly Board board;
            private readonly IEnumerable<Layer> layers;
            private readonly ApertureDictionary apertureDict;
            private Matrix partMatrix = Matrix.Identity;
            private double partRotation = 0;

            public ImageGeneratorVisitor(GerberBuilder gb, Board board, IEnumerable<Layer> layers, ApertureDictionary apertureDict) {

                this.gb = gb;
                this.board = board;
                this.layers = layers;
                this.apertureDict = apertureDict;
            }

            /// <summary>
            /// Visita un element Line
            /// </summary>
            /// <param name="line">El element a visitar.</param>
            /// 
            public override void Visit(LineElement line) {

                if (board.IsOnAnyLayer(line, layers)) {
                    Aperture ap = apertureDict.GetCircleAperture(Math.Max(line.Thickness, 0.01));
                    gb.SelectAperture(ap);
                    Point p1 = partMatrix.Transform(line.StartPosition);
                    gb.MoveTo(p1);
                    Point p2 = partMatrix.Transform(line.EndPosition);
                    gb.LineTo(p2);
                }
            }

            /// <summary>
            /// Visita un element de tipus Arc.
            /// </summary>
            /// <param name="arc">El element a visitar.</param>
            /// 
            public override void Visit(ArcElement arc) {

                if (board.IsOnAnyLayer(arc, layers)) {
                    Aperture ap = apertureDict.GetCircleAperture(Math.Max(arc.Thickness, 0.01));
                    gb.SelectAperture(ap);
                    Point p1 = partMatrix.Transform(arc.StartPosition);
                    gb.MoveTo(p1);
                    Point p2 = partMatrix.Transform(arc.EndPosition);
                    Point c = partMatrix.Transform(arc.Center);
                    gb.ArcTo(
                        p2.X, p2.Y,
                        c.X - p1.X, c.Y - p1.Y,
                        arc.Angle < 0 ? ArcDirection.CW : ArcDirection.CCW);
                }
            }

            /// <summary>
            /// Visita un element de tipus Rectangle.
            /// </summary>
            /// <param name="rectangle">El element a visitar.</param>
            /// 
            public override void Visit(RectangleElement rectangle) {

                if (board.IsOnAnyLayer(rectangle, layers)) {
                    if (rectangle.Thickness == 0) {
                        double rotate = rectangle.Rotation;
                        if (VisitingPart != null)
                            rotate += VisitingPart.Rotation;
                        Aperture ap = apertureDict.GetRectangleAperture(rectangle.Size.Width, rectangle.Size.Height, rotate);
                        gb.SelectAperture(ap);
                        Point p = partMatrix.Transform(rectangle.Position);
                        gb.FlashAt(p);
                    }
                }
            }

            public override void Visit(CircleElement circle) {

                if (board.IsOnAnyLayer(circle, layers)) {
                    if (circle.Thickness == 0) {
                        Aperture ap = apertureDict.GetCircleAperture(circle.Diameter);
                        gb.SelectAperture(ap);
                        Point p = partMatrix.Transform(circle.Position);
                        gb.FlashAt(p);
                    }
                }
            }

            public override void Visit(ViaElement via) {

                if (board.IsOnAnyLayer(via, layers)) {
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
            /// Visita un element de tipus ThPad
            /// </summary>
            /// <param name="pad">El element a visitar.</param>
            /// 
            public override void Visit(ThPadElement pad) {

                if (board.IsOnAnyLayer(pad, layers)) {
                    double rotate = pad.Rotation;
                    if (VisitingPart != null)
                        rotate += VisitingPart.Rotation;
                    Aperture ap = null;
                    switch (pad.Shape) {
                        case ThPadElement.ThPadShape.Circular:
                            ap = apertureDict.GetCircleAperture(pad.Size);
                            break;

                        case ThPadElement.ThPadShape.Square:
                            ap = apertureDict.GetRectangleAperture(pad.Size, pad.Size, rotate);
                            break;

                        case ThPadElement.ThPadShape.Octogonal:
                            ap = apertureDict.GetOctagonAperture(pad.Size, rotate);
                            break;

                        case ThPadElement.ThPadShape.Oval:
                            ap = apertureDict.GetOvalAperture(pad.Size * 2, pad.Size, rotate);
                            break;
                    }
                    gb.SelectAperture(ap);
                    Point p = partMatrix.Transform(pad.Position);
                    gb.FlashAt(p);
                }
            }

            /// <summary>
            /// Visita un element de tipus SmdPad
            /// </summary>
            /// <param name="pad">El element a visitar.</param>
            /// 
            public override void Visit(SmdPadElement pad) {

                if (board.IsOnAnyLayer(pad, layers)) {
                    double rotation = pad.Rotation + partRotation;
                    double radius = pad.Roundnes * Math.Min(pad.Size.Width, pad.Size.Height) / 2;
                    Aperture ap = radius == 0 ?
                        apertureDict.GetRectangleAperture(pad.Size.Width, pad.Size.Height, rotation) :
                        apertureDict.GetRoundRectangleAperture(pad.Size.Width, pad.Size.Height, radius, rotation);
                    gb.SelectAperture(ap);
                    Point p = partMatrix.Transform(pad.Position);
                    gb.FlashAt(p);
                }
            }

            /// <summary>
            /// Visita un objecte Part
            /// </summary>
            /// <param name="part">El objecte a visitar</param>
            /// 
            public override void Visit(Part part) {

                partMatrix = part.Transformation;
                partRotation = part.Rotation;

                base.Visit(part);

                partMatrix = Matrix.Identity;
                partRotation = 0;
            }
        }

        private sealed class RegionGeneratorVisitor : BoardVisitor {

            private readonly GerberBuilder gb;
            private readonly Board board;
            private readonly IEnumerable<Layer> layers;
            private readonly ApertureDictionary apertureDict;

            public RegionGeneratorVisitor(GerberBuilder gb, Board board, IEnumerable<Layer> layers, ApertureDictionary apertureDict) {

                this.gb = gb;
                this.board = board;
                this.layers = layers;
                this.apertureDict = apertureDict;
            }

            public override void Visit(RegionElement region) {

                if (board.IsOnAnyLayer(region, layers)) {

                    PolygonNode polygonTree = CreatePolygonTree(board, region);
                    ProcessPolygonTree(polygonTree, region.Thickness);
                }
            }

            /// <summary>
            /// Crea el poligon de la regio, amb els forats corresponents
            /// </summary>
            /// <param name="board">La placa</param>
            /// <param name="region">La regio.</param>
            /// <returns>La coleccio de poligons.</returns>
            /// 
            private PolygonNode CreatePolygonTree(Board board, RegionElement region) {

                Polygon regionPolygon = PolygonBuilder.Build(region);
                IEnumerable<Layer> regionLayers = board.GetLayers(region);
                Signal regionSignal = board.GetSignal(region, false);

                double inflate = 0.15 + region.Thickness / 2;
                List<Polygon> holePolygons = new List<Polygon>();

                // Procesa els elements de la placa
                //
                foreach (Element element in board.Elements) {
                    if ((element != region) && (board.IsOnAnyLayer(element, regionLayers))) {
                        IConectable item = element as IConectable;
                        if ((item == null) || (board.GetSignal(item, false) != regionSignal)) {
                            Polygon elementPolygon = element.GetPolygon(inflate);
                            holePolygons.AddRange(PolygonProcessor.Clip(elementPolygon, regionPolygon, PolygonProcessor.ClipOperation.Intersection));
                        }
                    }
                }

                // Procesa els elements dels blocs
                //
                foreach (Part part in board.Parts) {
                    foreach (Element element in part.Component.Elements) {
                        if ((element != region) && (board.IsOnAnyLayer(element, regionLayers))) {
                            IConectable item = element as IConectable;
                            if ((item == null) || (board.GetSignal(item, false) != regionSignal)) {
                                Polygon elementPolygon = element.GetPolygon(inflate);
                                elementPolygon.Transform(part.Transformation);
                                holePolygons.AddRange(PolygonProcessor.Clip(elementPolygon, regionPolygon, PolygonProcessor.ClipOperation.Intersection));
                            }
                        }
                    }
                }

                return PolygonProcessor.ClipExtended(regionPolygon, holePolygons, PolygonProcessor.ClipOperation.Diference);
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