namespace MikroPic.EdaTools.v1.PanelEditor.Render {

    using MikroPic.EdaTools.v1.Panel.Model.Items;
    using MikroPic.EdaTools.v1.Panel.Model.Visitors;
    using MikroPic.EdaTools.v1.PanelEditor.Render.WPF.Visuals;
    using System.Windows.Media;

    public sealed class RenderVisitor: DefaultVisitor {

        private readonly DrawingVisual parentVisual;

        public RenderVisitor(DrawingVisual rootVisual) {

            this.parentVisual = rootVisual;
        }

        public override void Visit(CutItem cut) {

            ItemVisual visual = new CutVisual(parentVisual, cut);
            visual.Draw();
        }

        public override void Visit(PcbItem pcb) {

            ItemVisual visual = new PcbVisual(parentVisual, pcb);
            visual.Draw();
        }
    }
}
