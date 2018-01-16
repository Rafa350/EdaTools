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

            string fileName;

            List<Layer> layers = new List<Layer>();

            GerberImageGenerator imageGenerator = new GerberImageGenerator(board);

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.Top));
            layers.Add(board.GetLayer(LayerId.Profile));
            fileName = MakeFileName(board, folder, name, GerberImageGenerator.ImageType.Top);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.Generate(writer, layers, GerberImageGenerator.ImageType.Top);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.Bottom));
            layers.Add(board.GetLayer(LayerId.Profile));
            fileName = MakeFileName(board, folder, name, GerberImageGenerator.ImageType.Bottom);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.Generate(writer, layers, GerberImageGenerator.ImageType.Bottom);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.TopStop));
            layers.Add(board.GetLayer(LayerId.Profile));
            fileName = MakeFileName(board, folder, name, GerberImageGenerator.ImageType.TopSolderMask);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.Generate(writer, layers, GerberImageGenerator.ImageType.TopSolderMask);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.BottomStop));
            layers.Add(board.GetLayer(LayerId.Profile));
            fileName = MakeFileName(board, folder, name, GerberImageGenerator.ImageType.BottomSolderMask);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.Generate(writer, layers, GerberImageGenerator.ImageType.BottomSolderMask);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.TopPlace));
            layers.Add(board.GetLayer(LayerId.Profile));
            fileName = MakeFileName(board, folder, name, GerberImageGenerator.ImageType.TopLegend);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.Generate(writer, layers, GerberImageGenerator.ImageType.TopLegend);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.BottomPlace));
            layers.Add(board.GetLayer(LayerId.Profile));
            fileName = MakeFileName(board, folder, name, GerberImageGenerator.ImageType.BottomLegend);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.Generate(writer, layers, GerberImageGenerator.ImageType.BottomLegend);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.Profile));
            fileName = MakeFileName(board, folder, name, GerberImageGenerator.ImageType.Profile);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.Generate(writer, layers, GerberImageGenerator.ImageType.Profile);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.TopPlace));
            layers.Add(board.GetLayer(LayerId.Profile));
            fileName = MakeFileName(board, folder, name, GerberImageGenerator.ImageType.TopLegend);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.Generate(writer, layers, GerberImageGenerator.ImageType.TopLegend);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.BottomPlace));
            layers.Add(board.GetLayer(LayerId.Profile));
            fileName = MakeFileName(board, folder, name, GerberImageGenerator.ImageType.BottomLegend);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.Generate(writer, layers, GerberImageGenerator.ImageType.BottomLegend);
                }
            }

            GerberDrillGenerator drillGenerator = new GerberDrillGenerator(board);

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.Drills));
            fileName = @"..\..\..\Data\board_Plated$1$2$PTH$Drill.gbr";
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    drillGenerator.Generate(writer, layers, GerberDrillGenerator.DrillType.PlatedDrill);
                }
            }

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.Holes));
            fileName = @"..\..\..\Data\board_NonPlated$1$2$NPTH$Drill.gbr";
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    drillGenerator.Generate(writer, layers, GerberDrillGenerator.DrillType.NonPlatedDrill);
                }
            }
        }

        private string MakeFileName(Board board, string folder, string name, GerberImageGenerator.ImageType imageType) {

            string suffix = null;
            switch (imageType) {
                case GerberImageGenerator.ImageType.Top:
                    suffix = String.Format("_Copper$L{0}", 1);
                    break;

                case GerberImageGenerator.ImageType.Bottom:
                    suffix = String.Format("_Copper$L{0}", 2);
                    break;

                case GerberImageGenerator.ImageType.TopSolderMask:
                    suffix = "_Soldermask$Top";
                    break;

                case GerberImageGenerator.ImageType.BottomSolderMask:
                    suffix = "_Soldermask$Bottom";
                    break;

                case GerberImageGenerator.ImageType.TopLegend:
                    suffix = "_Legend$Top";
                    break;

                case GerberImageGenerator.ImageType.BottomLegend:
                    suffix = "_Legend$Bottom";
                    break;

                case GerberImageGenerator.ImageType.Profile:
                    suffix = "_Profile$NP";
                    break;
            }

            return String.Format(@"{0}\{1}{02}.gbr", folder, name, suffix);
        }
    }
}

