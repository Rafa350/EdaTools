using System;

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

            return HashCode.Combine("FillDesc", fill, tag);
        }

        public bool Fill =>
            _fill;
    }
}


