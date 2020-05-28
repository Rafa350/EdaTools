namespace MikroPic.NetMVVMToolkit.v1.WindowState {

    using System;
    using System.Collections.Generic;

    public sealed class DataInfo {

        private Dictionary<string, object> items;

        public int Count {
            get {
                return items == null ? 0 : items.Count;
            }
        }

        public string[] Names {
            get {
                string[] names = null;
                if (Count > 0) {
                    names = new string[Count];
                    items.Keys.CopyTo(names, 0);
                }
                return names;
            }
        }

        public object this[string name] {
            get {
                if (String.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name");

                return items == null ? null : items[name];
            }
            set {
                if (String.IsNullOrEmpty(name))
                    throw new ArgumentNullException("name");

                if (items == null)
                    items = new Dictionary<string, object>();
                items[name] = value;
            }
        }
    }
}
