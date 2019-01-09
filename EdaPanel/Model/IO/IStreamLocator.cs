namespace MikroPic.EdaTools.v1.Panel.Model.IO {

    using System.IO;

    public interface IStreamLocator {

        Stream GetStream(string path);
    }
}
