namespace MikroPic.EdaTools.v1.Designer {

    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;

    public sealed class MainMenuViewModel {

        public ObservableCollection<MenuItemViewModel> Items {
            get {

                ObservableCollection<MenuItemViewModel> menuItems = new ObservableCollection<MenuItemViewModel>();
                menuItems.Add(new MenuItemViewModel("File", new List<MenuItemViewModel>() {
                    new MenuItemViewModel("New", (Action) null),
                    new MenuItemViewModel("Open...", (Action) null),
                    new MenuItemViewModel("Save", (Action) null),
                    new MenuItemViewModel("Save as...", (Action) null),
                    new MenuItemViewModel("Exit", (Action) null)
                }));
                menuItems.Add(new MenuItemViewModel("Edit", (Action) null));
                menuItems.Add(new MenuItemViewModel("Design", (Action) null));
                menuItems.Add(new MenuItemViewModel("Help", (Action) null));

                return menuItems;
            }
        }
    }
}
