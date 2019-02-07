namespace MikroPic.EdaTools.v1.Core.Model.Net {

    using System;
    using System.Collections.Generic;

    public sealed partial class Net {

        private readonly Dictionary<string, NetPart> parts = new Dictionary<string, NetPart>();

        private void InitializeParts(IEnumerable<NetPart> parts) {

            foreach (var part in parts)
                this.parts.Add(part.Name, part);
        }

        public NetPart GetPart(string name, bool throwOnError = true) {

            if (parts.TryGetValue(name, out var component))
                return component;

            else if (throwOnError)
                throw new InvalidOperationException(
                    String.Format("No se encontro el componente '{0}'.", name));
            else
                return null;
        }

        public bool HasParts {
            get {
                return parts.Count > 0;
            }
        }

        public IEnumerable<string> PartNames {
            get {
                return parts.Keys;
            }
        }

        public IEnumerable<NetPart> Parts {
            get {
                return parts.Values;
            }
        }
    }
}
