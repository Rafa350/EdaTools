namespace MikroPic.EdaTools.v1.Panel.Model {

    using MikroPic.EdaTools.v1.Panel.Model.Items;

    public interface IEdaPanelVisitor {

        void Visit(EdaPanel project);
        void Visit(CutItem cut);
        void Visit(PcbItem pcb);
    }
}
