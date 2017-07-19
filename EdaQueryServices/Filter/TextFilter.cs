namespace MikroPic.EdaTools.v1.Filter {

    using System;

    public sealed class TextFilter: Filter {

        private readonly string pattern;
        private readonly bool caseSensitive;

        public TextFilter(string pattern, FilterMode mode, bool caseSensitive)
            : base(mode) {

            if (String.IsNullOrEmpty(pattern))
                throw new ArgumentNullException("pattern");

            this.pattern = pattern;
            this.caseSensitive = caseSensitive;
        }

        public override bool Check(string text) {

            if (String.IsNullOrEmpty(text))
                return false;
            else
                return Mode == FilterMode.Include ?
                    pattern == text :
                    pattern != text;
        }
    }
}
