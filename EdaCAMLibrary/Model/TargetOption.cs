namespace MikroPic.EdaTools.v1.Cam.Model {

    using System;

    public sealed class TargetOption {

        private readonly string name;
        private readonly string value;

        public TargetOption(string name, string value) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            this.name = name;
            this.value = value;
        }

        public string Name {
            get {
                return name;
            }
        }

        public string Value {
            get {
                return value;
            }
        }
    }
}
