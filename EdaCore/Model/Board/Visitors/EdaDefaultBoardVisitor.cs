using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

namespace MikroPic.EdaTools.v1.Core.Model.Board.Visitors {

    /// <summary>
    /// Clase visitador per defecte. Defineix tots els visitadors,
    /// pero no fa cap accio.
    /// </summary>
    /// 
    public abstract class EdaDefaultBoardVisitor: IEdaBoardVisitor {

        /// <inheritdoc/>
        /// 
        public virtual void Visit(EdaBoard board) {
        }

        /// <inheritdoc/>
        /// 
        public virtual void Visit(EdaLibrary library) {
        }

        /// <inheritdoc/>
        /// 
        public virtual void Visit(EdaLayer layer) {
        }

        /// <inheritdoc/>
        /// 
        public virtual void Visit(EdaSignal signal) {
        }

        /// <inheritdoc/>
        /// 
        public virtual void Visit(EdaLineElement element) {
        }

        /// <inheritdoc/>
        /// 
        public virtual void Visit(EdaArcElement element) {
        }

        /// <inheritdoc/>
        /// 
        public virtual void Visit(EdaRectangleElement element) {
        }

        /// <inheritdoc/>
        /// 
        public virtual void Visit(EdaCircleElement element) {
        }

        /// <inheritdoc/>
        /// 
        public virtual void Visit(EdaPolygonElement element) {
        }

        /// <inheritdoc/>
        /// 
        public virtual void Visit(EdaSmdPadElement element) {
        }

        /// <inheritdoc/>
        /// 
        public virtual void Visit(EdaThPadElement element) {
        }

        /// <inheritdoc/>
        /// 
        public virtual void Visit(EdaRegionElement element) {
        }

        /// <inheritdoc/>
        /// 
        public virtual void Visit(EdaTextElement element) {
        }

        /// <inheritdoc/>
        /// 
        public virtual void Visit(EdaViaElement element) {
        }

        /// <inheritdoc/>
        /// 
        public virtual void Visit(EdaPart part) {
        }

        /// <inheritdoc/>
        /// 
        public virtual void Visit(EdaComponent component) {
        }

        /// <inheritdoc/>
        /// 
        public virtual void Visit(EdaPartAttribute attribute) {
        }

        /// <inheritdoc/>
        /// 
        public virtual void Visit(EdaComponentAttribute attribute) {
        }

        /// <inheritdoc/>
        /// 
        public virtual void Visit(EdaCircleHoleElement element) {
        }

        /// <inheritdoc/>
        /// 
        public virtual void Visit(EdaLineHoleElement element) {
        }
    }
}
