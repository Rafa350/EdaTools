namespace Eda.PCBViewer.DrawEditor {

    using System;
    using System.Collections.Generic;
    using System.Windows.Media;
    using Eda.PCBViewer.DrawEditor.Visuals;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.Elements;
    using MikroPic.EdaTools.v1.Pcb.Model.Visitors;

    public sealed class VisualGenerator {

        private class RenderItem {
            public Part Part { get; set; }
            public Element Element { get; set; }
        }

        /// <summary>
        /// Visitador per les senyals d'una placa.
        /// </summary>
        private sealed class PopulateRenderListSignalsVisitor: BoardVisitor {

            private readonly Layer layer;
            private readonly IList<RenderItem> renderList;

            /// <summary>
            /// Constructor e la clase.
            /// </summary>
            /// <param name="layer">La capa a visitar.</param>
            /// <param name="renderList">La llista on deixar el resultat.</param>
            public PopulateRenderListSignalsVisitor(Layer layer, IList<RenderItem> renderList) {

                this.layer = layer;
                this.renderList = renderList;
            }

            /// <summary>
            /// Visita un objecte 'Board'.
            /// </summary>
            /// <param name="board">La placa a visitar.</param>
            public override void Visit(Board board) {

                foreach (Signal signal in board.Signals)
                    signal.AcceptVisitor(this);
            }
        }

        private sealed class PopulateRenderListPartsVisitor: BoardVisitor {

            private readonly Layer layer;
            private readonly IList<RenderItem> renderList;

            public PopulateRenderListPartsVisitor(Layer layer, IList<RenderItem> renderList) {

                this.layer = layer;
                this.renderList = renderList;
            }

            public override void Visit(Board board) {

                foreach (Part part in board.Parts)
                    part.AcceptVisitor(this);
            }

            public override void Visit(Part part) {

                if (part.Component != null && part.Component.Elements != null)
                    foreach (Element element in part.Component.Elements) {
                        //Layer elementLayer = part.IsMirror ? element.Layer.Mirror : element.Layer;
                        if (element.IsOnLayer(layer)) {
                            RenderItem renderItem = new RenderItem {
                                Part = part,
                                Element = element
                            };
                            renderList.Add(renderItem);
                        }
                    }
            }
        }

        private sealed class CreateVisualsVisitor: BoardVisitor {

            private readonly IList<RenderItem> renderList;
            private readonly IList<Visual> visualList;
            private Part currentPart;

            public CreateVisualsVisitor(IList<RenderItem> renderList, IList<Visual> visualList) {

                this.renderList = renderList;
                this.visualList = visualList;
            }

            public override void Visit(Board board) {

                foreach (RenderItem renderItem in renderList) {
                    currentPart = renderItem.Part;
                    renderItem.Element.AcceptVisitor(this);
                }
            }

            /// <summary>
            /// Visita una linia de component.
            /// </summary>
            /// <param name="line">La linia a visitar.</param>
            public override void Visit(LineElement line) {

                visualList.Add(new LineVisual(line, currentPart));
            }

            /// <summary>
            /// Visita un arc de component.
            /// </summary>
            /// <param name="arc">L'arc a visitar.</param>
            public override void Visit(ArcElement arc) {

                visualList.Add(new ArcVisual(arc, currentPart));
            }

            public override void Visit(RectangleElement rectangle) {

                visualList.Add(new RectangleVisual(rectangle, currentPart));
            }

            public override void Visit(CircleElement circle) {

                visualList.Add(new CircleVisual(circle, currentPart));
            }

            public override void Visit(RegionElement polygon) {

                visualList.Add(new PolygonVisual(polygon, currentPart));
            }

            public override void Visit(TextElement text) {

                visualList.Add(new TextVisual(text, currentPart));
            }

            public override void Visit(SmdPadElement pad) {

                visualList.Add(new SmdPadVisual(pad, currentPart));
            }

            public override void Visit(ThPadElement pad) {

                visualList.Add(new ThPadVisual(pad, currentPart));
            }

            public override void Visit(ViaElement via) {

                visualList.Add(new ViaVisual(via));
            }

            public override void Visit(HoleElement hole) {

                visualList.Add(new HoleVisual(hole, currentPart));
            }
        }

        private readonly Board board;


        /// <summary>
        /// Contructor. 
        /// </summary>
        /// <param name="board">La placa a procesar.</param>
        public VisualGenerator(Board board) {

            if (board == null)
                throw new ArgumentNullException("board");

            this.board = board;
        }

        /// <summary>
        /// Genera totes les visuals per representasr la placa.
        /// </summary>
        /// <returns>Un enumerador de les visuals creades.</returns>
        public IEnumerable<Visual> CreateVisuals() {

            List<Visual> visualList = new List<Visual>();

            // Procesa la placa, capa a capa
            //
            ProcessLayer(board, LayerIdentifier.BottomNames, visualList);
            ProcessLayer(board, LayerIdentifier.BottomValues, visualList);
            ProcessLayer(board, LayerIdentifier.BottomDocument, visualList);
            ProcessLayer(board, LayerIdentifier.BottomCream, visualList);
            ProcessLayer(board, LayerIdentifier.BottomGlue, visualList);
            ProcessLayer(board, LayerIdentifier.BottomKeepout, visualList);
            ProcessLayer(board, LayerIdentifier.BottomRestrict, visualList);
            ProcessLayer(board, LayerIdentifier.BottomPlace, visualList);
            ProcessLayer(board, LayerIdentifier.Bottom, visualList);
            ProcessLayer(board, LayerIdentifier.ViaRestrict, visualList);
            ProcessLayer(board, LayerIdentifier.Top, visualList);
            ProcessLayer(board, LayerIdentifier.Holes, visualList);
            ProcessLayer(board, LayerIdentifier.TopPlace, visualList);
            ProcessLayer(board, LayerIdentifier.TopRestrict, visualList);
            ProcessLayer(board, LayerIdentifier.TopKeepout, visualList);
            ProcessLayer(board, LayerIdentifier.TopGlue, visualList);
            ProcessLayer(board, LayerIdentifier.TopCream, visualList);
            ProcessLayer(board, LayerIdentifier.TopDocument, visualList);
            ProcessLayer(board, LayerIdentifier.TopValues, visualList);
            ProcessLayer(board, LayerIdentifier.TopNames, visualList);
            ProcessLayer(board, LayerIdentifier.Vias, visualList);
            ProcessLayer(board, LayerIdentifier.Pads, visualList);

            return visualList;
        }

        private void ProcessLayer(Board board, LayerIdentifier layerId, IList<Visual> visualList) {

            Layer layer = board.GetLayer(layerId);
            if ((layer != null) && layer.IsVisible) {

                // Lista d'elements a renderitzar
                //
                List<RenderItem> renderList = new List<RenderItem>();

                // Obte els elements de la llista 'Signals'
                //
                PopulateRenderListSignalsVisitor populateRenderListSignalVisitor = new PopulateRenderListSignalsVisitor(layer, renderList);
                board.AcceptVisitor(populateRenderListSignalVisitor);

                // Obte els elements de la llista 'Parts'
                //
                PopulateRenderListPartsVisitor populateRenderListPartsVisitor = new PopulateRenderListPartsVisitor(layer, renderList);
                board.AcceptVisitor(populateRenderListPartsVisitor);

                // Crea les visuals
                //
                CreateVisualsVisitor createVisualsVisitor = new CreateVisualsVisitor(renderList, visualList);
                board.AcceptVisitor(createVisualsVisitor);
            }
        }
    }
}
