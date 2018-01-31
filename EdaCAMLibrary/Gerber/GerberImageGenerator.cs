namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using MikroPic.EdaTools.v1.Cam.Gerber.Builder;
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

    /// <summary>
    /// Clase per generar el fitxers gerber d'imatge
    /// </summary>
    public sealed class GerberImageGenerator : GerberGenerator {

        public enum ImageType {
            Copper,
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
        /// Genera el nom del fitxer.
        /// </summary>
        /// <param name="prefix">Prefix.</param>
        /// <param name="imageType">Tipus d'imatge.</param>
        /// <param name="level">Nivell de capa de coure.</param>
        /// <returns>El nom del fitxer.</returns>
        /// 
        public string GenerateFileName(string prefix, ImageType imageType, int level = 0) {

            if (String.IsNullOrEmpty(prefix))
                throw new ArgumentNullException("prefix");

            if ((imageType == ImageType.Copper) && (level == 0))
                throw new ArgumentOutOfRangeException("level");

            StringBuilder sb = new StringBuilder();

            sb.Append(prefix);

            switch (imageType) {
                case ImageType.Copper:
                    sb.AppendFormat("_Copper$L{0}", level);
                    break;

                case ImageType.TopSolderMask:
                    sb.Append("_Soldermask$Top");
                    break;

                case ImageType.BottomSolderMask:
                    sb.Append("_Soldermask$Bottom");
                    break;

                case ImageType.TopLegend:
                    sb.Append("_Legend$Top");
                    break;

                case ImageType.BottomLegend:
                    sb.Append("_Legend$Bottom");
                    break;

                case ImageType.Profile:
                    sb.Append("_Profile$NP");
                    break;
            }

            sb.Append(".gbr");

            return sb.ToString();
        }

        /// <summary>
        /// Genera un document gerber.
        /// </summary>
        /// <param name="writer">Writer de sortida.</param>
        /// <param name="layers">Llista de capes a procesar.</param>
        /// <param name="imageType">Tipus de fitxer a generar.</param>
        /// <param name="level">Nivell de la capa de coure.</param>
        /// 
        public void GenerateContent(TextWriter writer, IEnumerable<Layer> layers, ImageType imageType, int level = 0) {

            if (writer == null)
                throw new ArgumentNullException("writer");

            if ((imageType == ImageType.Copper) && (level == 0))
                throw new ArgumentOutOfRangeException("level");

            GerberBuilder gb = new GerberBuilder(writer);

            ApertureDictionary apertures = CreateApertures(layers);

            GenerateFileHeader(gb, imageType, level);
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
        /// <param name="level">Nivell de capa de coure.</param>
        /// 
        private void GenerateFileHeader(GerberBuilder gb, ImageType imageType, int level) {

            gb.Comment("EdaTools v1.0.");
            gb.Comment("EdaTools CAM processor. Gerber generator.");
            gb.Comment(String.Format("Start timestamp: {0}", DateTime.Now));
            gb.Comment("BEGIN HEADER");
            switch (imageType) {
                case ImageType.Copper:
                    gb.Attribute(String.Format(".FileFunction,Copper,L{0},{1},Signal", level, level == 1 ? "Top" : "Bot"));
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
        /// Clase utilitzada per crear el diccionari d'apertures.
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

            /// <summary>
            /// Visita un objecte 'Board'
            /// </summary>
            /// <param name="board">L'objecte a visitar.</param>
            /// 
            public override void Visit(Board board) {

                foreach (Part part in board.Parts)
                    part.AcceptVisitor(this);

                foreach (Element element in board.Elements)
                    if (board.IsOnAnyLayer(element, layers))
                        element.AcceptVisitor(this);
            }

            /// <summary>
            /// Visita un objecte 'Part'
            /// </summary>
            /// <param name="part">L'objecte a visitar.</param>
            /// 
            public override void Visit(Part part) {

                localRotation = part.Rotation;
                foreach (Element element in part.Elements)
                    if (board.IsOnAnyLayer(element, layers))
                        element.AcceptVisitor(this);
                localRotation = 0;
            }

            public override void Visit(LineElement line) {

                apertures.DefineCircleAperture(Math.Max(line.Thickness, 0.01));
            }

            public override void Visit(ArcElement arc) {

                apertures.DefineCircleAperture(Math.Max(arc.Thickness, 0.01));
            }

            public override void Visit(RectangleElement rectangle) {

                if (rectangle.Thickness == 0) {
                    double rotation = localRotation + rectangle.Rotation;
                    apertures.DefineRectangleAperture(rectangle.Size.Width, rectangle.Size.Height, rotation);
                }
            }

            public override void Visit(CircleElement circle) {

                if (circle.Thickness == 0)
                    apertures.DefineCircleAperture(circle.Diameter);
            }

            public override void Visit(ViaElement via) {

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

            public override void Visit(ThPadElement pad) {

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

            public override void Visit(SmdPadElement pad) {

                double rotation = localRotation + pad.Rotation;
                double radius = pad.Roundnes * Math.Min(pad.Size.Width, pad.Size.Height) / 2;
                if (radius == 0)
                    apertures.DefineRectangleAperture(pad.Size.Width, pad.Size.Height, rotation);
                else
                    apertures.DefineRoundRectangleAperture(pad.Size.Width, pad.Size.Height, radius, rotation);
            }

            public override void Visit(RegionElement region) {

                apertures.DefineCircleAperture(region.Thickness);
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
        /// Clase generar la imatge a base d'apertures.
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
            /// Constructor del objecte.
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
            
            /// <summary>
            /// Visita objecte LineElement
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
            /// Visita objecte ArcElement.
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
            /// Visita un objecte RectangleElement.
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
            /// Visita un objecte CircleElement
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
            /// Visita un objecte SmdPadElement
            /// </summary>
            /// <param name="pad">L'objecte a visitar.</param>
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
        }

        /// <summary>
        /// Clase per generar la imatge amb regions poligonals.
        /// </summary>
        private sealed class RegionGeneratorVisitor : BoardVisitor {

            private readonly GerberBuilder gb;
            private readonly Board board;
            private readonly IEnumerable<Layer> layers;
            private readonly ApertureDictionary apertureDict;
            private Matrix currentTransformation = Matrix.Identity;
            private Layer currentLayer;

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
            /// Visita un objecte Board
            /// </summary>
            /// <param name="board">L'objecte a visitar.</param>
            /// 
            public override void Visit(Board board) {

                // Procesa capa a capa
                //
                foreach (Layer layer in layers) {
                    currentLayer = layer;

                    // Procesa els elements dels components
                    //
                    foreach (Part part in board.Parts)
                        part.AcceptVisitor(this);

                    // Procesa els components de la placa
                    //
                    foreach (Element element in board.Elements)
                        if (board.IsOnLayer(element, layer))
                            element.AcceptVisitor(this);

                    currentLayer = null;
                }
            }

            /// <summary>
            /// Visita un objecte Part.
            /// </summary>
            /// <param name="part">L'objecte a visitar.</param>
            /// 
            public override void Visit(Part part) {

                foreach (Element element in part.Elements) {
                    if (board.IsOnLayer(element, currentLayer)) {
                        currentTransformation = part.Transformation;
                        element.AcceptVisitor(this);
                        currentTransformation = Matrix.Identity;
                    }
                }
            }
            
            /// <summary>
            /// Visita un objecte RegionElement
            /// </summary>
            /// <param name="region">L'objecte a visitar.</param>
            /// 
            public override void Visit(RegionElement region) {

                Polygon polygon = board.GetRegionPolygon(region, currentLayer, 0.15, currentTransformation);
                DrawPolygon(polygon, region.Thickness);
            }

            /// <summary>
            /// Dibuixa un poligon
            /// </summary>
            /// <param name="polygon">El poligon a dibuixar.</param>
            /// <param name="thickness">Amplada del perfil.</param>
            /// 
            private void DrawPolygon(Polygon polygon, double thickness) {

                DrawPolygon(polygon, polygon.HasPoints ? 1 : 0, thickness);
            }

            /// <summary>
            /// Dibuixa un poligon
            /// </summary>
            /// <param name="polygon">El poligon a dibuixar.</param>
            /// <param name="level">Nivell d'anidad del poligon.</param>
            /// <param name="thickness">Amplada del perfil.</param>
            /// 
            private void DrawPolygon(Polygon polygon, int level, double thickness) {

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
                        DrawPolygon(child, level + 1, thickness);
            }
        }
    }
}
