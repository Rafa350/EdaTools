using System;
using System.Globalization;
using System.IO;
using System.Text.Json;

using MikroPic.EdaTools.v1.Cam.Model;
using MikroPic.EdaTools.v1.Core.Model.Board;

namespace MikroPic.EdaTools.v1.Cam.Generators.GerberJob {

    public sealed class GerberJobGenerator : Generator {

        public GerberJobGenerator(Target target) :
            base(target) {
        }

        public override void Generate(EdaBoard board, string outputFolder, GeneratorOptions options = null) {

            if (board == null)
                throw new ArgumentNullException(nameof(board));

            if (String.IsNullOrEmpty(outputFolder))
                throw new ArgumentNullException(nameof(outputFolder));

            string fileName = Path.Combine(outputFolder, Target.FileName);

            var jsonOptions = new JsonWriterOptions();
            jsonOptions.Indented = true;

            using (var writer = new Utf8JsonWriter(
                new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None),
                jsonOptions)) {

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

                writer.WriteStartObject();

                writer.WriteStartObject("Header");
                writer.WriteEndObject();

                writer.WriteStartObject("GeneralSpecs");

                writer.WriteStartObject("Size");
                writer.WriteNumber("X", labelWidth);
                writer.WriteNumber("Y", labelHeight);
                writer.WriteEndObject();

                writer.WriteNumber("LayerNumber", layerNumber);

                writer.WriteEndObject();

                writer.WriteEndObject();
            }
        }
    }
}
