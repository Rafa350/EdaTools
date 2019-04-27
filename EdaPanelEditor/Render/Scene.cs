namespace MikroPic.EdaTools.v1.PanelEditor.Render {

    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;
    using MikroPic.EdaTools.v1.Panel.Model;
    using MikroPic.EdaTools.v1.Panel.Model.Items;
    using MikroPic.EdaTools.v1.Panel.Model.Visitors;
    using MikroPic.EdaTools.v1.PanelEditor.Render.Visuals;
    using MikroPic.EdaTools.v1.PanelEditor.VisualEditor;

    public sealed class Scene {

        private sealed class Visitor : DefaultPanelVisitor {

            private readonly VisualItem parentVisual;
            private readonly IDictionary<PanelItem, PanelItemVisual> itemMap;

            public Visitor(VisualItem parentVisual, IDictionary<PanelItem, PanelItemVisual> itemMap) {

                this.parentVisual = parentVisual;
                this.itemMap = itemMap;
            }

            public override void Visit(Panel project) {

                PanelVisual projectVisual = new PanelVisual(parentVisual, project);
                projectVisual.Renderize();

                base.Visit(project);
            }

            public override void Visit(CutItem item) {

                PanelItemVisual itemVisual = new CutItemVisual(parentVisual, item);
                itemMap.Add(item, itemVisual);
                itemVisual.Renderize();
            }

            public override void Visit(PcbItem item) {

                PanelItemVisual itemVisual = new PcbItemVisual(parentVisual, item);
                itemMap.Add(item, itemVisual);
                itemVisual.Renderize();
            }
        }

        private readonly Dictionary<PanelItem, PanelItemVisual> itemMap = new Dictionary<PanelItem, PanelItemVisual>();
        private readonly VisualItem visual = new VisualItem();

        public void Initialize(Panel project) {

            itemMap.Clear();

            IPanelVisitor visitor = new Visitor(visual, itemMap);
            project.AcceptVisitor(visitor);
        }

        /// <summary>
        /// Afegeig un item
        /// </summary>
        /// <param name="item">El item a afeigir.</param>
        /// 
        public void AddItem(PanelItem item) {

            IPanelVisitor visitor = new Visitor(visual, itemMap);
            item.AcceptVisitor(visitor);
        }

        /// <summary>
        /// Actualitza el item especiticat
        /// </summary>
        /// <param name="item">El item.</param>
        /// 
        public void UpdateItem(PanelItem item) {

            if (itemMap.TryGetValue(item, out PanelItemVisual itemVisual))
                itemVisual.Renderize();
        }

        /// <summary>
        /// Elimina un item.
        /// </summary>
        /// <param name="item">El item a eliminar.</param>
        /// 
        public void RemoveItem(PanelItem item) {

            if (itemMap.TryGetValue(item, out PanelItemVisual itemVisual))
                (itemVisual.Parent as DrawingVisual).Children.Remove(itemVisual);
        }

        /// <summary>
        /// Obte el item en la posicio especificada
        /// </summary>
        /// <param name="position">La posicio.</param>
        /// <returns>El item, null si no el troba.</returns>
        /// 
        public PanelItem GetItem(Point position) {

            HitTestResult result = VisualTreeHelper.HitTest(visual, position);
            if ((result != null) && (result.VisualHit is PanelItemVisual itemVisual)) 
                return itemVisual.Item;
            else
                return null;
        }

        /// <summary>
        /// Obte la visual de l'escena.
        /// </summary>
        /// 
        public VisualItem Visual {
            get {
                return visual;
            }
        }
    }
}
