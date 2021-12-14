namespace MikroPic.EdaTools.v1.Core.Import {

    public sealed class EdaImporterFactory {

        private static EdaImporterFactory instance = null;

        private EdaImporterFactory() {

        }

        public IEdaImporter GetImporter(string id) {

            return null;
        }

        public EdaImporterFactory Instance {
            get {
                if (instance == null)
                    instance = new EdaImporterFactory();
                return instance;
            }
        }
    }
}
