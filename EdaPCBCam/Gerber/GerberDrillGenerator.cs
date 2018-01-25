namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using MikroPic.EdaTools.v1.Cam.Gerber.Builder;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows.Media;

    /// <summary>
    /// Clase per generar fitxers gerber de taladrat.
    /// </summary>
    public sealed class GerberDrillGenerator: GerberGenerator {

        public enum DrillType {
            PlatedDrill,
            NonPlatedDrill
        }

        public GerberDrillGenerator(Board board) :
            base(board) {
        }

        public void Generate(TextWriter writer, IEnumerable<Layer> layers, DrillType drillType) {

            if (writer == null)
                throw new ArgumentNullException("writer");

            GerberBuilder gb = new GerberBuilder(writer);

            ApertureDictionary apertures = CreateApertures(layers);

            GenerateFileHeader(gb, drillType);
            GenerateApertures(gb, apertures);
            GenerateImage(gb, layers, apertures);
            GenerateFileTail(gb);
        }

        /// <summary>
        /// Crea el diccionari d'aperturess.
        /// </summary>
        /// <param name="layers">Les capes a tenir en compte.</param>
        /// <returns>El diccionari d'aperturees.</returns>
        /// 
        private ApertureDictionary CreateApertures(IEnumerable<Layer> layers) {

            AperturesCreatorVisitor visitor = new AperturesCreatorVisitor(Board, layers);
            visitor.Visit(Board);
            return visitor.AperturesDict;
        }

        /// <summary>
        /// Genera la capcelera del fitxer.
        /// </summary>
        /// <param name="gb">El generador de gerbers</param>
        /// <param name="drillType">El tipus de forat.</param>
        /// 
        private void GenerateFileHeader(GerberBuilder gb, DrillType drillType) {

            gb.Comment("EdaTools v1.0.");
            gb.Comment("EdaTools CAM processor. Gerber generator.");
            gb.Comment("BEGIN HEADER");

            if (drillType == DrillType.PlatedDrill)
                gb.Attribute(String.Format(".FileFunction,Plated,{0},{1},PTH,Drill", 1, 2));
            else
                gb.Attribute(".FileFunction,Profile,NP");
            gb.Attribute(".FilePolarity,Positive");
            gb.Attribute(".Part,Single");
            gb.SetUnits(Units.Milimeters);
            gb.SetCoordinateFormat(8, 5);
            gb.LoadPolarity(Polarity.Dark);
            gb.LoadRotation(0);
            gb.Comment("END HEADER");
        }

        /// <summary>
        /// Genera el peu del fitxer.
        /// </summary>
        /// <param name="gb">El generador de gerbers.</param>
        /// 
        private void GenerateFileTail(GerberBuilder gb) {

            gb.EndFile();
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
        /// <param name="layers">Les capes a tenir en compte.</param>
        /// <param name="apertures">El diccionari d'apoertures.</param>
        /// 
        private void GenerateImage(GerberBuilder gb, IEnumerable<Layer> layers, ApertureDictionary apertures) {

            gb.Comment("BEGIN IMAGE");
            IVisitor visitor = new ImageGeneratorVisitor(gb, Board, layers, apertures);
            visitor.Visit(Board);
            gb.Comment("END IMAGE");
        }

        /// <summary>
        /// Clase utilitzada per crear les apertures.
        /// </summary>
        private sealed class AperturesCreatorVisitor : BoardVisitor {

            private readonly Board board;
            private readonly IEnumerable<Layer> layers;
            private readonly ApertureDictionary apertureDict;

            public AperturesCreatorVisitor(Board board, IEnumerable<Layer> layers) {

                this.board = board;
                this.layers = layers;
                apertureDict = new ApertureDictionary();
            }

            public override void Visit(HoleElement hole) {

                if (board.IsOnAnyLayer(hole, layers)) 
                    apertureDict.DefineCircleAperture(hole.Drill);
            }

            public override void Visit(ViaElement via) {

                if (board.IsOnAnyLayer(via, layers))
                    apertureDict.DefineCircleAperture(via.Drill);
            }

            public override void Visit(ThPadElement pad) {

                if (board.IsOnAnyLayer(pad, layers))
                    apertureDict.DefineCircleAperture(pad.Drill);
            }

            public ApertureDictionary AperturesDict {
                get {
                    return apertureDict;
                }
            }
        }

        /// <summary>
        /// Clase utilitzada per generar la imatge
        /// </summary>
        private sealed class ImageGeneratorVisitor : BoardVisitor {

            private readonly GerberBuilder gb;
            private readonly Board board;
            private readonly IEnumerable<Layer> layers;
            private readonly ApertureDictionary apertures;
            private Matrix localTransformation = Matrix.Identity;

            /// <summary>
            /// Constructor de la clase.
            /// </summary>
            /// <param name="gb">El generador de gerbers.</param>
            /// <param name="board">La placa a procesar.</param>
            /// <param name="layers">Les capes tenir en compte.</param>
            /// <param name="apertures">El diccionari d'apertures.</param>
            /// 
            public ImageGeneratorVisitor(GerberBuilder gb, Board board, IEnumerable<Layer> layers, ApertureDictionary apertures) {

                this.gb = gb;
                this.board = board;
                this.layers = layers;
                this.apertures = apertures;
            }

            /// <summary>
            /// Visita un element de tipus Hole
            /// </summary>
            /// <param name="hole">El element a visitar.</param>
            /// 
            public override void Visit(HoleElement hole) {

                if (board.IsOnAnyLayer(hole, layers)) {
                    Aperture ap = apertures.GetCircleAperture(hole.Drill);
                    gb.SelectAperture(ap);
                    gb.FlashAt(localTransformation.Transform(hole.Position));
                }
            }

            /// <summary>
            /// Visita un element de tipus Via.
            /// </summary>
            /// <param name="via">El element a visitar.</param>
            /// 
            public override void Visit(ViaElement via) {

                if (board.IsOnAnyLayer(via, layers)) {
                    Aperture ap = apertures.GetCircleAperture(via.Drill);
                    gb.SelectAperture(ap);
                    gb.FlashAt(localTransformation.Transform(via.Position));
                }
            }

            /// <summary>
            /// Visita un element de tipus ThPadElement
            /// </summary>
            /// <param name="pad">El element a visitar.</param>
            /// 
            public override void Visit(ThPadElement pad) {

                if (board.IsOnAnyLayer(pad, layers)) {
                    Aperture ap = apertures.GetCircleAperture(pad.Drill);
                    gb.SelectAperture(ap);
                    gb.FlashAt(localTransformation.Transform(pad.Position));
                }
            }

            /// <summary>
            /// Visita un objecte Part
            /// </summary>
            /// <param name="part">El objecte a visitar.</param>
            /// 
            public override void Visit(Part part) {

                localTransformation = part.Transformation;
                base.Visit(part);
                localTransformation = Matrix.Identity;
            }
        }
    }
}
