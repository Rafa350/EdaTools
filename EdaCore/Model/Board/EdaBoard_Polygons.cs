﻿using System;
using System.Collections.Generic;
using System.Linq;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Polygons.Infrastructure;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

namespace MikroPic.EdaTools.v1.Core.Model.Board
{

    /// <summary>
    /// Classe que representa una placa de circuit imprès.
    /// </summary>
    /// 
    public sealed partial class EdaBoard {

        private readonly int _outlineClearance = 250000;

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
        public EdaPolygon GetRegionPolygon(EdaRegionElement region, EdaLayerId layerId, EdaTransformation transformation) {

            if (region == null)
                throw new ArgumentNullException(nameof(region));

            // Si el poligon no es troba en la capa d'interes, no cal fer res
            //
            if (region.IsOnLayer(layerId)) {

                // Obte el poligon de la regio i el transforma si s'escau
                //
                EdaPolygon regionPolygon = region.GetPolygon(layerId);
                regionPolygon = regionPolygon.Transform(transformation);

                // Si estem en capes de senyal, cal generar els porus i termals
                //
                if (layerId.IsSignal) {

                    EdaRect regionBBox = regionPolygon.BoundingBox.Inflated(region.Clearance, region.Clearance);

                    EdaSignal regionSignal = GetSignal(region, null, false);

                    int thicknessCompensation = region.Thickness / 2;

                    var holePolygons = new List<EdaPolygon>();

                    EdaLayerId restrictLayerId = layerId.Side == BoardSide.Top ? EdaLayerId.TopRestrict : EdaLayerId.BottomRestrict;

                    // Procesa els elements de la placa.
                    //
                    foreach (var element in Elements.Where(e => e != region)) {

                        // Comprova si l'element es troba en la mateixa capa que la regio
                        //
                        if (element.IsOnLayer(layerId)) {

                            // Comprova si el element pertany a una senyal diferent de la regio
                            //
                            if (GetSignal(element, null, false) != regionSignal) {

                                // Si l'element es  un altre regio, pero amb una prioritat inferior, aleshores no
                                // el foreda.
                                //
                                if (element is EdaRegionElement regionElement)
                                    if (region.Priority > regionElement.Priority)
                                        continue;

                                int signalClearance = regionSignal == null ? 0 : regionSignal.Clearance;
                                int clearance = thicknessCompensation + Math.Max(signalClearance, region.Clearance);
                                var elementPolygon = element.GetOutlinePolygon(layerId, clearance);
                                if (regionBBox.IntersectsWith(elementPolygon.BoundingBox))
                                    holePolygons.Add(elementPolygon);
                            }
                        }

                        // Comprova si l'element es troba en les capes restrict o forats.
                        //
                        else if (element.IsOnLayer(restrictLayerId) ||
                                 element.IsOnLayer(EdaLayerId.Unplatted) ||
                                 element.IsOnLayer(EdaLayerId.Platted)) {

                            var elementPolygon = element.GetPolygon(layerId);
                            if (regionBBox.IntersectsWith(elementPolygon.BoundingBox))
                                holePolygons.Add(elementPolygon);
                        }

                        // Comprova si l'element es troba en la capa profiles.
                        //
                        else if (element.IsOnLayer(EdaLayerId.Profile)) {

                            var elementPolygon = element.GetOutlinePolygon(EdaLayerId.Profile, _outlineClearance);
                            if (regionBBox.IntersectsWith(elementPolygon.BoundingBox))
                                holePolygons.Add(elementPolygon);
                        }
                    }

                    // Procesa els elements dels components
                    //
                    foreach (var part in Parts) {

                        // Obte la transformacio
                        //
                        EdaTransformation localTransformation = part.GetLocalTransformation();

                        // Procesa els elements del component que es troben en la mateixa capa que la regio.
                        // Si l'element no pertany a la mateixa senyal, genera un forat
                        //
                        foreach (var element in part.Elements.Where(e => e != region)) {

                            if (element.IsOnLayer(layerId) ||
                                element.IsOnLayer(restrictLayerId) ||
                                element.IsOnLayer(EdaLayerId.Unplatted)) {

                                // Si l'element no esta conectat a la mateixa senyal que la regio, genera un forat
                                //
                                if (GetSignal(element, part, false) != regionSignal) {

                                    int clearance = thicknessCompensation + Math.Max(regionSignal.Clearance, region.Clearance);
                                    var elementPolygon = element.GetOutlinePolygon(layerId, clearance);
                                    elementPolygon = elementPolygon.Transform(localTransformation);
                                    //if (part.IsFlipped)
                                    //    outlinePolygon.Reverse();
                                    if (regionBBox.IntersectsWith(elementPolygon.BoundingBox))
                                        holePolygons.Add(elementPolygon);
                                }


                                // En cas contrari (es a dir esta conectat) genera un thermal unicament
                                // en els pads
                                //
                                else if ((element.ElementType == ElementType.SmdPad) ||
                                         (element.ElementType == ElementType.ThPad)) {

                                    int signalClearance = regionSignal == null ? 0 : regionSignal.Clearance;
                                    int clearance = thicknessCompensation + Math.Max(signalClearance, region.Clearance);
                                    var thermalPolygon = ((EdaPadElement)element).GetThermalPolygon(layerId, clearance, 200000);
                                    thermalPolygon = thermalPolygon.Transform(localTransformation);
                                    if (thermalPolygon.HasChilds)
                                        foreach (var child in thermalPolygon.Childs) {
                                            if (regionBBox.IntersectsWith(child.BoundingBox))
                                                holePolygons.Add(child);
                                        }
                                }
                            }
                        }
                    }

                    // Genera el poligon amb els forats pertinents
                    //
                    return regionPolygon.Substract(holePolygons);
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
