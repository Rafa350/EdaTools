namespace MikroPic.EdaTools.v1.PanelEditor.Render.WPF.Visuals {

    using MikroPic.EdaTools.v1.Base.WPF;
    using MikroPic.EdaTools.v1.Panel.Model;
    using System;
    using System.Windows.Media;

    public abstract class ItemVisual: VisualBase {

        private readonly ProjectItem item;

        public ItemVisual(DrawingVisual parent, ProjectItem item):
            base(parent) {

            if (item == null)
                throw new ArgumentNullException("item");

            this.item = item;
        }

        /// <summary>
        /// Obte el item asociat.
        /// </summary>
        /// 
        public ProjectItem Item {
            get {
                return item;
            }
        }
    }
}
