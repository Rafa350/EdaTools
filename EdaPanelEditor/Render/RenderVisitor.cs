namespace MikroPic.EdaTools.v1.PanelEditor.Render {

    using MikroPic.EdaTools.v1.Panel.Model;
    using MikroPic.EdaTools.v1.Panel.Model.Items;
    using MikroPic.EdaTools.v1.Panel.Model.Visitors;
    using MikroPic.EdaTools.v1.PanelEditor.Render.WPF.Visuals;
    using System.Windows.Media;

    public sealed class RenderVisitor: DefaultVisitor {

        private readonly DrawingVisual parentVisual;
        private ProjectVisual projectVisual;

        public RenderVisitor(DrawingVisual parentVisual) {

            this.parentVisual = parentVisual;
        }

        public override void Visit(Project project) {

            projectVisual = new ProjectVisual(parentVisual, project);
            projectVisual.Draw();

            base.Visit(project);
        }

        public override void Visit(CutItem cut) {

            ItemVisual visual = new CutVisual(projectVisual, cut);
            visual.Draw();
        }

        public override void Visit(PcbItem pcb) {

            ItemVisual visual = new PcbVisual(projectVisual, pcb);
            visual.Draw();
        }
    }
}
