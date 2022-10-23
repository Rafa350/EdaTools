using System.Globalization;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Cache.Entries {

    internal enum FillDescMode {
        Solid,
        Void
    }

    internal sealed class FillDescEntry: DataCacheEntry {

        private readonly bool _fill;

        public FillDescEntry(int id, string tag, bool fill) :
            base(id, tag) {

            _fill = fill;
        }

        public static int GetId(bool fill, string tag) {

            string s = string.Format(CultureInfo.InvariantCulture, "FillDesc;{0};{1}", fill, tag);
            return s.GetHashCode();
        }

        public bool Fill =>
            _fill;
    }
}


