namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using MikroPic.EdaTools.v1.Cam.Gerber.Builder;
    using MikroPic.EdaTools.v1.Pcb.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Windows;

    public sealed class GerberDrillGenerator: GerberGenerator {

        public enum DrillType {
            PlatedDrill,
            NonPlatedDrill
        }

        public void Generate(Board board, IEnumerable<Layer> layers, DrillType drillType, string fileName) {

            if (board == null)
                throw new ArgumentNullException("board");

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            // Escriu el fitxer de sortida
            //
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {

                    // Crea el diccionari d'apertures
                    //
                    ApertureDictionary apertures = new ApertureDictionary();
                    IVisitor defineAperturesVisitor = new DefineAperturesVisitor(board, layers, apertures);
                    defineAperturesVisitor.Visit(board);

                    GerberBuilder gb = new GerberBuilder(writer);

                    // Definicio de capcelera, unitats, format, etc.
                    //
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

                    // Definicio de les apertures
                    //
                    gb.Comment("BEGIN APERTURES");
                    gb.DefineApertures(apertures.Apertures);
                    gb.Comment("END APERTURES");

                    // Definicio de la imatge
                    //
                    gb.Comment("BEGIN IMAGE");
                    IVisitor imageGeneratorVisitor = new ImageGeneratorVisitor(gb, board, layers, apertures);
                    imageGeneratorVisitor.Visit(board);
                    gb.Comment("END IMAGE");

                    // Final
                    //
                    gb.EndFile();
                }
            }
        }

        /// <summary>
        /// Clase utilitzada per generar les apertures.
        /// </summary>
        private sealed class DefineAperturesVisitor : BoardVisitor {

            private readonly Board board;
            private readonly IEnumerable<Layer> layers;
            private readonly ApertureDictionary apertures;

            public DefineAperturesVisitor(Board board, IEnumerable<Layer> layers, ApertureDictionary apertures) {

                this.board = board;
                this.layers = layers;
                this.apertures = apertures;
            }

            public override void Visit(HoleElement hole) {

                if (board.IsOnAnyLayer(hole, layers)) 
                    apertures.DefineCircleAperture(hole.Drill);
            }

            public override void Visit(ViaElement via) {

                if (board.IsOnAnyLayer(via, layers))
                    apertures.DefineCircleAperture(via.Drill);
            }

            public override void Visit(ThPadElement pad) {

                if (board.IsOnAnyLayer(pad, layers))
                    apertures.DefineCircleAperture(pad.Drill);
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

            public ImageGeneratorVisitor(GerberBuilder gb, Board board, IEnumerable<Layer> layers, ApertureDictionary apertures) {

                this.gb = gb;
                this.board = board;
                this.layers = layers;
                this.apertures = apertures;
            }

            public override void Visit(HoleElement hole) {

                if (board.IsOnAnyLayer(hole, layers)) {
                    Aperture ap = apertures.GetCircleAperture(hole.Drill);
                    gb.SelectAperture(ap);
                    Point p = VisitingPart.Transform(hole.Position);
                    gb.FlashAt(p.X, p.Y);
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
                    gb.FlashAt(via.Position.X, via.Position.Y);
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
                    Point p = VisitingPart.Transform(pad.Position);
                    gb.FlashAt(p.X, p.Y);
                }
            }
        }
    }
}
