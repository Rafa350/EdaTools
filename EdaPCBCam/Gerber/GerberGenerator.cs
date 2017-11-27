namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using MikroPic.EdaTools.v1.Pcb.Model;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public sealed class GerberGenerator {

        private readonly GerberGeneratorOptions options;

        public GerberGenerator(GerberGeneratorOptions options) {

            if (options == null)
                this.options = new GerberGeneratorOptions();
            else
                this.options = options;
        }

        public void Generate(Board board, string fileName) {

            List<Layer> layers = new List<Layer>();
            layers.Add(board.GetLayer(LayerId.Top));
            layers.Add(board.GetLayer(LayerId.Measures));

            // Escriu el fitxer de sortida
            //
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.UTF8)) {

                    GerberBuilder gb = new GerberBuilder(writer);

                    // Definicio d'unitats i format de coordinades
                    //
                    gb.SetUnits(Units.Milimeters);
                    gb.SetCoordinateFormat(8, 5);
                    gb.SetOffset(0, 0);
                    gb.SetPolarity(true);
                    gb.SetAperturePolarity(true);
                    gb.SetApertureRotation(0);

                    // Definicio de macros per l'apertura psd-smd
                    // $1: Amplada
                    // $2: Alçada
                    // $3: Radi de corvatura
                    // $4: Angle de rotacio
                    //
                    gb.DefineMacro(-1,
                        "21,1,$1,$2-$3-$3,0,0,$4*" +
                        "21,1,$1-$3-$3,$2,0,0,$4*" +
                        "$5=$1/2*" +
                        "$6=$2/2*" +
                        "$7=2X$3*" +
                        "1,1,$7,$5-$3,$6-$3,$4*" +
                        "1,1,$7,-$5+$3,$6-$3,$4*" +
                        "1,1,$7,-$5+$3,-$6+$3,$4*" +
                        "1,1,$7,$5-$3,-$6+$3,$4*");

                    // Definicio de les apertures
                    //
                    gb.DefineApertures(board, layers);

                    gb.Escape("%");

                    // Definicio de la imatge
                    //
                    gb.FlasApertures(board, layers);

                    // Final
                    //
                    gb.EndFile();
                }

            }
        }
    }
}
