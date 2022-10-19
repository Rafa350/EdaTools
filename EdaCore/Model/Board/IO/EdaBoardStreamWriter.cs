using System;
using System.IO;
using System.Xml;
using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
using MikroPic.EdaTools.v1.Base.Xml;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;
using MikroPic.EdaTools.v1.Core.Model.IO;

namespace MikroPic.EdaTools.v1.Core.Model.Board.IO {

    /// <summary>
    /// Clase per la escriptura de plaques en un stream.
    /// </summary>
    /// 
    public sealed class EdaBoardStreamWriter {

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

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaLineElement element) {

                _writer.WriteStartElement("line");

                _writer.WriteAttributeString("layer", EdaFormatter.FormatLayerSet(element.LayerSet));
                _writer.WriteAttributeString("startPosition", EdaFormatter.FormatPoint(element.StartPosition));
                _writer.WriteAttributeString("endPosition", EdaFormatter.FormatPoint(element.EndPosition));
                if (element.Thickness > 0)
                    _writer.WriteAttributeString("thickness", EdaFormatter.FormatScalar(element.Thickness));
                if (element.LineCap != EdaLineElement.CapStyle.Round)
                    _writer.WriteAttributeEnum("lineCap", element.LineCap);

                if (_currentBoard != null) {
                    EdaSignal signal = _currentBoard.GetSignal(element, _currentPart, false);
                    if (signal != null)
                        _writer.WriteAttributeString("signal", signal.Name);
                }

                _writer.WriteEndElement();
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaArcElement element) {

                _writer.WriteStartElement("arc");

                _writer.WriteAttributeString("layer", EdaFormatter.FormatLayerSet(element.LayerSet));
                _writer.WriteAttributeString("startPosition", EdaFormatter.FormatPoint(element.StartPosition));
                _writer.WriteAttributeString("endPosition", EdaFormatter.FormatPoint(element.EndPosition));
                _writer.WriteAttributeString("angle", EdaFormatter.FormatAngle(element.Angle));
                if (element.Thickness > 0)
                    _writer.WriteAttributeString("thickness", EdaFormatter.FormatScalar(element.Thickness));
                if (element.LineCap != EdaLineElement.CapStyle.Round)
                    _writer.WriteAttributeEnum("lineCap", element.LineCap);

                if (_currentBoard != null) {
                    EdaSignal signal = _currentBoard.GetSignal(element, _currentPart, false);
                    if (signal != null)
                        _writer.WriteAttributeString("signal", signal.Name);
                }

                _writer.WriteEndElement();
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaRectangleElement element) {

                _writer.WriteStartElement("rectangle");

                _writer.WriteAttributeString("layer", EdaFormatter.FormatLayerSet(element.LayerSet));
                _writer.WriteAttributeString("position", EdaFormatter.FormatPoint(element.Position));
                _writer.WriteAttributeString("size", EdaFormatter.FormatSize(element.Size));
                if (!element.Rotation.IsZero)
                    _writer.WriteAttributeString("rotation", EdaFormatter.FormatAngle(element.Rotation));
                if (element.Thickness > 0)
                    _writer.WriteAttributeString("thickness", EdaFormatter.FormatScalar(element.Thickness));
                if (element.Filled)
                    _writer.WriteAttributeString("filled", "true");

                _writer.WriteEndElement();
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaCircleElement element) {

                _writer.WriteStartElement("circle");

                _writer.WriteAttributeString("layer", EdaFormatter.FormatLayerSet(element.LayerSet));
                _writer.WriteAttributeString("position", EdaFormatter.FormatPoint(element.Position));
                _writer.WriteAttributeString("radius", EdaFormatter.FormatScalar(element.Radius));
                if (element.Thickness > 0)
                    _writer.WriteAttributeString("thickness", EdaFormatter.FormatScalar(element.Thickness));
                if (element.Filled)
                    _writer.WriteAttributeString("filled", "true");

                _writer.WriteEndElement();
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaTextElement element) {

                _writer.WriteStartElement("text");

                _writer.WriteAttributeString("layer", EdaFormatter.FormatLayerSet(element.LayerSet));
                _writer.WriteAttributeString("position", EdaFormatter.FormatPoint(element.Position));
                if (!element.Rotation.IsZero)
                    _writer.WriteAttributeString("rotation", EdaFormatter.FormatAngle(element.Rotation));
                _writer.WriteAttributeString("height", EdaFormatter.FormatScalar(element.Height));
                _writer.WriteAttributeString("thickness", EdaFormatter.FormatScalar(element.Thickness));
                if (element.HorizontalAlign != HorizontalTextAlign.Left)
                    _writer.WriteAttributeEnum("horizontalAlign", element.HorizontalAlign);
                if (element.VerticalAlign != VerticalTextAlign.Bottom)
                    _writer.WriteAttributeEnum("verticalAlign", element.VerticalAlign);
                if (!String.IsNullOrEmpty(element.Value))
                    _writer.WriteAttributeString("value", element.Value);

                _writer.WriteEndElement();
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaSmtPadElement element) {

                _writer.WriteStartElement("spad");

                _writer.WriteAttributeString("name", element.Name);
                _writer.WriteAttributeString("layers", EdaFormatter.FormatLayerSet(element.LayerSet));
                _writer.WriteAttributeString("position", EdaFormatter.FormatPoint(element.Position));
                if (!element.Rotation.IsZero)
                    _writer.WriteAttributeString("rotation", EdaFormatter.FormatAngle(element.Rotation));
                _writer.WriteAttributeString("size", EdaFormatter.FormatSize(element.Size));
                if (!element.CornerRatio.IsZero) {
                    _writer.WriteAttributeString("cornerRatio", EdaFormatter.FormatRatio(element.CornerRatio));
                    _writer.WriteAttributeEnum("cornerShape", element.CornerShape);
                }
                if (element.Clearance > 0)
                    _writer.WriteAttributeString("clearance", EdaFormatter.FormatScalar(element.Clearance));

                if (_currentBoard != null) {
                    EdaSignal signal = _currentBoard.GetSignal(element, _currentPart, false);
                    if (signal != null)
                        _writer.WriteAttributeString("signal", signal.Name);
                }

                _writer.WriteEndElement();
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaThtPadElement element) {

                _writer.WriteStartElement("tpad");

                _writer.WriteAttributeString("name", element.Name);
                _writer.WriteAttributeString("layers", EdaFormatter.FormatLayerSet(element.LayerSet));
                _writer.WriteAttributeString("position", EdaFormatter.FormatPoint(element.Position));
                if (!element.Rotation.IsZero)
                    _writer.WriteAttributeString("rotation", EdaFormatter.FormatAngle(element.Rotation));
                if (!element.CornerRatio.IsZero) {
                    _writer.WriteAttributeString("cornerRatio", EdaFormatter.FormatRatio(element.CornerRatio));
                    _writer.WriteAttributeEnum("cornerShape", element.CornerShape);
                }
                if (element.Clearance > 0)
                    _writer.WriteAttributeString("clearance", EdaFormatter.FormatScalar(element.Clearance));
                _writer.WriteAttributeString("topSize", EdaFormatter.FormatSize(element.TopSize));
                if (element.InnerSize != element.TopSize)
                    _writer.WriteAttributeString("innerSize", EdaFormatter.FormatSize(element.InnerSize));
                if (element.BottomSize != element.TopSize)
                    _writer.WriteAttributeString("bottomSize", EdaFormatter.FormatSize(element.BottomSize));
                _writer.WriteAttributeString("drill", EdaFormatter.FormatScalar(element.DrillDiameter));

                if (_currentBoard != null) {
                    EdaSignal signal = _currentBoard.GetSignal(element, _currentPart, false);
                    if (signal != null)
                        _writer.WriteAttributeString("signal", signal.Name);
                }

                _writer.WriteEndElement();
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaViaElement element) {

                _writer.WriteStartElement("via");

                _writer.WriteAttributeString("layers", EdaFormatter.FormatLayerSet(element.LayerSet));
                _writer.WriteAttributeString("position", EdaFormatter.FormatPoint(element.Position));
                _writer.WriteAttributeString("drill", EdaFormatter.FormatScalar(element.DrillDiameter));
                _writer.WriteAttributeString("outerSize", EdaFormatter.FormatScalar(element.OuterSize));
                if (element.InnerSize != element.OuterSize)
                    _writer.WriteAttributeString("innerSize", EdaFormatter.FormatScalar(element.InnerSize));

                if (_currentBoard != null) {
                    EdaSignal signal = _currentBoard.GetSignal(element, null, false);
                    if (signal != null)
                        _writer.WriteAttributeString("signal", signal.Name);
                }

                _writer.WriteEndElement();
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaCircularHoleElement element) {

                _writer.WriteStartElement("circleHole");

                _writer.WriteAttributeString("position", EdaFormatter.FormatPoint(element.Position));
                _writer.WriteAttributeString("diameter", EdaFormatter.FormatScalar(element.Diameter));
                _writer.WriteAttributeBool("platted", element.Platted);

                _writer.WriteEndElement();
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaLinearHoleElement element) {

                _writer.WriteStartElement("lineHole");

                _writer.WriteAttributeString("startPosition", EdaFormatter.FormatPoint(element.StartPosition));
                _writer.WriteAttributeString("endPosition", EdaFormatter.FormatPoint(element.EndPosition));
                _writer.WriteAttributeString("diameter", EdaFormatter.FormatScalar(element.Diameter));
                _writer.WriteAttributeBool("platted", element.Platted);

                _writer.WriteEndElement();
            }

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaRegionElement element) {

                _writer.WriteStartElement("region");

                _writer.WriteAttributeString("layer", EdaFormatter.FormatLayerSet(element.LayerSet));
                if (element.Thickness > 0)
                    _writer.WriteAttributeString("thickness", EdaFormatter.FormatScalar(element.Thickness));
                if (element.Clearance > 0)
                    _writer.WriteAttributeString("clearance", EdaFormatter.FormatScalar(element.Clearance));
                if (element.ThermalThickness > element.Thickness)
                    _writer.WriteAttributeString("thermalThickness", EdaFormatter.FormatScalar(element.ThermalThickness));
                if (element.ThermalClearance > element.Clearance)
                    _writer.WriteAttributeString("thermalClearance", EdaFormatter.FormatScalar(element.ThermalClearance));
                if (element.Priority > 0)
                    _writer.WriteAttributeInteger("priority", element.Priority);

                if (_currentBoard != null) {
                    EdaSignal signal = _currentBoard.GetSignal(element, _currentPart, false);
                    if (signal != null)
                        _writer.WriteAttributeString("signal", signal.Name);
                }

                if (element.Segments != null)
                    foreach (var segment in element.Segments) {
                        _writer.WriteStartElement("segment");
                        _writer.WriteAttributeString("position", EdaFormatter.FormatPoint(segment.Position));
                        if (!segment.Arc.IsZero)
                            _writer.WriteAttributeString("angle", EdaFormatter.FormatAngle(segment.Arc));
                        _writer.WriteEndElement();
                    }

                _writer.WriteEndElement();
            }

            /// <inheritdoc/>
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

            /// <inheritdoc/>
            /// 
            public override void Visit(EdaComponentAttribute attr) {

                _writer.WriteStartElement("attribute");

                _writer.WriteAttributeString("name", attr.Name);
                _writer.WriteAttributeString("value", attr.Value);

                _writer.WriteEndElement();
            }

            /// <inheritdoc/>
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
            /// Visita un objecte 'EdaLayer'.
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
            /// Visita un objecte 'EdaSignal'.
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
            /// Visita un objecte 'EdaComponent'
            /// </summary>
            /// <param name="component">L'objecte a visitar.</param>
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
            /// Visita un objecte 'EdaBoard'.
            /// </summary>
            /// <param name="board">L'objecte a visitar.</param>
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
            /// Visita un objecte 'EdaLibrary'
            /// </summary>
            /// <param name="library">L'objecte a visitar.</param>
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
        public EdaBoardStreamWriter(Stream stream) {

            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (!stream.CanWrite)
                throw new InvalidOperationException("El stream no es de escritura.");

            this._stream = stream;
        }

        /// <summary>
        /// Escriu una placa en un stream.
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
        /// Escriu una llibraria en un stream.
        /// </summary>
        /// <param name="library">La llibreria.</param>
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
