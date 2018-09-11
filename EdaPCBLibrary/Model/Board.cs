namespace MikroPic.EdaTools.v1.Pcb.Model {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Geometry.Polygons;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Identifica la cara de la placa.
    /// </summary>
    /// 
    public enum BoardSide {
        None,
        Top,
        Inner,
        Bottom
    }

    /// <summary>
    /// Clase que representa una placa de circuit impres.
    /// </summary>
    /// 
    public sealed partial class Board : IVisitable {

        private Point position;
        private Angle rotation;

        /// <summary>
        /// Constructor del objecte amb els parametres per defecte.
        /// </summary>
        /// 
        public Board() {
        }

        /// <summary>
        /// Procesa un visitador.
        /// </summary>
        /// <param name="visitor">Visitador.</param>
        /// 
        public void AcceptVisitor(IVisitor visitor) {

            visitor.Visit(this);
        }

        #region Metodes per operacions amb poligons

        /// <summary>
        /// Obte el poligon del perfil de la placa. Es calcula amb es elements de la capa profile.
        /// </summary>
        /// <returns>El poligon.</returns>
        /// 
        public Polygon GetOutlinePolygon() {

            IEnumerable<Element> elements = GetElements(outlineLayer);
            List<Segment> segments = new List<Segment>();
            foreach (var element in elements) {
                if (element is LineElement) {
                    LineElement line = (LineElement)element;
                    segments.Add(new Segment(line.StartPosition, line.EndPosition));
                }
                else if (element is ArcElement) {
                    ArcElement arc = (ArcElement)element;
                    segments.Add(new Segment(arc.StartPosition, arc.EndPosition));
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
        public Polygon GetRegionPolygon(RegionElement region, Layer layer, Transformation transformation) {

            if (region == null)
                throw new ArgumentNullException("region");

            if (layer == null)
                throw new ArgumentNullException("layer");

            // Si el poligon no es troba en la capa d'interes, no cal fer res
            //
            if (region.IsOnLayer(layer)) {

                // Obte el poligon de la regio i el transforma si s'escau
                //
                Polygon regionPolygon = region.GetPolygon(layer.Id.Side);
                regionPolygon = regionPolygon.Transformed(transformation);

                // Si estem en capes de senyal, cal generar els porus i termals
                //
                if (layer.Function == LayerFunction.Signal) {

                    Signal regionSignal = GetSignal(region, null, false);

                    int thicknessCompensation = 150000 + region.Thickness / 2;
                    List<Polygon> holePolygons = new List<Polygon>();

                    Layer restrictLayer = GetLayer(layer.Id.Side == BoardSide.Top ? Layer.TopRestrictId : Layer.BottomRestrictId);

                    // Procesa els elements de la placa que es troben en la mateixa capa que 
                    // la regio, o en les capes restrict o profile.
                    //
                    foreach (var element in elements) {
                        if (element != region) {

                            // El element es en la mateixa capa que la regio
                            //
                            if (element.IsOnLayer(layer)) {

                                // Si no esta en la mateixa senyal que la regio, genera un forat.
                                //
                                if (GetSignal(element, null, false) != regionSignal) {
                                    int signalClearance = regionSignal == null ? 0 : regionSignal.Clearance;
                                    int clearance = thicknessCompensation + Math.Max(signalClearance, region.Clearance);
                                    Polygon elementPolygon = element.GetOutlinePolygon(layer.Id.Side, clearance);
                                    holePolygons.Add(elementPolygon);
                                }
                            }

                            // El element esta el la capa restrict
                            //
                            else if (element.IsOnLayer(restrictLayer)) {
                                Polygon elementPolygon = element.GetPolygon(restrictLayer.Id.Side);
                                holePolygons.Add(elementPolygon);
                            }

                            // El element esta el la capa profile
                            //
                            else if (element.IsOnLayer(outlineLayer)) {
                                Polygon elementPolygon = element.GetOutlinePolygon(BoardSide.None, 250000);
                                holePolygons.Add(elementPolygon);
                            }
                        }
                    }

                    // Procesa els elements dels components
                    //
                    foreach (var part in parts) {

                        // Obte la transformacio
                        //
                        Transformation localTransformation = part.GetLocalTransformation();

                        foreach (var element in part.Elements) {

                            if ((element != region) &&
                                (element.IsOnLayer(layer) || element.IsOnLayer(restrictLayer))) {

                                // Si l'element no esta conectat a la mateixa senyal que la regio, genera un forat
                                //
                                if (GetSignal(element, part, false) != regionSignal) {
                                    int clearance = thicknessCompensation + Math.Max(regionSignal.Clearance, region.Clearance);
                                    Polygon outlinePolygon = element.GetOutlinePolygon(layer.Id.Side, clearance);
                                    outlinePolygon = outlinePolygon.Transformed(localTransformation);
                                    holePolygons.Add(outlinePolygon);
                                }

                                // En es un pad i esta conectat per tant, genera un thermal
                                //
                                else if (element is PadElement) {
                                    int signalClearance = regionSignal == null ? 0 : regionSignal.Clearance;
                                    int clearance = thicknessCompensation + Math.Max(signalClearance, region.Clearance);
                                    Polygon thermalPolygon = ((PadElement)element).GetThermalPolygon(layer.Id.Side, clearance, 200000);
                                    thermalPolygon = thermalPolygon.Transformed(localTransformation);
                                    for (int i = 0; i < thermalPolygon.Childs.Length; i++)
                                        holePolygons.Add(thermalPolygon.Childs[i]);
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

            if (outlineLayer != null) {
                int minX = Int32.MaxValue;
                int minY = Int32.MaxValue;
                int maxX = Int32.MinValue;
                int maxY = Int32.MinValue;
                foreach (Element element in GetElements(outlineLayer)) {
                    Rect r = element.GetBoundingBox(BoardSide.None);
                    if (minX > r.MinX)
                        minX = r.MinX;
                    if (minY > r.MinY)
                        minY = r.MinY;
                    if (maxX < r.MaxX)
                        maxX = r.MaxX;
                    if (maxY < r.MaxY)
                        maxY = r.MaxY;
                }

                return new Rect(minX, minY, maxX - minX, maxY - minY);
            }
            else
                return new Rect(0, 0, 100000000, 100000000);
        }

        #endregion

        /// <summary>
        /// Obte o asigna la posicio de la placa.
        /// </summary>
        /// 
        public Point Position {
            get {
                return position;
            }
            set {
                position = value;
            }
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

        /// <summary>
        /// Obte o asigna l'angle de rotacio de la placa.
        /// </summary>
        /// 
        public Angle Rotation {
            get {
                return rotation;
            }
            set {
                rotation = value;
            }
        }
    }
}
