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

            /// <summary>
            /// Visita un objecte 'Signal'.
            /// </summary>
            /// <param name="signal">La senyal a visitar.</param>
            public override void Visit(Signal signal) {

                if (signal.Elements != null)
                    foreach (Element element in signal.Elements) {
                        if (element.IsOnLayer(layer)) {
                            RenderItem renderItem = new RenderItem {
                                Part = null,
                                Element = element
                            };
                            renderList.Add(renderItem);
                        }
                    }
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
            ProcessLayer(board, LayerId.BottomNames, visualList);
            ProcessLayer(board, LayerId.BottomValues, visualList);
            ProcessLayer(board, LayerId.BottomDocument, visualList);
            ProcessLayer(board, LayerId.BottomCream, visualList);
            ProcessLayer(board, LayerId.BottomGlue, visualList);
            ProcessLayer(board, LayerId.BottomKeepout, visualList);
            ProcessLayer(board, LayerId.BottomRestrict, visualList);
            ProcessLayer(board, LayerId.BottomPlace, visualList);
            ProcessLayer(board, LayerId.Bottom, visualList);
            ProcessLayer(board, LayerId.ViaRestrict, visualList);
            ProcessLayer(board, LayerId.Top, visualList);
            ProcessLayer(board, LayerId.Holes, visualList);
            ProcessLayer(board, LayerId.TopPlace, visualList);
            ProcessLayer(board, LayerId.TopRestrict, visualList);
            ProcessLayer(board, LayerId.TopKeepout, visualList);
            ProcessLayer(board, LayerId.TopGlue, visualList);
            ProcessLayer(board, LayerId.TopCream, visualList);
            ProcessLayer(board, LayerId.TopDocument, visualList);
            ProcessLayer(board, LayerId.TopValues, visualList);
            ProcessLayer(board, LayerId.TopNames, visualList);
            ProcessLayer(board, LayerId.Vias, visualList);
            ProcessLayer(board, LayerId.Pads, visualList);

            return visualList;
        }

        private void ProcessLayer(Board board, LayerId layerId, IList<Visual> visualList) {

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
