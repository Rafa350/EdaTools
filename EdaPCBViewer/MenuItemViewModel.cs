namespace MikroPic.EdaTools.v1.Designer {

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    public sealed class MenuItemViewModel {

        private readonly string header;
        private readonly Action action;
        private readonly ObservableCollection<MenuItemViewModel> items;

        public MenuItemViewModel(string header, Action action) {

            this.header = header;
            this.action = action;
        }

        public MenuItemViewModel(string header, IEnumerable<MenuItemViewModel> items) {

            this.header = header;
            this.items = new ObservableCollection<MenuItemViewModel>(items);
        }

        public string Header {
            get {
                return header;
            }
        }

        public ObservableCollection<MenuItemViewModel> Items {
            get {
                return items;
            }
        }

        public ICommand Command {
            get {
                return null;
            }
        }
    }
}
