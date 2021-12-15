using System;

namespace MikroPic.EdaTools.v1.Cam.Model {

    public sealed class TargetOption {

        private readonly string name;
        private readonly string value;

        public TargetOption(string name, string value) {

            if (String.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            this.name = name;
            this.value = value;
        }

        public string Name =>
            name;

        public string Value =>
            value;
    }
}
