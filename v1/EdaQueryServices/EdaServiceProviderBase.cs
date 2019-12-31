namespace MikroPic.EdaTools.v1 {

    using System.IO;
    using System.Collections.Generic;

    public abstract class EdaServiceProviderBase: IPartQueryService {

        private List<string> manufacturers;
        private List<string> providers;

        public abstract string GetName();

        public void AddManufacturerFilter(string manufacturer) {

            if (manufacturers == null)
                manufacturers = new List<string>();
            manufacturers.Add(manufacturer);
        }

        public void AddManufacturerFilter(IEnumerable<string> manufacturers) {

            foreach (string manufacturer in manufacturers)
                AddManufacturerFilter(manufacturer);
        }

        public void AddProviderFilter(string provider) {

            if (providers == null)
                providers = new List<string>();
            providers.Add(provider);
        }

        public void AddProviderFilter(IEnumerable<string> providers) {

            foreach (string provider in providers)
                AddProviderFilter(provider);
        }
        
        public abstract void FindPart(Stream stream, string query);

        protected IList<string> Manufacturers {
            get {
                return manufacturers;
            }
        }

        protected IList<string> Providers {
            get {
                return providers;
            }
        }
    }
}
