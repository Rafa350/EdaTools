namespace MikroPic.EdaTools.v1.Cam.DXF.Builder {

    using System;
    using System.IO;

    public sealed class DXFBuilder {

        private readonly TextWriter writer;

        public DXFBuilder(TextWriter writer) {

            if (writer == null)
                throw new ArgumentNullException("writer");

            this.writer = writer;
        }
    }
}
