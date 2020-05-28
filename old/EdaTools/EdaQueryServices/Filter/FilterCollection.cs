namespace MikroPic.EdaTools.v1.Filter {

    using System;
    using System.Collections.Generic;

    public sealed class FilterCollection {

        private readonly FilterMode defaultMode;
        private readonly List<Filter> filters = new List<Filter>();

        public FilterCollection(FilterMode defaultMode) {

            this.defaultMode = defaultMode;
        }

        public FilterCollection Add(Filter filter) {

            if (filter == null)
                throw new ArgumentNullException("filter");

            filters.Add(filter);

            return this;
        }

        public bool Check(string text) {

            if (String.IsNullOrEmpty(text))
                return false;

            foreach (Filter filter in filters)
                if (!filter.Check(text))
                    return false;

            return true;
        }
    }

}
