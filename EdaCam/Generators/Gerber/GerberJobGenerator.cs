using System;
using System.IO;
using System.Text;

using MikroPic.EdaTools.v1.Cam.Model;
using MikroPic.EdaTools.v1.Core.Model.Board;

namespace MikroPic.EdaTools.v1.Cam.Generators.Gerber {

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

                writer.WriteLine('{');
                writer.WriteLine("    Header: {");
                writer.WriteLine("        FilesAttributes: [");
                for(int i = 0; i < 10; i++) {
                    writer.WriteLine("        {");
                    writer.WriteLine("            Path:");
                    writer.WriteLine("            FileFunction:");
                    writer.WriteLine("        }");
                }
                writer.WriteLine("        ]");
                writer.WriteLine("    }");
                writer.WriteLine('}');
            }
        }
    }
}
