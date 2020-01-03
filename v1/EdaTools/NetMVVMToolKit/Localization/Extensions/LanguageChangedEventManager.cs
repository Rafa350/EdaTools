﻿namespace Media.NetGui.v1.Localization.Extensions {

    using System;
    using System.Windows;

    public class LanguageChangedEventManager: WeakEventManager {

        public static void AddListener(TranslationManager source, IWeakEventListener listener) {
            
            CurrentManager.ProtectedAddListener(source, listener);
        }

        public static void RemoveListener(TranslationManager source, IWeakEventListener listener) {
            
            CurrentManager.ProtectedRemoveListener(source, listener);
        }

        private void OnLanguageChanged(object sender, EventArgs e) {
            
            DeliverEvent(sender, e);
        }

        protected override void StartListening(object source) {
            
            var manager = (TranslationManager) source;
            manager.LanguageChanged += OnLanguageChanged;
        }

        protected override void StopListening(Object source) {
            
            var manager = (TranslationManager) source;
            manager.LanguageChanged -= OnLanguageChanged;
        }

        private static LanguageChangedEventManager CurrentManager {
            get {
                Type managerType = typeof(LanguageChangedEventManager);
                var manager = (LanguageChangedEventManager) GetCurrentManager(managerType);
                if (manager == null) {
                    manager = new LanguageChangedEventManager();
                    SetCurrentManager(managerType, manager);
                }
                return manager;
            }
        }
    }
}