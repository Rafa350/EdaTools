namespace MikroPic.EdaTools.v1.Panel.Model {

    using MikroPic.EdaTools.v1.Panel.Model.Items;

    public interface IPanelVisitor {

        void Visit(Panel project);
        void Visit(CutItem cut);
        void Visit(PcbItem pcb);
    }
}
