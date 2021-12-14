namespace MikroPic.EdaTools.v1.Panel.Model.Visitors {

    using System;

    using MikroPic.EdaTools.v1.Panel.Model.Items;

    public abstract class EdaDefaultPanelVisitor : IEdaPanelVisitor {

        public virtual void Visit(EdaPanel project) {

            if (project == null)
                throw new ArgumentNullException(nameof(project));

            if (project.HasItems)
                foreach (var item in project.Items)
                    item.AcceptVisitor(this);
        }

        public virtual void Visit(CutItem cut) {
        }

        public virtual void Visit(PcbItem pcb) {
        }
    }
}
