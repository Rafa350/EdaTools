namespace MikroPic.EdaTools.v1.Cam.Gerber.Infrastructure {

    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Geometry.Polygons;

    internal static class PolygonDictionaryBuilder {

        /// <summary>
        /// Clase utilitzada per inicialitzar el diccionari de poligons
        /// </summary>
        /// 
        private sealed class PolygonBuilderVisitor : BoardVisitor {

            private readonly IList<Layer> layers;
            private readonly IDictionary<Polygon, Signal> polygonDict;
            private Part currentPart = null;

            /// <summary>
            /// Constructor de la clase.
            /// </summary>
            /// <param name="layers">Llista de capes.</param>
            /// <param name="polygonDict">El diccionari a inicialitzar.</param>
            /// 
            public PolygonBuilderVisitor(IList<Layer> layers, IDictionary<Polygon, Signal> polygonDict) {

                this.layers = layers;
                this.polygonDict = polygonDict;
            }

            /// <summary>
            /// Visita la placa.
            /// </summary>
            /// <param name="board">La placa a visitar.</param>
            /// 
            public override void Visit(Board board) {

                foreach (Signal signal in board.Signals)
                    signal.AcceptVisitor(this);
            }

            /// <summary>
            /// Visita un terminal.
            /// </summary>
            /// <param name="terminal">El terminal a visitar.</param>
            /// 
            public override void Visit(Terminal terminal) {

                Component component = terminal.Part.Component;
                currentPart = terminal.Part;
                foreach (Element element in component.Elements)
                    element.AcceptVisitor(this);
            }

            /// <summary>
            /// Visita una via.
            /// </summary>
            /// <param name="via">La via a visitar</param>
            /// 
            public override void Visit(ViaElement via) {

                if (via.IsOnAnyLayer(layers)) {
                    Polygon polygon = Polygon.FromElement(via, VisitingPart, 0);
                    Add(VisitingSignal, polygon);
                }
            }

            /// <summary>
            /// Visita un pad throug hole
            /// </summary>
            /// <param name="pad">El pad a visitar.</param>
            /// 
            public override void Visit(ThPadElement pad) {

                if (pad.IsOnAnyLayer(layers)) {
                    Polygon polygon = Polygon.FromElement(pad, currentPart, 0);
                    Add(VisitingSignal, polygon);
                }
            }

            /// <summary>
            /// Visita un pad smd
            /// </summary>
            /// <param name="pad">El pad a visitar.</param>
            /// 
            public override void Visit(SmdPadElement pad) {

                if (pad.IsOnAnyLayer(layers)) {
                    Polygon polygon = Polygon.FromElement(pad, currentPart, 0);
                    Add(VisitingSignal, polygon);
                }
            }

            /// <summary>
            /// Afegeig un element al diccionari.
            /// </summary>
            /// <param name="signal">La senyal.</param>
            /// <param name="polygon">El poligon.</param>
            /// 
            private void Add(Signal signal, Polygon polygon) {

                if (polygon.NumPoints > 2)
                    polygonDict.Add(polygon, signal);
            }
        }

        public static IDictionary<Polygon, Signal> Build(Board board, IList<Layer> layers) {

            Dictionary<Polygon, Signal> polygonDict = new Dictionary<Polygon, Signal>();
            PolygonBuilderVisitor visitor = new PolygonBuilderVisitor(layers, polygonDict);
            board.AcceptVisitor(visitor);

            return polygonDict;
        }
    }
}
