namespace MikroPic.EdaTools.v1 {

    using System.IO;
    using System.Collections.Generic;

    public interface IPartQueryService {

        string GetName();

        void AddManufacturerFilter(string manufacturer);
        void AddManufacturerFilter(IEnumerable<string> manufacturers);

        void AddProviderFilter(string provider);
        void AddProviderFilter(IEnumerable<string> providers);

        void FindPart(Stream stream, string query);
    }
}
