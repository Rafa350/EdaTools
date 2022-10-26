using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Xml;
using MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Cache.Entries;
using MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Visitors;
using MikroPic.EdaTools.v1.Cam.Generators.IPC2581.Xml;
using MikroPic.EdaTools.v1.Cam.Model;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.CoreExtensions.Bom;

namespace MikroPic.EdaTools.v1.Cam.Generators.IPC2581 {

    public sealed class IPC2581Generator: Generator {

        private const double _scale = 1000000.0;

        private readonly List<IPCLayer> _layers = new List<IPCLayer>();
        private readonly DataCache _dataCache = new DataCache();

        private IPCLayer _topLayer;
        private IPCLayer _bottomLayer;

        private XmlWriter _writer;

        public IPC2581Generator(Target target) :
            base(target) {

        }

        public override void Generate(EdaBoard board, string outputFolder, GeneratorOptions options = null) {

            if (board == null)
                throw new ArgumentNullException(nameof(board));

            if (String.IsNullOrEmpty(outputFolder))
                throw new ArgumentNullException(nameof(outputFolder));

            // Afegeix les capes
            //
            _topLayer = new IPCLayer("top", IPCLayerSide.Top, IPCLayerFunction.Conductor, new EdaLayerSet(EdaLayerId.TopCopper));
            _layers.Add(_topLayer);
            _bottomLayer = new IPCLayer("bottom", IPCLayerSide.Bottom, IPCLayerFunction.Conductor, new EdaLayerSet(EdaLayerId.BottomCopper));
            _layers.Add(_bottomLayer);
            _layers.Add(new IPCLayer("drill_all", IPCLayerSide.All, IPCLayerFunction.Drill, new EdaLayerSet(EdaLayerId.Platted)));
            _layers.Add(new IPCLayer("top_mask", IPCLayerSide.Top, IPCLayerFunction.SolderMask, new EdaLayerSet(EdaLayerId.TopStop)));
            _layers.Add(new IPCLayer("bottom_mask", IPCLayerSide.Bottom, IPCLayerFunction.SolderMask, new EdaLayerSet(EdaLayerId.BottomStop)));
            _layers.Add(new IPCLayer("top_paste", IPCLayerSide.Top, IPCLayerFunction.SolderPaste, new EdaLayerSet(EdaLayerId.TopCream)));
            _layers.Add(new IPCLayer("bottom`_paste", IPCLayerSide.Bottom, IPCLayerFunction.SolderPaste, new EdaLayerSet(EdaLayerId.BottomCream)));

            // Afegeix les definicions
            //
            _dataCache.AddBoardEntries(board);
            _dataCache.AddDefaultEntries();

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

                WriteContentSection();
                WriteSection_Bom(board);
                WriteSection_Ecad(board);

                _writer.WriteEndElement();
                _writer.WriteEndDocument();
            }
        }

        /// <summary>
        /// Escriu la seccio 'Content'
        /// </summary>
        /// 
        private void WriteContentSection() {

            _writer.WriteStartElement("Content");

            _writer.WriteStartElement("FunctionMode");
            _writer.WriteAttributeString("mode", "Fabrication");
            _writer.WriteEndElement();

            // Escriu la seccions 'LayerRef'
            //
            foreach (var layer in _layers) {
                _writer.WriteStartElement("LayerRef");
                _writer.WriteAttributeString("name", layer.Name);
                _writer.WriteEndElement();
            }

            // Escriu la seccio 'DictionaryLineDesc'
            //
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

            // Escriu la seccio 'DictionaryFillDesc'
            //
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

            // Escriu la seccio 'DictionaryStandard'
            //
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
                _writer.WriteAttributeDouble("radius", entry.Radius / _scale);
                _writer.WriteEndElement();
                _writer.WriteEndElement();
            }
            _writer.WriteEndElement();

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

            _writer.WriteStartElement("BomHeader");
            _writer.WriteAttributeString("assembly", "assembly_0");
            _writer.WriteAttributeString("revision", "1.0");
            _writer.WriteEndElement();

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

            _writer.WriteEndElement();
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
            _writer.WritePointElement("Location", new EdaPoint(0, 0), _scale);
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

            WriteSection_Layer();
            WriteSection_Stackup();

            _writer.WriteStartElement("Step");
            _writer.WriteAttributeString("name", "board_0");
            _writer.WriteAttributeString("type", "BOARD");
            _writer.WriteAttributeString("stackupRef", "stackup_0");

            _writer.WritePointElement("Datum", board.Position, _scale);

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

            var polygon = board.GetOutlinePolygon();
            if (polygon != null)
                _writer.WritePolygonElement("Profile", polygon, -1, _scale);
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

            //var generator = new EdaLogicalNetGenerator(board);
            //var logicalNet = generator.Generate();

            foreach (var signal in board.Signals) {
                _writer.WriteStartElement("LogicalNet");
                _writer.WriteAttributeString("name", signal.Name);

                var signalNodes = board.GetConnectedItems(signal);
                if (signalNodes != null) {
                    foreach (var signalNode in signalNodes) {
                        if (signalNode.Conectable is EdaPadElement pad) {
                            var part = signalNode.Part;
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
        /// 
        private void WriteSection_Layer() {

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
                    _writer.WriteAttributeString("fromLayer", _topLayer.Name);
                    _writer.WriteAttributeString("toLayer", _bottomLayer.Name);
                    _writer.WriteEndElement();
                }

                _writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Escriu la seccio 'Stackup'
        /// </summary>
        /// 
        private void WriteSection_Stackup() {

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
            foreach (var layer in _layers) {
                if (layer.Function == IPCLayerFunction.Conductor) {
                    _writer.WriteStartElement("StackupLayer");
                    _writer.WriteAttributeString("layerOfGroupRef", layer.Name);
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

                _writer.WriteStartElement("LayerFeature");
                _writer.WriteAttributeString("layerRef", layer.Name);

                foreach (var layerId in layer.LayerSet) {
                    var visitor = new ConductorLayerVisitor(layerId, _dataCache, _writer);
                    visitor.Visit(board);
                }

                _writer.WriteEndElement();
            }

            // Procesa les capes de forats (Vias i pads THT).
            //
            foreach (var layer in _layers.Where(l => l.Function == IPCLayerFunction.Drill)) {

                _writer.WriteStartElement("LayerFeature");
                _writer.WriteAttributeString("layerRef", layer.Name);

                foreach (var layerId in layer.LayerSet) {
                    var visitor = new DrillLayerVisitor(layerId, _dataCache, _writer);
                    visitor.Visit(board);
                }

                _writer.WriteEndElement();
            }

            // Procesa les capes de mascara de soldadura.
            //
            foreach (var layer in _layers.Where(x => x.Function == IPCLayerFunction.SolderMask)) {

                _writer.WriteStartElement("LayerFeature");
                _writer.WriteAttributeString("layerRef", layer.Name);

                foreach (var layerId in layer.LayerSet) {
                    var visitor = new SolderMaskLayerVisitor(layerId, _dataCache, _writer);
                    visitor.Visit(board);
                }

                _writer.WriteEndElement();
            }

            // Procesa les capes de pasta de soldadura.
            //
            foreach (var layer in _layers.Where(x => x.Function == IPCLayerFunction.SolderPaste)) {

                _writer.WriteStartElement("LayerFeature");
                _writer.WriteAttributeString("layerRef", layer.Name);

                foreach (var layerId in layer.LayerSet) {
                    var visitor = new SolderPasteLayerVisitor(layerId, _dataCache, _writer);
                    visitor.Visit(board);
                }

                _writer.WriteEndElement();
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
                _writer.WriteAttributeString("layerRef", part.Side == PartSide.Top ? _topLayer.Name : _bottomLayer.Name);
                _writer.WriteAttributeString("packageRef", part.Component.Name);
                _writer.WritePointElement("Location", part.Position, _scale);
                _writer.WriteEndElement();
            }
        }

        private void WriteSection_Xform(EdaAngle angle) {

            _writer.WriteStartElement("Xform");
            _writer.WriteAttributeDouble("rotation", angle.AsDegrees);
            _writer.WriteEndElement();
        }    
    }
}
