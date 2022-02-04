using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Cam.Generators.Gerber.Builder;
using MikroPic.EdaTools.v1.Cam.Model;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;
using System;
using System.IO;

namespace MikroPic.EdaTools.v1.Cam.Generators.Gerber {

    /// <summary>
    /// Clase per generar fitxers gerber de forats i fresats.
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
        /// Visitador per preparar les apertures. Visita els element que tenen forats.
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

            /// <summary>
            /// Visita un element de tipus 'Line'.
            /// </summary>
            /// <param name="line">L'element a visitar.</param>
            /// 
            public override void Visit(EdaLineElement line) {

                if (line.IsOnLayer(_layerId))
                    _apertures.DefineCircleAperture(line.Thickness);
            }

            /// <summary>
            /// Visuita un element de tipus 'Arc'
            /// </summary>
            /// <param name="arc">L'element a visitar.</param>
            /// 
            public override void Visit(EdaArcElement arc) {

                if (arc.IsOnLayer(_layerId))
                    _apertures.DefineCircleAperture(arc.Thickness);
            }

            /// <summary>
            /// Visita un element de tipus 'Circle'
            /// </summary>
            /// <param name="circle">L'element a visitar.</param>
            /// 
            public override void Visit(EdaCircleElement circle) {

                if (circle.IsOnLayer(_layerId) && !circle.Filled)
                    _apertures.DefineCircleAperture(circle.Thickness);
            }

            /// <summary>
            /// Visita un element de tipus 'Rectangle'
            /// </summary>
            /// <param name="rectangle">L'element a visitar.</param>
            /// 
            public override void Visit(EdaRectangleElement rectangle) {

                if (rectangle.IsOnLayer(_layerId) && !rectangle.Filled)
                    _apertures.DefineCircleAperture(rectangle.Thickness);
            }
        }

        /// <summary>
        /// Visitador per generar la imatge. Visita els elements que tenen forars.
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

            /// <summary>
            /// Visita un object 'Arc'
            /// </summary>
            /// <param name="arc">L'element a visitar.</param>
            /// 
            public override void Visit(EdaArcElement arc) {

                if (arc.IsOnLayer(_layerId)) {

                    EdaPoint startPosition = arc.StartPosition;
                    EdaPoint endPosition = arc.EndPosition;
                    EdaPoint center = arc.Center;
                    if (Part != null) {
                        Transformation t = Part.GetLocalTransformation();
                        startPosition = t.Transform(startPosition);
                        endPosition = t.Transform(endPosition);
                        center = t.Transform(center);
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
            /// Visita un object 'Line'
            /// </summary>
            /// <param name="line">L'element a visitar.</param>
            /// 
            public override void Visit(EdaLineElement line) {

                if (line.IsOnLayer(_layerId)) {

                    EdaPoint startPosition = line.StartPosition;
                    EdaPoint endPosition = line.EndPosition;
                    if (Part != null) {
                        Transformation t = Part.GetLocalTransformation();
                        startPosition = t.Transform(startPosition);
                        endPosition = t.Transform(endPosition);
                    }

                    Aperture ap = _apertures.GetCircleAperture(line.Thickness);
                    _gb.SelectAperture(ap);

                    _gb.MoveTo(startPosition);
                    _gb.LineTo(endPosition);
                }
            }

            /// <summary>
            /// Visita un object 'Circle'
            /// </summary>
            /// <param name="circle">L'element a visitar.</param>
            /// 
            public override void Visit(EdaCircleElement circle) {

                if (circle.IsOnLayer(_layerId) && !circle.Filled) {

                    EdaPoint position = circle.Position;
                    if (Part != null) {
                        Transformation t = Part.GetLocalTransformation();
                        position = t.Transform(position);
                    }

                    Aperture ap = _apertures.GetCircleAperture(circle.Thickness);
                }
            }

            /// <summary>
            /// Visita un object 'Rectangle'
            /// </summary>
            /// <param name="rectangle">L'element a visitar.</param>
            /// 
            public override void Visit(EdaRectangleElement rectangle) {

                if (rectangle.IsOnLayer(_layerId) && !rectangle.Filled) {

                    EdaPoint position = rectangle.Position;
                    if (Part != null) {
                        Transformation t = Part.GetLocalTransformation();
                        position = t.Transform(position);
                    }

                    Aperture ap = _apertures.GetRectangleAperture(rectangle.Size.Width, rectangle.Size.Height, rectangle.Rotation);
                }
            }
        }
    }
}

