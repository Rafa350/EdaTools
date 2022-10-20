namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581 {

    internal class Primitive {

        private readonly int _id;
        private readonly string _tag;

        public Primitive(int id, string tag) {

            _id = id;
            _tag = tag;
        }

        public int Id =>
            _id;

        public string Tag =>
            _tag;
    }
}
