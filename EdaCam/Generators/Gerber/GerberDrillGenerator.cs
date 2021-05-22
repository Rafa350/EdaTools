using System;
using System.IO;
using System.Linq;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Cam.Generators.Gerber.Builder;
using MikroPic.EdaTools.v1.Cam.Model;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;

namespace MikroPic.EdaTools.v1.Cam.Generators.Gerber {

    /// <summary>
    /// Clase per generar fitxers gerber de forats i fresats.
    /// </summary>
    public sealed class GerberDrillGenerator : Generator {

        public enum DrillType {
            PlatedDrill,
            NonPlatedDrill,
            PlatedRoute,
            NonPlatedRoute
        }

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="target">El target.</param>
        /// 
        public GerberDrillGenerator(Target target) :
            base(target) {
        }

        /// <summary>
        /// Genera el fitxer corresponent a la place.
        /// </summary>
        /// <param name="board">La place.</param>
        /// <param name="outputFolder">La carpeta de sortida.</param>
        /// <param name="options">Opcions.</param>
        /// 
        public override void Generate(Board board, string outputFolder, GeneratorOptions options = null) {

            if (board == null)
                throw new ArgumentNullException("panel");

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
        private void PrepareApertures(ApertureDictionary apertures, Board board) {

            foreach (var layerId in Target.LayerNames.Select(name => LayerId.Get(name))) {
                IBoardVisitor visitor = new PrepareAperturesVisitor(layerId, apertures);
                board.AcceptVisitor(visitor);
            }
        }

        /// <summary>
        /// Genera la capcelera del fitxer.
        /// </summary>
        /// <param name="gb">El generador de gerbers</param>
        /// 
        private void GenerateFileHeader(GerberBuilder gb) {

            gb.Comment("EdaTools v1.0.");
            gb.Comment("EdaTools CAM processor. Gerber generator.");
            gb.Comment(String.Format("Start timestamp: {0:HH:mm:ss.fff}", DateTime.Now));
            gb.Comment("BEGIN HEADER");

            DrillType drillType = (DrillType)Enum.Parse(typeof(DrillType), Target.GetOptionValue("drillType"), true);
            int topLevel = Int32.Parse(Target.GetOptionValue("topLevel"));
            int bottomLevel = Int32.Parse(Target.GetOptionValue("bottomLevel"));

            switch (drillType) {
                case DrillType.PlatedDrill:
                    gb.Attribute(AttributeScope.File, String.Format(".FileFunction,Plated,{0},{1},PTH,Drill", topLevel, bottomLevel));
                    gb.Attribute(AttributeScope.File, ".FilePolarity,Positive");
                    break;

                case DrillType.NonPlatedDrill:
                    gb.Attribute(AttributeScope.File, String.Format(".FileFunction,NonPlated,{0},{1},NPTH,Drill", topLevel, bottomLevel));
                    gb.Attribute(AttributeScope.File, ".FilePolarity,Positive");
                    break;

                case DrillType.PlatedRoute:
                    gb.Attribute(AttributeScope.File, String.Format(".FileFunction,Plated,{0},{1},PTH,Route", topLevel, bottomLevel));
                    gb.Attribute(AttributeScope.File, ".FilePolarity,Positive");
                    break;

                case DrillType.NonPlatedRoute:
                    gb.Attribute(AttributeScope.File, String.Format(".FileFunction,NonPlated,{0},{1},NPTH,Route", topLevel, bottomLevel));
                    gb.Attribute(AttributeScope.File, ".FilePolarity,Positive");
                    break;
            }
            gb.Attribute(AttributeScope.File, ".Part,Single");
            gb.SetUnits(Units.Milimeters);
            gb.SetCoordinateFormat(8, 5);
            gb.LoadPolarity(Polarity.Dark);
            gb.LoadRotation(Angle.Zero);
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
            gb.DefineApertures(apertures.Apertures);
            gb.Comment("END APERTURES");
        }

        /// <summary>
        /// Genera la imatge.
        /// </summary>
        /// <param name="gb">El generador de gerbers.</param>
        /// <param name="board">La placa a procesar.</param>
        /// <param name="apertures">El diccionari d'apoertures.</param>
        /// 
        private void GenerateImage(GerberBuilder gb, Board board, ApertureDictionary apertures) {

            gb.Comment("BEGIN IMAGE");
            foreach (var layerId in Target.LayerNames.Select(name => LayerId.Get(name))) {
                IBoardVisitor visitor = new ImageGeneratorVisitor(gb, layerId, apertures);
                board.AcceptVisitor(visitor);
            }
            gb.Comment("END IMAGE");
        }

        /// <summary>
        /// Visitador per preparar les apertures. Visita els element que tenen forats.
        /// </summary>
        private sealed class PrepareAperturesVisitor : ElementVisitor {

            private readonly LayerId _layerId;
            private readonly ApertureDictionary _apertures;

            /// <summary>
            /// Constructor de l'objecte.
            /// </summary>
            /// <param name="layerId">El identificador de la capa a procesar.</param>
            /// <param name="apertures">El diccionari d'apertures a preparar.</param>
            /// 
            public PrepareAperturesVisitor(LayerId layerId, ApertureDictionary apertures) {

                _layerId = layerId;
                _apertures = apertures;
            }

            /// <summary>
            /// Visita un element de tipus 'LineElement'.
            /// </summary>
            /// <param name="line">L'element a visitar.</param>
            /// 
            public override void Visit(LineElement line) {

                if (CanVisit(line))
                    _apertures.DefineCircleAperture(line.Thickness);
            }

            /// <summary>
            /// Visuita un element de tipus 'ArcElement'
            /// </summary>
            /// <param name="arc">L'element a visitar.</param>
            /// 
            public override void Visit(ArcElement arc) {

                if (CanVisit(arc))
                    _apertures.DefineCircleAperture(arc.Thickness);
            }

            /// <summary>
            /// Visita un element de tipus 'HoleElement'
            /// </summary>
            /// <param name="hole">L'element a visitar.</param>
            /// 
            public override void Visit(HoleElement hole) {

                if (CanVisit(hole))
                    _apertures.DefineCircleAperture(hole.Drill);
            }

            /// <summary>
            /// Visita un element de tipus 'ViaElement'
            /// </summary>
            /// <param name="via">L'element a visitar.</param>
            /// 
            public override void Visit(ViaElement via) {

                if (CanVisit(via))
                    _apertures.DefineCircleAperture(via.Drill);
            }

            /// <summary>
            /// Viita un element de tipus 'ThPadElement'
            /// </summary>
            /// <param name="pad">L'element a visitar.</param>
            /// 
            public override void Visit(ThPadElement pad) {

                if (CanVisit(pad))
                    _apertures.DefineCircleAperture(pad.Drill);
            }

            private bool CanVisit(Element element) {

                return element.LayerSet.Contains(_layerId);
            }
        }

        /// <summary>
        /// Visitador per generar la imatge. Visita els elements que tenen forars.
        /// </summary>
        private sealed class ImageGeneratorVisitor : ElementVisitor {

            private readonly GerberBuilder _gb;
            private readonly LayerId _layerId;
            private readonly ApertureDictionary _apertures;

            /// <summary>
            /// Constructor de la clase.
            /// </summary>
            /// <param name="gb">El generador de gerbers.</param>
            /// <param name="layerId">Identificador de la capa a procesar.</param>
            /// <param name="apertures">El diccionari d'apertures.</param>
            /// 
            public ImageGeneratorVisitor(GerberBuilder gb, LayerId layerId, ApertureDictionary apertures) {

                _gb = gb;
                _layerId = layerId;
                _apertures = apertures;
            }

            /// <summary>
            /// Visita un object 'ArcElement'
            /// </summary>
            /// <param name="arc">L'element a visitar.</param>
            /// 
            public override void Visit(ArcElement arc) {

                if (CanVisit(arc)) {

                    Point startPosition = arc.StartPosition;
                    Point endPosition = arc.EndPosition;
                    Point center = arc.Center;
                    if (Part != null) {
                        Transformation t = Part.GetLocalTransformation();
                        startPosition = t.ApplyTo(startPosition);
                        endPosition = t.ApplyTo(endPosition);
                        center = t.ApplyTo(center);
                    }

                    Aperture ap = _apertures.GetCircleAperture(arc.Thickness);

                    _gb.SelectAperture(ap);
                    _gb.MoveTo(startPosition);
                    _gb.ArcTo(endPosition.X, endPosition.Y,
                        center.X - startPosition.X, center.Y - startPosition.Y,
                        arc.Angle.Value < 0 ? ArcDirection.CW : ArcDirection.CCW);
                }
            }

            /// <summary>
            /// Visita un object 'LineElement'
            /// </summary>
            /// <param name="line">L'element a visitar.</param>
            /// 
            public override void Visit(LineElement line) {

                if (CanVisit(line)) {

                    Point startPosition = line.StartPosition;
                    Point endPosition = line.EndPosition;
                    if (Part != null) {
                        Transformation t = Part.GetLocalTransformation();
                        startPosition = t.ApplyTo(startPosition);
                        endPosition = t.ApplyTo(endPosition);
                    }

                    Aperture ap = _apertures.GetCircleAperture(line.Thickness);

                    _gb.SelectAperture(ap);
                    _gb.MoveTo(startPosition);
                    _gb.LineTo(endPosition);
                }
            }

            /// <summary>
            /// Visita un object 'HoleElement'
            /// </summary>
            /// <param name="hole">El element a visitar.</param>
            /// 
            public override void Visit(HoleElement hole) {

                if (CanVisit(hole)) {

                    Point position = hole.Position;
                    if (Part != null) {
                        Transformation t = Part.GetLocalTransformation();
                        position = t.ApplyTo(position);
                    }

                    Aperture ap = _apertures.GetCircleAperture(hole.Drill);

                    _gb.SelectAperture(ap);
                    _gb.FlashAt(position);
                }
            }

            /// <summary>
            /// Visita un objecte 'ViaElement'.
            /// </summary>
            /// <param name="via">L'objecte a visitar.</param>
            /// 
            public override void Visit(ViaElement via) {

                if (CanVisit(via)) {

                    Point position = via.Position;
                    if (Part != null) {
                        Transformation t = Part.GetLocalTransformation();
                        position = t.ApplyTo(position);
                    }

                    Aperture ap = _apertures.GetCircleAperture(via.Drill);

                    _gb.SelectAperture(ap);
                    _gb.FlashAt(position);
                }
            }

            /// <summary>
            /// Visita un objecte 'ThPadElement'.
            /// </summary>
            /// <param name="pad">L'objecte a visitar.</param>
            /// 
            public override void Visit(ThPadElement pad) {

                if (CanVisit(pad)) {

                    Point position = pad.Position;
                    if (Part != null) {
                        Transformation t = Part.GetLocalTransformation();
                        position = t.ApplyTo(position);
                    }

                    Aperture ap = _apertures.GetCircleAperture(pad.Drill);

                    _gb.SelectAperture(ap);
                    _gb.FlashAt(position);
                }
            }

            private bool CanVisit(Element element) {

                return element.LayerSet.Contains(_layerId);
            }
        }
    }
}

