namespace MikroPic.EdaTools.v1.Cam {

    using MikroPic.EdaTools.v1.Cam.Gerber;
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
            layers.Add(board.GetLayer(LayerId.Top));
            fileName = imageGenerator.GenerateFileName(prefix, GerberImageGenerator.ImageType.Copper, 1);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.GenerateContent(writer, layers, GerberImageGenerator.ImageType.Copper, 1);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.Bottom));
            fileName = imageGenerator.GenerateFileName(prefix, GerberImageGenerator.ImageType.Copper, 2);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.GenerateContent(writer, layers, GerberImageGenerator.ImageType.Copper, 2);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.TopStop));
            fileName = imageGenerator.GenerateFileName(prefix, GerberImageGenerator.ImageType.TopSolderMask);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.GenerateContent(writer, layers, GerberImageGenerator.ImageType.TopSolderMask);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.BottomStop));
            fileName = imageGenerator.GenerateFileName(prefix, GerberImageGenerator.ImageType.BottomSolderMask);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.GenerateContent(writer, layers, GerberImageGenerator.ImageType.BottomSolderMask);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.TopPlace));
            fileName = imageGenerator.GenerateFileName(prefix, GerberImageGenerator.ImageType.TopLegend);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.GenerateContent(writer, layers, GerberImageGenerator.ImageType.TopLegend);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.BottomPlace));
            fileName = imageGenerator.GenerateFileName(prefix, GerberImageGenerator.ImageType.BottomLegend);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.GenerateContent(writer, layers, GerberImageGenerator.ImageType.BottomLegend);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.Profile));
            fileName = imageGenerator.GenerateFileName(prefix, GerberImageGenerator.ImageType.Profile);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.GenerateContent(writer, layers, GerberImageGenerator.ImageType.Profile);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.TopPlace));
            fileName = imageGenerator.GenerateFileName(prefix, GerberImageGenerator.ImageType.TopLegend);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.GenerateContent(writer, layers, GerberImageGenerator.ImageType.TopLegend);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.BottomPlace));
            fileName = imageGenerator.GenerateFileName(prefix, GerberImageGenerator.ImageType.BottomLegend);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.GenerateContent(writer, layers, GerberImageGenerator.ImageType.BottomLegend);
                }
            }

            GerberDrillGenerator drillGenerator = new GerberDrillGenerator(board);

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.Drills));
            fileName = drillGenerator.GenerateFileName(prefix, GerberDrillGenerator.DrillType.PlatedDrill, 1, 2);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    drillGenerator.GenerateContent(writer, layers, GerberDrillGenerator.DrillType.PlatedDrill, 1, 2);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.Holes));
            fileName = drillGenerator.GenerateFileName(prefix, GerberDrillGenerator.DrillType.NonPlatedDrill, 1, 2);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    drillGenerator.GenerateContent(writer, layers, GerberDrillGenerator.DrillType.NonPlatedDrill, 1, 2);
                }
            }
        }
    }
}

