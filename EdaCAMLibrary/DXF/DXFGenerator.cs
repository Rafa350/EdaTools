namespace MikroPic.EdaTools.v1.Cam.DXF {

    using System;
    using System.IO;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Cam.DXF.Builder;

    public sealed class DXFGenerator {

        private readonly Board board;

        public DXFGenerator(Board board) {

            if (board == null)
                throw new ArgumentNullException("board");

            this.board = board;
        }

        public void GenerateContent(TextWriter writer) {

            if (writer == null)
                throw new ArgumentNullException("writer");


            DXFBuilder db = new DXFBuilder(writer);

            GenerateHeader(db);
            GenerateProfile(db);
        }

        private void GenerateHeader(DXFBuilder db) {

        }

        private void GenerateProfile(DXFBuilder db) {

        }
    }
}
