namespace MikroPic.EdaTools.v1.Core.Model.Board.Visitors {

    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

    /// <summary>
    /// Clase visitador per defecte. Defineix tots els visitadors,
    /// pero no fa cap accio.
    /// </summary>
    /// 
    public abstract class EdaDefaultBoardVisitor : IEdaBoardVisitor {

        public virtual void Visit(EdaBoard board) {
        }

        public virtual void Visit(EdaLibrary library) {
        }

        public virtual void Visit(EdaLayer layer) {
        }

        public virtual void Visit(EdaSignal signal) {
        }

        public virtual void Visit(EdaLineElement line) {
        }

        public virtual void Visit(EdaArcElement arc) {
        }

        public virtual void Visit(EdaRectangleElement rectangle) {
        }

        public virtual void Visit(EdaCircleElement circle) {
        }

        public virtual void Visit(EdaPolygonElement polygon) {
        }

        public virtual void Visit(EdaSmdPadElement pad) {
        }

        public virtual void Visit(EdaThPadElement pad) {
        }

        public virtual void Visit(EdaRegionElement region) {
        }

        public virtual void Visit(EdaTextElement text) {
        }

        public virtual void Visit(EdaHoleElement hole) {
        }

        public virtual void Visit(EdaViaElement via) {
        }

        public virtual void Visit(EdaPart part) {
        }

        public virtual void Visit(EdaComponent block) {
        }

        public virtual void Visit(EdaPartAttribute attribute) {
        }

        public virtual void Visit(EdaComponentAttribute attribute) {
        }
    }
}
