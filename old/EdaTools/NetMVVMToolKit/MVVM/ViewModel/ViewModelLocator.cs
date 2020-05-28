namespace MikroPic.NetMVVMToolkit.v1.MVVM.ViewModel {

    using System;
    using System.Collections.Generic;

    public class ViewModelLocator {

        private static ViewModelLocator instance;

        private ViewModelLocator() {
        }

        public static ViewModelLocator Instance {
            get {
                if (instance == null)
                    instance = new ViewModelLocator();
                return instance;
            }
        }
    }
}
