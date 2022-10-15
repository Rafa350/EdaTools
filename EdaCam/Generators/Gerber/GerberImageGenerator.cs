using System;
using System.Collections.Generic;
using System.IO;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
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
    public sealed class GerberImageGenerator: Generator {

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
            gb.SetPolarity(Polarity.Dark);
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

            if (apertures.Apertures != null) {
                gb.Comment("BEGIN APERTURES");
                gb.DefineApertures(apertures.Apertures);
                gb.Comment("END APERTURES");
            }
        }

        /// <summary>
        /// Genera la seccio de macros del fitxer.
        /// </summary>
        /// <param name="gb">El generador de codi gerber.</param>
        /// <param name="apertures">El diccionari d'apertures.</param>
        /// 
        private void GenerateMacros(GerberBuilder gb, ApertureDictionary apertures) {

            if (apertures.Macros != null) {
                gb.Comment("BEGIN MACROS");
                gb.DefineMacros(apertures.Macros);
                gb.Comment("END MACROS");
            }
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
        private sealed class ApertureCreatorVisitor: EdaElementVisitor {

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

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaLineElement element) {

                if (element.IsOnLayer(_layerId))
                    _apertures.DefineCircleAperture(Math.Max(element.Thickness, 10000));
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaArcElement element) {

                if (element.IsOnLayer(_layerId))
                    _apertures.DefineCircleAperture(Math.Max(element.Thickness, 10000));
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaRectangleElement element) {

                if (element.IsOnLayer(_layerId)) {

                    // Si es ple, es 'flashea'
                    //
                    if (element.Filled) {
                        EdaAngle rotation = element.Rotation + (Part == null ? EdaAngle.Zero : Part.Rotation);
                        _apertures.DefineRectangleAperture(element.Size.Width, element.Size.Height, rotation);
                    }

                    // En cas contrari es dibuixa
                    //
                    else
                        _apertures.DefineCircleAperture(element.Thickness);
                }
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaCircleElement element) {

                if (element.IsOnLayer(_layerId)) {

                    // Si es ple, es 'flashea'
                    //
                    if (element.Filled)
                        _apertures.DefineCircleAperture(element.Diameter);

                    // En cas contrari es dibuixa
                    //
                    else
                        _apertures.DefineCircleAperture(element.Thickness);
                }
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaTextElement element) {

                if (element.IsOnLayer(_layerId))
                    _apertures.DefineCircleAperture(element.Thickness);
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaViaElement element) {

                _apertures.DefineCircleAperture(element.OuterSize);
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaThPadElement element) {

                if (element.IsOnLayer(_layerId)) {
                    EdaAngle rotation = element.Rotation;
                    if (Part != null)
                        rotation += Part.Rotation;
                    if (element.CornerRatio.IsMax)
                        _apertures.DefineOvalAperture(element.TopSize.Width, element.TopSize.Height, rotation);
                    else if (element.CornerRatio.IsZero)
                        _apertures.DefineRectangleAperture(element.TopSize.Width, element.TopSize.Height, rotation);
                    else {
                        int radius = element.CornerRatio * Math.Min(element.TopSize.Width, element.TopSize.Height) / 2;
                        _apertures.DefineRoundRectangleAperture(element.TopSize.Width, element.TopSize.Height, radius, rotation);
                    }
                }
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaSmdPadElement element) {

                if (element.IsOnLayer(_layerId)) {
                    EdaAngle rotation = element.Rotation;
                    if (Part != null)
                        rotation += Part.Rotation;
                    if (element.CornerRatio.IsZero)
                        _apertures.DefineRectangleAperture(element.Size.Width, element.Size.Height, rotation);
                    else if (element.CornerRatio.IsMax)
                        _apertures.DefineOvalAperture(element.Size.Width, element.Size.Height, rotation);
                    else {
                        int radius = element.CornerRatio * Math.Min(element.Size.Width, element.Size.Height) / 2;
                        _apertures.DefineRoundRectangleAperture(element.Size.Width, element.Size.Height, radius, rotation);
                    }
                }
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaCircularHoleElement element) {

                if (element.IsOnLayer(_layerId))
                    _apertures.DefineCircleAperture(element.Diameter);
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaRegionElement element) {

                if (element.IsOnLayer(_layerId))
                    _apertures.DefineCircleAperture(Math.Max(100000, element.Thickness));
            }
        }

        /// <summary>
        /// Visitador per generar la imatge.
        /// </summary>
        /// 
        private sealed class ImageGeneratorVisitor: EdaElementVisitor {

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

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaLineElement element) {

                if (element.IsOnLayer(_layerId)) {

                    // Calcula les coordinades, mesures, rotacions, etc
                    //
                    EdaPoint startPosition = element.StartPosition;
                    EdaPoint endPosition = element.EndPosition;
                    if (Part != null) {
                        EdaTransformation t = Part.GetLocalTransformation();
                        startPosition = t.Transform(startPosition);
                        endPosition = t.Transform(endPosition);
                    }

                    // Selecciona l'apertura
                    //
                    Aperture ap = _apertures.GetCircleAperture(Math.Max(element.Thickness, 10000));

                    // Escriu el gerber
                    //
                    _gb.SelectAperture(ap);
                    _gb.MoveTo(startPosition);
                    _gb.LineTo(endPosition);
                }
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaArcElement element) {

                if (element.IsOnLayer(_layerId)) {

                    // Calcula les coordinades, mesures, rotacions, etc
                    //
                    EdaPoint startPosition = element.StartPosition;
                    EdaPoint endPosition = element.EndPosition;
                    EdaPoint center = element.Center;
                    if (Part != null) {
                        EdaTransformation t = Part.GetLocalTransformation();
                        startPosition = t.Transform(startPosition);
                        endPosition = t.Transform(endPosition);
                        center = t.Transform(center);
                    }

                    // Selecciona l'apertura
                    //
                    Aperture ap = _apertures.GetCircleAperture(Math.Max(element.Thickness, 10000));

                    // Escriu el gerber
                    //
                    _gb.SelectAperture(ap);
                    _gb.MoveTo(startPosition);
                    _gb.ArcTo(endPosition.X, endPosition.Y,
                        center.X - startPosition.X, center.Y - startPosition.Y,
                        element.Angle.Value < 0 ? ArcDirection.CW : ArcDirection.CCW);
                }
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaRectangleElement element) {

                if (element.IsOnLayer(_layerId)) {

                    if (element.Filled) {

                        // Calcula les coordinades, mesures, rotacions, etc
                        //
                        EdaPoint position = element.Position;
                        EdaAngle rotation = element.Rotation;
                        if (Part != null) {
                            EdaTransformation t = Part.GetLocalTransformation();
                            position = t.Transform(position);
                            rotation += Part.Rotation;
                        }

                        // Selecciona l'apertura
                        //
                        Aperture ap = _apertures.GetRectangleAperture(element.Size.Width, element.Size.Height, rotation);

                        // Escriu el gerber
                        //
                        _gb.SelectAperture(ap);
                        _gb.FlashAt(position);
                    }

                    else {
                        // Selecciona l'apertura
                        //
                        Aperture ap = _apertures.GetCircleAperture(element.Thickness);

                        // Obte el poligon
                        //
                        EdaLayer layer = Board.GetLayer(_layerId);
                        EdaPolygon polygon = element.GetPolygon(layer.Id);
                        IEnumerable<EdaPoint> points = polygon.Outline;

                        if (Part != null) {
                            EdaTransformation t = Part.GetLocalTransformation();
                            points = t.Transform(points);
                        }

                        // Escriu el gerber
                        //
                        _gb.SelectAperture(ap);
                        _gb.Polygon(points);
                    }
                }
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaCircleElement element) {

                if (element.IsOnLayer(_layerId)) {

                    if (element.Filled) {

                        // Calcula les coordinades, mesures, rotacions, etc
                        //
                        EdaPoint position = element.Position;
                        if (Part != null) {
                            EdaTransformation t = Part.GetLocalTransformation();
                            position = t.Transform(position);
                        }

                        // Selecciona l'apertura
                        //
                        Aperture ap = _apertures.GetCircleAperture(element.Diameter);

                        // Escriu el gerber
                        //
                        _gb.SelectAperture(ap);
                        _gb.FlashAt(position);
                    }

                    else {

                        // Selecciona l'apertura
                        //
                        Aperture ap = _apertures.GetCircleAperture(element.Thickness);

                        // Obte el poligon
                        //
                        EdaLayer layer = Board.GetLayer(_layerId);
                        EdaPolygon polygon = element.GetPolygon(layer.Id);
                        IEnumerable<EdaPoint> points = polygon.Outline;

                        if (Part != null) {
                            EdaTransformation t = Part.GetLocalTransformation();
                            points = t.Transform(points);
                        }

                        // Escriu el gerber
                        //
                        _gb.SelectAperture(ap);
                        _gb.Polygon(points);
                    }
                }
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaTextElement element) {

                if (element.IsOnLayer(_layerId)) {

                    // Selecciona l'apertura
                    //
                    Aperture ap = _apertures.GetCircleAperture(element.Thickness);

                    // Escriu el gerber
                    //
                    _gb.SelectAperture(ap);
                    Font font = FontFactory.Instance.GetFont("Standard");
                    GerberTextDrawer dr = new GerberTextDrawer(font, _gb);

                    PartAttributeAdapter paa = new PartAttributeAdapter(Part, element);
                    EdaPoint position = paa.Position;
                    if (Part != null) {
                        EdaTransformation t = Part.GetLocalTransformation();
                        position = t.Transform(position);
                    }

                    dr.Draw(paa.Value, position, paa.HorizontalAlign, paa.VerticalAlign, paa.Height);
                }
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaViaElement element) {

                if (element.IsOnLayer(_layerId)) {

                    // Selecciona l'apertura
                    //
                    Aperture ap = _apertures.GetCircleAperture(element.OuterSize);

                    // Escriu el gerber
                    //
                    _gb.SelectAperture(ap);
                    _gb.FlashAt(element.Position);
                }
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaThPadElement element) {

                if (element.IsOnLayer(_layerId)) {

                    // Calcula les coordinades, mesures, rotacions, etc
                    //
                    EdaPoint position = element.Position;
                    EdaAngle rotation = element.Rotation;
                    if (Part != null) {
                        rotation += Part.Rotation;
                        EdaTransformation t = Part.GetLocalTransformation();
                        position = t.Transform(position);
                    }

                    // Selecciona l'apertura
                    //
                    Aperture ap;
                    if (element.CornerRatio.IsMax)
                        ap = _apertures.GetOvalAperture(element.TopSize.Width, element.TopSize.Height, rotation);
                    else if (element.CornerRatio.IsZero)
                        ap = _apertures.GetRectangleAperture(element.TopSize.Width, element.TopSize.Height, rotation);
                    else {
                        int radius = element.CornerRatio * Math.Min(element.TopSize.Width, element.TopSize.Height) / 2;
                        ap = _apertures.GetRoundRectangleAperture(element.TopSize.Width, element.TopSize.Height, radius, rotation);
                    }
                    _gb.SelectAperture(ap);

                    // Afegeix atributs
                    // 
                    if (_imageType == ImageType.Copper) {
                        _gb.Attribute(AttributeScope.Object, $".P,{Part.Name},{element.Name}");
                        var signal = Board.GetSignal(element, Part, false);
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

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaSmdPadElement element) {

                if (element.IsOnLayer(_layerId)) {

                    // Calcula les coordinades, mesures, rotacions, etc
                    //
                    EdaPoint position = element.Position;
                    EdaAngle rotation = element.Rotation;
                    if (Part != null) {
                        rotation += Part.Rotation;
                        EdaTransformation t = Part.GetLocalTransformation();
                        position = t.Transform(position);
                    }

                    // Selecciona l'apertura
                    //
                    Aperture ap;
                    if (element.CornerRatio.IsZero)
                        ap = _apertures.GetRectangleAperture(element.Size.Width, element.Size.Height, rotation);
                    else if (element.CornerRatio.IsMax)
                        ap = _apertures.GetOvalAperture(element.Size.Width, element.Size.Height, rotation);
                    else
                        ap = _apertures.GetRoundRectangleAperture(element.Size.Width, element.Size.Height, element.CornerSize, rotation);
                    _gb.SelectAperture(ap);

                    // Afegeix atributs
                    //
                    if (_imageType == ImageType.Copper) {
                        _gb.Attribute(AttributeScope.Object, $".P,{Part.Name},{element.Name}");
                        var signal = Board.GetSignal(element, Part, false);
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
        }

        /// <summary>
        /// Clase per generar la imatge dels texts
        /// </summary>
        /// 
        private class GerberTextDrawer: TextDrawer {

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
        private sealed class RegionGeneratorVisitor: EdaElementVisitor {

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

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaRegionElement element) {

                if (element.IsOnLayer(_layerId)) {
                    var t = new EdaTransformation();
                    if (Part != null)
                        t = Part.GetLocalTransformation();
                    var polygons = Board.GetRegionPolygons(element, _layerId, t);
                    foreach (var polygon in polygons)
                        DrawPolygon(polygon, element.Thickness);
                }
            }

            /// <summary>
            /// Dibuixa un poligon
            /// </summary>
            /// <param name="polygon">El poligon a dibuixar.</param>
            /// <param name="level">Nivell d'anidad del poligon.</param>
            /// <param name="thickness">Amplada del perfil.</param>
            /// 
            private void DrawPolygon(EdaPolygon polygon, int thickness) {

                // Procesa el contorn
                //
                if (polygon.Outline != null) {

                    // Dibuixa el contingut de la regio
                    //
                    _gb.SetPolarity(Polarity.Dark);
                    _gb.BeginRegion();
                    _gb.Region(polygon.Outline, true);
                    _gb.EndRegion();

                    // Dibuixa el perfil de la regio per arrodonir les cantonades
                    //
                    Aperture ap = _apertures.GetCircleAperture(Math.Max(100000, thickness));
                    _gb.SelectAperture(ap);
                    _gb.SetPolarity(Polarity.Dark);
                    _gb.Polygon(polygon.Outline);
                }

                // Processa els forats
                //
                if (polygon.HasHoles) {
                    _gb.SetPolarity(Polarity.Clear);
                    foreach (var hole in polygon.Holes) {
                        _gb.BeginRegion();
                        _gb.Region(hole, true);
                        _gb.EndRegion();
                    }
                    _gb.SetPolarity(Polarity.Dark);
                }
            }
        }
    }
}
