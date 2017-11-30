namespace MikroPic.EdaTools.v1.Cam.Gerber {

    public abstract class GerberAperture {

        private readonly int id;

        public GerberAperture(int id) {

            this.id = id;
        }

        protected abstract string GetCommand();

        public int Id {
            get {
                return id;
            }
        }

        public string Command {
            get {
                return GetCommand();
            }
        }
    }
}
