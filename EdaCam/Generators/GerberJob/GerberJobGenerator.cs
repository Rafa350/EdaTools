using System;
using System.IO;
using System.Globalization;

using MikroPic.EdaTools.v1.Cam.Model;
using MikroPic.EdaTools.v1.Core.Model.Board;

namespace MikroPic.EdaTools.v1.Cam.Generators.GerberJob {

    public sealed  class GerberJobGenerator: Generator {

        public GerberJobGenerator(Target target):
            base(target) {
        }

        public override void Generate(EdaBoard board, string outputFolder, GeneratorOptions options = null) {

            if (board == null)
                throw new ArgumentNullException(nameof(board));

            if (String.IsNullOrEmpty(outputFolder))
                throw new ArgumentNullException(nameof(outputFolder));

            string fileName = Path.Combine(outputFolder, Target.FileName);
            using (TextWriter writer = new StreamWriter(
                new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))) {

                CultureInfo ci = CultureInfo.InvariantCulture;

                // Obte el tamany de la placa
                //
                double labelWidth = board.Size.Width / 1000000.0;
                double labelHeight = board.Size.Height / 1000000.0;

                // Obte el nombre de capes conductores
                //
                int layerNumber = 0;
                foreach (var layer in board.Layers)
                    if (layer.Function == LayerFunction.Signal)
                        layerNumber++;

                writer.WriteLine('{');
                writer.WriteLine("    \"Header\": {");
                writer.WriteLine("    },");
                writer.WriteLine("    \"GeneralSpecs\": {");
                writer.WriteLine("        \"Size\": {");
                writer.WriteLine(String.Format(ci, "            \"X\": {0},", labelWidth));
                writer.WriteLine(String.Format(ci, "            \"Y\": {0}", labelHeight));
                writer.WriteLine("        },");
                writer.WriteLine(String.Format("        \"LayerNumber\": {0}", layerNumber));
                writer.WriteLine("    }");
                writer.WriteLine('}');
            }
        }
    }
}
