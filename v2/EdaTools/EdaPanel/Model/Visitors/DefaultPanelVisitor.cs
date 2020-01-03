namespace MikroPic.EdaTools.v1.Panel.Model.Visitors {

    using System;
    using MikroPic.EdaTools.v1.Panel.Model.Items;

    public abstract class DefaultPanelVisitor: IPanelVisitor {

        public virtual void Visit(Panel project) {

            if (project == null)
                throw new ArgumentNullException("project");

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
