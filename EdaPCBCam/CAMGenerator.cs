namespace MikroPic.EdaTools.v1.Cam {

    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Cam.Gerber;

    public sealed class CAMGenerator {

        public void Generate(Board board, string fileName) {

            GerberGenerator generator = new GerberGenerator(null);

            List<Layer> layers = new List<Layer>();

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.Top));
            layers.Add(board.GetLayer(LayerId.Pads));
            layers.Add(board.GetLayer(LayerId.Measures));
            generator.Generate(board, layers, @"c:\temp\board3.cmp");

            layers.Clear();
            layers.Add(board.GetLayer(LayerId.Measures));
            generator.Generate(board, layers, @"c:\temp\board3.miling");
        }
    }
}
