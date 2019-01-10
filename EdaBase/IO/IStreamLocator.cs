namespace MikroPic.EdaTools.v1.Base.IO {

    using System.IO;

    public interface IStreamLocator {

        string GetPath(string path);
        Stream GetStream(string path);
    }
}
