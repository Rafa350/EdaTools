namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using MikroPic.EdaTools.v1.Cam.Gerber.Builder;
    using MikroPic.EdaTools.v1.Cam.Model;
    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Fonts;
    using MikroPic.EdaTools.v1.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Infrastructure;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.BoardElements;
    using MikroPic.EdaTools.v1.Pcb.Model.PanelElements;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Clase per generar el fitxers gerber d'imatge
    /// </summary>
    public sealed class GerberImageGenerator : Generator {

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
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="target">El target.</param>
        /// 
        public GerberImageGenerator(Target target):
            base(target) {

        }

        /// <summary>
        /// Genera el fitxer corresponent al panell.
        /// </summary>
        /// <param name="panel">El panell.</param>
        /// 
        public override void Generate(Panel panel) {

            if (panel == null)
                throw new ArgumentNullException("panel");

            // Crea el fitxer de sortida
            //
            using (TextWriter writer = new StreamWriter(
                new FileStream(Target.FileName, FileMode.Create, FileAccess.Write, FileShare.None))) {

                // Prepara el diccionari d'apertures
                //
                ApertureDictionary apertures = new ApertureDictionary();
                foreach (var element in panel.Elements) {
                    if (element is PlaceElement) {
                        PlaceElement place = (PlaceElement)element;
                        PrepareApertures(apertures, place.Board);
                    }
                }

                // Prepara el generador de gerbers
                //
                GerberBuilder gb = new GerberBuilder(writer);

                // Genera la capcelera del fitxer
                //
                GenerateFileHeader(gb);

                // Genera el diccionari de macros i apertures
                //
                GenerateMacros(gb, apertures);
                GenerateApertures(gb, apertures);

                // Genera la imatge de les plaques
                //
                int boardId = 1;
                foreach (var element in panel.Elements) {
                    if (element is PlaceElement) {
                        PlaceElement place = (PlaceElement)element;

                        gb.Comment(String.Format("BEGIN BOARD {0}", boardId));
                        gb.SetTransformation(place.Position, place.Rotation);
                        GenerateRegions(gb, place.Board, apertures);
                        GenerateImage(gb, place.Board, apertures);
                        gb.Comment(String.Format("END BOARD {0}", boardId));

                        boardId++;
                    }
                }

                // Genera el final del fitxer
                //
                GenerateFileTail(gb);
            }
        }

        /// <summary>
        /// Prepara el diccionari d'apertures.
        /// </summary>
        /// <param name="apertures">El diccionari a preparar.</param>
        /// <param name="board">La placa a procesar.</param>
        /// 
        private void PrepareApertures(ApertureDictionary apertures, Board board) {

            foreach (var layerName in Target.LayerIds) {
                Layer layer = board.GetLayer(layerName);
                ApertureCreatorVisitor visitor = new ApertureCreatorVisitor(board, layer, apertures);
                visitor.Run();
            }
        }

        /// <summary>
        /// Genera la capcelera del fitxer.
        /// </summary>
        /// <param name="gb">El generador de codi gerber.</param>
        /// 
        private void GenerateFileHeader(GerberBuilder gb) {

            gb.Comment("BEGIN FILE");
            gb.Comment("EdaTools v1.0.");
            gb.Comment("EdaTools CAM processor. Gerber generator.");
            gb.Comment(String.Format("Start timestamp: {0:HH:mm:ss.fff}", DateTime.Now));
            gb.Comment("BEGIN HEADER");

            ImageType imageType = (ImageType)Enum.Parse(typeof(ImageType), Target.GetOptionValue("imageType"));
            switch (imageType) {
                case ImageType.Copper: {
                    int layerLevel = Int32.Parse(Target.GetOptionValue("layerLevel"));
                    gb.Attribute(AttributeScope.File, String.Format(".FileFunction,Copper,L{0},{1},Signal", layerLevel, layerLevel == 1 ? "Top" : "Bot"));
                    gb.Attribute(AttributeScope.File, ".FilePolarity,Positive");
                    break;
                }

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
        /// <param name="board">La placa.</param>
        /// <param name="apertures">El diccionari d'apertures.</param>
        /// 
        private void GenerateRegions(GerberBuilder gb, Board board, ApertureDictionary apertures) {

            gb.Comment("BEGIN POLYGONS");
            foreach (var layerName in Target.LayerIds) {
                Layer layer = board.GetLayer(layerName);
                IVisitor visitor = new RegionGeneratorVisitor(gb, board, layer, apertures);
                visitor.Visit(board);
            }
            gb.Comment("END POLYGONS");
        }

        /// <summary>
        /// Genera la seccio d'imatges del fitxer.
        /// </summary>
        /// <param name="gb">El generador de codi gerber.</param>
        /// <param name="board">La placa.</param>
        /// <param name="apertures">El diccionari d'apertures.</param>
        /// 
        private void GenerateImage(GerberBuilder gb, Board board, ApertureDictionary apertures) {

            gb.Comment("BEGIN IMAGE");
            foreach (var layerName in Target.LayerIds) {
                Layer layer = board.GetLayer(layerName);
                IVisitor visitor = new ImageGeneratorVisitor(gb, board, layer, apertures);
                visitor.Run();
            }
            gb.Comment("END IMAGE");
        }
        
        /// <summary>
        /// Visitador per preparar les apertures.
        /// </summary>
        /// 
        private sealed class ApertureCreatorVisitor : ElementVisitor {

            private readonly ApertureDictionary apertures;

            /// <summary>
            /// Constructor de l'objecte.
            /// </summary>
            /// <param name="board">La placa.</param>
            /// <param name="layer">La capa a procesar.</param>
            /// <param name="apertures">El diccionari d'apertures.</param>
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

                apertures.DefineCircleAperture(Math.Max(line.Thickness, 10000));
            }

            /// <summary>
            /// Visita un objecte 'ArcElement'
            /// </summary>
            /// <param name="arc">L'objecte a visitar.</param>
            /// 
            public override void Visit(ArcElement arc) {

                apertures.DefineCircleAperture(Math.Max(arc.Thickness, 10000));
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
                    Angle rotation = rectangle.Rotation + (Part == null ? Angle.Zero : Part.Rotation);
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

                // Si es ple, es 'flashea'
                //
                if (circle.Filled)
                    apertures.DefineCircleAperture(circle.Diameter);

                // En cas contrari es dibuixa
                //
                else
                    apertures.DefineCircleAperture(circle.Thickness);
            }

            /// <summary>
            /// Visita un objecte 'TextElement'
            /// </summary>
            /// <param name="text">L'objecte a visitar.</param>
            /// 
            public override void Visit(TextElement text) {

                apertures.DefineCircleAperture(text.Thickness);
            }

            /// <summary>
            /// Visita un objecte 'ViaElement'
            /// </summary>
            /// <param name="via">L'objecte a visitar.</param>
            /// 
            public override void Visit(ViaElement via) {

                switch (via.Shape) {
                    case ViaElement.ViaShape.Circle:
                        apertures.DefineCircleAperture(via.OuterSize);
                        break;

                    case ViaElement.ViaShape.Square:
                        apertures.DefineRectangleAperture(via.OuterSize, via.OuterSize, Angle.Zero);
                        break;

                    case ViaElement.ViaShape.Octagon:
                        apertures.DefineOctagonAperture(via.OuterSize, Angle.Zero);
                        break;
                }
            }

            /// <summary>
            /// Visita un objecte 'ThPadElement'
            /// </summary>
            /// <param name="pad">L'objecte a visitar.</param>
            /// 
            public override void Visit(ThPadElement pad) {

                Angle rotation = pad.Rotation;
                if (Part != null)
                    rotation += Part.Rotation;
                switch (pad.Shape) {
                    case ThPadElement.ThPadShape.Circle:
                        apertures.DefineCircleAperture(pad.TopSize);
                        break;

                    case ThPadElement.ThPadShape.Square:
                        apertures.DefineRectangleAperture(pad.TopSize, pad.TopSize, rotation);
                        break;

                    case ThPadElement.ThPadShape.Octagon:
                        apertures.DefineOctagonAperture(pad.TopSize, rotation);
                        break;

                    case ThPadElement.ThPadShape.Oval:
                        apertures.DefineOvalAperture(pad.TopSize * 2, pad.TopSize, rotation);
                        break;
                }
            }

            /// <summary>
            /// Visita un objecte 'SmdPadElement'
            /// </summary>
            /// <param name="pad">L'objecte a visitar.</param>
            /// 
            public override void Visit(SmdPadElement pad) {

                Angle rotation = pad.Rotation;
                if (Part != null)
                    rotation += Part.Rotation;
                if (pad.Roundness.IsZero)
                    apertures.DefineRectangleAperture(pad.Size.Width, pad.Size.Height, rotation);
                else if (pad.Roundness.IsMax)
                    apertures.DefineOvalAperture(pad.Size.Width, pad.Size.Height, rotation);
                else {
                    int radius = pad.Roundness * Math.Min(pad.Size.Width, pad.Size.Height) / 2;
                    apertures.DefineRoundRectangleAperture(pad.Size.Width, pad.Size.Height, radius, rotation);
                }
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
        /// Visitador per generar la imatge.
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
                Point startPosition = line.StartPosition;
                Point endPosition = line.EndPosition;
                if (Part != null) {
                    Transformation t = Part.GetLocalTransformation();
                    startPosition = t.ApplyTo(startPosition);
                    endPosition = t.ApplyTo(endPosition);
                }

                // Selecciona l'apertura
                //
                Aperture ap = apertures.GetCircleAperture(Math.Max(line.Thickness, 10000));

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
                Point startPosition = arc.StartPosition;
                Point endPosition = arc.EndPosition;
                Point center = arc.Center;
                if (Part != null) {
                    Transformation t = Part.GetLocalTransformation();
                    startPosition = t.ApplyTo(startPosition);
                    endPosition = t.ApplyTo(endPosition);
                    center = t.ApplyTo(center);
                }

                // Selecciona l'apertura
                //
                Aperture ap = apertures.GetCircleAperture(Math.Max(arc.Thickness, 10000));

                // Escriu el gerber
                //
                gb.SelectAperture(ap);
                gb.MoveTo(startPosition);
                gb.ArcTo(endPosition.X, endPosition.Y, 
                    center.X - startPosition.X, center.Y - startPosition.Y,
                    arc.Angle.Degrees < 0 ? ArcDirection.CW : ArcDirection.CCW);
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
                    Point position = rectangle.Position;
                    Angle rotation = rectangle.Rotation;
                    if (Part != null) {
                        Transformation t = Part.GetLocalTransformation();
                        position = t.ApplyTo(position);
                        rotation += Part.Rotation;
                    }

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
                    Polygon polygon = rectangle.GetPolygon(Layer.Id.Side);
                    Point[] points = polygon.ClonePoints();

                    if (Part != null) {
                        Transformation t = Part.GetLocalTransformation();
                        t.ApplyTo(points);
                    }

                    // Escriu el gerber
                    //
                    gb.SelectAperture(ap);
                    gb.Polygon(points);
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
                    Point position = circle.Position;
                    if (Part != null) {
                        Transformation t = Part.GetLocalTransformation();
                        position = t.ApplyTo(position);
                    }

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
                    Point[] points = polygon.ClonePoints();

                    if (Part != null) {
                        Transformation t = Part.GetLocalTransformation();
                        t.ApplyTo(points);
                    }

                    // Escriu el gerber
                    //
                    gb.SelectAperture(ap);
                    gb.Polygon(points);
                }
            }

            public override void Visit(TextElement text) {

                // Selecciona l'apertura
                //
                Aperture ap = apertures.GetCircleAperture(text.Thickness);

                // Escriu el gerber
                //
                gb.SelectAperture(ap);
                Font font = FontFactory.Instance.GetFont("Standard");
                GerberTextDrawer dr = new GerberTextDrawer(font, gb);

                PartAttributeAdapter paa = new PartAttributeAdapter(Part, text);
                Point position = paa.Position;
                if (Part != null) {
                    Transformation t = Part.GetLocalTransformation();
                    position = t.ApplyTo(position);
                }

                dr.Draw(paa.Value, position, paa.HorizontalAlign, paa.VerticalAlign, paa.Height);
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
                    case ViaElement.ViaShape.Circle:
                        ap = apertures.GetCircleAperture(via.OuterSize);
                        break;

                    case ViaElement.ViaShape.Square:
                        ap = apertures.GetRectangleAperture(via.OuterSize, via.OuterSize, Angle.Zero);
                        break;

                    case ViaElement.ViaShape.Octagon:
                        ap = apertures.GetOctagonAperture(via.OuterSize, Angle.Zero);
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
                Point position = pad.Position;
                Angle rotation = pad.Rotation;
                if (Part != null) {
                    rotation += Part.Rotation;
                    Transformation t = Part.GetLocalTransformation();
                    position = t.ApplyTo(position);
                }

                // Selecciona l'apertura
                //
                Aperture ap = null;
                switch (pad.Shape) {
                    case ThPadElement.ThPadShape.Circle:
                        ap = apertures.GetCircleAperture(pad.TopSize);
                        break;

                    case ThPadElement.ThPadShape.Square:
                        ap = apertures.GetRectangleAperture(pad.TopSize, pad.TopSize, rotation);
                        break;

                    case ThPadElement.ThPadShape.Octagon:
                        ap = apertures.GetOctagonAperture(pad.TopSize, rotation);
                        break;

                    case ThPadElement.ThPadShape.Oval:
                        ap = apertures.GetOvalAperture(pad.TopSize * 2, pad.TopSize, rotation);
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
                Point position = pad.Position;
                Angle rotation = pad.Rotation;
                if (Part != null) {
                    rotation += Part.Rotation;
                    Transformation t = Part.GetLocalTransformation();
                    position = t.ApplyTo(position);
                }

                // Selecciona l'apertura
                //
                Aperture ap;
                if (pad.Roundness.IsZero)
                    ap = apertures.GetRectangleAperture(pad.Size.Width, pad.Size.Height, rotation);
                else if (pad.Roundness.IsMax)
                    ap = apertures.GetOvalAperture(pad.Size.Width, pad.Size.Height, rotation);
                else 
                    ap = apertures.GetRoundRectangleAperture(pad.Size.Width, pad.Size.Height, pad.Radius, rotation);

                // Escriu el gerber
                //
                gb.SelectAperture(ap);
                gb.FlashAt(position);
            }
        }

        /// <summary>
        /// Clase per generar la imatge dels texts
        /// </summary>
        private class GerberTextDrawer: TextDrawer {

            private readonly GerberBuilder gb;

            public GerberTextDrawer(Font font, GerberBuilder gb):
                base(font) {

                this.gb = gb;
            }

            protected override void Trace(Point position, bool stroke, bool first) {
                
                if (first || !stroke)
                    gb.MoveTo(position);
                else
                    gb.LineTo(position);
            }
        }

        /// <summary>
        /// Clase per generar la imatge amb regions poligonals.
        /// </summary>
        private sealed class RegionGeneratorVisitor : ElementVisitor {

            private readonly GerberBuilder gb;
            private readonly ApertureDictionary apertures;
            private readonly Point boardPosition;

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

                Transformation t = new Transformation();
                if (Part != null)
                    t = Part.GetLocalTransformation();
                Polygon polygon = Board.GetRegionPolygon(region, Layer, t);
                DrawPolygon(polygon, region.Thickness);
            }

            /// <summary>
            /// Dibuixa un poligon
            /// </summary>
            /// <param name="polygon">El poligon a dibuixar.</param>
            /// <param name="thickness">Amplada del perfil.</param>
            /// 
            private void DrawPolygon(Polygon polygon, int thickness) {

                DrawPolygon(polygon, (polygon.Points != null) ? 1 : 0, thickness);
            }

            /// <summary>
            /// Dibuixa un poligon
            /// </summary>
            /// <param name="polygon">El poligon a dibuixar.</param>
            /// <param name="level">Nivell d'anidad del poligon.</param>
            /// <param name="thickness">Amplada del perfil.</param>
            /// 
            private void DrawPolygon(Polygon polygon, int level, int thickness) {

                // Procesa el poligon
                //
                if (polygon.Points != null) {

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
                if ((polygon.Childs != null) && (level < 2))
                    foreach (Polygon child in polygon.Childs)
                        DrawPolygon(child, level + 1, thickness);
            }
        }
    }
}
