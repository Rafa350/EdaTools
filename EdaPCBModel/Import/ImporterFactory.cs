namespace MikroPic.EdaTools.v1.Pcb.Import {

    public sealed class ImporterFactory {

        private static ImporterFactory instance = null;

        private ImporterFactory() {

        }

        public Importer GetImporter(string id) {

            return null;
        }

        public ImporterFactory Instance {
            get {
                if (instance == null)
                    instance = new ImporterFactory();
                return instance;
            }
        }
    }
}
