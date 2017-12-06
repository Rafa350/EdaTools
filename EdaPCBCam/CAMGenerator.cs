namespace MikroPic.EdaTools.v1.Cam {

    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Cam.Gerber;

    public sealed class CAMGenerator {

        public void Generate(Board board, string fileName) {

            List<Layer> layers = new List<Layer>();

            GerberImageGenerator imageGenerator = new GerberImageGenerator();

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.Top));
            layers.Add(board.GetLayer(LayerId.Pads));
            layers.Add(board.GetLayer(LayerId.Vias));
            layers.Add(board.GetLayer(LayerId.Profile));
            imageGenerator.Generate(board, layers, GerberImageGenerator.ImageType.Top, @"..\..\..\Data\board_Copper$L1.gbr");

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.Bottom));
            layers.Add(board.GetLayer(LayerId.Pads));
            layers.Add(board.GetLayer(LayerId.Vias));
            layers.Add(board.GetLayer(LayerId.Profile));
            imageGenerator.Generate(board, layers, GerberImageGenerator.ImageType.Bottom, @"..\..\..\Data\board_Copper$L2.gbr");

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.TopStop));
            layers.Add(board.GetLayer(LayerId.Profile));
            imageGenerator.Generate(board, layers, GerberImageGenerator.ImageType.TopSolderMask, @"..\..\..\Data\board_Soldermask$Top.gbr");

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.BottomStop));
            layers.Add(board.GetLayer(LayerId.Profile));
            imageGenerator.Generate(board, layers, GerberImageGenerator.ImageType.BottomSolderMask, @"..\..\..\Data\board_Soldermask$Bottom.gbr");

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.Profile));
            imageGenerator.Generate(board, layers, GerberImageGenerator.ImageType.Profile, @"..\..\..\Data\board_Profile$NP.gbr");

            /*GerberDrillGenerator drillGenerator = new GerberDrillGenerator();
            drillGenerator.Generate(board, GerberDrillGenerator.DrillType.PlatedDrill, @"..\..\..\Data\board_Plated$1$2$PTH$Drill.gbr");
            drillGenerator.Generate(board, GerberDrillGenerator.DrillType.NonPlatedDrill, @"..\..\..\Data\board_NonPlated$1$2$NPTH$Drill.gbr");
            */
        }
    }
}

