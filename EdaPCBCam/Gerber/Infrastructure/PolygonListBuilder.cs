﻿namespace MikroPic.EdaTools.v1.Cam.Gerber.Infrastructure {

    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using System.Collections.Generic;

    internal static class PolygonListBuilder { 

        private class Visitor: BoardVisitor {

            private readonly LayerId layerId;
            private readonly List<Polygon> resultPolygons;
            private readonly double inflate;
            private readonly Polygon clipPolygon;

            public Visitor(LayerId layerId, Polygon clipPolygon, double inflate, List<Polygon> resultPolygons) {

                this.layerId = layerId;
                this.clipPolygon = clipPolygon;
                this.inflate = inflate;
                this.resultPolygons = resultPolygons;
            }

            public override void Visit(LineElement line) {

                if (line.IsOnLayer(layerId)) {

                    Polygon polygon = line.GetPolygon(inflate);
                    if (VisitingPart != null)
                        polygon.Transform(VisitingPart.Transformation);

                    resultPolygons.AddRange(PolygonProcessor.Clip(polygon, clipPolygon, PolygonProcessor.ClipOperation.Intersection));
                }
            }

            public override void Visit(HoleElement hole) {

                if (hole.IsOnLayer(layerId)) {

                    Polygon polygon = hole.GetPolygon(inflate);
                    if (VisitingPart != null)
                        polygon.Transform(VisitingPart.Transformation);

                    resultPolygons.AddRange(PolygonProcessor.Clip(polygon, clipPolygon, PolygonProcessor.ClipOperation.Intersection));
                }
            }

            public override void Visit(ViaElement via) {

                if (via.IsOnLayer(layerId)) {

                    Polygon polygon = via.GetPolygon(inflate);
                    if (VisitingPart != null)
                        polygon.Transform(VisitingPart.Transformation);

                    resultPolygons.AddRange(PolygonProcessor.Clip(polygon, clipPolygon, PolygonProcessor.ClipOperation.Intersection));
                }
            }

            public override void Visit(ThPadElement pad) {

                if (pad.IsOnLayer(layerId)) {

                    Polygon polygon = pad.GetPolygon(inflate);
                    if (VisitingPart != null)
                        polygon.Transform(VisitingPart.Transformation);

                    resultPolygons.AddRange(PolygonProcessor.Clip(polygon, clipPolygon, PolygonProcessor.ClipOperation.Intersection));
                }
            }

            public override void Visit(SmdPadElement pad) {

                if (pad.IsOnLayer(layerId)) {

                    Polygon polygon = pad.GetPolygon(inflate);
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
        /// <param name="layerId">La capa.</param>
        /// <param name="clipPolygon">Poligon de retall.</param>
        /// <param name="inflate">Aument de tamany dels poligons.</param>
        /// <returns>La llista de poligons.</returns>
        /// 
        public static IEnumerable<Polygon> Build(Board board, LayerId layerId, Polygon clipPolygon, double inflate = 0) {

            List<Polygon> polygons = new List<Polygon>();
            Visitor visitor = new Visitor(layerId, clipPolygon, inflate, polygons);
            board.AcceptVisitor(visitor);

            return PolygonProcessor.Union(polygons);
        }
    }
}
