namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using MikroPic.EdaTools.v1.Cam.Gerber.Builder;
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
    /// Clase per generar fitxers gerber de taladrat.
    /// </summary>
    public sealed class GerberDrillGenerator: GerberGenerator {

        public enum DrillType {
            PlatedDrill,
            NonPlatedDrill,
            PlatedRoute,
            NonPlatedRoute,
            Profile
        }

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="board"></param>
        /// 
        public GerberDrillGenerator(Board board) :
            base(board) {
        }

        /// <summary>
        /// Genera el nom del fitxer.
        /// </summary>
        /// <param name="prefix">Prefix del nom.</param>
        /// <param name="drillType">Tipus de fitxer a generar.</param>
        /// <param name="firstLevel">Primer nivell de coure.</param>
        /// <param name="lastLevel">Ultim nivell de coure.</param>
        /// <returns>El nom de fitxer.</returns>
        /// 
        public string GenerateFileName(string prefix, DrillType drillType, int firstLevel = 0, int lastLevel = 0) {

            if (String.IsNullOrEmpty(prefix))
                throw new ArgumentNullException("prefix");

            if ((drillType != DrillType.Profile) && (firstLevel == 0))
                throw new ArgumentOutOfRangeException("firstLevel");

            if ((drillType != DrillType.Profile) && (lastLevel == 0))
                throw new ArgumentOutOfRangeException("lastLevel");

            if (lastLevel <= firstLevel)
                throw new ArgumentOutOfRangeException("lastLevel");

            StringBuilder sb = new StringBuilder();

            sb.Append(prefix);

            switch (drillType) {
                case DrillType.PlatedDrill:
                    sb.AppendFormat("_Plated${0}${1}$PTH$Drill", firstLevel, lastLevel);
                    break;

                case DrillType.NonPlatedDrill:
                    sb.AppendFormat("_NonPlated${0}${1}$NPTH$Drill", firstLevel, lastLevel);
                    break;

                case DrillType.PlatedRoute:
                    sb.AppendFormat("_Plated${0}${1}$PTH$Route", firstLevel, lastLevel);
                    break;

                case DrillType.NonPlatedRoute:
                    sb.AppendFormat("_NonPlated${0}${1}$NPTH$Route", firstLevel, lastLevel);
                    break;

                case DrillType.Profile:
                    sb.AppendFormat("_Profile$NP");
                    break;
            }

            sb.Append(".gbr");

            return sb.ToString();
        }

        /// <summary>
        /// Genera el contingut
        /// </summary>
        /// <param name="writer">Escriptor de sortida.</param>
        /// <param name="layers">Lapes a procesar.</param>
        /// <param name="drillType">Tipus de forat.</param>
        /// <param name="firstLevel">Primer nivell de coure.</param>
        /// <param name="lastLevel">Eltim nivell de coute.</param>
        /// 
        public void GenerateContent(TextWriter writer, IEnumerable<Layer> layers, DrillType drillType, int firstLevel, int lastLevel) {

            if (writer == null)
                throw new ArgumentNullException("writer");

            GerberBuilder gb = new GerberBuilder(writer);

            ApertureDictionary apertures = CreateApertures(layers);

            GenerateFileHeader(gb, drillType, firstLevel, lastLevel);
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

            ApertureDictionary apertures = new ApertureDictionary();

            foreach (Layer layer in layers) {
                IVisitor visitor = new AperturesCreatorVisitor(Board, layer, apertures);
                visitor.Run();
            }

            return apertures;
        }

        /// <summary>
        /// Genera la capcelera del fitxer.
        /// </summary>
        /// <param name="gb">El generador de gerbers</param>
        /// <param name="drillType">El tipus de forat.</param>
        /// <param name="firstLevel">Primer nivell</param>
        /// <param name="lastLevel">Ultim nivell</param>
        /// 
        private void GenerateFileHeader(GerberBuilder gb, DrillType drillType, int firstLevel, int lastLevel) {

            gb.Comment("EdaTools v1.0.");
            gb.Comment("EdaTools CAM processor. Gerber generator.");
            gb.Comment("BEGIN HEADER");

            switch (drillType) {
                case DrillType.PlatedDrill:
                    gb.Attribute(String.Format(".FileFunction,Plated,{0},{1},PTH,Drill", firstLevel, lastLevel));
                    gb.Attribute(".FilePolarity,Positive");
                    break;

                case DrillType.NonPlatedDrill:
                    gb.Attribute(String.Format(".FileFunction,NonPlated,{0},{1},NPTH,Drill", firstLevel, lastLevel));
                    gb.Attribute(".FilePolarity,Positive");
                    break;

                case DrillType.PlatedRoute:
                    gb.Attribute(String.Format(".FileFunction,Plated,{0},{1},PTH,Route", firstLevel, lastLevel));
                    gb.Attribute(".FilePolarity,Positive");
                    break;

                case DrillType.NonPlatedRoute:
                    gb.Attribute(String.Format(".FileFunction,NonPlated,{0},{1},NPTH,Route", firstLevel, lastLevel));
                    gb.Attribute(".FilePolarity,Positive");
                    break;

                case DrillType.Profile:
                    gb.Attribute(".FileFunction,Profile,NP");
                    gb.Attribute(".FilePolarity,Positive");
                    break;
            }
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

            foreach (Layer layer in layers) {
                IVisitor visitor = new ImageGeneratorVisitor(gb, Board, layer, apertures);
                visitor.Run();
            }

            gb.Comment("END IMAGE");
        }

        /// <summary>
        /// Clase utilitzada per crear les apertures.
        /// </summary>
        private sealed class AperturesCreatorVisitor : ElementVisitor {

            private readonly ApertureDictionary apertureDict;

            public AperturesCreatorVisitor(Board board, Layer layer, ApertureDictionary apertures):
                base(board, layer) {

                apertureDict = apertures;
            }

            public override void Visit(HoleElement hole) {

                apertureDict.DefineCircleAperture(hole.Drill);
            }

            public override void Visit(ViaElement via) {

                apertureDict.DefineCircleAperture(via.Drill);
            }

            public override void Visit(ThPadElement pad) {

                apertureDict.DefineCircleAperture(pad.Drill);
            }
        }

        /// <summary>
        /// Clase utilitzada per generar la imatge
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
            /// Visita un object HoleElement
            /// </summary>
            /// <param name="hole">El element a visitar.</param>
            /// 
            public override void Visit(HoleElement hole) {

                Matrix transformation = Part == null ? Matrix.Identity : Part.Transformation;
                Point position = transformation.Transform(hole.Position);

                Aperture ap = apertures.GetCircleAperture(hole.Drill);

                gb.SelectAperture(ap);
                gb.FlashAt(position);
            }

            /// <summary>
            /// Visita un objecte ViaElement.
            /// </summary>
            /// <param name="via">L'objecte a visitar.</param>
            /// 
            public override void Visit(ViaElement via) {

                Matrix transformation = Part == null ? Matrix.Identity : Part.Transformation;
                Point position = transformation.Transform(via.Position);

                Aperture ap = apertures.GetCircleAperture(via.Drill);

                gb.SelectAperture(ap);
                gb.FlashAt(position);
            }

            /// <summary>
            /// Visita un objecte ThPadElement
            /// </summary>
            /// <param name="pad">L'objecte a visitar.</param>
            /// 
            public override void Visit(ThPadElement pad) {

                Matrix transformation = Part == null ? Matrix.Identity : Part.Transformation;
                Point position = transformation.Transform(pad.Position);

                Aperture ap = apertures.GetCircleAperture(pad.Drill);

                gb.SelectAperture(ap);
                gb.FlashAt(position);
            }
        }
    }
}
