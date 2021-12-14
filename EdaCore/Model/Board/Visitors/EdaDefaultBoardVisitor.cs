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

        public virtual void Visit(LineElement line) {
        }

        public virtual void Visit(ArcElement arc) {
        }

        public virtual void Visit(RectangleElement rectangle) {
        }

        public virtual void Visit(CircleElement circle) {
        }

        public virtual void Visit(PolygonElement polygon) {
        }

        public virtual void Visit(SmdPadElement pad) {
        }

        public virtual void Visit(ThPadElement pad) {
        }

        public virtual void Visit(RegionElement region) {
        }

        public virtual void Visit(TextElement text) {
        }

        public virtual void Visit(HoleElement hole) {
        }

        public virtual void Visit(ViaElement via) {
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
