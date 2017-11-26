namespace MikroPic.EdaTools.v1.Cam.Gerber.Infrastructure {

    internal abstract class ApertureBase {

        private readonly int id;

        public ApertureBase(int id) {

            this.id = id;
        }

        public abstract string GetDeclarationCommand();

        public string GetSelectionCommand() {

            return string.Format("D{0}*", id);
        }

        public int Id {
            get {
                return id;
            }
        }
    }
}
