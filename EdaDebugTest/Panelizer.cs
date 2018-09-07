namespace EdaDebugTest {

    using MikroPic.EdaTools.v1.Geometry;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using MikroPic.EdaTools.v1.Pcb.Model.BoardElements;
    using MikroPic.EdaTools.v1.Pcb.Model.PanelElements;
    using System;
    using System.Collections.Generic;

    public sealed class Panelizer {

        private readonly Board dstBoard;

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="board">La placa de desti.</param>
        /// 
        public Panelizer(Board board) {

            if (board == null)
                throw new ArgumentNullException("board");

            this.dstBoard = board;
        }

        public void Panelize(Panel panel) {

            foreach (var element in panel.Elements) {
                PlaceElement place = element as PlaceElement;
                if (place != null)
                    Panelize(place.Board, place.Position, place.Rotation);
            }
        }

        public void Panelize(Board board, Point position, Angle rotation) {

            // Afegeix les capes que no existeixin en la placa de destinacio
            //
            foreach (var layer in board.Layers) 
                if (dstBoard.GetLayer(layer.Name, false) == null)
                    CloneLayer(layer);

            // Afegeix els blocs que no existeixin en la placa de destinacio.
            //
            foreach (var block in board.Blocks) {
                if (dstBoard.GetBlock(block.Name, false) == null) {
                    CloneBlock(block, board);
                }
            }
        }

        /// <summary>
        /// Clona una capa.
        /// </summary>
        /// <param name="layer">La capa a clonar.</param>
        /// 
        private void CloneLayer(Layer layer) {

            Layer clonedLayer = new Layer(layer.Name, layer.Side, layer.Function, 
                layer.Color, layer.IsVisible);

            dstBoard.AddLayer(clonedLayer);
        }

        /// <summary>
        /// Clona un block.
        /// </summary>
        /// <param name="block">El bloc a clonar.</param>
        /// 
        private void CloneBlock(Block block, Board board) {

            // Clona els elements del bloc
            //
            List<BoardElement> clonedElements = null;
            if (block.HasElements) {
                clonedElements = new List<BoardElement>();
                foreach (var element in block.Elements) {

                    BoardElement clonedElement = null;
                    Type elementType = element.GetType();

                    // Intenta clonar un 'LineElement'
                    //
                    if (elementType == typeof(LineElement)) {
                        LineElement line = element as LineElement;
                        clonedElement = new LineElement(
                            line.LayerSet,
                            line.StartPosition,
                            line.EndPosition,
                            line.Thickness,
                            line.LineCap);
                    }

                    // Intenta clonar un 'ArcElement'
                    //
                    if (elementType == typeof(ArcElement)) {
                        ArcElement arc = element as ArcElement;
                        clonedElement = new ArcElement(
                            arc.LayerSet,
                            arc.StartPosition,
                            arc.EndPosition,
                            arc.Thickness,
                            arc.Angle,
                            arc.LineCap);
                    }

                    // Intenta clonar un 'RectangleElement'
                    //
                    else if (elementType == typeof(RectangleElement)) {
                        RectangleElement rectangle = element as RectangleElement;
                        clonedElement = new RectangleElement(
                            rectangle.LayerSet,
                            rectangle.Position,
                            rectangle.Size,
                            rectangle.Roundness,
                            rectangle.Rotation,
                            rectangle.Thickness,
                            rectangle.Filled);
                    }

                    // Intenta clonar un 'CircleElement'
                    //
                    else if (elementType == typeof(CircleElement)) {
                        CircleElement circle = element as CircleElement;
                        clonedElement = new CircleElement(
                            circle.LayerSet,
                            circle.Position,
                            circle.Radius,
                            circle.Thickness,
                            circle.Filled);
                    }

                    // Intenta clonar un 'SmdPadElement'
                    //
                    else if (elementType == typeof(SmdPadElement)) {
                        SmdPadElement pad = element as SmdPadElement;
                        clonedElement = new SmdPadElement(
                            pad.Name,
                            pad.LayerSet,
                            pad.Position, 
                            pad.Size,
                            pad.Rotation,
                            pad.Roundness);
                    }

                    // Intenta clonar un 'ThPadElement'
                    //
                    else if (elementType == typeof(ThPadElement)) {
                        ThPadElement pad = element as ThPadElement;
                        clonedElement = new ThPadElement(
                            pad.Name,
                            pad.LayerSet,
                            pad.Position,
                            pad.Rotation,
                            pad.TopSize,
                            pad.InnerSize,
                            pad.BottomSize,
                            pad.Shape,
                            pad.Drill);
                    }

                    // Intenta clonar un 'HoleElement'
                    //
                    else if (elementType == typeof(HoleElement)) {
                        HoleElement hole = element as HoleElement;
                        clonedElement = new HoleElement(
                            hole.LayerSet,
                            hole.Position,
                            hole.Drill);
                    }

                    // Intenta clonar un 'RegionElement'
                    //
                    else if (elementType == typeof(RegionElement)) {
                        RegionElement region = element as RegionElement;
                        clonedElement = new RegionElement(
                            region.LayerSet,
                            region.Thickness,
                            region.Filled, 
                            region.Clearance);
                        foreach (var segment in region.Segments)
                            (clonedElement as RegionElement).Add(new RegionElement.Segment(segment.Position, segment.Angle));
                    }

                    // Intenta clonar un 'TextElement'
                    //
                    else if (elementType == typeof(TextElement)) {
                        TextElement text = element as TextElement;
                        clonedElement = new TextElement(
                            text.LayerSet,
                            text.Position,
                            text.Rotation,
                            text.Height,
                            text.Thickness,
                            text.HorizontalAlign,
                            text.VerticalAlign);
                    }

                    if (clonedElement == null)
                        throw new InvalidOperationException("No se ha posido clonar el elemento.");

                    // Situa l'element clonat en la capa corresponent
                    //
                    /*foreach (var layer in board.GetLayers(element)) {
                        Layer dstLayer = dstBoard.GetLayer(layer.Name);
                        dstBoard.Place(dstLayer, clonedElement);
                    }*/

                    clonedElements.Add(clonedElement);
                }
            }

            // Clona els atributs del bloc
            //
            List<BlockAttribute> clonedAttributes = null;
            if (block.HasAttributes) {
                foreach (var attribute in block.Attributes) {
                    BlockAttribute clonedAttribute = new BlockAttribute(attribute.Name, attribute.Value);
                    clonedAttributes.Add(clonedAttribute);
                }
            }

            dstBoard.AddBlock(new Block(block.Name, clonedElements, clonedAttributes));
        }

    }
}
