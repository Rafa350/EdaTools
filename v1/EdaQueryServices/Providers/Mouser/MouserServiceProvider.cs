namespace MikroPic.EdaTools.v1.Providers.Mouser {

    using System.IO;

    public sealed class MouserServiceProvider: EdaServiceProviderBase {

        public MouserServiceProvider()
            : base() {
        }

        public override string GetName() {

            return "Mouser API Search Services";
        }

        public override void FindPart(Stream stream, string query) {

        }
    }
}
