﻿namespace MikroPic.EdaTools.v1.Core.Import {

    using System.IO;
    using MikroPic.EdaTools.v1.Core.Model.Board;

    public abstract class Importer {

        public Board Read(string fileName) {

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                return Read(stream);
        }

        public abstract Board Read(Stream stream);
    }
}
