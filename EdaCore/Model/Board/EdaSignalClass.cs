namespace MikroPic.EdaTools.v1.Core.Model.Board {

    public sealed class EdaSignalClass {

        private string _name;
        private int _clearance;
        private int _trackThickness;
        private int _viaOuterSize;
        private int _viaInnerSize;
        private int _viaDrill;

        public string Name {
            get => _name;
            set => _name = value;
        }

        public int Clearance {
            get => _clearance;
            set => _clearance = value;
        }

        public int TrackThickness {
            get => _trackThickness;
            set => _trackThickness = value;
        }

        public int ViaOuterSize {
            get => _viaOuterSize;
            set => _viaOuterSize = value;
        }

        public int ViaInnerSize {
            get => _viaInnerSize;
            set => _viaInnerSize = value;
        }

        public int ViaDrill {
            get => _viaDrill;
            set => _viaDrill = value;
        }
    }
}
