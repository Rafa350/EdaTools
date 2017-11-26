namespace MikroPic.EdaTools.v1.Cam.Gerber {

    using System.Globalization;
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Text;
    using MikroPic.EdaTools.v1.Model;
    using MikroPic.EdaTools.v1.Model.Elements;
    using MikroPic.EdaTools.v1.Cam.Gerber.Infrastructure;

    public sealed class GerberGenerator {

        private readonly GerberGeneratorOptions options;
        private GerberBuilder gb;

        public GerberGenerator(GerberGeneratorOptions options) {

            if (options == null)
                this.options = new GerberGeneratorOptions();
            else
                this.options = options;
        }

        public void Generate(Board board, string fileName) {

            // Crea totes les apertures necesaries
            //
            IDictionary<string, ApertureBase> apertures = ApertureTableGenerator.Generate(board);

            // Escriu el fitxer de sortida
            //
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.UTF8)) {

                    GerberBuilder gb = new GerberBuilder(writer);

                    // Definicio d'unitats i format de coordinades
                    //
                    gb.SetUnits(Units.Milimeters);
                    gb.SetPrecision(7, 4);

                    // Definicio de macros per l'apertura X
                    // $1: Amplada
                    // $2: Alçada
                    // $3: Radi de corvatura
                    // $4: Angle de rotacio
                    //
                    gb.DefineMacro(
                        "21,1,$1,$2-$3-$3,0,0,$4*" +
                        "21,1,$2-$3-$3,$2,0,0,$4*" +
                        "$5=$1/2*" +
                        "$6=$2/2*" +
                        "$7=2X$3*" +
                        "1,1,$7,$5-$3,$6-$3,$4*" +
                        "1,1,$7,-$5+$3,$6-$3,$4*" +
                        "1,1,$7,-$5+$3,-$6+$3,$4*" +
                        "1,1,$7,$5-$3,-$6+$3,$4*");

                    // Definicio de les apertures
                    //
                    foreach (ApertureBase aperture in apertures.Values)
                        gb.Escape(aperture.GetDeclarationCommand());

                    gb.Escape("%");

                    // Definicio de la imatge
                    //
                     if (board.Signals != null)
                        foreach (Signal signal in board.Signals) {
                            if (signal.Elements != null)
                                foreach (ElementBase element in signal.Elements) {
                                    ViaElement via = element as ViaElement;
                                    if (via != null) {
                                        string key = ApertureKeyGenerator.GenerateKey(via);
                                        ApertureBase aperture = apertures[key];

                                        gb.SelectAperture(aperture.Id);
                                        gb.Operation(via.Position.X, via.Position.Y, OperationCode.Flash);
                                    }
                                }
                        }

                    // Final
                    //
                    gb.EndFile();
                }

            }
        }
    }
}
