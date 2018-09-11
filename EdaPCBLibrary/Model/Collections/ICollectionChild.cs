namespace MikroPic.EdaTools.v1.Pcb.Model.Collections {

    public interface ICollectionChild<TParent> {

        void AssignParent(TParent parent);
    }
}
