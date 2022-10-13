﻿using System;
using System.Collections.Generic;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons.Infrastructure;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Classe que representa una placa de circuit imprès.
    /// </summary>
    /// 
    public sealed partial class EdaBoard {

        private sealed class HoleGeneratorVisitor: EdaElementVisitor {

            private readonly int _outlineClearance = 250000;

            private readonly EdaLayerId _layerId;
            private readonly EdaRegionElement _region;
            private readonly EdaRect _regionBounds;
            private readonly List<EdaPolygon> _polygons;

            public HoleGeneratorVisitor(EdaLayerId layerId, EdaRegionElement region, List<EdaPolygon> holes) {

                _layerId = layerId;
                _region = region;
                _polygons = holes;

                _regionBounds = _region.GetBoundingBox(_layerId).Inflated(_region.Clearance, _region.Clearance);
            }

            public override void Visit(EdaLineElement element) {

                if (element.IsOnLayer(EdaLayerId.Profile)) 
                    VisitProfileElement(element);

                else if (element.IsOnLayer(_layerId))
                    VisitSignalElement(element);
            }

            public override void Visit(EdaArcElement element) {

                if (element.IsOnLayer(EdaLayerId.Profile))
                    VisitProfileElement(element);

                else if (element.IsOnLayer(_layerId))
                    VisitSignalElement(element);
            }

            public override void Visit(EdaThPadElement element) {

                if (element.IsOnLayer(_layerId))
                    VisitSignalElement(element);
            }

            public override void Visit(EdaSmdPadElement element) {

                if (element.IsOnLayer(_layerId))
                    VisitSignalElement(element);
            }

            public override void Visit(EdaViaElement element) {

                if (element.IsOnLayer(_layerId))
                    VisitSignalElement(element);
            }

            public override void Visit(EdaRegionElement element) {

                if ((element != _region) && element.IsOnLayer(_layerId)) {

                    var regionSignal = Board.GetSignal(_region, null, false);
                    var signal = Board.GetSignal(element, Part, false);

                    if (signal != regionSignal) {

                        if (element.Priority > _region.Priority) {

                            int signalClearance = regionSignal == null ? 0 : regionSignal.Clearance;
                            int clearance = Math.Max(signalClearance, _region.Clearance);

                            var polygon = element.GetOutlinePolygon(_layerId, clearance);
                            if (_regionBounds.IntersectsWith(polygon.BoundingBox))
                                _polygons.Add(polygon);
                        }
                    }
                }
            }

            public override void Visit(EdaCircularHoleElement element) {

                var polygon = GetOutlinePolygon(element, _outlineClearance);
                if (_regionBounds.IntersectsWith(polygon.BoundingBox))
                    _polygons.Add(polygon);
            }

            public override void Visit(EdaLinearHoleElement element) {

                var polygon = GetOutlinePolygon(element, _outlineClearance);
                if (_regionBounds.IntersectsWith(polygon.BoundingBox))
                    _polygons.Add(polygon);
            }

            /// <summary>
            /// Visita un element de perfil
            /// </summary>
            /// <param name="element">L'element a visitar.</param>
            /// 
            private void VisitProfileElement(EdaElement element) {

                var polygon = element.GetOutlinePolygon(EdaLayerId.Profile, _outlineClearance);
                if (_regionBounds.IntersectsWith(polygon.BoundingBox))
                    _polygons.Add(polygon);
            }

            /// <summary>
            /// Visita un element de senyal.
            /// </summary>
            /// <param name="element">L'element a visitar.</param>
            /// 
            private void VisitSignalElement(EdaElement element) {

                var regionSignal = Board.GetSignal(_region, null, false);
                var signal = Board.GetSignal(element, Part, false);

                // Si la senyal del element es la mateixa que la regio, genera un termal
                //
                if (signal == regionSignal) {

                    if (element is EdaPadElement padElement) {
                        var polygons = GetThermalPolygons(padElement);
                        foreach(var polygon in polygons)
                            if (_regionBounds.IntersectsWith(polygon.BoundingBox))
                                _polygons.Add(polygon);
                    }
                }

                // En cas contrari genera un forat.
                //
                else {
                    var clearance = Math.Max(regionSignal.Clearance, _region.Clearance);
                    var polygon = GetOutlinePolygon(element, clearance);
                    if (_regionBounds.IntersectsWith(polygon.BoundingBox))
                        _polygons.Add(polygon);
                }
            }

            /// <summary>
            /// Genera el poligon d'un termal.
            /// </summary>
            /// <param name="element">L'element.</param>
            /// <returns>El resultat. Pot ser mes d'un poligon.</returns>
            /// 
            private IEnumerable<EdaPolygon> GetThermalPolygons(EdaPadElement element) {

                var elementPoligon = element.GetOutlinePolygon(_layerId, _region.ThermalClearance);

                var bounds = elementPoligon.BoundingBox;
                var thermalPoints = EdaPointFactory.CreateCross(element.Position, bounds.Size, _region.ThermalThickness, element.Rotation);
                var thermalPoligon = new EdaPolygon(thermalPoints);
                
                var results = elementPoligon.Substract(thermalPoligon);
                if (Part != null) {
                    var transformation = Part.GetLocalTransformation();
                    var polygons = new List<EdaPolygon>();
                    foreach (var result in results)
                        polygons.Add(result.Transform(transformation));
                    return polygons;
                }
                else 
                    return results;
            }

            /// <summary>
            /// Genera un poligon.
            /// </summary>
            /// <param name="element">L'element.</param>
            /// <param name="clearance">L'espaiat.</param>
            /// <returns>El poligon.</returns>
            /// 
            private EdaPolygon GetOutlinePolygon(EdaElement element, int clearance) {

                var polygon = element.GetOutlinePolygon(_layerId, clearance);
                if (Part != null) {
                    var transformation = Part.GetLocalTransformation();
                    polygon = polygon.Transform(transformation);
                }

                return polygon;
            }
        }

        /// <summary>
        /// Obte el poligon del perfil de la placa. Es calcula amb els elements de la capa profile.
        /// </summary>
        /// <returns>El poligon.</returns>
        /// 
        public EdaPolygon GetOutlinePolygon() {

            var elements = GetElements(GetLayer(EdaLayerId.Profile));
            var segments = new List<Segment>();
            foreach (var element in elements) {
                if (element is EdaLineElement line)
                    segments.Add(new Segment(line.StartPosition, line.EndPosition));

                else if (element is EdaArcElement arc) {
                    var points = new List<EdaPoint>(EdaPointFactory.CreateArc(arc.Center, arc.Radius, arc.StartAngle, arc.Angle, true));
                    for (int i = 1; i < points.Count; i++)
                        segments.Add(new Segment(points[i - 1], points[i]));
                }
            }

            var p = Polygonizer.Poligonize(segments);
            return p == null ? null : new EdaPolygon(p);
        }

        /// <summary>
        /// Calcula el poligon d'una regio.
        /// </summary>
        /// <param name="region">L'element de tipus regio.</param>
        /// <param name="layer">La capa a procesar.</param>
        /// <param name="transformation">Transformacio a aplicar al poligon.</param>
        /// <returns>El poligon generat.</returns>
        /// 
        public IEnumerable<EdaPolygon> GetRegionPolygons(EdaRegionElement region, EdaLayerId layerId, EdaTransformation transformation) {

            if (region == null)
                throw new ArgumentNullException(nameof(region));

            var holes = new List<EdaPolygon>();

            var visitor = new HoleGeneratorVisitor(layerId, region, holes);
            visitor.Visit(this);

            EdaPolygon polygon = region.GetPolygon(layerId);
            var polygons = polygon.Substract(holes);

            //return (transformation == null) ? polygons : polygons.Transform(transformation);
            return polygons;
        }

        /// <summary>
        /// Calcula el rectangle envolvent de la placa
        /// </summary>
        /// <returns>El resultat.</returns>
        /// 
        public EdaRect GetBoundingBox() {

            int minX = Int32.MaxValue;
            int minY = Int32.MaxValue;
            int maxX = Int32.MinValue;
            int maxY = Int32.MinValue;
            foreach (var element in GetElements(_outlineLayer)) {
                EdaRect r = element.GetBoundingBox(_outlineLayer.Id);
                if (minX > r.Left)
                    minX = r.Left;
                if (minY > r.Bottom)
                    minY = r.Bottom;
                if (maxX < r.Right)
                    maxX = r.Right;
                if (maxY < r.Top)
                    maxY = r.Top;
            }

            return new(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// Obte el tamany de la placa, definit pel seu contingut.
        /// </summary>
        /// 
        public EdaSize Size =>
            GetBoundingBox().Size;
    }
}
