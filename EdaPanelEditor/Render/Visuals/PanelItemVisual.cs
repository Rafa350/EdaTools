namespace MikroPic.EdaTools.v1.PanelEditor.Render.Visuals {

    using System;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Base.WPF;
    using MikroPic.EdaTools.v1.Panel.Model;

    public abstract class PanelItemVisual: VisualBase {

        private readonly PanelItem item;

        public PanelItemVisual(DrawingVisual parent, PanelItem item):
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
