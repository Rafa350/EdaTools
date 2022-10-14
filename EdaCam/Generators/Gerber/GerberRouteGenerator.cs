using System;
using System.IO;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Cam.Generators.Gerber.Builder;
using MikroPic.EdaTools.v1.Cam.Model;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;

namespace MikroPic.EdaTools.v1.Cam.Generators.Gerber {

    /// <summary>
    /// Clase per generar fitxers gerber de forats ranurats.
    /// </summary>
    public sealed class GerberRouteGenerator: Generator {

        public enum RouteType {
            Plated,
            NonPlated,
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="target">El target.</param>
        /// 
        public GerberRouteGenerator(Target target) :
            base(target) {
        }

        /// <summary>
        /// Genera el fitxer corresponent a la place.
        /// </summary>
        /// <param name="board">La place.</param>
        /// <param name="outputFolder">La carpeta de sortida.</param>
        /// <param name="options">Opcions.</param>
        /// 
        public override void Generate(EdaBoard board, string outputFolder, GeneratorOptions options = null) {

            if (board == null)
                throw new ArgumentNullException(nameof(board));

            if (String.IsNullOrEmpty(outputFolder))
                throw new ArgumentNullException(nameof(outputFolder));

            string fileName = Path.Combine(outputFolder, Target.FileName);

            // Crea el fitxer de sortida
            //
            using (TextWriter writer = new StreamWriter(
                new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))) {

                // Prepara el diccionari d'apertures
                //
                var apertures = new ApertureDictionary();
                PrepareApertures(apertures, board);

                // Prepara el generador de gerbers
                //
                var gb = new GerberBuilder(writer);
                gb.SetTransformation(Target.Position, Target.Rotation);

                // Genera la capcelera del fitxer
                //
                GenerateFileHeader(gb);

                // Genera la llista d'apertures
                //
                GenerateApertures(gb, apertures);

                // Genera les imatges de la placa
                //
                GenerateImage(gb, board, apertures);

                // Genera el final del fitxer
                //
                GenerateFileTail(gb);
            }
        }

        /// <summary>
        /// Prepara el diccionari d'apertures
        /// </summary>
        /// <param name="apertures">El diccionari d'apertures.</param>
        /// <param name="board">La placa.</param>
        /// <param name="layerNames">Els noms de les capes a procesar.</param>
        /// 
        private void PrepareApertures(ApertureDictionary apertures, EdaBoard board) {

            foreach (var name in Target.LayerNames) {
                var visitor = new PrepareAperturesVisitor(EdaLayerId.Get(name), apertures);
                board.AcceptVisitor(visitor);
            }
        }

        /// <summary>
        /// Genera la capcelera del fitxer.
        /// </summary>
        /// <param name="gb">El generador de gerbers</param>
        /// 
        private void GenerateFileHeader(GerberBuilder gb) {

            gb.Comment("EdaTools v2.0.");
            gb.Comment("EdaTools CAM processor. Gerber generator.");
            gb.Comment(String.Format("Start timestamp: {0:HH:mm:ss.fff}", DateTime.Now));
            gb.Comment("BEGIN HEADER");

            RouteType routeType = (RouteType)Enum.Parse(typeof(RouteType), Target.GetOptionValue("routeType"), true);
            int topLevel = Int32.Parse(Target.GetOptionValue("topLevel"));
            int bottomLevel = Int32.Parse(Target.GetOptionValue("bottomLevel"));

            switch (routeType) {
                case RouteType.Plated:
                    gb.Attribute(AttributeScope.File, String.Format(".FileFunction,Plated,{0},{1},PTH,Route", topLevel, bottomLevel));
                    gb.Attribute(AttributeScope.File, ".FilePolarity,Positive");
                    break;

                case RouteType.NonPlated:
                    gb.Attribute(AttributeScope.File, String.Format(".FileFunction,NonPlated,{0},{1},NPTH,Route", topLevel, bottomLevel));
                    gb.Attribute(AttributeScope.File, ".FilePolarity,Positive");
                    break;
            }
            gb.Attribute(AttributeScope.File, ".Part,Single");
            gb.SetUnits(Units.Milimeters);
            gb.SetCoordinateFormat(8, 5);
            gb.SetPolarity(Polarity.Dark);
            gb.SetRotation(EdaAngle.Zero);
            gb.Comment("END HEADER");
        }

        /// <summary>
        /// Genera el peu del fitxer.
        /// </summary>
        /// <param name="gb">El generador de gerbers.</param>
        /// 
        private void GenerateFileTail(GerberBuilder gb) {

            gb.EndFile();
            gb.Comment(String.Format("End timestamp: {0:HH:mm:ss.fff}", DateTime.Now));
            gb.Comment("END FILE");
        }

        /// <summary>
        /// Genera les apertures.
        /// </summary>
        /// <param name="gb">El generador de gerbers.</param>
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
        /// Genera la imatge.
        /// </summary>
        /// <param name="gb">El generador de gerbers.</param>
        /// <param name="board">La placa a procesar.</param>
        /// <param name="apertures">El diccionari d'apoertures.</param>
        /// 
        private void GenerateImage(GerberBuilder gb, EdaBoard board, ApertureDictionary apertures) {

            gb.Comment("BEGIN IMAGE");
            foreach (var name in Target.LayerNames) {
                var visitor = new ImageGeneratorVisitor(gb, EdaLayerId.Get(name), apertures);
                board.AcceptVisitor(visitor);
            }
            gb.Comment("END IMAGE");
        }

        /// <summary>
        /// Visitador per preparar les apertures. Visita els element que generen ranures.
        /// </summary>
        private sealed class PrepareAperturesVisitor: EdaElementVisitor {

            private readonly EdaLayerId _layerId;
            private readonly ApertureDictionary _apertures;

            /// <summary>
            /// Constructor de l'objecte.
            /// </summary>
            /// <param name="layerId">El identificador de la capa a procesar.</param>
            /// <param name="apertures">El diccionari d'apertures a preparar.</param>
            /// 
            public PrepareAperturesVisitor(EdaLayerId layerId, ApertureDictionary apertures) {

                _layerId = layerId;
                _apertures = apertures;
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaThPadElement element) {

                if (element.IsOnLayer(_layerId) && (element.Slot > element.DrillDiameter))
                    _apertures.DefineCircleAperture(element.DrillDiameter);
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaLinearHoleElement element) {

                if (element.IsOnLayer(_layerId))
                    _apertures.DefineCircleAperture(element.Diameter);
            }
        }

        /// <summary>
        /// Visitador per generar la imatge. Visita els elements que generen ranures.
        /// </summary>
        private sealed class ImageGeneratorVisitor: EdaElementVisitor {

            private readonly GerberBuilder _gb;
            private readonly EdaLayerId _layerId;
            private readonly ApertureDictionary _apertures;

            /// <summary>
            /// Constructor de la clase.
            /// </summary>
            /// <param name="gb">El generador de gerbers.</param>
            /// <param name="layerId">Identificador de la capa a procesar.</param>
            /// <param name="apertures">El diccionari d'apertures.</param>
            /// 
            public ImageGeneratorVisitor(GerberBuilder gb, EdaLayerId layerId, ApertureDictionary apertures) {

                _gb = gb;
                _layerId = layerId;
                _apertures = apertures;
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaThPadElement element) {

                if (element.IsOnLayer(_layerId) && (element.Slot > element.DrillDiameter)) {

                }
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaLinearHoleElement element) {

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
                    Aperture ap = _apertures.GetCircleAperture(element.Diameter);

                    // Escriu el gerber
                    //
                    _gb.SelectAperture(ap);
                    _gb.MoveTo(startPosition);
                    _gb.LineTo(endPosition);
                }
            }
        }
    }
}

