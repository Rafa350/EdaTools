namespace MikroPic.EdaTools.v1.Core.Model.Board {

    public interface IBoardVisitable {

        /// <summary>
        /// Accepta un visitador
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        void AcceptVisitor(IBoardVisitor visitor);
    }
}
