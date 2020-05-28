namespace MikroPic.EdaTools.v1.PanelEditor.Render.Visuals {

    using System;
    using MikroPic.EdaTools.v1.Panel.Model;
    using MikroPic.EdaTools.v1.PanelEditor.VisualEditor;

    public abstract class PanelItemVisual: VisualItem {

        private readonly PanelItem item;

        public PanelItemVisual(VisualItem parent, PanelItem item):
            base(parent) {

            if (item == null)
                throw new ArgumentNullException("item");

            this.item = item;
        }

        /// <summary>
        /// Obte el item asociat.
        /// </summary>
        /// 
        public PanelItem Item {
            get {
                return item;
            }
        }
    }
}
