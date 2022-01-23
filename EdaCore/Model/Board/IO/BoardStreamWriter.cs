using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
using MikroPic.EdaTools.v1.Base.Xml;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;
using MikroPic.EdaTools.v1.Core.Model.IO;
using System;
using System.IO;
using System.Xml;

namespace MikroPic.EdaTools.v1.Core.Model.Board.IO {

    /// <summary>
    /// Clase per la escriptura de plaques en un stream.
    /// </summary>
    /// 
    public sealed class BoardStreamWriter {

        private const int _version = 213;
        private const string _distanceUnits = "mm";
        private const string _angleUnits = "deg";

        private readonly Stream _stream;

        private class Visitor: EdaDefaultBoardVisitor {

            private readonly XmlWriter _writer;
            private EdaBoard _currentBoard;
            private EdaPart _currentPart;

            /// <summary>
            /// Constructor del objecte. Visita els objectes d'una placa,
            /// per generar el stream de sortida.
            /// </summary>
            /// <param name="writer">Objecte per escriure el stream de sortida.</param>
            /// 
            public Visitor(XmlWriter writer) {

                _writer = writer;
            }

            /// <summary>
            /// Visita un element de tipus 'LineElement'
            /// </summary>
            /// <param name="line">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaLineElement line) {

                _writer.WriteStartElement("line");

                _writer.WriteAttributeString("layer", EdaFormatter.FormatLayerSet(line.LayerSet));
                _writer.WriteAttributeString("startPosition", EdaFormatter.FormatPoint(line.StartPosition));
                _writer.WriteAttributeString("endPosition", EdaFormatter.FormatPoint(line.EndPosition));
                if (line.Thickness > 0)
                    _writer.WriteAttributeString("thickness", EdaFormatter.FormatScalar(line.Thickness));
                if (line.LineCap != EdaLineElement.CapStyle.Round)
                    _writer.WriteAttributeEnum("lineCap", line.LineCap);

                if (_currentBoard != null) {
                    EdaSignal signal = _currentBoard.GetSignal(line, _currentPart, false);
                    if (signal != null)
                        _writer.WriteAttributeString("signal", signal.Name);
                }

                _writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'ArcElement'.
            /// </summary>
            /// <param name="arc">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaArcElement arc) {

                _writer.WriteStartElement("arc");

                _writer.WriteAttributeString("layer", EdaFormatter.FormatLayerSet(arc.LayerSet));
                _writer.WriteAttributeString("startPosition", EdaFormatter.FormatPoint(arc.StartPosition));
                _writer.WriteAttributeString("endPosition", EdaFormatter.FormatPoint(arc.EndPosition));
                _writer.WriteAttributeString("angle", EdaFormatter.FormatAngle(arc.Angle));
                if (arc.Thickness > 0)
                    _writer.WriteAttributeString("thickness", EdaFormatter.FormatScalar(arc.Thickness));
                if (arc.LineCap != EdaLineElement.CapStyle.Round)
                    _writer.WriteAttributeEnum("lineCap", arc.LineCap);

                if (_currentBoard != null) {
                    EdaSignal signal = _currentBoard.GetSignal(arc, _currentPart, false);
                    if (signal != null)
                        _writer.WriteAttributeString("signal", signal.Name);
                }

                _writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'RectangleElement'.
            /// </summary>
            /// <param name="rectangle">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaRectangleElement rectangle) {

                _writer.WriteStartElement("rectangle");

                _writer.WriteAttributeString("layer", EdaFormatter.FormatLayerSet(rectangle.LayerSet));
                _writer.WriteAttributeString("position", EdaFormatter.FormatPoint(rectangle.Position));
                _writer.WriteAttributeString("size", EdaFormatter.FormatSize(rectangle.Size));
                if (!rectangle.Rotation.IsZero)
                    _writer.WriteAttributeString("rotation", EdaFormatter.FormatAngle(rectangle.Rotation));
                if (rectangle.Thickness > 0) 
                    _writer.WriteAttributeString("thickness", EdaFormatter.FormatScalar(rectangle.Thickness));

                _writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'CircleElement'
            /// </summary>
            /// <param name="circle">L'element a visitar.</param>
            /// 
            public override void Visit(EdaCircleElement circle) {

                _writer.WriteStartElement("circle");

                _writer.WriteAttributeString("layer", EdaFormatter.FormatLayerSet(circle.LayerSet));
                _writer.WriteAttributeString("position", EdaFormatter.FormatPoint(circle.Position));
                _writer.WriteAttributeString("radius", EdaFormatter.FormatScalar(circle.Radius));
                if (circle.Thickness > 0) 
                    _writer.WriteAttributeString("thickness", EdaFormatter.FormatScalar(circle.Thickness));

                _writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'TextElement'
            /// </summary>
            /// <param name="text">L'element a visitar</param>
            /// 
            public override void Visit(EdaTextElement text) {

                _writer.WriteStartElement("text");

                _writer.WriteAttributeString("layer", EdaFormatter.FormatLayerSet(text.LayerSet));
                _writer.WriteAttributeString("position", EdaFormatter.FormatPoint(text.Position));
                if (!text.Rotation.IsZero)
                    _writer.WriteAttributeString("rotation", EdaFormatter.FormatAngle(text.Rotation));
                _writer.WriteAttributeString("height", EdaFormatter.FormatScalar(text.Height));
                _writer.WriteAttributeString("thickness", EdaFormatter.FormatScalar(text.Thickness));
                if (text.HorizontalAlign != HorizontalTextAlign.Left)
                    _writer.WriteAttributeEnum("horizontalAlign", text.HorizontalAlign);
                if (text.VerticalAlign != VerticalTextAlign.Bottom)
                    _writer.WriteAttributeEnum("verticalAlign", text.VerticalAlign);
                if (!String.IsNullOrEmpty(text.Value))
                    _writer.WriteAttributeString("value", text.Value);

                _writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'SmdPadElement'
            /// </summary>
            /// <param name="pad">L'element a visitar-</param>
            /// 
            public override void Visit(EdaSmdPadElement pad) {

                _writer.WriteStartElement("spad");

                _writer.WriteAttributeString("name", pad.Name);
                _writer.WriteAttributeString("layers", EdaFormatter.FormatLayerSet(pad.LayerSet));
                _writer.WriteAttributeString("position", EdaFormatter.FormatPoint(pad.Position));
                if (!pad.Rotation.IsZero)
                    _writer.WriteAttributeString("rotation", EdaFormatter.FormatAngle(pad.Rotation));
                _writer.WriteAttributeString("size", EdaFormatter.FormatSize(pad.Size));
                if (!pad.CornerRatio.IsZero) {
                    _writer.WriteAttributeString("cornerRatio", EdaFormatter.FormatRatio(pad.CornerRatio));
                    _writer.WriteAttributeEnum("cornerShape", pad.CornerShape);
                }

                if (_currentBoard != null) {
                    EdaSignal signal = _currentBoard.GetSignal(pad, _currentPart, false);
                    if (signal != null)
                        _writer.WriteAttributeString("signal", signal.Name);
                }

                _writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un element de tipus 'ThPadElement'
            /// </summary>
            /// <param name="pad">L'element a visitar.</param>
            /// 
            public override void Visit(EdaThPadElement pad) {

                _writer.WriteStartElement("tpad");

                _writer.WriteAttributeString("name", pad.Name);
                _writer.WriteAttributeString("layers", EdaFormatter.FormatLayerSet(pad.LayerSet));
                _writer.WriteAttributeString("position", EdaFormatter.FormatPoint(pad.Position));
                if (!pad.Rotation.IsZero)
                    _writer.WriteAttributeString("rotation", EdaFormatter.FormatAngle(pad.Rotation));
                if (!pad.CornerRatio.IsZero) {
                    _writer.WriteAttributeString("cornerRatio", EdaFormatter.FormatRatio(pad.CornerRatio));
                    _writer.WriteAttributeEnum("cornerShape", pad.CornerShape);
                }
                _writer.WriteAttributeString("topSize", EdaFormatter.FormatSize(pad.TopSize));
                if (pad.InnerSize != pad.TopSize)
                    _writer.WriteAttributeString("innerSize", EdaFormatter.FormatSize(pad.InnerSize));
                if (pad.BottomSize != pad.TopSize)
                    _writer.WriteAttributeString("bottomSize", EdaFormatter.FormatSize(pad.BottomSize));
                _writer.WriteAttributeString("drill", EdaFormatter.FormatScalar(pad.Drill));

                if (_currentBoard != null) {
                    EdaSignal signal = _currentBoard.GetSignal(pad, _currentPart, false);
                    if (signal != null)
                        _writer.WriteAttributeString("signal", signal.Name);
                }

                _writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte de tipus 'PartAttribute'
            /// </summary>
            /// <param name="attr">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaPartAttribute attr) {

                _writer.WriteStartElement("attribute");

                _writer.WriteAttributeString("name", attr.Name);
                _writer.WriteAttributeString("value", attr.Value);
                if (!attr.IsVisible)
                    _writer.WriteAttributeBool("visible", attr.IsVisible);
                if (attr.UsePosition)
                    _writer.WriteAttributeString("position", EdaFormatter.FormatPoint(attr.Position));
                if (attr.UseRotation)
                    _writer.WriteAttributeString("rotation", EdaFormatter.FormatAngle(attr.Rotation));
                if (attr.UseHeight)
                    _writer.WriteAttributeString("height", EdaFormatter.FormatScalar(attr.Height));
                if (attr.UseAlign) {
                    if (attr.HorizontalAlign != HorizontalTextAlign.Left)
                        _writer.WriteAttributeEnum("horizontalAlign", attr.HorizontalAlign);
                    if (attr.VerticalAlign != VerticalTextAlign.Bottom)
                        _writer.WriteAttributeEnum("verticalAlign", attr.VerticalAlign);
                }

                _writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte de tipus 'BlockAttribute'
            /// </summary>
            /// <param name="attr">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaComponentAttribute attr) {

                _writer.WriteStartElement("attribute");

                _writer.WriteAttributeString("name", attr.Name);
                _writer.WriteAttributeString("value", attr.Value);

                _writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte de tipus 'RegionElement'
            /// </summary>
            /// <param name="region">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaRegionElement region) {

                _writer.WriteStartElement("region");

                _writer.WriteAttributeString("layer", EdaFormatter.FormatLayerSet(region.LayerSet));
                if (region.Thickness > 0) 
                    _writer.WriteAttributeString("thickness", EdaFormatter.FormatScalar(region.Thickness));
                if (region.Clearance > 0)
                    _writer.WriteAttributeString("clearance", EdaFormatter.FormatScalar(region.Clearance));

                if (_currentBoard != null) {
                    EdaSignal signal = _currentBoard.GetSignal(region, _currentPart, false);
                    if (signal != null)
                        _writer.WriteAttributeString("signal", signal.Name);
                }

                if (region.Segments != null)
                    foreach (var segment in region.Segments) {
                        _writer.WriteStartElement("segment");
                        _writer.WriteAttributeString("position", EdaFormatter.FormatPoint(segment.Position));
                        if (!segment.Arc.IsZero)
                            _writer.WriteAttributeString("angle", EdaFormatter.FormatAngle(segment.Arc));
                        _writer.WriteEndElement();
                    }

                _writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte de tipus 'Part'
            /// </summary>
            /// <param name="part">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaPart part) {

                _currentPart = part;
                try {
                    _writer.WriteStartElement("part");

                    // Escriu els parametres
                    //
                    _writer.WriteAttributeString("name", part.Name);
                    _writer.WriteAttributeString("component", part.Component.Name);
                    _writer.WriteAttributeString("position", EdaFormatter.FormatPoint(part.Position));
                    if (!part.Rotation.IsZero)
                        _writer.WriteAttributeString("rotation", EdaFormatter.FormatAngle(part.Rotation));
                    if (part.Side != PartSide.Top)
                        _writer.WriteAttributeEnum("side", part.Side);

                    // Escriu la llista de pads que tenen conexio.
                    //
                    if (part.HasPads) {
                        bool empty = true;
                        foreach (EdaPadElement pad in part.Pads) {
                            EdaSignal signal = _currentBoard.GetSignal(pad, part, false);
                            if (signal != null) {
                                if (empty) {
                                    empty = false;
                                    _writer.WriteStartElement("pads");
                                }
                                _writer.WriteStartElement("pad");
                                _writer.WriteAttributeString("name", pad.Name);
                                _writer.WriteAttributeString("signal", signal.Name);
                                _writer.WriteEndElement();
                            }
                        }
                        if (!empty)
                            _writer.WriteEndElement();
                    }

                    // Escriu la llista d'atributs.
                    //
                    if (part.HasAttributes) {
                        _writer.WriteStartElement("attributes");
                        foreach (var attribute in part.Attributes)
                            attribute.AcceptVisitor(this);
                        _writer.WriteEndElement();
                    }

                    _writer.WriteEndElement();
                }
                finally {
                    _currentPart = null;
                }
            }

            /// <summary>
            /// Visita un objecte de tipus 'ViaElement'.
            /// </summary>
            /// <param name="via">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaViaElement via) {

                _writer.WriteStartElement("via");

                _writer.WriteAttributeString("layers", EdaFormatter.FormatLayerSet(via.LayerSet));
                _writer.WriteAttributeString("position", EdaFormatter.FormatPoint(via.Position));
                _writer.WriteAttributeString("drill", EdaFormatter.FormatScalar(via.Drill));
                _writer.WriteAttributeString("outerSize", EdaFormatter.FormatScalar(via.OuterSize));
                if (via.InnerSize != via.OuterSize)
                    _writer.WriteAttributeString("innerSize", EdaFormatter.FormatScalar(via.InnerSize));
                if (via.Shape != EdaViaElement.ViaShape.Circle)
                    _writer.WriteAttributeEnum("shape", via.Shape);

                if (_currentBoard != null) {
                    EdaSignal signal = _currentBoard.GetSignal(via, null, false);
                    if (signal != null)
                        _writer.WriteAttributeString("signal", signal.Name);
                }

                _writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte de tipus 'Layer'.
            /// </summary>
            /// <param name="layer">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaLayer layer) {

                _writer.WriteStartElement("layer");

                _writer.WriteAttributeString("id", layer.Id.ToString());
                _writer.WriteAttributeEnum("side", layer.Side);
                _writer.WriteAttributeEnum("function", layer.Function);

                _writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte de tipous 'Signal'.
            /// </summary>
            /// <param name="signal">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaSignal signal) {

                _writer.WriteStartElement("signal");

                _writer.WriteAttributeString("name", signal.Name);
                if (signal.Clearance > 0)
                    _writer.WriteAttributeString("clearance", EdaFormatter.FormatScalar(signal.Clearance));

                _writer.WriteEndElement();
            }

            /// <summary>
            /// Visita un objecte de tipus 'Component'
            /// </summary>
            /// <param name="component">E'objecte a visitar.</param>
            /// 
            public override void Visit(EdaComponent component) {

                _writer.WriteStartElement("component");

                _writer.WriteAttributeString("name", component.Name);
                if (!String.IsNullOrEmpty(component.Description))
                    _writer.WriteAttributeString("description", component.Description);

                if (component.HasElements) {
                    _writer.WriteStartElement("elements");
                    foreach (var element in component.Elements)
                        element.AcceptVisitor(this);
                    _writer.WriteEndElement();
                }

                if (component.HasAttributes) {
                    _writer.WriteStartElement("attributes");
                    foreach (var attribute in component.Attributes)
                        attribute.AcceptVisitor(this);
                    _writer.WriteEndElement();
                }

                _writer.WriteEndElement();
            }

            /// <summary>
            /// Visita una placa.
            /// </summary>
            /// <param name="board">La placa a visitar.</param>
            /// 
            public override void Visit(EdaBoard board) {

                _currentBoard = board;

                _writer.WriteStartElement("board");
                _writer.WriteAttributeString("position", EdaFormatter.FormatPoint(board.Position));
                _writer.WriteAttributeString("rotation", EdaFormatter.FormatAngle(board.Rotation));

                _writer.WriteStartElement("layers");
                foreach (var layer in board.Layers)
                    layer.AcceptVisitor(this);
                _writer.WriteEndElement();

                if (board.HasSignals) {
                    _writer.WriteStartElement("signals");
                    foreach (var signal in board.Signals)
                        signal.AcceptVisitor(this);
                    _writer.WriteEndElement();
                }

                if (board.HasComponents) {
                    _writer.WriteStartElement("components");
                    foreach (var component in board.Components)
                        component.AcceptVisitor(this);
                    _writer.WriteEndElement();
                }

                if (board.HasParts) {
                    _writer.WriteStartElement("parts");
                    foreach (var part in board.Parts)
                        part.AcceptVisitor(this);
                    _writer.WriteEndElement();
                }

                if (board.HasElements) {
                    _writer.WriteStartElement("elements");
                    foreach (var element in board.Elements)
                        element.AcceptVisitor(this);
                    _writer.WriteEndElement();
                }

                _writer.WriteEndElement();
            }

            /// <summary>
            /// Visita una llibreria
            /// </summary>
            /// <param name="library">La llibreria.</param>
            /// 
            public override void Visit(EdaLibrary library) {

                _writer.WriteStartElement("library");

                _writer.WriteAttributeString("name", library.Name);
                if (!String.IsNullOrEmpty(library.Description))
                    _writer.WriteAttributeString("description", library.Description);

                if (library.HasComponents) {
                    _writer.WriteStartElement("components");
                    foreach (var component in library.Components)
                        component.AcceptVisitor(this);
                    _writer.WriteEndElement();
                }

                _writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Constructor del objecte.
        /// </summary>
        /// <param name="stream">Stream de sortida.</param>
        /// 
        public BoardStreamWriter(Stream stream) {

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanWrite)
                throw new InvalidOperationException("El stream no es de escritura.");

            this._stream = stream;
        }

        /// <summary>
        /// Escriu la placa en el stream de sortida.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        public void Write(EdaBoard board) {

            if (board == null)
                throw new ArgumentNullException(nameof(board));

            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";
            settings.CloseOutput = true;

            using (var writer = XmlWriter.Create(_stream, settings)) {

                writer.WriteStartDocument();

                writer.WriteStartElement("document", "http://MikroPic.com/schemas/edatools/v1/BoardDocument.xsd");
                writer.WriteAttributeInteger("version", _version);
                writer.WriteAttributeString("documentType", "board");
                writer.WriteAttributeString("distanceUnits", _distanceUnits);
                writer.WriteAttributeString("angleUnits", _angleUnits);

                IEdaBoardVisitor visitor = new Visitor(writer);
                board.AcceptVisitor(visitor);

                writer.WriteEndElement();

                writer.WriteEndDocument();
            }
        }

        /// <summary>
        /// Escriu la placa en el stream de sortida.
        /// </summary>
        /// <param name="library">La placa.</param>
        /// 
        public void Write(EdaLibrary library) {

            if (library == null)
                throw new ArgumentNullException(nameof(library));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";
            settings.CloseOutput = true;

            using (XmlWriter writer = XmlWriter.Create(_stream, settings)) {

                writer.WriteStartDocument();

                writer.WriteStartElement("document", "http://MikroPic.com/schemas/edatools/v1/BoardDocument.xsd");
                writer.WriteAttributeInteger("version", _version);
                writer.WriteAttributeString("documentType", "componentLibrary");
                writer.WriteAttributeString("distanceUnits", _distanceUnits);
                writer.WriteAttributeString("angleUnits", _angleUnits);

                IEdaBoardVisitor visitor = new Visitor(writer);
                library.AcceptVisitor(visitor);

                writer.WriteEndElement();

                writer.WriteEndDocument();
            }
        }
    }
}
