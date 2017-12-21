namespace MikroPic.EdaTools.v1.Cam.Gerber.Infrastructure {

    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;

    internal static class PolygonListBuilder { 

        private class Visitor: BoardVisitor {

            private readonly Layer layer;
            private readonly List<Polygon> resultPolygons;
            private readonly double inflate;
            private readonly Polygon clipPolygon;

            public Visitor(Layer layer, Polygon clipPolygon, double inflate, List<Polygon> resultPolygons) {

                this.layer = layer;
                this.clipPolygon = clipPolygon;
                this.inflate = inflate;
                this.resultPolygons = resultPolygons;
            }

            public override void Visit(ViaElement via) {

                if (via.IsOnLayer(layer)) {
                    Polygon polygon = Polygon.FromElement(via, VisitingPart, inflate);
                    resultPolygons.AddRange(polygon.Clip(clipPolygon, ClipperLib.ClipType.ctIntersection));
                }
            }

            public override void Visit(ThPadElement pad) {

                if (pad.IsOnLayer(layer)) {
                    Polygon polygon = Polygon.FromElement(pad, VisitingPart, inflate);
                    resultPolygons.AddRange(polygon.Clip(clipPolygon, ClipperLib.ClipType.ctIntersection));
                }
            }

            public override void Visit(SmdPadElement pad) {

                if (pad.IsOnLayer(layer)) {
                    Polygon polygon = Polygon.FromElement(pad, VisitingPart, inflate);
                    resultPolygons.AddRange(polygon.Clip(clipPolygon, ClipperLib.ClipType.ctIntersection));
                }
            }
        }

        /// <summary>
        /// Construeix la llista de poligons d'una capa retallats.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// <param name="layer">La capa.</param>
        /// <param name="clipPolygon">Poligon de retall.</param>
        /// <param name="inflate">Aument de tamany dels poligons.</param>
        /// <returns></returns>
        public static IEnumerable<Polygon> Build(Board board, Layer layer, Polygon clipPolygon, double inflate = 0) {

            List<Polygon> polygons = new List<Polygon>();
            Visitor visitor = new Visitor(layer, clipPolygon, inflate, polygons);
            board.AcceptVisitor(visitor);

            return polygons;
        }
    }
}
