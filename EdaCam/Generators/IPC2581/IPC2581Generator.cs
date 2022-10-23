using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Xml;
using MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Cache.Entries;
using MikroPic.EdaTools.v1.Cam.Model;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.CoreExtensions.Bom;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581 {

    public sealed class IPC2581Generator: Generator {

        private const double _scale = 1000000.0;

        private List<IPCLayer> _layers;
        private readonly DataCache _dataCache = new DataCache();
        private XmlWriter _writer;

        public IPC2581Generator(Target target) :
            base(target) {

            _layers = new List<IPCLayer>() {
                new IPCLayer("TOP", IPCLayerSide.Top, IPCLayerFunction.Conductor, new EdaLayerSet(EdaLayerId.TopCopper)),
                new IPCLayer("BOTTOM", IPCLayerSide.Bottom, IPCLayerFunction.Conductor, new EdaLayerSet(EdaLayerId.BottomCopper)),
                new IPCLayer("DRILL_TOP_BOTTOM", IPCLayerSide.All, IPCLayerFunction.Drill, new EdaLayerSet(EdaLayerId.Platted)),
                new IPCLayer("OUTLINE", IPCLayerSide.None, IPCLayerFunction.Outline, new EdaLayerSet(EdaLayerId.Profile)),
                new IPCLayer("TOP_MASK", IPCLayerSide.Top, IPCLayerFunction.SolderMask, new EdaLayerSet(EdaLayerId.TopStop)),
                new IPCLayer("BOTTOM_MASK", IPCLayerSide.Bottom, IPCLayerFunction.SolderMask, new EdaLayerSet(EdaLayerId.BottomStop))
            };
        }

        public override void Generate(EdaBoard board, string outputFolder, GeneratorOptions options = null) {

            if (board == null)
                throw new ArgumentNullException(nameof(board));

            if (String.IsNullOrEmpty(outputFolder))
                throw new ArgumentNullException(nameof(outputFolder));

            string fileName = Path.Combine(outputFolder, Target.FileName);

            var settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";

            using (_writer = XmlWriter.Create(
                new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None),
                settings)) {

                _writer.WriteStartDocument();
                _writer.WriteStartElement("IPC-2581", "http://webstds.ipc.org/2581");
                _writer.WriteAttributeString("revision", "C");

                _dataCache.AddBoardEntries(board);
                _dataCache.AddFillDescEntry(true);
                _dataCache.AddFillDescEntry(false);

                WriteContentSection(board);
                WriteSection_Bom(board);
                WriteSection_Ecad(board);

                _writer.WriteEndElement();
                _writer.WriteEndDocument();
            }
        }

        /// <summary>
        /// Escriu la seccio 'Content'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteContentSection(EdaBoard board) {

            _writer.WriteStartElement("Content");

            _writer.WriteStartElement("FunctionMode");
            _writer.WriteAttributeString("mode", "Fabrication");
            _writer.WriteEndElement();

            WriteSection_DictionaryLineDesc(board);
            WriteSection_DictionaryFillDesc(board);
            WriteSection_DictionaryStandard(board);

            _writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la seccio 'DictionaryLineDesc'.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_DictionaryLineDesc(EdaBoard board) {

            _writer.WriteStartElement("DictionaryLineDesc");
            _writer.WriteAttributeString("units", "MILLIMETER");

            foreach (var entry in _dataCache.Entries.OfType<LineDescEntry>()) {

                _writer.WriteStartElement("EntryLineDesc");
                _writer.WriteAttributeInteger("id", entry.Id);
                _writer.WriteStartElement("LineDesc");
                _writer.WriteAttributeString("lineEnd", "ROUND"); // ?????
                _writer.WriteAttributeDouble("lineWidth", entry.Thickness / _scale);
                _writer.WriteEndElement();
                _writer.WriteEndElement();
            }

            _writer.WriteEndElement();
        }

        private void WriteSection_DictionaryFillDesc(EdaBoard board) {

            _writer.WriteStartElement("DictionaryFillDesc");
            _writer.WriteAttributeString("units", "MILLIMETER");

            foreach (var entry in _dataCache.Entries.OfType<FillDescEntry>()) {

                _writer.WriteStartElement("EntryFillDesc");
                _writer.WriteAttributeInteger("id", entry.Id);
                _writer.WriteStartElement("FillDesc");
                _writer.WriteAttributeString("fillProperty", entry.Fill ? "SOLID" : "HOLLOW");
                _writer.WriteEndElement();
                _writer.WriteEndElement();
            }

            _writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la seccio 'DictionaryStandard'.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_DictionaryStandard(EdaBoard board) {

            _writer.WriteStartElement("DictionaryStandard");
            _writer.WriteAttributeString("units", "MILLIMETER");

            foreach (var entry in _dataCache.Entries.OfType<CircleEntry>()) {

                _writer.WriteStartElement("EntryStandard");
                _writer.WriteAttributeInteger("id", entry.Id);

                _writer.WriteStartElement("Circle");
                _writer.WriteAttributeDouble("diameter", entry.Diameter / _scale);
                _writer.WriteEndElement();

                _writer.WriteEndElement();
            }

            foreach (var entry in _dataCache.Entries.OfType<RectRoundEntry>()) {

                _writer.WriteStartElement("EntryStandard");
                _writer.WriteAttributeInteger("id", entry.Id);

                _writer.WriteStartElement("RectRound");
                _writer.WriteAttributeDouble("width", entry.Size.Width / _scale);
                _writer.WriteAttributeDouble("height", entry.Size.Height / _scale);
                double radius = (Math.Min(entry.Size.Width, entry.Size.Height) * entry.Ratio) / 2.0;
                _writer.WriteAttributeDouble("radius", radius / _scale);
                _writer.WriteEndElement();

                _writer.WriteEndElement();
            }

            _writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la seccio 'Bom'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_Bom(EdaBoard board) {

            _writer.WriteStartElement("Bom");
            _writer.WriteAttributeString("name", "bom_0");

            WriteSection_BomHeader(board);
            WriteSection_BomItem(board);

            _writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la seccio 'BomHeader'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_BomHeader(EdaBoard board) {

            _writer.WriteStartElement("BomHeader");
            _writer.WriteAttributeString("assembly", "assembly_0");
            _writer.WriteAttributeString("revision", "1.0");
            _writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la seccio 'BomItem'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_BomItem(EdaBoard board) {

            var bomGenerator = new EdaBomGenerator(board);
            var bom = bomGenerator.Generate();

            foreach (var entry in bom.Entries) {

                _writer.WriteStartElement("BomItem");
                _writer.WriteAttributeString("OEMDesignNumberRef", entry.Name);
                _writer.WriteAttributeInteger("quantity", entry.ReferenceCount);

                foreach (var reference in entry.References) {

                    var part = board.GetPart(reference);
                    var package = part.Component.Name;

                    _writer.WriteStartElement("RefDes");
                    _writer.WriteAttributeString("name", reference);
                    _writer.WriteAttributeString("packageRef", package);
                    _writer.WriteAttributeBool("populate", true);
                    _writer.WriteEndElement();
                }

                _writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Escriu la seccio 'ECad'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_Ecad(EdaBoard board) {

            _writer.WriteStartElement("Ecad");
            _writer.WriteAttributeString("name", "ecad_0");
            WriteSection_CadHeader(board);
            WriteSection_CadData(board);
            _writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la seccio 'CadHeader'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_CadHeader(EdaBoard board) {

            _writer.WriteStartElement("CadHeader");
            _writer.WriteAttributeString("units", "MILLIMETER");

            _writer.WriteStartElement("Spec");
            _writer.WriteAttributeString("name", "name_example");
            _writer.WriteAttributeString("value", "value_example");
            _writer.WriteAttributeDouble("tolPlus", 0);
            _writer.WriteAttributeDouble("tolMinus", 0);
            _writer.WriteAttributeString("global", "TRUE");
            WriteSection_Point("Location", new EdaPoint(0, 0));
            _writer.WriteEndElement();

            _writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la seccio 'CadData'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_CadData(EdaBoard board) {

            _writer.WriteStartElement("CadData");

            WriteSection_Layer(board);
            WriteSection_Stackup(board);

            _writer.WriteStartElement("Step");
            _writer.WriteAttributeString("name", "board");

            WriteSection_Point("Datum", board.Position);

            WriteSection_Profile(board);
            WriteSection_Package(board);
            WriteSection_Component(board);
            WriteSection_LogicalNet(board);
            WriteSection_LayerFeature(board);

            _writer.WriteEndElement();

            _writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la descripcio del perfil de la placa.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_Profile(EdaBoard board) {

            _writer.WriteStartElement("Profile");

            var polygon = board.GetOutlinePolygon();
            if (polygon != null)
                WriteSection_Polygon(polygon);

            _writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la seccio 'Package'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_Package(EdaBoard board) {

            foreach (var component in board.Components) {
                _writer.WriteStartElement("Package");
                _writer.WriteAttributeString("name", component.Name);
                _writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Escriu la seccio 'LogicalNet'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_LogicalNet(EdaBoard board) {

            foreach (var signal in board.Signals) {
                _writer.WriteStartElement("LogicalNet");
                _writer.WriteAttributeString("name", signal.Name);

                var items = board.GetConnectedItems(signal);
                if (items != null) {
                    foreach (var item in items) {
                        if (item.Item1 is EdaPadElement pad) {
                            var part = item.Item2;
                            _writer.WriteStartElement("PinRef");
                            _writer.WriteAttributeString("componentRef", part.Name);
                            _writer.WriteAttributeString("pin", pad.Name);
                            _writer.WriteEndElement();
                        }
                    }
                }

                _writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Escriu la seccio 'Layer'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_Layer(EdaBoard board) {

            foreach (var layer in _layers) {

                string functionName = "CONDUCTOR";
                switch (layer.Function) {
                    case IPCLayerFunction.Drill:
                        functionName = "DRILL";
                        break;

                    case IPCLayerFunction.Outline:
                        functionName = "BOARD_OUTLINE";
                        break;

                    case IPCLayerFunction.SolderMask:
                        functionName = "SOLDERMASK";
                        break;

                    case IPCLayerFunction.SolderPaste:
                        functionName = "SOLDERPASTE";
                        break;
                }

                string sideName = "NONE";
                switch (layer.Side) {
                    case IPCLayerSide.Top:
                        sideName = "TOP";
                        break;

                    case IPCLayerSide.Bottom:
                        sideName = "BOTTOM";
                        break;

                    case IPCLayerSide.All:
                        sideName = "ALL";
                        break;
                }

                _writer.WriteStartElement("Layer");
                _writer.WriteAttributeString("name", layer.Name);
                _writer.WriteAttributeString("layerFunction", functionName);
                _writer.WriteAttributeString("side", sideName);

                if (layer.Function == IPCLayerFunction.Drill) {
                    _writer.WriteStartElement("Span");
                    _writer.WriteAttributeString("fromLayer", "TOP");
                    _writer.WriteAttributeString("toLayer", "BOTTOM");
                    _writer.WriteEndElement();
                }

                _writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Escriu la seccio 'Stackup'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_Stackup(EdaBoard board) {

            _writer.WriteStartElement("Stackup");
            _writer.WriteAttributeString("name", "stackup_0");
            _writer.WriteAttributeDouble("overallThickness", 1.44);
            _writer.WriteAttributeString("whereMeasured", "METAL");
            _writer.WriteAttributeDouble("tolPlus", 0);
            _writer.WriteAttributeDouble("tolMinus", 0);
            _writer.WriteStartElement("StackupGroup");
            _writer.WriteAttributeString("name", "stackup_group_0");
            _writer.WriteAttributeDouble("thickness", 1.45);
            _writer.WriteAttributeDouble("tolPlus", 0);
            _writer.WriteAttributeDouble("tolMinus", 0);

            int sequence = 1;
            foreach (var ipcLayer in _layers) {
                if (ipcLayer.Function == IPCLayerFunction.Conductor) {
                    _writer.WriteStartElement("StackupLayer");
                    _writer.WriteAttributeString("layerOfGroupRef", ipcLayer.Name);
                    _writer.WriteAttributeDouble("thickness", 0.35);
                    _writer.WriteAttributeDouble("tolPlus", 0);
                    _writer.WriteAttributeDouble("tolMinus", 0);
                    _writer.WriteAttributeInteger("sequence", sequence++);
                    _writer.WriteEndElement();
                }
            }

            _writer.WriteEndElement();
            _writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la la seccio 'LayerFeature'.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_LayerFeature(EdaBoard board) {

            // Procesa les capes conductores.
            //
            foreach (var layer in _layers.Where(l => l.Function == IPCLayerFunction.Conductor)) {
                foreach (var layerId in layer.LayerSet) {

                    _writer.WriteStartElement("LayerFeature");
                    _writer.WriteAttributeString("layerRef", layer.Name);

                    foreach (var part in board.Parts) {
                        var component = part.Component;
                        var tr = part.GetLocalTransformation();

                        foreach (var element in component.Elements.OfType<EdaSmtPadElement>().Where(e => e.IsOnLayer(layerId))) {

                            var rectRoundEntry = _dataCache.GetRectRoundEntry(element.Size, element.CornerRatio);
                            var signal = board.GetSignal(element, part, false);

                            _writer.WriteStartElement("Set");
                            if (signal != null)
                                _writer.WriteAttributeString("net", signal.Name);
                            _writer.WriteStartElement("Pad");
                            if (!part.Rotation.IsZero)
                                WriteSection_Xform(part.Rotation);
                            WriteSection_Point("Location", tr.Transform(element.Position));
                            _writer.WriteStartElement("StandardPrimitiveRef");
                            _writer.WriteAttributeInteger("id", rectRoundEntry.Id);
                            _writer.WriteEndElement();
                            _writer.WriteEndElement();
                            _writer.WriteEndElement();
                        }

                        foreach (var element in component.Elements.OfType<EdaThtPadElement>().Where(e => e.IsOnLayer(layerId))) {

                            var size =
                                layerId.IsTop ? element.TopSize :
                                layerId.IsBottom ? element.BottomSize :
                                element.InnerSize;
                            var rectRoundEntry = _dataCache.GetRectRoundEntry(size, element.CornerRatio);
                            var signal = board.GetSignal(element, part, false);

                            _writer.WriteStartElement("Set");
                            if (signal != null)
                                _writer.WriteAttributeString("net", signal.Name);
                            _writer.WriteStartElement("Pad");
                            WriteSection_Point("Location", tr.Transform(element.Position));
                            _writer.WriteStartElement("StandardPrimitiveRef");
                            _writer.WriteAttributeInteger("id", rectRoundEntry.Id);
                            _writer.WriteEndElement();
                            _writer.WriteEndElement();
                            _writer.WriteEndElement();
                        }
                    }

                    foreach (var element in board.Elements.OfType<EdaViaElement>().Where(e => e.IsOnLayer(layerId))) {

                        var circleEntry = _dataCache.GetCircleEntry(element.OuterSize);
                        var signal = board.GetSignal(element, null, false);

                        _writer.WriteStartElement("Set");
                        if (signal != null)
                            _writer.WriteAttributeString("net", signal.Name);
                        _writer.WriteAttributeString("padUsage", "VIA");
                        _writer.WriteStartElement("Pad");
                        WriteSection_Point("Location", element.Position);
                        _writer.WriteStartElement("StandardPrimitiveRef");
                        _writer.WriteAttributeInteger("id", circleEntry.Id);
                        _writer.WriteEndElement();
                        _writer.WriteEndElement();
                        _writer.WriteEndElement();
                    }

                    foreach (var element in board.Elements.OfType<EdaLineElement>().Where(e => e.IsOnLayer(layerId))) {

                        var lineDescEntry = _dataCache.GetLineDescEntry(element.Thickness, element.LineCap);
                        var signal = board.GetSignal(element, null, false);

                        _writer.WriteStartElement("Set");
                        if (signal != null)
                            _writer.WriteAttributeString("net", signal.Name);
                        _writer.WriteStartElement("Features");
                        _writer.WriteStartElement("Line");
                        _writer.WriteAttributeDouble("startX", element.StartPosition.X / _scale);
                        _writer.WriteAttributeDouble("startY", element.StartPosition.Y / _scale);
                        _writer.WriteAttributeDouble("endX", element.EndPosition.X / _scale);
                        _writer.WriteAttributeDouble("endY", element.EndPosition.Y / _scale);
                        _writer.WriteStartElement("LineDescRef");
                        _writer.WriteAttributeInteger("id", lineDescEntry.Id);
                        _writer.WriteEndElement();
                        _writer.WriteEndElement();
                        _writer.WriteEndElement();
                        _writer.WriteEndElement();
                    }

                    foreach (var element in board.Elements.OfType<EdaRegionElement>().Where(e => e.IsOnLayer(layerId))) {

                        //var lineDescEntry = _dataCache.GetLineDescEntry(element.Thickness);
                        var signal = board.GetSignal(element, null, false);

                        _writer.WriteStartElement("Set");
                        if (signal != null)
                            _writer.WriteAttributeString("net", signal.Name);
                        _writer.WriteStartElement("Features");
                        _writer.WriteStartElement("Contour");

                        var polygons = board.GetRegionPolygons(element, layerId, null);
                        foreach (var polygon in polygons)
                            WriteSection_Polygon(polygon);

                        //_writer.WriteStartElement("LineDescRef");
                        //_writer.WriteAttributeInteger("id", lineDescEntry.Id);
                        //_writer.WriteEndElement();
                        _writer.WriteEndElement();
                        _writer.WriteEndElement();
                        _writer.WriteEndElement();
                    }

                    _writer.WriteEndElement();
                }
            }
            // Procesa les capes de forats conductors (Vias i pads THT).
            //
            int holeNumber = 0;
            foreach (var layer in _layers.Where(l => l.Function == IPCLayerFunction.Drill)) {

                _writer.WriteStartElement("LayerFeature");
                _writer.WriteAttributeString("layerRef", layer.Name);

                foreach (var part in board.Parts) {
                    var component = part.Component;
                    var tr = part.GetLocalTransformation();
                    foreach (var element in component.Elements.OfType<EdaThtPadElement>()) {
                        var position = tr.Transform(element.Position);
                        _writer.WriteStartElement("Set");
                        _writer.WriteStartElement("Hole");
                        _writer.WriteAttributeString("name", String.Format("H{0}", holeNumber++));
                        _writer.WriteAttributeString("platingStatus", "VIA");
                        _writer.WriteAttributeDouble("x", position.X / _scale);
                        _writer.WriteAttributeDouble("y", position.Y / _scale);
                        _writer.WriteAttributeDouble("diameter", element.DrillDiameter / _scale);
                        _writer.WriteAttributeDouble("plusTol", 0);
                        _writer.WriteAttributeDouble("minusTol", 0);
                        _writer.WriteEndElement();
                        _writer.WriteEndElement();
                    }
                }

                foreach (var element in board.Elements.OfType<EdaViaElement>()) {
                    _writer.WriteStartElement("Set");
                    _writer.WriteStartElement("Hole");
                    _writer.WriteAttributeString("name", String.Format("H{0}", holeNumber++));
                    _writer.WriteAttributeString("platingStatus", "VIA");
                    _writer.WriteAttributeDouble("x", element.Position.X / _scale);
                    _writer.WriteAttributeDouble("y", element.Position.Y / _scale);
                    _writer.WriteAttributeDouble("diameter", element.DrillDiameter / _scale);
                    _writer.WriteAttributeDouble("plusTol", 0);
                    _writer.WriteAttributeDouble("minusTol", 0);
                    _writer.WriteEndElement();
                    _writer.WriteEndElement();
                }

                _writer.WriteEndElement();
            }

            // Procesa les capes de mascara de soldadura.
            //
            foreach (var layer in _layers.Where(x => x.Function == IPCLayerFunction.SolderMask)) {
                foreach (var layerId in layer.LayerSet) {

                    _writer.WriteStartElement("LayerFeature");
                    _writer.WriteAttributeString("layerRef", layer.Name);

                    foreach (var part in board.Parts) {
                        var component = part.Component;
                        var tr = part.GetLocalTransformation();
                        foreach (var element in component.Elements.OfType<EdaSmtPadElement>().Where(e => e.IsOnLayer(layerId))) {

                            var size = element.Size.Inflated(element.MaskClearance);
                            var rectRoundEntry = _dataCache.GetRectRoundEntry(size, element.CornerRatio);

                            _writer.WriteStartElement("Set");
                            _writer.WriteStartElement("Pad");
                            if (!part.Rotation.IsZero)
                                WriteSection_Xform(part.Rotation);
                            WriteSection_Point("Location", tr.Transform(element.Position));
                            _writer.WriteStartElement("StandardPrimitiveRef");
                            _writer.WriteAttributeInteger("id", rectRoundEntry.Id);
                            _writer.WriteEndElement();
                            _writer.WriteEndElement();
                            _writer.WriteEndElement();
                        }

                        foreach (var element in component.Elements.OfType<EdaThtPadElement>()) {

                            var size =
                                layerId.IsTop ? element.TopSize :
                                layerId.IsBottom ? element.BottomSize :
                                element.InnerSize;
                            var maskSize = size.Inflated(element.MaskClearance);
                            var rectRoundEntry = _dataCache.GetRectRoundEntry(maskSize, element.CornerRatio);

                            _writer.WriteStartElement("Set");
                            _writer.WriteStartElement("Pad");
                            if (!part.Rotation.IsZero)
                                WriteSection_Xform(part.Rotation);
                            WriteSection_Point("Location", tr.Transform(element.Position));
                            _writer.WriteStartElement("StandardPrimitiveRef");
                            _writer.WriteAttributeInteger("id", rectRoundEntry.Id);
                            _writer.WriteEndElement();
                            _writer.WriteEndElement();
                            _writer.WriteEndElement();
                        }
                    }

                    _writer.WriteEndElement();
                }
            }
        }

        /// <summary>
        /// Escriu la la seccio 'Component'.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_Component(EdaBoard board) {

            foreach (var part in board.Parts) {
                _writer.WriteStartElement("Component");
                _writer.WriteAttributeString("refDes", part.Name);
                _writer.WriteAttributeString("layerRef", part.Side == PartSide.Top ? "TOP" : "BOTTOM");
                _writer.WriteAttributeString("packageRef", part.Component.Name);
                WriteSection_Point("Location", part.Position);
                _writer.WriteEndElement();
            }
        }

        private void WriteSection_Xform(EdaAngle angle) {

            _writer.WriteStartElement("Xform");
            _writer.WriteAttributeDouble("rotation", angle.AsDegrees);
            _writer.WriteEndElement();
        }

        /// <summary>
        /// Escriu la descripocio d'un poligon.
        /// </summary>
        /// <param name="polygon">El poligon.</param>
        /// 
        private void WriteSection_Polygon(EdaPolygon polygon) {

            void WritePolygonPoints(IEnumerable<EdaPoint> points) {

                bool first = true;
                EdaPoint firstPoint = default;
                foreach (var point in points) {
                    if (first) {
                        first = false;
                        firstPoint = point;
                        WriteSection_Point("PolyBegin", point);
                    }
                    else
                        WriteSection_Point("PolyStepSegment", point);
                }
                WriteSection_Point("PolyStepSegment", firstPoint);
            }

            _writer.WriteStartElement("Polygon");
            WritePolygonPoints(polygon.Outline);
            _writer.WriteStartElement("FillDescRef");
            var fillDescEntry = _dataCache.GetFillDescEntry(true);
            _writer.WriteAttributeInteger("id", fillDescEntry.Id);
            _writer.WriteEndElement();
            _writer.WriteEndElement();

            if (polygon.HasHoles) {
                foreach (var hole in polygon.Holes) {
                    _writer.WriteStartElement("Cutout");
                    WritePolygonPoints(hole);
                    _writer.WriteEndElement();
                }
            }
        }

        private void WriteSection_Point(string name, EdaPoint point) {

            _writer.WriteStartElement(name);
            _writer.WriteAttributeDouble("x", point.X / _scale);
            _writer.WriteAttributeDouble("y", point.Y / _scale);
            _writer.WriteEndElement();

        }
    }
}
