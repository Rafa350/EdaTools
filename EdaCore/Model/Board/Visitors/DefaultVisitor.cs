﻿namespace MikroPic.EdaTools.v1.Core.Model.Board.Visitors {

    using MikroPic.EdaTools.v1.Core.Model.Board;
    using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

    /// <summary>
    /// Clase visitador per defecte. Defineix tots els visitadors,
    /// pero no fa cap accio.
    /// </summary>
    /// 
    public abstract class DefaultVisitor: IBoardVisitor {

        public virtual void Visit(Board board) {
        }

        public virtual void Visit(Layer layer) {
        }

        public virtual void Visit(Signal signal) {
        }

        public virtual void Visit(LineElement line) {
        }

        public virtual void Visit(ArcElement arc) {
        }

        public virtual void Visit(RectangleElement rectangle) {
        }

        public virtual void Visit(CircleElement circle) {
        }

        public virtual void Visit(SmdPadElement pad) {
        }

        public virtual void Visit(ThPadElement pad) {
        }

        public virtual void Visit(SlotElement pad) {
        }

        public virtual void Visit(RegionElement region) {
        }

        public virtual void Visit(TextElement text) {
        }

        public virtual void Visit(HoleElement hole) {
        }

        public virtual void Visit(ViaElement via) {
        }

        public virtual void Visit(Part part) {
        }

        public virtual void Visit(Component block) {
        }

        public virtual void Visit(PartAttribute attribute) {
        }

        public virtual void Visit(ComponentAttribute attribute) {
        }
    }
}
