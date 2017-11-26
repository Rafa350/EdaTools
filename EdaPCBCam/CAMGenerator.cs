namespace MikroPic.EdaTools.v1.Cam {

    using MikroPic.EdaTools.v1.Model;
    using MikroPic.EdaTools.v1.Cam.Gerber;

    public sealed class CAMGenerator {

        public void Generate(Board board, string fileName) {

            GerberGenerator generator = new GerberGenerator(null);
            generator.Generate(board, fileName);
        }
    }
}
