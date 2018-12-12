namespace MikroPic.EdaTools.v1.Panel.Model {

    using MikroPic.EdaTools.v1.Base.Geometry;
    using MikroPic.EdaTools.v1.Collections;
    using System;
    using System.Collections.Generic;

    public sealed class Project: IVisitable {

        private Size size;
        private Collection<ProjectItem> items;

        /// <summary>
        /// Afegeix un element
        /// </summary>
        /// <param name="item">El item a afeigir.</param>
        /// 
        public void AddElement(ProjectItem item) {

            if (item == null)
                throw new ArgumentNullException("item");

            if (items == null)
                items = new Collection<ProjectItem>();

            items.Add(item);
        }

        public void  AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        /// <summary>
        /// Afegeix una coleccio d'items.
        /// </summary>
        /// <param name="item">El item a afeigir.</param>
        /// 
        public void AddElements(IEnumerable<ProjectItem> item) {

            if (item == null)
                throw new ArgumentNullException("item");

            foreach (var element in item)
                AddElement(element);
        }

        /// <summary>
        /// Indica si te items
        /// </summary>
        /// 
        public bool HasItems {
            get {
                return items != null;
            }
        }

        /// <summary>
        /// Enumera els elements
        /// </summary>
        /// 
        public IEnumerable<ProjectItem> Items {
            get {
                return items;
            }
        }

        /// <summary>
        /// Obte o asigna el tamany del panell
        /// </summary>
        /// 
        public Size Size {
            get {
                return size;
            }
            set {
                size = value;
            }
        }
    }
}
