namespace MikroPic.EdaTools.v1.Designer.DrawEditor {

    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Contenidor d'objectes de tipus VisualItem.
    /// </summary>
    /// 
    public abstract class DrawingVisualContainer: FrameworkElement {

        private VisualCollection visuals;

        /// <summary>
        /// Afegeix un objecte al contenidor.
        /// </summary>
        /// <param name="visual">L'objecte a afeigir.</param>
        /// 
        public void Add(DrawingVisual visual) {

            if (visual == null)
                throw new ArgumentNullException("visual");

            if ((visuals != null) && visuals.Contains(visual))
                throw new InvalidOperationException("El objeto ya pertenece al contenedor.");

            if (visuals == null)
                visuals = new VisualCollection(this);

            visuals.Add(visual);
        }

        /// <summary>
        /// Afegeix diversos objectes al contenidor.
        /// </summary>
        /// <param name="visuals">Els objectes a afeigir.</param>
        /// 
        public void Add(IEnumerable<DrawingVisual> visuals) {

            if (visuals == null)
                throw new ArgumentNullException("visuals");

            foreach (DrawingVisual item in visuals)
                Add(item);
        }

        /// <summary>
        /// Elimina un item del contenidor.
        /// </summary>
        /// <param name="visual">El item a eliminar.</param>
        /// 
        public void Remove(DrawingVisual visual) {

            if (visual == null)
                throw new ArgumentNullException("visual");

            if (visuals == null || !visuals.Contains(visual))
                throw new InvalidOperationException("El objeto no pertenece a la contenedor.");

            visuals.Remove(visual);

            if (visuals.Count == 0)
                visuals = null;
        }

        /// <summary>
        /// Elimina tots els objectes del contenidor.
        /// </summary>
        /// 
        public void RemoveAll() {

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
        /// Obte el numero d'objectes en el contenidor.
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
