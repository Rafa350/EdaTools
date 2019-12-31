namespace MikroPic.EdaTools.v1.PanelEditor.VisualEditor {

    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Contenidor visual que gestiona una col·leccio d'objectes de tipus ItemVisual.
    /// </summary>
    /// 
    public abstract class VisualContainer: FrameworkElement, IVisualContainer {

        private VisualCollection visuals;

        /// <summary>
        /// Afegeix un objecte a la col·leccio.
        /// </summary>
        /// <param name="visual">L'objecte a afeigir.</param>
        /// 
        public void AddVisualItem(VisualItem visual) {

            if (visual == null)
                throw new ArgumentNullException("visual");

            if ((visuals != null) && visuals.Contains(visual))
                throw new InvalidOperationException("El objeto ya pertenece a la coleccion.");

            if (visuals == null)
                visuals = new VisualCollection(this);

            visuals.Add(visual);
        }

        /// <summary>
        /// Afegeix diversos objectes a la cole·leccio.
        /// </summary>
        /// <param name="visuals">Els objectes a afeigir.</param>
        /// 
        public void AddVisualItem(IEnumerable<VisualItem> visuals) {

            if (visuals == null)
                throw new ArgumentNullException("visuals");

            foreach (var visual in visuals)
                AddVisualItem(visual);
        }

        /// <summary>
        /// Elimina un objecte de la col·leccio.
        /// </summary>
        /// <param name="visual">El objecte a eliminar.</param>
        /// 
        public void RemoveVisualItem(VisualItem visual) {

            if (visual == null)
                throw new ArgumentNullException("visual");

            if (visuals == null || !visuals.Contains(visual))
                throw new InvalidOperationException("El objeto no pertenece a la coleccion.");

            visuals.Remove(visual);

            if (visuals.Count == 0)
                visuals = null;
        }

        /// <summary>
        /// Elimina tots els objectes de la col·leccio.
        /// </summary>
        /// 
        public void RemoveAllVisualItems() {

            if (visuals != null) {
                while (visuals.Count > 0)
                    visuals.Remove(visuals[0]);
                visuals = null;
            }
        }

        /// <summary>
        /// Obte l'objecte especificat.
        /// </summary>
        /// <param name="index">Index de l'objecte.</param>
        /// <returns>L'objecte.</returns>
        /// 
        protected override Visual GetVisualChild(int index) {

            if ((visuals == null) || (index < 0) || (index >= visuals.Count))
                throw new ArgumentOutOfRangeException("index");

            return visuals[index];
        }

        /// <summary>
        /// Obte el numero d'objectes de la col·leccio.
        /// </summary>
        /// <returns>El numero d'objectes.</returns>
        /// 
        protected override int VisualChildrenCount {
            get {
                return visuals == null ? 0 : visuals.Count;
            }
        }
    }
}
