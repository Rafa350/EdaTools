namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using MikroPic.EdaTools.v1.Cam.Gerber.Builder;
    using MikroPic.EdaTools.v1.Cam.Model;
    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using System;
    using System.IO;

    /// <summary>
    /// Clase per generar fitxers gerber de forats i fresats.
    /// </summary>
    public sealed class GerberDrillGenerator: Generator {

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
                throw new ArgumentNullException("outputFolder");

            string fileName = Path.Combine(outputFolder, Target.FileName);

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

            foreach (var layerName in Target.LayerNames) {
                Layer layer = board.GetLayer(new LayerId(layerName));
                IVisitor visitor = new PrepareAperturesVisitor(board, layer, apertures);
                visitor.Run();
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
            foreach (var layerName in Target.LayerNames) {
                Layer layer = board.GetLayer(new LayerId(layerName));
                IVisitor visitor = new ImageGeneratorVisitor(gb, board, layer, apertures);
                visitor.Run();
            }
            gb.Comment("END IMAGE");
        }

        /// <summary>
        /// Visitador per preparar les apertures. Visita els element que tenen forats.
        /// </summary>
        private sealed class PrepareAperturesVisitor : ElementVisitor {

            private readonly ApertureDictionary apertures;

            /// <summary>
            /// Constructor de l'objecte.
            /// </summary>
            /// <param name="board">La placa a procesar.</param>
            /// <param name="layer">Les capes a procesar.</param>
            /// <param name="apertures">El diccionari d'apertures a preparar.</param>
            /// 
            public PrepareAperturesVisitor(Board board, Layer layer, ApertureDictionary apertures) :
                base(board, layer) {

                this.apertures = apertures;
            }

            /// <summary>
            /// Visita un element de tipus 'HoleElement'
            /// </summary>
            /// <param name="hole">L'element a visitar.</param>
            /// 
            public override void Visit(HoleElement hole) {

                apertures.DefineCircleAperture(hole.Drill);
            }

            /// <summary>
            /// Visita un element de tipus 'ViaElement'
            /// </summary>
            /// <param name="via">L'element a visitar.</param>
            /// 
            public override void Visit(ViaElement via) {

                apertures.DefineCircleAperture(via.Drill);
            }

            /// <summary>
            /// Viita un element de tipus 'ThPadElement'
            /// </summary>
            /// <param name="pad">L'element a visitar.</param>
            /// 
            public override void Visit(ThPadElement pad) {

                apertures.DefineCircleAperture(pad.Drill);
            }
        }

        /// <summary>
        /// Visitador per generar la imatge. Visita els elements que tenen forars.
        /// </summary>
        private sealed class ImageGeneratorVisitor : ElementVisitor {

            private readonly GerberBuilder gb;
            private readonly ApertureDictionary apertures;

            /// <summary>
            /// Constructor de la clase.
            /// </summary>
            /// <param name="gb">El generador de gerbers.</param>
            /// <param name="board">La placa.</param>
            /// <param name="layer">La capa a procesar.</param>
            /// <param name="apertures">El diccionari d'apertures.</param>
            /// 
            public ImageGeneratorVisitor(GerberBuilder gb, Board board, Layer layer, ApertureDictionary apertures) :
                base(board, layer) {

                this.gb = gb;
                this.apertures = apertures;
            }

            /// <summary>
            /// Visita un object 'HoleElement'
            /// </summary>
            /// <param name="hole">El element a visitar.</param>
            /// 
            public override void Visit(HoleElement hole) {

                Point position = hole.Position;
                if (Part != null) {
                    Transformation t = Part.GetLocalTransformation();
                    position = t.ApplyTo(position);
                }

                Aperture ap = apertures.GetCircleAperture(hole.Drill);

                gb.SelectAperture(ap);
                gb.FlashAt(position);
            }

            /// <summary>
            /// Visita un objecte 'ViaElement'.
            /// </summary>
            /// <param name="via">L'objecte a visitar.</param>
            /// 
            public override void Visit(ViaElement via) {

                Point position = via.Position;
                if (Part != null) {
                    Transformation t = Part.GetLocalTransformation();
                    position = t.ApplyTo(position);
                }

                Aperture ap = apertures.GetCircleAperture(via.Drill);

                gb.SelectAperture(ap);
                gb.FlashAt(position);
            }

            /// <summary>
            /// Visita un objecte 'ThPadElement'.
            /// </summary>
            /// <param name="pad">L'objecte a visitar.</param>
            /// 
            public override void Visit(ThPadElement pad) {

                Point position = pad.Position;
                if (Part != null) {
                    Transformation t = Part.GetLocalTransformation();
                    position = t.ApplyTo(position);
                }

                Aperture ap = apertures.GetCircleAperture(pad.Drill);

                gb.SelectAperture(ap);
                gb.FlashAt(position);
            }
        }
    }
}

