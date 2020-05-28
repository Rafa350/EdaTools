namespace MikroPic.EdaTools.v1.Collections {

    public interface ICollectionChild<TParent> {

        void AssignParent(TParent parent);
    }
}
