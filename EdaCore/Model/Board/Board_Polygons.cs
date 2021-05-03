using System;
using System.Collections.Generic;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons;
using MikroPic.EdaTools.v1.Core.Infrastructure.Polygons;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

namespace MikroPic.EdaTools.v1.Core.Model.Board {

    /// <summary>
    /// Classe que representa una placa de circuit imprès.
    /// </summary>
    /// 
    public sealed partial class Board {

        private int _outlineClearance = 250000;

        /// <summary>
        /// Obte el poligon del perfil de la placa. Es calcula amb els elements de la capa profile.
        /// </summary>
        /// <returns>El poligon.</returns>
        /// 
        public Polygon GetOutlinePolygon() {

            IEnumerable<Element> elements = GetElements(_outlineLayer.Name);
            List<Segment> segments = new List<Segment>();
            foreach (var element in elements) {
                if (element is LineElement line)
                    segments.Add(new Segment(line.StartPosition, line.EndPosition));

                else if (element is ArcElement arc) {
                    Point[] points = PolygonBuilder.MakeArc(arc.Center, arc.Radius, arc.StartAngle, arc.Angle, true);
                    for (int i = 1; i < points.Length; i++)
                        segments.Add(new Segment(points[i - 1], points[i]));
                }
            }
            return PolygonProcessor.CreateFromSegments(segments);
        }

        /// <summary>
        /// Calcula el poligon d'una regio.
        /// </summary>
        /// <param name="region">L'element de tipus regio.</param>
        /// <param name="layerName">El nom de la capa a procesar.</param>
        /// <param name="transformation">Transformacio a aplicar al poligon.</param>
        /// <returns>El poligon generat.</returns>
        /// 
        public Polygon GetRegionPolygon(RegionElement region, string layerName, Transformation transformation) {

            if (region == null)
                throw new ArgumentNullException(nameof(region));

            // Si el poligon no es troba en la capa d'interes, no cal fer res
            //
            if (region.IsOnLayer(layerName)) {

                Layer layer = GetLayer(layerName);

                // Obte el poligon de la regio i el transforma si s'escau
                //
                Polygon regionPolygon = region.GetPolygon(layer.Side);
                regionPolygon = regionPolygon.Transformed(transformation);

                // Si estem en capes de senyal, cal generar els porus i termals
                //
                if (layer.Function == LayerFunction.Signal) {

                    Rect regionBBox = regionPolygon.BoundingBox.Inflated(region.Clearance, region.Clearance);

                    Signal regionSignal = GetSignal(region, null, false);

                    int thicknessCompensation = region.Thickness / 2;

                    List<Polygon> holePolygons = new List<Polygon>();

                    Layer restrictLayer = GetLayer(layer.Side, "Restrict");

                    // Procesa els elements de la placa que es troben en la mateixa capa que 
                    // la regio, o en les capes restrict o profile.
                    //
                    foreach (var element in Elements) {
                        if (element != region && !(element is TextElement)) {

                            // El element es en la capa d'interes
                            //
                            if (element.IsOnLayer(layerName)) {

                                // Si no esta en la mateixa senyal que la regio, genera un forat.
                                //
                                if (GetSignal(element, null, false) != regionSignal) {
                                    int signalClearance = regionSignal == null ? 0 : regionSignal.Clearance;
                                    int clearance = thicknessCompensation + Math.Max(signalClearance, region.Clearance);
                                    Polygon elementPolygon = element.GetOutlinePolygon(layer.Side, clearance);
                                    if (regionBBox.IntersectsWith(elementPolygon.BoundingBox))
                                        holePolygons.Add(elementPolygon);
                                }
                            }

                            // El element esta el la capa restrict o la capa holes
                            //
                            else if (element.IsOnLayer(restrictLayer.Name) || element.IsOnLayer("Holes")) {
                                Polygon elementPolygon = element.GetPolygon(restrictLayer.Side);
                                if (regionBBox.IntersectsWith(elementPolygon.BoundingBox))
                                    holePolygons.Add(elementPolygon);
                            }

                            // El element esta el la capa profile
                            //
                            else if (element.IsOnLayer(_outlineLayer.Name)) {
                                Polygon elementPolygon = element.GetOutlinePolygon(BoardSide.None, _outlineClearance);
                                if (regionBBox.IntersectsWith(elementPolygon.BoundingBox))
                                    holePolygons.Add(elementPolygon);
                            }
                        }
                    }

                    // Procesa els elements dels components
                    //
                    foreach (var part in Parts) {

                        // Obte la transformacio
                        //
                        Transformation localTransformation = part.GetLocalTransformation();

                        // Procesa tots els elements del part, que no siguin la propia regio
                        //
                        foreach (var element in part.Elements) {
                            if (element != region) {

                                LayerSet elementLayers = part.GetLocalLayerSet(element);
                                if (elementLayers.Contains(layer.Name) || elementLayers.Contains(restrictLayer.Name) || elementLayers.Contains("Holes")) {

                                    // Si l'element no esta conectat a la mateixa senyal que la regio, genera un forat
                                    //
                                    if (GetSignal(element, part, false) != regionSignal) {
                                        int clearance = thicknessCompensation + Math.Max(regionSignal.Clearance, region.Clearance);
                                        Polygon outlinePolygon = element.GetOutlinePolygon(layer.Side, clearance);
                                        outlinePolygon = outlinePolygon.Transformed(localTransformation);
                                        if (part.Flip)
                                            outlinePolygon.Reverse();
                                        if (regionBBox.IntersectsWith(outlinePolygon.BoundingBox))
                                            holePolygons.Add(outlinePolygon);
                                    }

                                    // En es un pad i esta conectat per tant, genera un thermal
                                    //
                                    else if (element is PadElement) {
                                        int signalClearance = regionSignal == null ? 0 : regionSignal.Clearance;
                                        int clearance = thicknessCompensation + Math.Max(signalClearance, region.Clearance);
                                        Polygon thermalPolygon = ((PadElement)element).GetThermalPolygon(layer.Side, clearance, 200000);
                                        thermalPolygon = thermalPolygon.Transformed(localTransformation);
                                        foreach (var child in thermalPolygon.Childs) {
                                            if (regionBBox.IntersectsWith(child.BoundingBox))
                                                holePolygons.Add(child);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Genera el poligon amb els forats pertinents
                    //
                    return PolygonProcessor.ClipExtended(regionPolygon, holePolygons, PolygonProcessor.ClipOperation.Diference);
                }

                // Si no es capa de senyal no cal fer res mes, ja que no te forats
                //
                else
                    return regionPolygon;
            }
            else
                return null;
        }

        /// <summary>
        /// Calcula el rectangle envolvent de la placa
        /// </summary>
        /// <returns>El resultat.</returns>
        /// 
        public Rect GetBoundingBox() {

            if (_outlineLayer != null) {
                int minX = Int32.MaxValue;
                int minY = Int32.MaxValue;
                int maxX = Int32.MinValue;
                int maxY = Int32.MinValue;
                foreach (var element in GetElements(_outlineLayer.Name)) {
                    Rect r = element.GetBoundingBox(BoardSide.None);
                    if (minX > r.Left)
                        minX = r.Left;
                    if (minY > r.Bottom)
                        minY = r.Bottom;
                    if (maxX < r.Right)
                        maxX = r.Right;
                    if (maxY < r.Top)
                        maxY = r.Top;
                }

                return new Rect(minX, minY, maxX - minX, maxY - minY);
            }
            else
                return new Rect(0, 0, 100000000, 100000000);
        }

        /// <summary>
        /// Obte el tamany de la placa, definit pel seu contingut.
        /// </summary>
        /// 
        public Size Size {
            get {
                return GetBoundingBox().Size;
            }
        }
    }
}
