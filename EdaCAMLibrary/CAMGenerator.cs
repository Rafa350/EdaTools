namespace MikroPic.EdaTools.v1.Cam {

    using MikroPic.EdaTools.v1.Cam.Gerber;
    using MikroPic.EdaTools.v1.Cam.Ipcd356;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using System;
    using System.Text;
    using System.IO;
    using System.Collections.Generic;

    public sealed class CAMGenerator {

        public void Generate(Board board, string folder, string name) {

            if (board == null)
                throw new ArgumentNullException("board");

            string prefix = Path.Combine(folder, name);
            string fileName;

            List<Layer> layers = new List<Layer>();

            GerberImageGenerator imageGenerator = new GerberImageGenerator(board);

            layers.Clear();
            layers.Add(board.GetLayer(Layer.TopName));
            fileName = imageGenerator.GenerateFileName(prefix, GerberImageGenerator.ImageType.Copper, 1);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.GenerateContent(writer, layers, GerberImageGenerator.ImageType.Copper, 1);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(Layer.BottomName));
            fileName = imageGenerator.GenerateFileName(prefix, GerberImageGenerator.ImageType.Copper, 2);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.GenerateContent(writer, layers, GerberImageGenerator.ImageType.Copper, 2);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(Layer.TopStopName));
            fileName = imageGenerator.GenerateFileName(prefix, GerberImageGenerator.ImageType.TopSolderMask);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.GenerateContent(writer, layers, GerberImageGenerator.ImageType.TopSolderMask);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(Layer.BottomStopName));
            fileName = imageGenerator.GenerateFileName(prefix, GerberImageGenerator.ImageType.BottomSolderMask);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.GenerateContent(writer, layers, GerberImageGenerator.ImageType.BottomSolderMask);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(Layer.TopPlaceName));
            layers.Add(board.GetLayer(Layer.TopNamesName));
            fileName = imageGenerator.GenerateFileName(prefix, GerberImageGenerator.ImageType.TopLegend);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.GenerateContent(writer, layers, GerberImageGenerator.ImageType.TopLegend);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(Layer.BottomPlaceName));
            layers.Add(board.GetLayer(Layer.BottomNamesName));
            fileName = imageGenerator.GenerateFileName(prefix, GerberImageGenerator.ImageType.BottomLegend);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.GenerateContent(writer, layers, GerberImageGenerator.ImageType.BottomLegend);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(Layer.ProfileName));
            fileName = imageGenerator.GenerateFileName(prefix, GerberImageGenerator.ImageType.Profile);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.GenerateContent(writer, layers, GerberImageGenerator.ImageType.Profile);
                }
            }

            GerberDrillGenerator drillGenerator = new GerberDrillGenerator(board);

            layers.Clear();
            layers.Add(board.GetLayer(Layer.DrillsName));
            fileName = drillGenerator.GenerateFileName(prefix, GerberDrillGenerator.DrillType.PlatedDrill, 1, 2);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    drillGenerator.GenerateContent(writer, layers, GerberDrillGenerator.DrillType.PlatedDrill, 1, 2);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(Layer.HolesName));
            fileName = drillGenerator.GenerateFileName(prefix, GerberDrillGenerator.DrillType.NonPlatedDrill, 1, 2);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    drillGenerator.GenerateContent(writer, layers, GerberDrillGenerator.DrillType.NonPlatedDrill, 1, 2);
                }
            }

            Ipcd356Generator ipcd356Generator = new Ipcd356Generator(board);
            fileName = ipcd356Generator.GenerateFileName(prefix);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    ipcd356Generator.GenerateContent(writer);
                }
            }
        }
    }
}

