namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using MikroPic.EdaTools.v1.Cam.Gerber.Builder;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Clase per generar el fitxers gerber d'imatge
    /// </summary>
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

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="board">La placa a procesar.</param>
        /// 
        public GerberImageGenerator(Board board):
            base(board) {
        }

        /// <summary>
        /// Genera un document gerber.
        /// </summary>
        /// <param name="writer">Writer de sortida.</param>
        /// <param name="layers">Llista de capes a procesar.</param>
        /// <param name="fileName">Nom del fitxer de sortida.</param>
        /// 
        public void Generate(TextWriter writer, IEnumerable<Layer> layers, ImageType imageType) {

            if (writer == null)
                throw new ArgumentNullException("writer");

            GerberBuilder gb = new GerberBuilder(writer);

            ApertureDictionary apertures = CreateApertures(layers);

            GenerateFileHeader(gb, imageType);
            GenerateMacros(gb, apertures);
            GenerateApertures(gb, apertures);
            GenerateRergions(gb, layers, apertures);
            GenerateImage(gb, layers, apertures);
            GenerateFileTail(gb);
        }

        /// <summary>
        /// Genera el diccionari d'apertures.
        /// </summary>
        /// <param name="layers">La coleccio de capes a comprobar.</param>
        /// <returns>El diccionari.</returns>
        /// 
        private ApertureDictionary CreateApertures(IEnumerable<Layer> layers) {

            ApertureCreatorVisitor visitor = new ApertureCreatorVisitor(Board, layers);
            visitor.Visit(Board);
            return visitor.Apertures;
        }

        /// <summary>
        /// Genera la capcelera del fitxer.
        /// </summary>
        /// <param name="gb">El generador de codi gerber.</param>
        /// <param name="imageType">Tipus d'imatge a generar.</param>
        /// 
        private void GenerateFileHeader(GerberBuilder gb, ImageType imageType) {

            gb.Comment("EdaTools v1.0.");
            gb.Comment("EdaTools CAM processor. Gerber generator.");
            gb.Comment(String.Format("Start timestamp: {0}", DateTime.Now));
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

                case ImageType.TopLegend:
                    gb.Attribute(".FileFunction,Legend,Top");
                    gb.Attribute(".FilePolarity,Positive");
                    break;

                case ImageType.BottomLegend:
                    gb.Attribute(".FileFunction,Legend,Bot");
                    gb.Attribute(".FilePolarity,Positive");
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
        }

        /// <summary>
        /// Genera el final de fitxer.
        /// </summary>
        /// <param name="gb">El generador de codi gerber.</param>
        /// 
        private void GenerateFileTail(GerberBuilder gb) {

            gb.EndFile();
            gb.Comment(String.Format("End timestamp: {0}", DateTime.Now));
            gb.Comment("END FILE");
        }

        /// <summary>
        /// Genera la seccio d'apertures del fitxer.
        /// </summary>
        /// <param name="gb">El generador de codi gerber.</param>
        /// <param name="apertures">El diccionari d'apertures.</param>
        /// 
        private void GenerateApertures(GerberBuilder gb, ApertureDictionary apertures) {

            gb.Comment("BEGIN APERTURES");
            gb.DefineApertures(apertures.Apertures);
            gb.Comment("END APERTURES");
        }

        /// <summary>
        /// Genera la seccio de macros del fitxer.
        /// </summary>
        /// <param name="gb">El generador de codi gerber.</param>
        /// <param name="apertures">El diccionari d'apertures.</param>
        private void GenerateMacros(GerberBuilder gb, ApertureDictionary apertures) {

            gb.Comment("BEGIN MACROS");
            gb.DefineMacros(apertures.Macros);
            gb.Comment("END MACROS");
        }

        /// <summary>
        /// Genera la seccio de poligons del fitxer.
        /// </summary>
        /// <param name="gb">El generador de codi gerber.</param>
        /// <param name="layers">Les capes a procesar.</param>
        /// <param name="apertures">El diccionari d'apertures.</param>
        /// 
        private void GenerateRergions(GerberBuilder gb, IEnumerable<Layer> layers, ApertureDictionary apertures) {

            gb.Comment("BEGIN POLYGONS");
            IVisitor visitor = new RegionGeneratorVisitor(gb, Board, layers, apertures);
            visitor.Visit(Board);
            gb.Comment("END POLYGONS");
        }

        /// <summary>
        /// Genera la seccio d'imatges del fitxer.
        /// </summary>
        /// <param name="gb">El generador de codi gerber.</param>
        /// <param name="layers">Les capes a procesar.</param>
        /// <param name="apertures">El diccionari d'apertures.</param>
        /// 
        private void GenerateImage(GerberBuilder gb, IEnumerable<Layer> layers, ApertureDictionary apertures) {

            gb.Comment("BEGIN IMAGE");
            IVisitor visitor = new ImageGeneratorVisitor(gb, Board, layers, apertures);
            visitor.Visit(Board);
            gb.Comment("END IMAGE");
        }

        /// <summary>
        /// Clase utilitzada per la creacio d'apertures.
        /// </summary>
        /// 
        private sealed class ApertureCreatorVisitor : BoardVisitor {

            private readonly Board board;
            private readonly IEnumerable<Layer> layers;
            private readonly ApertureDictionary apertures;
            private double localRotation = 0;

            /// <summary>
            /// Constructor de la clase.
            /// </summary>
            /// <param name="board">La placa a procesar.</param>
            /// <param name="layers">El conjunt de capes a consultar.</param>
            /// 
            public ApertureCreatorVisitor(Board board, IEnumerable<Layer> layers) {

                this.board = board;
                this.layers = layers;
                apertures = new ApertureDictionary();
            }

            public override void Visit(LineElement line) {

                if (board.IsOnAnyLayer(line, layers))
                    apertures.DefineCircleAperture(Math.Max(line.Thickness, 0.01));
            }

            public override void Visit(ArcElement arc) {

                if (board.IsOnAnyLayer(arc, layers))
                    apertures.DefineCircleAperture(Math.Max(arc.Thickness, 0.01));
            }

            public override void Visit(RectangleElement rectangle) {

                if (board.IsOnAnyLayer(rectangle, layers)) {
                    if (rectangle.Thickness == 0) {
                        double rotation = localRotation + rectangle.Rotation;
                        apertures.DefineRectangleAperture(rectangle.Size.Width, rectangle.Size.Height, rotation);
                    }
                }
            }

            public override void Visit(CircleElement circle) {

                if (board.IsOnAnyLayer(circle, layers)) {
                    if (circle.Thickness == 0)
                        apertures.DefineCircleAperture(circle.Diameter);
                }
            }

            public override void Visit(ViaElement via) {

                if (board.IsOnAnyLayer(via, layers)) {
                    switch (via.Shape) {
                        case ViaElement.ViaShape.Circular:
                            apertures.DefineCircleAperture(via.OuterSize);
                            break;

                        case ViaElement.ViaShape.Square:
                            apertures.DefineRectangleAperture(via.OuterSize, via.OuterSize, 0);
                            break;

                        case ViaElement.ViaShape.Octogonal:
                            apertures.DefineOctagonAperture(via.OuterSize, 0);
                            break;
                    }
                }
            }

            public override void Visit(ThPadElement pad) {

                if (board.IsOnAnyLayer(pad, layers)) {
                    double rotation = localRotation + pad.Rotation;
                    switch (pad.Shape) {
                        case ThPadElement.ThPadShape.Circular:
                            apertures.DefineCircleAperture(pad.Size);
                            break;

                        case ThPadElement.ThPadShape.Square:
                            apertures.DefineRectangleAperture(pad.Size, pad.Size, rotation);
                            break;

                        case ThPadElement.ThPadShape.Octogonal:
                            apertures.DefineOctagonAperture(pad.Size, rotation);
                            break;

                        case ThPadElement.ThPadShape.Oval:
                            apertures.DefineOvalAperture(pad.Size * 2, pad.Size, rotation);
                            break;
                    }
                }
            }

            public override void Visit(SmdPadElement pad) {

                if (board.IsOnAnyLayer(pad, layers)) {
                    double rotation = localRotation + pad.Rotation;
                    double radius = pad.Roundnes * Math.Min(pad.Size.Width, pad.Size.Height) / 2;
                    if (radius == 0)
                        apertures.DefineRectangleAperture(pad.Size.Width, pad.Size.Height, rotation);
                    else
                        apertures.DefineRoundRectangleAperture(pad.Size.Width, pad.Size.Height, radius, rotation);
                }
            }

            public override void Visit(RegionElement region) {

                if (board.IsOnAnyLayer(region, layers))
                    apertures.DefineCircleAperture(region.Thickness);
            }

            public override void Visit(Part part) {

                localRotation = part.Rotation;
                base.Visit(part);
                localRotation = 0;
            }

            /// <summary>
            /// Obte el diccionari d'apertures generat.
            /// </summary>
            /// 
            public ApertureDictionary Apertures {
                get {
                    return apertures;
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
            private Matrix localTransformation = Matrix.Identity;
            private double localRotation = 0;

            /// <summary>
            /// Construcxtor del objecte.
            /// </summary>
            /// <param name="gb">L'bjecte GerberBuilder.</param>
            /// <param name="board">La placa a procesar.</param>
            /// <param name="layers">Capes a tenir en compte.</param>
            /// <param name="apertureDict">Diccionari d'apertures.</param>
            /// 
            public ImageGeneratorVisitor(GerberBuilder gb, Board board, IEnumerable<Layer> layers, ApertureDictionary apertureDict) {

                this.gb = gb;
                this.board = board;
                this.layers = layers;
                this.apertureDict = apertureDict;
            }

            /// <summary>
            /// Visita un element Line
            /// </summary>
            /// <param name="line">L'element a visitar.</param>
            /// 
            public override void Visit(LineElement line) {

                if (board.IsOnAnyLayer(line, layers)) {

                    Aperture ap = apertureDict.GetCircleAperture(Math.Max(line.Thickness, 0.01));
                    gb.SelectAperture(ap);

                    Point p1 = localTransformation.Transform(line.StartPosition);
                    Point p2 = localTransformation.Transform(line.EndPosition);

                    gb.MoveTo(p1);
                    gb.LineTo(p2);
                }
            }

            /// <summary>
            /// Visita un element de tipus Arc.
            /// </summary>
            /// <param name="arc">L' element a visitar.</param>
            /// 
            public override void Visit(ArcElement arc) {

                if (board.IsOnAnyLayer(arc, layers)) {

                    Aperture ap = apertureDict.GetCircleAperture(Math.Max(arc.Thickness, 0.01));
                    gb.SelectAperture(ap);

                    Point p1 = localTransformation.Transform(arc.StartPosition);
                    Point p2 = localTransformation.Transform(arc.EndPosition);
                    Point c = localTransformation.Transform(arc.Center);

                    gb.MoveTo(p1);
                    gb.ArcTo(
                        p2.X,
                        p2.Y,
                        c.X - p1.X,
                        c.Y - p1.Y,
                        arc.Angle < 0 ? ArcDirection.CW : ArcDirection.CCW);
                }
            }

            /// <summary>
            /// Visita un element de tipus Rectangle.
            /// </summary>
            /// <param name="rectangle">L'element a visitar.</param>
            /// 
            public override void Visit(RectangleElement rectangle) {

                if (board.IsOnAnyLayer(rectangle, layers)) {

                    if (rectangle.Thickness == 0) {
                        double rotate = localRotation + rectangle.Rotation;
                        Aperture ap = apertureDict.GetRectangleAperture(rectangle.Size.Width, rectangle.Size.Height, rotate);
                        gb.SelectAperture(ap);

                        Point p = localTransformation.Transform(rectangle.Position);
                        gb.FlashAt(p);
                    }
                }
            }

            /// <summary>
            /// Visita un element de tipus cercle
            /// </summary>
            /// <param name="circle">L'element a visitar.</param>
            /// 
            public override void Visit(CircleElement circle) {

                if (board.IsOnAnyLayer(circle, layers)) {

                    if (circle.Thickness == 0) {

                        Aperture ap = apertureDict.GetCircleAperture(circle.Diameter);
                        gb.SelectAperture(ap);

                        Point p = localTransformation.Transform(circle.Position);
                        gb.FlashAt(p);
                    }
                }
            }

            /// <summary>
            /// Visita un element de tipus via.
            /// </summary>
            /// <param name="via">L'element a visitar.</param>
            /// 
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
            /// <param name="pad">L'element a visitar.</param>
            /// 
            public override void Visit(ThPadElement pad) {

                if (board.IsOnAnyLayer(pad, layers)) {
                    double rotate = localRotation + pad.Rotation;
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

                    Point p = localTransformation.Transform(pad.Position);
                    gb.FlashAt(p);
                }
            }

            /// <summary>
            /// Visita un element de tipus SmdPad
            /// </summary>
            /// <param name="pad">L'element a visitar.</param>
            /// 
            public override void Visit(SmdPadElement pad) {

                if (board.IsOnAnyLayer(pad, layers)) {

                    double rotation = localRotation + pad.Rotation;
                    double radius = pad.Roundnes * Math.Min(pad.Size.Width, pad.Size.Height) / 2;
                    Aperture ap = radius == 0 ?
                        apertureDict.GetRectangleAperture(pad.Size.Width, pad.Size.Height, rotation) :
                        apertureDict.GetRoundRectangleAperture(pad.Size.Width, pad.Size.Height, radius, rotation);
                    gb.SelectAperture(ap);

                    Point p = localTransformation.Transform(pad.Position);
                    gb.FlashAt(p);
                }
            }

            /// <summary>
            /// Visita un objecte Part
            /// </summary>
            /// <param name="part">L'objecte a visitar</param>
            /// 
            public override void Visit(Part part) {

                localTransformation = part.Transformation;
                localRotation = part.Rotation;

                base.Visit(part);

                localTransformation = Matrix.Identity;
                localRotation = 0;
            }
        }

        /// <summary>
        /// Clase per generar regions poligonals.
        /// </summary>
        private sealed class RegionGeneratorVisitor : BoardVisitor {

            private readonly GerberBuilder gb;
            private readonly Board board;
            private readonly IEnumerable<Layer> layers;
            private readonly ApertureDictionary apertureDict;
            private Matrix localTransformation = Matrix.Identity;

            /// <summary>
            /// Constructor del objecte.
            /// </summary>
            /// <param name="gb">Generador de codi gerber.</param>
            /// <param name="board">La placa.</param>
            /// <param name="layers">El conjunt de capes a procesar.</param>
            /// <param name="apertureDict">Diccionari d'apertures.</param>
            /// 
            public RegionGeneratorVisitor(GerberBuilder gb, Board board, IEnumerable<Layer> layers, ApertureDictionary apertureDict) {

                this.gb = gb;
                this.board = board;
                this.layers = layers;
                this.apertureDict = apertureDict;
            }

            /// <summary>
            /// Visita un element de tipus regio
            /// </summary>
            /// <param name="region">L'element a visitar.</param>
            /// 
            public override void Visit(RegionElement region) {

                if (board.IsOnAnyLayer(region, layers)) {

                    Polygon polygon = CreatePolygon(board, region);
                    ProcessPolygon(polygon, region.Thickness);
                }
            }

            /// <summary>
            /// Visita un element de tipus Part.
            /// </summary>
            /// <param name="part">L'objecte a visitar.</param>
            /// 
            public override void Visit(Part part) {

                localTransformation = part.Transformation;

                base.Visit(part);

                localTransformation = Matrix.Identity;
            }

            /// <summary>
            /// Crea el poligon de la regio, amb els forats corresponents
            /// </summary>
            /// <param name="board">La placa</param>
            /// <param name="region">La regio.</param>
            /// <returns>La coleccio de poligons.</returns>
            /// <remarks>
            /// El primer poligon no te punts, no mes s'utilitza per contindre els altres poligons fills.
            /// A nivell de dibuix el nivell 0 es transparent
            /// </remarks>
            /// 
            private Polygon CreatePolygon(Board board, RegionElement region) {

                Polygon regionPolygon = region.GetPolygon();
                regionPolygon.Transform(localTransformation);

                // Si estem en capes de senyal, cal generar els porus i termals
                //
                if (board.IsOnLayer(region, board.GetLayer(LayerId.Top)) ||
                    board.IsOnLayer(region, board.GetLayer(LayerId.Bottom))) {

                    IEnumerable<Layer> regionLayers = board.GetLayers(region);
                    Signal regionSignal = board.GetSignal(region, null, false);

                    double spacing = 0.15 + region.Thickness / 2;
                    List<Polygon> holePolygons = new List<Polygon>();

                    Layer restrict = board.GetLayer(LayerId.TopRestrict);

                    // Procesa els elements de la placa en la mateixa capa que la regio o el la capa restrict
                    //
                    foreach (Element element in board.Elements) {
                        if ((element != region) &&
                            (board.IsOnAnyLayer(element, regionLayers) || board.IsOnLayer(element, restrict))) {
                            IConectable item = element as IConectable;
                            if ((item == null) || (board.GetSignal(item, null, false) != regionSignal)) {
                                Polygon elementPolygon = element.GetPourPolygon(spacing);
                                holePolygons.AddRange(PolygonProcessor.Clip(elementPolygon, regionPolygon, PolygonProcessor.ClipOperation.Intersection));
                            }
                        }
                    }

                    // Procesa els elements dels components
                    //
                    foreach (Part part in board.Parts) {
                        foreach (Element element in part.Elements) {
                            if ((element != region) &&
                                (board.IsOnAnyLayer(element, regionLayers) || board.IsOnLayer(element, restrict))) {

                                PadElement padElement = element as PadElement;

                                // Si l'element no esta conectat a la mateixa senyal que la regio, genera un forat
                                //
                                if ((padElement == null) || (board.GetSignal(padElement, part, false) != regionSignal)) {
                                    Polygon elementPolygon = element.GetPourPolygon(spacing);
                                    elementPolygon.Transform(part.Transformation);
                                    holePolygons.Add(elementPolygon);

                                    // No esta clar si cal o no
                                    //holePolygons.AddRange(PolygonProcessor.Clip(elementPolygon, regionPolygon, PolygonProcessor.ClipOperation.Intersection));
                                }

                                // En es un pad i esta conectat per tant, genera un thermal
                                //
                                else if (padElement != null) { 
                                    Polygon thermalPolygon = padElement.GetThermalPolygon(spacing, 0.2);
                                    thermalPolygon.Transform(part.Transformation);
                                    foreach (Polygon polygon in thermalPolygon.Childs)
                                        holePolygons.Add(polygon);
                                    // No esta clar si cal o no
                                    //holePolygons.AddRange(PolygonProcessor.Clip(thermalPolygon, regionPolygon, PolygonProcessor.ClipOperation.Intersection));
                                }
                            }
                        }
                    }

                    return PolygonProcessor.ClipExtended(regionPolygon, holePolygons, PolygonProcessor.ClipOperation.Diference);
                }

                // Si no es capa de senyal no cal fer res mes
                //
                else 
                    return regionPolygon;
            }

            private void ProcessPolygon(Polygon polygon, double thickness) {

                ProcessPolygon(polygon, polygon.HasPoints ? 1 : 0, thickness);
            }

            private void ProcessPolygon(Polygon polygon, int level, double thickness) {

                // Procesa el poligon
                //
                if (polygon.HasPoints) {

                    // Dibuixa el contingut de la regio
                    //
                    gb.LoadPolarity((level % 2) == 0 ? Polarity.Clear : Polarity.Dark);
                    gb.BeginRegion();
                    gb.Region(polygon.Points, true);
                    gb.EndRegion();

                    // Dibuixa el perfil de la regio
                    //
                    Aperture ap = apertureDict.GetCircleAperture(thickness);
                    gb.SelectAperture(ap);
                    gb.LoadPolarity(Polarity.Dark);
                    gb.Polygon(polygon.Points);
                }

                // Processa els fills. Amb level < 2 evitem els poligons orfres
                //
                if (polygon.HasChilds && (level < 2))
                    foreach (Polygon child in polygon.Childs)
                        ProcessPolygon(child, level + 1, thickness);
            }
        }
    }
}
