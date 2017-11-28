namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using MikroPic.EdaTools.v1.Pcb.Model;
    using System.Collections.Generic;
    using System;
    using System.IO;
    using System.Text;

    public sealed class GerberGenerator {

        // Definicio de macros per l'apertura pad-smd
        // $1: Amplada
        // $2: Alçada
        // $3: Radi de corvatura
        // $4: Angle de rotacio
        //
        private string padApertureMacro =
            "21,1,$1,$2-$3-$3,0,0,$4*" +
            "21,1,$1-$3-$3,$2,0,0,$4*" +
            "$5=$1/2*" +
            "$6=$2/2*" +
            "$7=2x$3*" +
            "1,1,$7,$5-$3,$6-$3,$4*" +
            "1,1,$7,-$5+$3,$6-$3,$4*" +
            "1,1,$7,-$5+$3,-$6+$3,$4*" +
            "1,1,$7,$5-$3,-$6+$3,$4*";

        private readonly GerberGeneratorOptions options;

        public GerberGenerator(GerberGeneratorOptions options) {

            if (options == null)
                this.options = new GerberGeneratorOptions();
            else
                this.options = options;
        }

        public void Generate(Board board, IList<Layer> layers, string fileName) {

            if (board == null)
                throw new ArgumentNullException("board");

            if (layers == null)
                throw new ArgumentNullException("layers");

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

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

                    // Definicio de macros per les apertures
                    //
                    gb.DefineMacro(-1, padApertureMacro);

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
