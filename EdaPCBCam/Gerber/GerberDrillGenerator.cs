namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Cam.Gerber.Builder;
    using MikroPic.EdaTools.v1.Cam.Gerber.Builder.Apertures;
    using System;
    using System.Text;
    using System.Windows;
    using System.IO;
    using System.Windows.Media;
    using System.Collections.Generic;

    public sealed class GerberDrillGenerator: GerberGenerator {

        public enum DrillType {
            PlatedDrill,
            NonPlatedDrill
        }

        public void Generate(Board board, DrillType drillType, string fileName) {

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
                    IDictionary<string, Aperture> apertures = new Dictionary<string, Aperture>();
                    board.AcceptVisitor(new DefineAperturesVisitor(apertures));

                    GerberBuilder gb = new GerberBuilder(writer);

                    // Definicio de capcelera, unitats, format, etc.
                    //
                    gb.Comment("EdaTools v1.0.");
                    gb.Comment("EdaTools CAM processor. Gerber generator.");
                    gb.Comment("BEGIN HEADER");

                    if (drillType == DrillType.PlatedDrill) 
                        gb.Attribute(".FileFunction,Plated,1,2,PTH,Drill");
                    else
                        gb.Attribute(".FileFunction,Profile,NP");
                    gb.Attribute(".FilePolarity,Positive");
                    gb.SetUnits(Units.Milimeters);
                    gb.SetCoordinateFormat(8, 5);
                    gb.SetOffset(0, 0);
                    gb.SetPolarity(true);
                    gb.SetAperturePolarity(AperturePolarity.Dark);
                    gb.SetApertureRotation(0);
                    gb.Comment("END HEADER");

                    // Definicio de les apertures
                    //
                    gb.Comment("BEGIN APERTURES");
                    gb.DefineApertures(apertures.Values);
                    gb.Comment("END APERTURES");

                    // Definicio de la imatge
                    //
                    gb.Comment("BEGIN IMAGE");
                    board.AcceptVisitor(new ImageGeneratorVisitor(gb, apertures));
                    gb.Comment("END IMAGE");

                    // Final
                    //
                    gb.EndFile();
                }
            }
        }

        private sealed class DefineAperturesVisitor : DefaultVisitor {

            private readonly IDictionary<string, Aperture> apertures;

            public DefineAperturesVisitor(IDictionary<string, Aperture> apertures) {

                this.apertures = apertures;
            }

            public override void Visit(ViaElement via) {

                string key = ApertureKeyGenerator.GenerateKey(via, true);
                if (!apertures.ContainsKey(key))
                    apertures.Add(key, new CircleAperture(via.Drill));
            }

            public override void Visit(ThPadElement pad) {

                string key = ApertureKeyGenerator.GenerateKey(pad, true);
                if (!apertures.ContainsKey(key))
                    apertures.Add(key, new CircleAperture(pad.Drill));
            }
        }


        private sealed class ImageGeneratorVisitor : DefaultVisitor {

            private readonly GerberBuilder gb;
            private readonly IDictionary<string, Aperture> apertures;

            public ImageGeneratorVisitor(GerberBuilder gb, IDictionary<string, Aperture> apertures) {

                this.gb = gb;
                this.apertures = apertures;
            }

            /// <summary>
            /// Visita un element de tipus Via.
            /// </summary>
            /// <param name="via">El element a visitar.</param>
            /// 
            public override void Visit(ViaElement via) {

                string key = ApertureKeyGenerator.GenerateKey(via, true);

                gb.SelectAperture(apertures[key]);
                gb.Operation(via.Position.X, via.Position.Y, OperationCode.Flash);
            }

            /// <summary>
            /// Visita un element de tipus ThPadElement
            /// </summary>
            /// <param name="pad">El element a visitar.</param>
            /// 
            public override void Visit(ThPadElement pad) {

                string key = ApertureKeyGenerator.GenerateKey(pad, true);

                Point p = pad.Position;
                if (VisitingPart != null) {
                    Matrix m = new Matrix();
                    m.Translate(VisitingPart.Position.X, VisitingPart.Position.Y);
                    m.RotateAt(VisitingPart.Rotate, VisitingPart.Position.X, VisitingPart.Position.Y);
                    p = m.Transform(p);
                }

                gb.SelectAperture(apertures[key]);
                gb.Operation(p.X, p.Y, OperationCode.Flash);
            }
        }
    }
}
