namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581 {

    internal abstract class Definition {

        private readonly int _id;
        private readonly string _tag;

        public Definition(int id, string tag) {

            _id = id;
            _tag = tag;
        }

        public int Id =>
            _id;

        public string Tag =>
            _tag;
    }
}
