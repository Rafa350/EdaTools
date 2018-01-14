namespace MikroPic.EdaTools.v1.Cam {

    using MikroPic.EdaTools.v1.Cam.Gerber;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using System;
    using System.Collections.Generic;

    public sealed class CAMGenerator {

        public void Generate(Board board, string folder, string name) {

            if (board == null)
                throw new ArgumentNullException("board");

            string fileName;

            List<Layer> layers = new List<Layer>();

            GerberImageGenerator imageGenerator = new GerberImageGenerator();

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.Top));
            layers.Add(board.GetLayer(LayerId.Profile));
            fileName = MakeFileName(board, folder, name, GerberImageGenerator.ImageType.Top);
            imageGenerator.Generate(board, layers, GerberImageGenerator.ImageType.Top, fileName);

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.Bottom));
            layers.Add(board.GetLayer(LayerId.Profile));
            fileName = MakeFileName(board, folder, name, GerberImageGenerator.ImageType.Bottom);
            imageGenerator.Generate(board, layers, GerberImageGenerator.ImageType.Bottom, fileName);

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.TopStop));
            layers.Add(board.GetLayer(LayerId.Profile));
            fileName = MakeFileName(board, folder, name, GerberImageGenerator.ImageType.TopSolderMask);
            imageGenerator.Generate(board, layers, GerberImageGenerator.ImageType.TopSolderMask, fileName);

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.BottomStop));
            layers.Add(board.GetLayer(LayerId.Profile));
            fileName = MakeFileName(board, folder, name, GerberImageGenerator.ImageType.BottomSolderMask);
            imageGenerator.Generate(board, layers, GerberImageGenerator.ImageType.BottomSolderMask, fileName);

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.Profile));
            fileName = MakeFileName(board, folder, name, GerberImageGenerator.ImageType.Profile);
            imageGenerator.Generate(board, layers, GerberImageGenerator.ImageType.Profile, fileName);

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.TopPlace));
            layers.Add(board.GetLayer(LayerId.Profile));
            fileName = MakeFileName(board, folder, name, GerberImageGenerator.ImageType.TopLegend);
            imageGenerator.Generate(board, layers, GerberImageGenerator.ImageType.TopLegend, fileName);

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.BottomPlace));
            layers.Add(board.GetLayer(LayerId.Profile));
            fileName = MakeFileName(board, folder, name, GerberImageGenerator.ImageType.BottomLegend);
            imageGenerator.Generate(board, layers, GerberImageGenerator.ImageType.BottomLegend, fileName);

            GerberDrillGenerator drillGenerator = new GerberDrillGenerator();

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.Drills));
            drillGenerator.Generate(board, layers, GerberDrillGenerator.DrillType.PlatedDrill, @"..\..\..\Data\board_Plated$1$2$PTH$Drill.gbr");

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.Holes));
            drillGenerator.Generate(board, layers, GerberDrillGenerator.DrillType.NonPlatedDrill, @"..\..\..\Data\board_NonPlated$1$2$NPTH$Drill.gbr");            
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

