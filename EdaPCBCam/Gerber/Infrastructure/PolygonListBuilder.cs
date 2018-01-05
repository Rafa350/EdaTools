namespace MikroPic.EdaTools.v1.Cam.Gerber.Infrastructure {

    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using System.Collections.Generic;
    using System.Windows.Media;

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

            public override void Visit(LineElement line) {

                if (line.IsOnLayer(layer)) {
                    Polygon polygon = PolygonBuilder.Build(line, VisitingPart, inflate);
                    resultPolygons.AddRange(PolygonProcessor.Clip(polygon, clipPolygon, PolygonProcessor.ClipOperation.Intersection));
                }
            }

            public override void Visit(HoleElement hole) {

                if (hole.IsOnLayer(layer)) {

                    Polygon polygon = PolygonProcessor.Offset(hole.Polygon, inflate, PolygonProcessor.OffsetJoin.Mitter);
                    if (VisitingPart != null)
                        polygon.Transform(VisitingPart.Transformation);

                    resultPolygons.AddRange(PolygonProcessor.Clip(polygon, clipPolygon, PolygonProcessor.ClipOperation.Intersection));
                }
            }

            public override void Visit(ViaElement via) {

                if (via.IsOnLayer(layer)) {

                    Polygon polygon = PolygonProcessor.Offset(via.Polygon, inflate, PolygonProcessor.OffsetJoin.Mitter);

                    resultPolygons.AddRange(PolygonProcessor.Clip(polygon, clipPolygon, PolygonProcessor.ClipOperation.Intersection));
                }
            }

            public override void Visit(ThPadElement pad) {

                if (pad.IsOnLayer(layer)) {

                    PolygonProcessor.OffsetJoin oj = pad.Shape == ThPadElement.ThPadShape.Square ?
                        PolygonProcessor.OffsetJoin.Round : PolygonProcessor.OffsetJoin.Mitter;

                    Polygon polygon = PolygonProcessor.Offset(pad.Polygon, inflate, oj);
                    if (VisitingPart != null)
                        polygon.Transform(VisitingPart.Transformation);

                    resultPolygons.AddRange(PolygonProcessor.Clip(polygon, clipPolygon, PolygonProcessor.ClipOperation.Intersection));
                }
            }

            public override void Visit(SmdPadElement pad) {

                if (pad.IsOnLayer(layer)) {

                    PolygonProcessor.OffsetJoin oj = pad.Roundnes == 0 ?
                        PolygonProcessor.OffsetJoin.Round : PolygonProcessor.OffsetJoin.Mitter;

                    Polygon polygon = PolygonProcessor.Offset(pad.Polygon, inflate, oj);
                    if (VisitingPart != null)
                        polygon.Transform(VisitingPart.Transformation);

                    resultPolygons.AddRange(PolygonProcessor.Clip(polygon, clipPolygon, PolygonProcessor.ClipOperation.Intersection));
                }
            }
        }

        /// <summary>
        /// Construeix la llista de poligons d'una capa.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// <param name="layer">La capa.</param>
        /// <param name="clipPolygon">Poligon de retall.</param>
        /// <param name="inflate">Aument de tamany dels poligons.</param>
        /// <returns>La llista de poligons.</returns>
        /// 
        public static IEnumerable<Polygon> Build(Board board, Layer layer, Polygon clipPolygon, double inflate = 0) {

            List<Polygon> polygons = new List<Polygon>();
            Visitor visitor = new Visitor(layer, clipPolygon, inflate, polygons);
            board.AcceptVisitor(visitor);

            return PolygonProcessor.Union(polygons);
        }
    }
}
