namespace MikroPic.EdaTools.v1.Panelizer {

    using System.IO;
    using MikroPic.EdaTools.v1.Base.IO;

    public sealed class BoardLocator : IStreamLocator {

        private readonly string folder;

        public BoardLocator(string folder) {

            this.folder = folder;
        }

        public Stream GetStream(string path) {

            if (!File.Exists(path)) {
                string fileName = Path.GetFileName(path);
                path = Path.Combine(folder, fileName);
                if (!File.Exists(path))
                    return null;
            }

            return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
    }
}
