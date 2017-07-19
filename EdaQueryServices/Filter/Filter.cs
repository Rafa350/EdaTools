namespace MikroPic.EdaTools.v1.Filter {

    public enum FilterMode {
        Include,
        Exclude
    }

    public abstract class Filter {

        private readonly FilterMode mode;

        public Filter(FilterMode mode) {

            this.mode = mode;
        }

        public abstract bool Check(string testWord);

        public FilterMode Mode {
            get {
                return mode;
            }
        }
    }
}
