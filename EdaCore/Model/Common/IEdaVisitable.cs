namespace MikroPic.EdaTools.v1.Core.Model.Common {

    public interface IEdaVisitable<T> {

        /// <summary>
        /// Accepta un visitador
        /// </summary>
        /// <param name="visitor">El visitador.</param>
        /// 
        void AcceptVisitor(T visitor);
    }
}
