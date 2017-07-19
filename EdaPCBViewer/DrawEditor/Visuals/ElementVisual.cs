namespace Eda.PCBViewer.DrawEditor.Visuals {

    using System;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Model;
    using MikroPic.EdaTools.v1.Model.Elements;

    public abstract class ElementVisual: DrawingVisual {

        private readonly ElementBase element;
        private readonly Part part;
        private bool isSelected = false;

        /// <summary>
        /// Constructor de la clase.
        /// </summary>
        /// <param name="element">El element.</param>
        /// <param name="part">El component del element. Si s'escau.</param>
        public ElementVisual(ElementBase element, Part part) {

            if (element == null)
                throw new ArgumentNullException("element");

            this.element = element;
            this.part = part;
        }

        /// <summary>
        /// Renderitza el element.
        /// </summary>
        public virtual void RenderVisual() {
        }

        /// <summary>
        /// Obte el element.
        /// </summary>
        public ElementBase Element {
            get {
                return element;
            }
        }

        /// <summary>
        /// Obte el component.
        /// </summary>
        public Part Part {
            get {
                return part;
            }
        }

        /// <summary>
        /// Obte o asigna el indicador de seleccio. Si hi ha canvis, es torna a renderitzar.
        /// </summary>
        public bool IsSelected {
            get {
                return isSelected;
            }
            set {
                if (isSelected != value) {
                    isSelected = value;
                    RenderVisual();
                }
            }
        }
    }
}
