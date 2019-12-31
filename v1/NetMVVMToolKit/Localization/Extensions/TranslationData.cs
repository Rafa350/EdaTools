namespace Media.NetGui.v1.Localization.Extensions {

    using System;
    using System.ComponentModel;
    using System.Windows;

    public sealed class TranslationData: IWeakEventListener, INotifyPropertyChanged, IDisposable {
        
        private readonly string key;

        public event PropertyChangedEventHandler PropertyChanged;

        public TranslationData(string key) {

            this.key = key;
            LanguageChangedEventManager.AddListener(TranslationManager.Instance, this);
        }

        ~TranslationData() {

            Dispose(false);
        }


        public void Dispose() {

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {

            if (disposing)
                LanguageChangedEventManager.RemoveListener(TranslationManager.Instance, this); 
        }


        public object Value {
            get {
                return TranslationManager.Instance.Translate(key);
            }
            set {
            }
        }

        public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {

            if (managerType == typeof(LanguageChangedEventManager)) {
                OnLanguageChanged(sender, e);
                return true;
            }
            else
                return false;
        }

        private void OnLanguageChanged(object sender, EventArgs e) {

            if (PropertyChanged != null) 
                PropertyChanged(this, new PropertyChangedEventArgs("Value"));
        }
    }
}
