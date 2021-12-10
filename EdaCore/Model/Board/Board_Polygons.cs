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

            var elements = GetElements(GetLayer(LayerId.Profile));
            var segments = new List<Segment>();
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
        /// <param name="layer">La capa a procesar.</param>
        /// <param name="transformation">Transformacio a aplicar al poligon.</param>
        /// <returns>El poligon generat.</returns>
        /// 
        public Polygon GetRegionPolygon(RegionElement region, LayerId layerId, Transformation transformation) {

            if (region == null)
                throw new ArgumentNullException(nameof(region));

            // Si el poligon no es troba en la capa d'interes, no cal fer res
            //
            if (region.LayerSet.Contains(layerId)) {

                // Obte el poligon de la regio i el transforma si s'escau
                //
                Polygon regionPolygon = region.GetPolygon(layerId.Side);
                regionPolygon = regionPolygon.Transformed(transformation);

                // Si estem en capes de senyal, cal generar els porus i termals
                //
                if (layerId.IsSignal) {

                    Rect regionBBox = regionPolygon.BoundingBox.Inflated(region.Clearance, region.Clearance);

                    Signal regionSignal = GetSignal(region, null, false);

                    int thicknessCompensation = region.Thickness / 2;

                    List<Polygon> holePolygons = new List<Polygon>();

                    LayerId restrictLayerId = layerId.Side == BoardSide.Top ? LayerId.TopRestrict : LayerId.BottomRestrict;

                    // Procesa els elements de la placa.
                    //
                    foreach (var element in Elements) {
                        if (element != region) {

                            // Comprova si l'element es troba en la mateixa capa que la regio
                            //
                            if (element.IsOnLayer(layerId)) {

                                // Comprova si el element pertany a una senal diferent de la regio
                                //
                                if (GetSignal(element, null, false) != regionSignal) {

                                    int signalClearance = regionSignal == null ? 0 : regionSignal.Clearance;
                                    int clearance = thicknessCompensation + Math.Max(signalClearance, region.Clearance);
                                    Polygon elementPolygon = element.GetOutlinePolygon(layerId.Side, clearance);
                                    if (regionBBox.IntersectsWith(elementPolygon.BoundingBox))
                                        holePolygons.Add(elementPolygon);
                                }
                            }

                            // Comprova si l'element es troba en les capes restrict o holes.
                            //
                            else if (element.IsOnLayer(restrictLayerId) ||
                                     element.IsOnLayer(LayerId.Holes) ||
                                     element.IsOnLayer(LayerId.Drills)) {

                                Polygon elementPolygon = element.GetPolygon(layerId.Side);
                                if (regionBBox.IntersectsWith(elementPolygon.BoundingBox))
                                    holePolygons.Add(elementPolygon);
                            }

                            // Comprova si l'element es troba en la capa profiles.
                            //
                            else if (element.IsOnLayer(LayerId.Profile)) {

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

                        // Procesa els elements del component que es troben en la mateixa capa que la regio.
                        // Si l'element no pertany a la mateixa senyal, genera un forat
                        //
                        foreach (var element in part.Elements) {
                            if (element != region) {

                                if (element.IsOnLayer(layerId) ||
                                    element.IsOnLayer(restrictLayerId) ||
                                    element.IsOnLayer(LayerId.Holes)) {

                                    // Si l'element no esta conectat a la mateixa senyal que la regio, genera un forat
                                    //
                                    if (GetSignal(element, part, false) != regionSignal) {

                                        int clearance = thicknessCompensation + Math.Max(regionSignal.Clearance, region.Clearance);
                                        Polygon outlinePolygon = element.GetOutlinePolygon(layerId.Side, clearance);
                                        outlinePolygon = outlinePolygon.Transformed(localTransformation);
                                        if (part.Flip)
                                            outlinePolygon.Reverse();
                                        if (regionBBox.IntersectsWith(outlinePolygon.BoundingBox))
                                            holePolygons.Add(outlinePolygon);
                                    }


                                    // En cas contrari (es a dir esta conectat) genera un thermal unicament
                                    // en els pads
                                    //
                                    else if ((element.ElementType == ElementType.SmdPad) ||
                                             (element.ElementType == ElementType.ThPad)) {

                                        int signalClearance = regionSignal == null ? 0 : regionSignal.Clearance;
                                        int clearance = thicknessCompensation + Math.Max(signalClearance, region.Clearance);
                                        Polygon thermalPolygon = ((PadElement)element).GetThermalPolygon(layerId.Side, clearance, 200000);
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

            int minX = Int32.MaxValue;
            int minY = Int32.MaxValue;
            int maxX = Int32.MinValue;
            int maxY = Int32.MinValue;
            foreach (var element in GetElements(_outlineLayer)) {
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

        /// <summary>
        /// Obte el tamany de la placa, definit pel seu contingut.
        /// </summary>
        /// 
        public Size Size =>
            GetBoundingBox().Size;
    }
}
