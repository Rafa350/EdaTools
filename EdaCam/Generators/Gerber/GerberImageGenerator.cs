using System;
using System.IO;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
using MikroPic.EdaTools.v1.Cam.Generators.Gerber.Builder;
using MikroPic.EdaTools.v1.Cam.Model;
using MikroPic.EdaTools.v1.Core.Infrastructure;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;

namespace MikroPic.EdaTools.v1.Cam.Generators.Gerber {

    /// <summary>
    /// Clase per generar el fitxers gerber d'imatge
    /// </summary>
    public sealed class GerberImageGenerator : Generator {

        public enum ImageType {
            Copper,
            SolderMask,
            Cream,
            Profile,
            Legend,
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="target">El target.</param>
        /// 
        public GerberImageGenerator(Target target) :
            base(target) {

        }

        /// <summary>
        /// Genera el fitxer corresponent a una placa.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// <param name="outputFolder">Carpeta de sortida.</param>
        /// <param name="options">Options.</param>
        /// 
        public override void Generate(EdaBoard board, string outputFolder, GeneratorOptions options) {

            if (board == null)
                throw new ArgumentNullException(nameof(board));

            if (String.IsNullOrEmpty(outputFolder))
                throw new ArgumentNullException(nameof(outputFolder));

            string fileName = Path.Combine(outputFolder, Target.FileName);

            ImageType imageType = (ImageType)Enum.Parse(typeof(ImageType), Target.GetOptionValue("imageType"), true);

            // Crea el fitxer de sortida
            //
            using (TextWriter writer = new StreamWriter(
                new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))) {

                // Prepara el diccionari d'apertures
                //
                ApertureDictionary apertures = new ApertureDictionary();
                PrepareApertures(apertures, board);

                // Prepara el generador de gerbers
                //
                GerberBuilder gb = new GerberBuilder(writer);
                gb.SetTransformation(Target.Position, Target.Rotation);

                // Genera la capcelera del fitxer
                //
                GenerateFileHeader(gb, imageType);

                // Genera el diccionari de macros i apertures
                //
                GenerateMacros(gb, apertures);
                GenerateApertures(gb, apertures);

                // Genera la imatge de les plaques
                //
                gb.Comment("BEGIN BOARD");
                GenerateRegions(gb, board, apertures);
                GenerateImage(gb, imageType, board, apertures);
                gb.Comment("END BOARD");

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
        private void PrepareApertures(ApertureDictionary apertures, EdaBoard board) {

            foreach (var name in Target.LayerNames) {
                var visitor = new ApertureCreatorVisitor(EdaLayerId.Get(name), apertures);
                board.AcceptVisitor(visitor);
            }
        }

        /// <summary>
        /// Genera la capcelera del fitxer.
        /// </summary>
        /// <param name="gb">El generador de codi gerber.</param>
        /// <param name="imageType">Tipus d'imatge a generar.</param>
        /// 
        private void GenerateFileHeader(GerberBuilder gb, ImageType imageType) {

            gb.Comment("BEGIN FILE");
            gb.Comment("EdaTools v2.0.");
            gb.Comment("EdaTools CAM processor. Gerber generator.");
            gb.Comment(String.Format("Start timestamp: {0:HH:mm:ss.fff}", DateTime.Now));
            gb.Comment("BEGIN HEADER");

            switch (imageType) {
                case ImageType.Copper: {
                    int layerLevel = Int32.Parse(Target.GetOptionValue("layerLevel"));
                    string layerSide = Target.GetOptionValue("layerSide");
                    gb.Attribute(AttributeScope.File, $".FileFunction,Copper,L{layerLevel},{layerSide},Signal");
                    gb.Attribute(AttributeScope.File, ".FilePolarity,Positive");
                    break;
                }

                case ImageType.SolderMask: {
                    string layerSide = Target.GetOptionValue("layerSide");
                    gb.Attribute(AttributeScope.File, $".FileFunction,Soldermask,{layerSide}");
                    gb.Attribute(AttributeScope.File, ".FilePolarity,Negative");
                    break;
                }

                case ImageType.Cream: {
                    string layerSide = Target.GetOptionValue("layerSide");
                    gb.Attribute(AttributeScope.File, $".FileFunction,Paste,{layerSide}");
                    gb.Attribute(AttributeScope.File, ".FilePolarity,Negative");
                    break;
                }

                case ImageType.Legend: {
                    string layerSide = Target.GetOptionValue("layerSide");
                    gb.Attribute(AttributeScope.File, $".FileFunction,Legend,{layerSide}");
                    gb.Attribute(AttributeScope.File, ".FilePolarity,Positive");
                    break;
                }

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
        private void GenerateRegions(GerberBuilder gb, EdaBoard board, ApertureDictionary apertures) {

            gb.Comment("BEGIN POLYGONS");
            foreach (var name in Target.LayerNames) {
                var visitor = new RegionGeneratorVisitor(gb, EdaLayerId.Get(name), apertures);
                visitor.Visit(board);
            }
            gb.Comment("END POLYGONS");
        }

        /// <summary>
        /// Genera la seccio d'imatges del fitxer.
        /// </summary>
        /// <param name="gb">El generador de codi gerber.</param>
        /// <param name="imageType">Tipus d'imatge a generar.</param>
        /// <param name="board">La placa.</param>
        /// <param name="apertures">El diccionari d'apertures.</param>
        /// 
        private void GenerateImage(GerberBuilder gb, ImageType imageType, EdaBoard board, ApertureDictionary apertures) {

            gb.Comment("BEGIN IMAGE");
            foreach (var name in Target.LayerNames) {
                var visitor = new ImageGeneratorVisitor(gb, imageType, EdaLayerId.Get(name), apertures);
                board.AcceptVisitor(visitor);
            }
            gb.Comment("END IMAGE");
        }

        /// <summary>
        /// Visitador per preparar les apertures.
        /// </summary>
        /// 
        private sealed class ApertureCreatorVisitor : EdaElementVisitor {

            private readonly EdaLayerId _layerId;
            private readonly ApertureDictionary _apertures;

            /// <summary>
            /// Constructor de l'objecte.
            /// </summary>
            /// <param name="layerId">El identificador de la capa a procesar.</param>
            /// <param name="apertures">El diccionari d'apertures.</param>
            /// 
            public ApertureCreatorVisitor(EdaLayerId layerId, ApertureDictionary apertures) {

                _layerId = layerId;
                _apertures = apertures;
            }

            /// <summary>
            /// Visita un objecte 'LineElement'
            /// </summary>
            /// <param name="line">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaLineElement line) {

                if (CanVisit(line))
                    _apertures.DefineCircleAperture(Math.Max(line.Thickness, 10000));
            }

            /// <summary>
            /// Visita un objecte 'ArcElement'
            /// </summary>
            /// <param name="arc">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaArcElement arc) {

                if (CanVisit(arc))
                    _apertures.DefineCircleAperture(Math.Max(arc.Thickness, 10000));
            }

            /// <summary>
            /// Visita un objecte 'RectangleElement'
            /// </summary>
            /// <param name="rectangle">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaRectangleElement rectangle) {

                if (CanVisit(rectangle)) {

                    // Si es ple, es 'flashea'
                    //
                    if (rectangle.Filled) {
                        EdaAngle rotation = rectangle.Rotation + (Part == null ? EdaAngle.Zero : Part.Rotation);
                        _apertures.DefineRectangleAperture(rectangle.Size.Width, rectangle.Size.Height, rotation);
                    }

                    // En cas contrari es dibuixa
                    //
                    else
                        _apertures.DefineCircleAperture(rectangle.Thickness);
                }
            }

            /// <summary>
            /// Visita un objecte 'CircleElement'
            /// </summary>
            /// <param name="circle">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaCircleElement circle) {

                if (CanVisit(circle)) {

                    // Si es ple, es 'flashea'
                    //
                    if (circle.Filled)
                        _apertures.DefineCircleAperture(circle.Diameter);

                    // En cas contrari es dibuixa
                    //
                    else
                        _apertures.DefineCircleAperture(circle.Thickness);
                }
            }

            /// <summary>
            /// Visita un objecte 'TextElement'
            /// </summary>
            /// <param name="text">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaTextElement text) {

                if (CanVisit(text))
                    _apertures.DefineCircleAperture(text.Thickness);
            }

            /// <summary>
            /// Visita un objecte 'ViaElement'
            /// </summary>
            /// <param name="via">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaViaElement via) {

                if (CanVisit(via))
                    switch (via.Shape) {
                        case EdaViaElement.ViaShape.Circle:
                            _apertures.DefineCircleAperture(via.OuterSize);
                            break;

                        case EdaViaElement.ViaShape.Square:
                            _apertures.DefineRectangleAperture(via.OuterSize, via.OuterSize, EdaAngle.Zero);
                            break;

                        case EdaViaElement.ViaShape.Octagon:
                            _apertures.DefineOctagonAperture(via.OuterSize, EdaAngle.Zero);
                            break;
                    }
            }

            /// <summary>
            /// Visita un objecte 'ThPadElement'
            /// </summary>
            /// <param name="pad">L'objecte.</param>
            /// 
            public override void Visit(EdaThPadElement pad) {

                if (CanVisit(pad)) {
                    EdaAngle rotation = pad.Rotation;
                    if (Part != null)
                        rotation += Part.Rotation;
                    if (pad.CornerRatio.IsMax)
                        _apertures.DefineOvalAperture(pad.TopSize.Width, pad.TopSize.Height, rotation);
                    else if (pad.CornerRatio.IsZero)
                        _apertures.DefineRectangleAperture(pad.TopSize.Width, pad.TopSize.Height, rotation);
                    else {
                        int radius = pad.CornerRatio * Math.Min(pad.TopSize.Width, pad.TopSize.Height) / 2;
                        _apertures.DefineRoundRectangleAperture(pad.TopSize.Width, pad.TopSize.Height, radius, rotation);
                    }
                }
            }

            /// <summary>
            /// Visita un objecte 'SmdPadElement'
            /// </summary>
            /// <param name="pad">L'objecte.</param>
            /// 
            public override void Visit(EdaSmdPadElement pad) {

                if (CanVisit(pad)) {
                    EdaAngle rotation = pad.Rotation;
                    if (Part != null)
                        rotation += Part.Rotation;
                    if (pad.CornerRatio.IsZero)
                        _apertures.DefineRectangleAperture(pad.Size.Width, pad.Size.Height, rotation);
                    else if (pad.CornerRatio.IsMax)
                        _apertures.DefineOvalAperture(pad.Size.Width, pad.Size.Height, rotation);
                    else {
                        int radius = pad.CornerRatio * Math.Min(pad.Size.Width, pad.Size.Height) / 2;
                        _apertures.DefineRoundRectangleAperture(pad.Size.Width, pad.Size.Height, radius, rotation);
                    }
                }
            }

            /// <summary>
            /// Visita un objecte 'RegionElement'
            /// </summary>
            /// <param name="region">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaRegionElement region) {

                if (CanVisit(region))
                    _apertures.DefineCircleAperture(region.Thickness);
            }

            private bool CanVisit(EdaElement element) {

                return element.IsOnLayer(_layerId);
            }
        }

        /// <summary>
        /// Visitador per generar la imatge.
        /// </summary>
        /// 
        private sealed class ImageGeneratorVisitor : EdaElementVisitor {

            private readonly ImageType _imageType;
            private readonly EdaLayerId _layerId;
            private readonly GerberBuilder _gb;
            private readonly ApertureDictionary _apertures;

            /// <summary>
            /// Constructor del objecte.
            /// </summary>
            /// <param name="gb">L'objecte GerberBuilder.</param>
            /// <param name="layerId">El identificador de la capa a procesar.</param>
            /// <param name="apertures">Diccionari d'apertures.</param>
            /// 
            public ImageGeneratorVisitor(GerberBuilder gb, ImageType imageType, EdaLayerId layerId, ApertureDictionary apertures) {

                _gb = gb;
                _imageType = imageType;
                _layerId = layerId;
                _apertures = apertures;
            }

            /// <summary>
            /// Visita un objecte 'EdaLineElement'
            /// </summary>
            /// <param name="line">L'objecte.</param>
            /// 
            public override void Visit(EdaLineElement line) {

                if (CanVisit(line)) {

                    // Calcula les coordinades, mesures, rotacions, etc
                    //
                    EdaPoint startPosition = line.StartPosition;
                    EdaPoint endPosition = line.EndPosition;
                    if (Part != null) {
                        Transformation t = Part.GetLocalTransformation();
                        startPosition = t.Transform(startPosition);
                        endPosition = t.Transform(endPosition);
                    }

                    // Selecciona l'apertura
                    //
                    Aperture ap = _apertures.GetCircleAperture(Math.Max(line.Thickness, 10000));

                    // Escriu el gerber
                    //
                    _gb.SelectAperture(ap);
                    _gb.MoveTo(startPosition);
                    _gb.LineTo(endPosition);
                }
            }

            /// <summary>
            /// Visita un objecte 'EdaArcElement'.
            /// </summary>
            /// <param name="arc">L'objecte.</param>
            /// 
            public override void Visit(EdaArcElement arc) {

                if (CanVisit(arc)) {

                    // Calcula les coordinades, mesures, rotacions, etc
                    //
                    EdaPoint startPosition = arc.StartPosition;
                    EdaPoint endPosition = arc.EndPosition;
                    EdaPoint center = arc.Center;
                    if (Part != null) {
                        Transformation t = Part.GetLocalTransformation();
                        startPosition = t.Transform(startPosition);
                        endPosition = t.Transform(endPosition);
                        center = t.Transform(center);
                    }

                    // Selecciona l'apertura
                    //
                    Aperture ap = _apertures.GetCircleAperture(Math.Max(arc.Thickness, 10000));

                    // Escriu el gerber
                    //
                    _gb.SelectAperture(ap);
                    _gb.MoveTo(startPosition);
                    _gb.ArcTo(endPosition.X, endPosition.Y,
                        center.X - startPosition.X, center.Y - startPosition.Y,
                        arc.Angle.Value < 0 ? ArcDirection.CW : ArcDirection.CCW);
                }
            }

            /// <summary>
            /// Visita un objecte 'EdaRectangleElement'.
            /// </summary>
            /// <param name="rectangle">L'objecte.</param>
            /// 
            public override void Visit(EdaRectangleElement rectangle) {

                if (CanVisit(rectangle)) {

                    if (rectangle.Filled) {

                        // Calcula les coordinades, mesures, rotacions, etc
                        //
                        EdaPoint position = rectangle.Position;
                        EdaAngle rotation = rectangle.Rotation;
                        if (Part != null) {
                            Transformation t = Part.GetLocalTransformation();
                            position = t.Transform(position);
                            rotation += Part.Rotation;
                        }

                        // Selecciona l'apertura
                        //
                        Aperture ap = _apertures.GetRectangleAperture(rectangle.Size.Width, rectangle.Size.Height, rotation);

                        // Escriu el gerber
                        //
                        _gb.SelectAperture(ap);
                        _gb.FlashAt(position);
                    }

                    else {
                        // Selecciona l'apertura
                        //
                        Aperture ap = _apertures.GetCircleAperture(rectangle.Thickness);

                        // Obte el poligon
                        //
                        EdaLayer layer = Board.GetLayer(_layerId);
                        EdaPolygon polygon = rectangle.GetPolygon(layer.Side);
                        EdaPoints points = EdaPoints.Create(polygon.Points);

                        if (Part != null) {
                            Transformation t = Part.GetLocalTransformation();
                            points.Transform(t);
                        }

                        // Escriu el gerber
                        //
                        _gb.SelectAperture(ap);
                        _gb.Polygon(points);
                    }
                }
            }

            /// <summary>
            /// Visita un objecte 'EdaCircleElement'
            /// </summary>
            /// <param name="circle">L'objecte.</param>
            /// 
            public override void Visit(EdaCircleElement circle) {

                if (CanVisit(circle)) {

                    if (circle.Filled) {

                        // Calcula les coordinades, mesures, rotacions, etc
                        //
                        EdaPoint position = circle.Position;
                        if (Part != null) {
                            Transformation t = Part.GetLocalTransformation();
                            position = t.Transform(position);
                        }

                        // Selecciona l'apertura
                        //
                        Aperture ap = _apertures.GetCircleAperture(circle.Diameter);

                        // Escriu el gerber
                        //
                        _gb.SelectAperture(ap);
                        _gb.FlashAt(position);
                    }

                    else {

                        // Selecciona l'apertura
                        //
                        Aperture ap = _apertures.GetCircleAperture(circle.Thickness);

                        // Obte el poligon
                        //
                        EdaLayer layer = Board.GetLayer(_layerId);
                        EdaPolygon polygon = circle.GetPolygon(layer.Side);
                        EdaPoints points = EdaPoints.Create(polygon.Points);

                        if (Part != null) {
                            Transformation t = Part.GetLocalTransformation();
                            points.Transform(t);
                        }

                        // Escriu el gerber
                        //
                        _gb.SelectAperture(ap);
                        _gb.Polygon(points);
                    }
                }
            }

            /// <summary>
            /// Visita un objecte 'EdaTextElement'
            /// </summary>
            /// <param name="text">L'objecte.</param>
            /// 
            public override void Visit(EdaTextElement text) {

                if (CanVisit(text)) {

                    // Selecciona l'apertura
                    //
                    Aperture ap = _apertures.GetCircleAperture(text.Thickness);

                    // Escriu el gerber
                    //
                    _gb.SelectAperture(ap);
                    Font font = FontFactory.Instance.GetFont("Standard");
                    GerberTextDrawer dr = new GerberTextDrawer(font, _gb);

                    PartAttributeAdapter paa = new PartAttributeAdapter(Part, text);
                    EdaPoint position = paa.Position;
                    if (Part != null) {
                        Transformation t = Part.GetLocalTransformation();
                        position = t.Transform(position);
                    }

                    dr.Draw(paa.Value, position, paa.HorizontalAlign, paa.VerticalAlign, paa.Height);
                }
            }

            /// <summary>
            /// Visita un objecte 'EdaViaElement'.
            /// </summary>
            /// <param name="via">L'objecte.</param>
            /// 
            public override void Visit(EdaViaElement via) {

                if (CanVisit(via)) {

                    // Selecciona l'apertura
                    //
                    Aperture ap = null;
                    switch (via.Shape) {
                        default:
                        case EdaViaElement.ViaShape.Circle:
                            ap = _apertures.GetCircleAperture(via.OuterSize);
                            break;

                        case EdaViaElement.ViaShape.Square:
                            ap = _apertures.GetRectangleAperture(via.OuterSize, via.OuterSize, EdaAngle.Zero);
                            break;

                        case EdaViaElement.ViaShape.Octagon:
                            ap = _apertures.GetOctagonAperture(via.OuterSize, EdaAngle.Zero);
                            break;
                    }

                    // Escriu el gerber
                    //
                    _gb.SelectAperture(ap);
                    _gb.FlashAt(via.Position);
                }
            }

            /// <summary>
            /// Visita un objecte 'ThPadElement'
            /// </summary>
            /// <param name="pad">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaThPadElement pad) {

                if (CanVisit(pad)) {

                    // Calcula les coordinades, mesures, rotacions, etc
                    //
                    EdaPoint position = pad.Position;
                    EdaAngle rotation = pad.Rotation;
                    if (Part != null) {
                        rotation += Part.Rotation;
                        Transformation t = Part.GetLocalTransformation();
                        position = t.Transform(position);
                    }

                    // Selecciona l'apertura
                    //
                    Aperture ap;
                    if (pad.CornerRatio.IsMax)
                        ap = _apertures.GetOvalAperture(pad.TopSize.Width, pad.TopSize.Height, rotation);
                    else if (pad.CornerRatio.IsZero)
                        ap = _apertures.GetRectangleAperture(pad.TopSize.Width, pad.TopSize.Height, rotation);
                    else {
                        int radius = pad.CornerRatio * Math.Min(pad.TopSize.Width, pad.TopSize.Height) / 2;
                        ap = _apertures.GetRoundRectangleAperture(pad.TopSize.Width, pad.TopSize.Height, radius, rotation);
                    }
                    _gb.SelectAperture(ap);

                    // Afegeix atributs
                    // 
                    if (_imageType == ImageType.Copper) {
                        _gb.Attribute(AttributeScope.Object, $".P,{Part.Name},{pad.Name}");
                        var signal = Board.GetSignal(pad, Part, false);
                        if (signal != null)
                            _gb.Attribute(AttributeScope.Object, $".N,{signal.Name}");
                    }

                    // Flashea
                    //
                    _gb.FlashAt(position);

                    // Borra els atribits
                    //
                    if (_imageType == ImageType.Copper)
                        _gb.Attribute(AttributeScope.Delete);
                }
            }

            /// <summary>
            /// Visita un objecte 'SmdPadElement'
            /// </summary>
            /// <param name="pad">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaSmdPadElement pad) {

                if (CanVisit(pad)) {

                    // Calcula les coordinades, mesures, rotacions, etc
                    //
                    EdaPoint position = pad.Position;
                    EdaAngle rotation = pad.Rotation;
                    if (Part != null) {
                        rotation += Part.Rotation;
                        Transformation t = Part.GetLocalTransformation();
                        position = t.Transform(position);
                    }

                    // Selecciona l'apertura
                    //
                    Aperture ap;
                    if (pad.CornerRatio.IsZero)
                        ap = _apertures.GetRectangleAperture(pad.Size.Width, pad.Size.Height, rotation);
                    else if (pad.CornerRatio.IsMax)
                        ap = _apertures.GetOvalAperture(pad.Size.Width, pad.Size.Height, rotation);
                    else
                        ap = _apertures.GetRoundRectangleAperture(pad.Size.Width, pad.Size.Height, pad.CornerSize, rotation);
                    _gb.SelectAperture(ap);

                    // Afegeix atributs
                    //
                    if (_imageType == ImageType.Copper) {
                        _gb.Attribute(AttributeScope.Object, $".P,{Part.Name},{pad.Name}");
                        var signal = Board.GetSignal(pad, Part, false);
                        if (signal != null)
                            _gb.Attribute(AttributeScope.Object, $".N,{signal.Name}");
                    }

                    // Flashea
                    //
                    _gb.FlashAt(position);

                    // Borra atributs
                    //
                    if (_imageType == ImageType.Copper)
                        _gb.Attribute(AttributeScope.Delete);
                }
            }

            private bool CanVisit(EdaElement element) {

                return element.IsOnLayer(_layerId);
            }
        }

        /// <summary>
        /// Clase per generar la imatge dels texts
        /// </summary>
        /// 
        private class GerberTextDrawer : TextDrawer {

            private readonly GerberBuilder gb;

            public GerberTextDrawer(Font font, GerberBuilder gb) :
                base(font) {

                this.gb = gb;
            }

            protected override void Trace(EdaPoint position, bool stroke, bool first) {

                if (first || !stroke)
                    gb.MoveTo(position);
                else
                    gb.LineTo(position);
            }
        }

        /// <summary>
        /// Clase per generar la imatge amb regions poligonals.
        /// </summary>
        private sealed class RegionGeneratorVisitor : EdaElementVisitor {

            private readonly GerberBuilder _gb;
            private readonly EdaLayerId _layerId;
            private readonly ApertureDictionary _apertures;

            /// <summary>
            /// Constructor del objecte.
            /// </summary>
            /// <param name="gb">Generador de codi gerber.</param>
            /// <param name="board">La placa.</param>
            /// <param name="layer">La capa a procesar.</param>
            /// <param name="apertures">Diccionari d'apertures.</param>
            /// 
            public RegionGeneratorVisitor(GerberBuilder gb, EdaLayerId layerId, ApertureDictionary apertures) {

                _gb = gb;
                _layerId = layerId;
                _apertures = apertures;
            }

            /// <summary>
            /// Visita un objecte RegionElement
            /// </summary>
            /// <param name="region">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaRegionElement region) {

                if (CanVisit(region)) {
                    Transformation t = new Transformation();
                    if (Part != null)
                        t = Part.GetLocalTransformation();
                    EdaPolygon polygon = Board.GetRegionPolygon(region, _layerId, t);
                    DrawPolygon(polygon, region.Thickness);
                }
            }

            /// <summary>
            /// Dibuixa un poligon
            /// </summary>
            /// <param name="polygon">El poligon a dibuixar.</param>
            /// <param name="thickness">Amplada del perfil.</param>
            /// 
            private void DrawPolygon(EdaPolygon polygon, int thickness) {

                DrawPolygon(polygon, (polygon.Points != null) ? 1 : 0, thickness);
            }

            /// <summary>
            /// Dibuixa un poligon
            /// </summary>
            /// <param name="polygon">El poligon a dibuixar.</param>
            /// <param name="level">Nivell d'anidad del poligon.</param>
            /// <param name="thickness">Amplada del perfil.</param>
            /// 
            private void DrawPolygon(EdaPolygon polygon, int level, int thickness) {

                // Procesa el poligon
                //
                if (polygon.Points != null) {

                    // Dibuixa el contingut de la regio
                    //
                    _gb.LoadPolarity((level % 2) == 0 ? Polarity.Clear : Polarity.Dark);
                    _gb.BeginRegion();
                    _gb.Region(polygon.Points, true);
                    _gb.EndRegion();

                    // Dibuixa el perfil de la regio
                    //
                    Aperture ap = _apertures.GetCircleAperture(thickness);
                    _gb.SelectAperture(ap);
                    _gb.LoadPolarity(Polarity.Dark);
                    _gb.Polygon(polygon.Points);
                }

                // Processa els fills. Amb level < 2 evitem els poligons orfres
                //
                if ((polygon.Childs != null) && (level < 2))
                    foreach (EdaPolygon child in polygon.Childs)
                        DrawPolygon(child, level + 1, thickness);
            }

            private bool CanVisit(EdaElement element) {

                return element.IsOnLayer(_layerId);
            }
        }
    }
}
