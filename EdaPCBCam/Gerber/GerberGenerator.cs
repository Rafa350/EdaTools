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
        private const int smdApertureMacroId = 0;
        private const string smdApertureMacro =
            "21,1,$1,$2-$3-$3,0,0,$4*" +
            "21,1,$1-$3-$3,$2,0,0,$4*" +
            "$5=$1/2*" +
            "$6=$2/2*" +
            "$7=2x$3*" +
            "1,1,$7,$5-$3,$6-$3,$4*" +
            "1,1,$7,-$5+$3,$6-$3,$4*" +
            "1,1,$7,-$5+$3,-$6+$3,$4*" +
            "1,1,$7,$5-$3,-$6+$3,$4*";

        // Definicio de macros per l'apertura pad-smd
        // $1: Amplada
        // $2: Alçada
        // $3: Diametre del forat
        // $4: Angle de rotacio
        //
        private const int squareApertureMacroId = 1;
        private const string squareApertureMacro =
            "21,1,$1,$2,0,0,$4*" +
            "1,0,$3,0,0,0*";

        private const string oblongApertureMacto = "";


        public GerberGenerator() {
        }

        /// <summary>
        /// Genera un fitxer gerber.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// <param name="layers">Llista de capes a procesar.</param>
        /// <param name="fileName">Nom del fitxer de sortida.</param>
        /// 
        public void Generate(Board board, IList<Layer> layers, string fileName) {

            if (board == null)
                throw new ArgumentNullException("board");

            if ((layers == null) || (layers.Count == 0))
                throw new ArgumentNullException("layers");

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            // Escriu el fitxer de sortida
            //
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {

                    GerberBuilder gb = new GerberBuilder(writer);

                    // Definicio d'unitats i format de coordinades
                    //
                    gb.Comment("EdaTools v1.0.");
                    gb.Comment("EdaTools CAM processor. Gerber generator.");
                    gb.SetUnits(Units.Milimeters);
                    gb.SetCoordinateFormat(8, 5);
                    gb.SetOffset(0, 0);
                    gb.SetPolarity(true);
                    gb.SetAperturePolarity(true);
                    gb.SetApertureRotation(0);

                    // Definicio de macros per les apertures
                    //
                    gb.DefineMacro(smdApertureMacroId, smdApertureMacro);
                    gb.DefineMacro(squareApertureMacroId, squareApertureMacro);

                    // Definicio de les apertures
                    //
                    gb.DefineApertures(board, layers);

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
