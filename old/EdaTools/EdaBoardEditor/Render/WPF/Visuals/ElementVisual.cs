namespace MikroPic.EdaTools.v1.BoardEditor.Render.WPF.Visuals {

    using MikroPic.EdaTools.v1.Base.WPF;
    using MikroPic.EdaTools.v1.Core.Model.Board;
    using System;
    using System.Windows.Media;

    public abstract class ElementVisual: VisualBase {

        private readonly Element element;

        /// <summary>
        /// Constructor de l'objecte.
        /// </summary>
        /// <param name="parent">El visual pare.</param>
        /// <param name="element">L'element associat.</param>
        /// 
        public ElementVisual(DrawingVisual parent, Element element):
            base(parent) {

            if (element == null)
                throw new ArgumentNullException("element");

            this.element = element;
        }

        /// <summary>
        /// Obte l'element asociat.
        /// </summary>
        /// 
        public Element Element {
            get {
                return element;
            }
        }
    }
}
