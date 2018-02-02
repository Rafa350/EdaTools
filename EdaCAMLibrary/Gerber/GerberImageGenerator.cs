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
            GenerateRegions(gb, layers, apertures);
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

            // Crea un diccionari d'apertures buit
            //
            ApertureDictionary apertures = new ApertureDictionary();

            // Recorre totes les capes seleccionades
            //
            foreach (Layer layer in layers) {

                // Procesa una capa per omplir el diccionari d'apertures
                //
                ApertureCreatorVisitor visitor = new ApertureCreatorVisitor(Board, layer, apertures);
                visitor.Run();
            }

            return apertures;
        }

        /// <summary>
        /// Genera la capcelera del fitxer.
        /// </summary>
        /// <param name="gb">El generador de codi gerber.</param>
        /// <param name="imageType">Tipus d'imatge a generar.</param>
        /// <param name="level">Nivell de capa de coure.</param>
        /// 
        private void GenerateFileHeader(GerberBuilder gb, ImageType imageType, int level) {

            gb.Comment("BEGIN FILE");
            gb.Comment("EdaTools v1.0.");
            gb.Comment("EdaTools CAM processor. Gerber generator.");
            gb.Comment(String.Format("Start timestamp: {0:HH:mm:ss.fff}", DateTime.Now));
            gb.Comment("BEGIN HEADER");
            switch (imageType) {
                case ImageType.Copper:
                    gb.Attribute(AttributeScope.File, String.Format(".FileFunction,Copper,L{0},{1},Signal", level, level == 1 ? "Top" : "Bot"));
                    gb.Attribute(AttributeScope.File, ".FilePolarity,Positive");
                    break;

                case ImageType.TopSolderMask:
                    gb.Attribute(AttributeScope.File, ".FileFunction,Soldermask,Top");
                    gb.Attribute(AttributeScope.File, ".FilePolarity,Negative");
                    break;

                case ImageType.BottomSolderMask:
                    gb.Attribute(AttributeScope.File, ".FileFunction,Soldermask,Bot");
                    gb.Attribute(AttributeScope.File, ".FilePolarity,Negative");
                    break;

                case ImageType.TopCream:
                    gb.Attribute(AttributeScope.File, ".FileFunction,Paste,Top");
                    gb.Attribute(AttributeScope.File, ".FilePolarity,Negative");
                    break;

                case ImageType.BottomCream:
                    gb.Attribute(AttributeScope.File, ".FileFunction,Paste,Bot");
                    gb.Attribute(AttributeScope.File, ".FilePolarity,Negative");
                    break;

                case ImageType.TopLegend:
                    gb.Attribute(AttributeScope.File, ".FileFunction,Legend,Top");
                    gb.Attribute(AttributeScope.File, ".FilePolarity,Positive");
                    break;

                case ImageType.BottomLegend:
                    gb.Attribute(AttributeScope.File, ".FileFunction,Legend,Bot");
                    gb.Attribute(AttributeScope.File, ".FilePolarity,Positive");
                    break;

                case ImageType.Profile:
                    gb.Attribute(AttributeScope.File, ".FileFunction,Profile,NP");
                    gb.Attribute(AttributeScope.File, ".FilePolarity,Positive");
                    break;
            }
            gb.Attribute(AttributeScope.File, ".Part,Single");
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
            gb.Comment(String.Format("End timestamp: {0:HH:mm:ss.fff}", DateTime.Now));
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
        /// 
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
        private void GenerateRegions(GerberBuilder gb, IEnumerable<Layer> layers, ApertureDictionary apertures) {

            gb.Comment("BEGIN POLYGONS");

            // Recorre totes les caper seleccionades
            //
            foreach (Layer layer in layers) {

                // Procesa una capa, i escriu els poligons en el gerber
                //
                IVisitor visitor = new RegionGeneratorVisitor(gb, Board, layer, apertures);
                visitor.Visit(Board);
            }

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

            // Recorre les capes una a una
            //
            foreach (Layer layer in layers) {

                // Procesa una capa, i escriu la geometria en el gerger
                //
                IVisitor visitor = new ImageGeneratorVisitor(gb, Board, layer, apertures);
                visitor.Run();
            }

            gb.Comment("END IMAGE");
        }
        
        /// <summary>
        /// Clase utilitzada per omplit el diccionari d'apertures.
        /// </summary>
        /// 
        private sealed class ApertureCreatorVisitor : ElementVisitor {

            private readonly ApertureDictionary apertures;

            /// <summary>
            /// Constructor de la clase.
            /// </summary>
            /// <param name="board">La placa.</param>
            /// <param name="layer">La capa a procesar.</param>
            /// <param name="apertures">El diccionari d'apertures a omplir.</param>
            /// 
            public ApertureCreatorVisitor(Board board, Layer layer, ApertureDictionary apertures):
                base(board, layer) {

                this.apertures = apertures;
            }

            /// <summary>
            /// Visita un objecte 'LineElement'
            /// </summary>
            /// <param name="line">L'objecte a visitar.</param>
            /// 
            public override void Visit(LineElement line) {

                apertures.DefineCircleAperture(Math.Max(line.Thickness, 0.01));
            }

            /// <summary>
            /// Visita un objecte 'ArcElement'
            /// </summary>
            /// <param name="arc">L'onbjecte a visitar.</param>
            /// 
            public override void Visit(ArcElement arc) {

                apertures.DefineCircleAperture(Math.Max(arc.Thickness, 0.01));
            }

            /// <summary>
            /// Visita un objecte 'RectangleElement'
            /// </summary>
            /// <param name="rectangle">L'objecte a visitar.</param>
            /// 
            public override void Visit(RectangleElement rectangle) {

                // Si es ple, es 'flashea'
                //
                if (rectangle.Filled) {
                    double rotation = rectangle.Rotation + (Part == null ? 0 : Part.Rotation);
                    apertures.DefineRectangleAperture(rectangle.Size.Width, rectangle.Size.Height, rotation);
                }

                // En cas contrari es dibuixa
                //
                else
                    apertures.DefineCircleAperture(rectangle.Thickness);
            }

            /// <summary>
            /// Visita un objecte 'CircleElement'
            /// </summary>
            /// <param name="circle">L'objecte a visitar.</param>
            /// 
            public override void Visit(CircleElement circle) {

                // Si es ple, es 'flashea'.
                //
                if (circle.Filled)
                    apertures.DefineCircleAperture(circle.Diameter);

                // En cas contrari es dibuixa
                //
                else
                    apertures.DefineCircleAperture(circle.Thickness);
            }

            /// <summary>
            /// Visita un objecte 'ViaElement'
            /// </summary>
            /// <param name="via">L'objecte a visitar.</param>
            /// 
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

            /// <summary>
            /// Visita un objecte 'ThPadElement'
            /// </summary>
            /// <param name="pad">L'objecte a visitar.</param>
            /// 
            public override void Visit(ThPadElement pad) {

                double rotation = pad.Rotation + (Part == null ? 0 : Part.Rotation);
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

            /// <summary>
            /// Visita un objecte 'SmdPadElement'
            /// </summary>
            /// <param name="pad">L'objecte a visitar.</param>
            /// 
            public override void Visit(SmdPadElement pad) {

                double rotation = pad.Rotation + (Part == null ? 0 : Part.Rotation);
                double radius = pad.Roundnes * Math.Min(pad.Size.Width, pad.Size.Height) / 2;
                if (radius == 0)
                    apertures.DefineRectangleAperture(pad.Size.Width, pad.Size.Height, rotation);
                // ******************** Detectar oval *********************
                else
                    apertures.DefineRoundRectangleAperture(pad.Size.Width, pad.Size.Height, radius, rotation);
            }

            /// <summary>
            /// Visita un objecte 'RegionElement'
            /// </summary>
            /// <param name="region">L'objecte a visitar.</param>
            /// 
            public override void Visit(RegionElement region) {

                apertures.DefineCircleAperture(region.Thickness);
            }
        }

        /// <summary>
        /// Clase generar la imatge a base d'apertures.
        /// </summary>
        /// 
        private sealed class ImageGeneratorVisitor : ElementVisitor {

            private readonly GerberBuilder gb;
            private readonly ApertureDictionary apertures;

            /// <summary>
            /// Constructor del objecte.
            /// </summary>
            /// <param name="gb">L'bjecte GerberBuilder.</param>
            /// <param name="board">La placa.</param>
            /// <param name="layer">la capa a procesar.</param>
            /// <param name="apertures">Diccionari d'apertures.</param>
            /// 
            public ImageGeneratorVisitor(GerberBuilder gb, Board board, Layer layer, ApertureDictionary apertures) :
                base(board, layer) {

                this.gb = gb;
                this.apertures = apertures;
            }

            /// <summary>
            /// Visita objecte 'LineElement'
            /// </summary>
            /// <param name="line">L'objecte a visitar.</param>
            /// 
            public override void Visit(LineElement line) {

                // Calcula les coordinades, mesures, rotacions, etc
                //
                Matrix transformation = Part == null ? Matrix.Identity : Part.Transformation;
                Point startPosition = transformation.Transform(line.StartPosition);
                Point endPosition = transformation.Transform(line.EndPosition);

                // Selecciona l'apertura
                //
                Aperture ap = apertures.GetCircleAperture(Math.Max(line.Thickness, 0.01));

                // Escriu el gerber
                //
                gb.SelectAperture(ap);
                gb.MoveTo(startPosition);
                gb.LineTo(endPosition);
            }

            /// <summary>
            /// Visita objecte 'ArcElement'.
            /// </summary>
            /// <param name="arc">L'objecte a visitar.</param>
            /// 
            public override void Visit(ArcElement arc) {

                // Calcula les coordinades, mesures, rotacions, etc
                //
                Matrix transformation = Part == null ? Matrix.Identity : Part.Transformation;
                Point startPosition = transformation.Transform(arc.StartPosition);
                Point endPosition = transformation.Transform(arc.EndPosition);
                Point center = transformation.Transform(arc.Center);

                // Selecciona l'apertura
                //
                Aperture ap = apertures.GetCircleAperture(Math.Max(arc.Thickness, 0.01));

                // Escriu el gerber
                //
                gb.SelectAperture(ap);
                gb.MoveTo(startPosition);
                gb.ArcTo(
                    endPosition.X,
                    endPosition.Y,
                    center.X - startPosition.X,
                    center.Y - startPosition.Y,
                    arc.Angle < 0 ? ArcDirection.CW : ArcDirection.CCW);
            }

            /// <summary>
            /// Visita un objecte 'RectangleElement'.
            /// </summary>
            /// <param name="rectangle">L'objecte a visitar.</param>
            /// 
            public override void Visit(RectangleElement rectangle) {

                if (rectangle.Filled) {

                    // Calcula les coordinades, mesures, rotacions, etc
                    //
                    double rotation = rectangle.Rotation + (Part == null ? 0 : Part.Rotation);
                    Matrix transformation = Part == null ? Matrix.Identity : Part.Transformation;
                    Point position = transformation.Transform(rectangle.Position);

                    // Selecciona l'apertura
                    //
                    Aperture ap = apertures.GetRectangleAperture(rectangle.Size.Width, rectangle.Size.Height, rotation);

                    // Escriu el gerber
                    //
                    gb.SelectAperture(ap);
                    gb.FlashAt(position);
                }

                else {
                    // Selecciona l'apertura
                    //
                    Aperture ap = apertures.GetCircleAperture(rectangle.Thickness);

                    // Obte el poligon
                    //
                    Polygon polygon = rectangle.GetPolygon(Layer.Side);
                    if (Part != null)
                        polygon.Transform(Part.Transformation);

                    // Escriu el gerber
                    //
                    gb.SelectAperture(ap);
                    gb.Polygon(polygon.Points);
                }
            }

            /// <summary>
            /// Visita un objecte CircleElement
            /// </summary>
            /// <param name="circle">L'objecte a visitar.</param>
            /// 
            public override void Visit(CircleElement circle) {

                if (circle.Filled) {

                    // Calcula les coordinades, mesures, rotacions, etc
                    //
                    Matrix transformation = Part == null ? Matrix.Identity : Part.Transformation;
                    Point position = transformation.Transform(circle.Position);

                    // Selecciona l'apertura
                    //
                    Aperture ap = apertures.GetCircleAperture(circle.Diameter);

                    // Escriu el gerber
                    //
                    gb.SelectAperture(ap);
                    gb.FlashAt(position);
                }

                else {

                    // Selecciona l'apertura
                    //
                    Aperture ap = apertures.GetCircleAperture(circle.Thickness);

                    // Obte el poligon
                    //
                    Polygon polygon = circle.GetPolygon(Layer.Side);
                    if (Part != null)
                        polygon.Transform(Part.Transformation);

                    // Escriu el gerber
                    //
                    gb.SelectAperture(ap);
                    gb.Polygon(polygon.Points);
                }
            }

            /// <summary>
            /// Visita un objecte 'ViaElement'.
            /// </summary>
            /// <param name="via">L'objecte a visitar.</param>
            /// 
            public override void Visit(ViaElement via) {

                // Selecciona l'apertura
                //
                Aperture ap = null;
                switch (via.Shape) {
                    default:
                    case ViaElement.ViaShape.Circular:
                        ap = apertures.GetCircleAperture(via.OuterSize);
                        break;

                    case ViaElement.ViaShape.Square:
                        ap = apertures.GetRectangleAperture(via.OuterSize, via.OuterSize, 0);
                        break;

                    case ViaElement.ViaShape.Octogonal:
                        ap = apertures.GetOctagonAperture(via.OuterSize, 0);
                        break;
                }

                // Escriu el gerber
                //
                gb.SelectAperture(ap);
                gb.FlashAt(via.Position);
            }

            /// <summary>
            /// Visita un objecte 'ThPadElement'
            /// </summary>
            /// <param name="pad">L'objecte a visitar.</param>
            /// 
            public override void Visit(ThPadElement pad) {

                // Calcula les coordinades, mesures, rotacions, etc
                //
                double rotation = pad.Rotation + (Part == null ? 0 : Part.Rotation);
                Matrix transformation = Part == null ? Matrix.Identity : Part.Transformation;
                Point position = transformation.Transform(pad.Position);

                // Selecciona l'apertura
                //
                Aperture ap = null;
                switch (pad.Shape) {
                    case ThPadElement.ThPadShape.Circular:
                        ap = apertures.GetCircleAperture(pad.Size);
                        break;

                    case ThPadElement.ThPadShape.Square:
                        ap = apertures.GetRectangleAperture(pad.Size, pad.Size, rotation);
                        break;

                    case ThPadElement.ThPadShape.Octogonal:
                        ap = apertures.GetOctagonAperture(pad.Size, rotation);
                        break;

                    case ThPadElement.ThPadShape.Oval:
                        ap = apertures.GetOvalAperture(pad.Size * 2, pad.Size, rotation);
                        break;
                }

                // Escriu el gerber
                //
                gb.SelectAperture(ap);
                gb.FlashAt(position);
            }

            /// <summary>
            /// Visita un objecte 'SmdPadElement'
            /// </summary>
            /// <param name="pad">L'objecte a visitar.</param>
            /// 
            public override void Visit(SmdPadElement pad) {

                // Calcula les coordinades, mesures, rotacions, etc
                //
                double rotation = pad.Rotation + (Part == null ? 0 : Part.Rotation);
                double radius = pad.Roundnes * Math.Min(pad.Size.Width, pad.Size.Height) / 2;
                Matrix transformation = Part == null ? Matrix.Identity : Part.Transformation;
                Point position = transformation.Transform(pad.Position);

                // Selecciona l'apertura
                //
                /// ************** Detectar OVAL ********************
                Aperture ap = radius == 0 ?
                    apertures.GetRectangleAperture(pad.Size.Width, pad.Size.Height, rotation) :
                    apertures.GetRoundRectangleAperture(pad.Size.Width, pad.Size.Height, radius, rotation);

                // Escriu el gerber
                //
                gb.SelectAperture(ap);
                gb.FlashAt(position);
            }
        }

        /// <summary>
        /// Clase per generar la imatge amb regions poligonals.
        /// </summary>
        private sealed class RegionGeneratorVisitor : ElementVisitor {

            private readonly GerberBuilder gb;
            private readonly ApertureDictionary apertures;

            /// <summary>
            /// Constructor del objecte.
            /// </summary>
            /// <param name="gb">Generador de codi gerber.</param>
            /// <param name="board">La placa.</param>
            /// <param name="layer">La capa a procesar.</param>
            /// <param name="apertures">Diccionari d'apertures.</param>
            /// 
            public RegionGeneratorVisitor(GerberBuilder gb, Board board, Layer layer, ApertureDictionary apertures):
                base(board, layer) { 

                this.gb = gb;
                this.apertures = apertures;
            }
            
            /// <summary>
            /// Visita un objecte RegionElement
            /// </summary>
            /// <param name="region">L'objecte a visitar.</param>
            /// 
            public override void Visit(RegionElement region) {

                Matrix transformation = Part == null ? Matrix.Identity : Part.Transformation;
                Polygon polygon = Board.GetRegionPolygon(region, Layer, 0.15, transformation);
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
                    Aperture ap = apertures.GetCircleAperture(thickness);
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
