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
    /// Clase per generar fitxers gerber del components.
    /// </summary>
    public sealed class GerberComponentGenerator: Generator {

        private readonly Aperture _outlineAperture;
        private Aperture _centroidAperture;
        private readonly Aperture _padKeyAperture;
        private readonly Aperture _padAperture;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="target">El target.</param>
        /// 
        public GerberComponentGenerator(Target target) :
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
        /// Prepara el diccionari d'apertures. Son apertures fixes per representar
        /// els centroids, els pads, etc.
        /// </summary>
        /// <param name="apertures">El diccionari d'apertures.</param>
        /// <param name="board">La placa.</param>
        /// <param name="layerNames">Els noms de les capes a procesar.</param>
        /// 
        private void PrepareApertures(ApertureDictionary apertures, EdaBoard board) {

            // Defineix la apertura pel centroid
            //
            _centroidAperture = apertures.DefineCircleAperture(300000);

            // Defineix la apertura pel pin 1
            //
            //_padKeyAperture = apertures.DefineCircleAperture();

            // Defineix la apertura els pins diferens de 1
            //
            //_padAperture = apertures.DefineCircleAperture(0);
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

            int layerLevel = Int32.Parse(Target.GetOptionValue("layerLevel"));
            string layerSide = Target.GetOptionValue("layerSide");

            gb.Attribute(AttributeScope.File, String.Format(".FileFunction,Component,L{0},{1}", layerLevel, layerSide));
            gb.Attribute(AttributeScope.File, ".FilePolarity,Positive");
            gb.SetUnits(Units.Milimeters);
            gb.SetCoordinateFormat(8, 5);
            gb.LoadPolarity(Polarity.Dark);
            gb.LoadRotation(EdaAngle.Zero);
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

            gb.Comment("BEGIN APERTURES");
            gb.Attribute(AttributeScope.Aperture, ".AperFunction,ComponentMain");
            gb.DefineAperture(_centroidAperture);
            gb.Comment("END APERTURES");
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
                PartSide side = EdaLayerId.Get(name).Side == BoardSide.Top ? PartSide.Top : PartSide.Bottom;
                var visitor = new ImageGeneratorVisitor(gb, side, _centroidAperture);
                board.AcceptVisitor(visitor);
            }
            gb.Comment("END IMAGE");
        }

        /// <summary>
        /// Visitador per generar la imatge
        /// </summary>
        private sealed class ImageGeneratorVisitor: EdaElementVisitor {

            private readonly GerberBuilder _gb;
            private readonly PartSide _side;
            private readonly Aperture _centroidAperture;

            /// <summary>
            /// Constructor de la clase.
            /// </summary>
            /// <param name="gb">El generador de gerbers.</param>
            /// <param name="layerId">Identificador de la capa a procesar.</param>
            /// <param name="centroidAperture">L'apertura pel centroid.</param>
            /// 
            public ImageGeneratorVisitor(GerberBuilder gb, PartSide side, Aperture centroidAperture) {

                _gb = gb;
                _side = side;
                _centroidAperture = centroidAperture;
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaPart part) {

                if (part.Side == _side) {
                    _gb.SelectAperture(_centroidAperture);
                    _gb.Attribute(AttributeScope.Object, $".C,{part.Name}");
                    _gb.Attribute(AttributeScope.Object, $".CRot,{part.Rotation.AsDegrees}");
                    _gb.FlashAt(part.Position.X, part.Position.Y);
                }
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaSmdPadElement pad) {

                //_gb.SelectAperture(_PadAperture);
            }
        }
    }
}

