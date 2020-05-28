namespace EdaQueryTest {

    using System.IO;
    using MikroPic.EdaQueryServices.v1;
    using MikroPic.EdaQueryServices.v1.Providers.Octopart;
    using MikroPic.EdaQueryServices.v1.Providers.Farnell;
    using MikroPic.EdaQueryServices.v1.Filter;

    class Program {

        static void Main(string[] args) {

            using (Stream stream = new FileStream(@"c:\temp\farnell.xml", FileMode.Create, FileAccess.Write, FileShare.None)) {

                //IPartQueryService service = new OctopartServiceProvider();
                IPartQueryService service = new FarnellServiceProvider();

                FilterCollection manufacturerFilter = new FilterCollection(FilterMode.Exclude)
                    .Add(new TextFilter("Vishay", FilterMode.Include))
                    .Add(new TextFilter("Yageo", FilterMode.Include));

                
                //service.AddManufacturerFilter("Vishay");
                //service.AddManufacturerFilter("Yageo");
                //service.AddManufacturerFilter("Kemet");

                service.AddProviderFilter("Mouser");
                service.AddProviderFilter("Digi-Key");

                service.FindPart(stream, "4.7uF 25V 0805");
            }
        }
    }
}
