namespace MikroPic.EdaTools.v1 {

    using System.IO;
    using MikroPic.EdaTools.v1;
    using MikroPic.EdaTools.v1.Providers.OctoPart;
    using MikroPic.EdaTools.v1.Providers.Farnell;
    using MikroPic.EdaTools.v1.Filter;

    class Program {

        static void Main(string[] args) {

            using (Stream stream = new FileStream(@"c:\temp\result.xml", FileMode.Create, FileAccess.Write, FileShare.None)) {

                IPartQueryService service = new OctopartServiceProvider();
                //IPartQueryService service = new FarnellServiceProvider();

                FilterCollection manufacturerFilter = new FilterCollection(FilterMode.Exclude)
                    .Add(new TextFilter("Vishay", FilterMode.Include, false))
                    .Add(new TextFilter("Yageo", FilterMode.Include, false));

                
                service.AddManufacturerFilter("Vishay");
                service.AddManufacturerFilter("Yageo");
                service.AddManufacturerFilter("Kemet");

                service.AddProviderFilter("Mouser");
                service.AddProviderFilter("Digi-Key");

                service.FindPart(stream, "3K3 1206 1%");
            }
        }
    }
}
