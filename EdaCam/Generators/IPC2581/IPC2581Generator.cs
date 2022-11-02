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

    public enum IPCFunctionMode {
        Fabrication,
        Assembly,
        Stackup,
        Stencil,
        Bom,
        All
    }

    public sealed class IPC2581Generator: Generator {

        private const double _scale = 1000000.0;

        private readonly List<IPCLayer> _layers = new List<IPCLayer>();
        private readonly DataCache _dataCache = new DataCache();

        private IPCLayer _topLayer;
        private IPCLayer _bottomLayer;
        private IPCFunctionMode _mode = IPCFunctionMode.All;
        private bool _bomDataEnabled;
        private bool _profileDataEnabled;
        private bool _componentDataEnabled;
        private bool _logicalNetDataEnabled = true;
        private bool _solderMaskDataEnabled = true;
        private bool _solderPasteDataEnabled = true;

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
            _layers.Add(new IPCLayer("bottom_paste", IPCLayerSide.Bottom, IPCLayerFunction.SolderPaste, new EdaLayerSet(EdaLayerId.BottomCream)));
            _layers.Add(new IPCLayer("top_silkscreen", IPCLayerSide.Top, IPCLayerFunction.SilkScreen, new EdaLayerSet(EdaLayerId.TopNames, EdaLayerId.TopPlace)));
            _layers.Add(new IPCLayer("bottom_silkscreen", IPCLayerSide.Bottom, IPCLayerFunction.SilkScreen, new EdaLayerSet(EdaLayerId.BottomNames, EdaLayerId.BottomPlace)));

            // Afegeix les definicions
            //
            _dataCache.AddBoardEntries(board);
            _dataCache.AddDefaultEntries();

            // Habilita les seccions
            //
            _bomDataEnabled = (_mode == IPCFunctionMode.All) || (_mode == IPCFunctionMode.Bom) || (_mode == IPCFunctionMode.Assembly);
            _profileDataEnabled = (_mode == IPCFunctionMode.All) || (_mode == IPCFunctionMode.Fabrication) || (_mode == IPCFunctionMode.Assembly) || (_mode == IPCFunctionMode.Stackup);
            _componentDataEnabled = true;

            string fileName = Path.Combine(outputFolder, Target.FileName);

            var settings = new XmlWriterSettings {
                Indent = true,
                IndentChars = "    "
            };

            using (_writer = XmlWriter.Create(
                new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None),
                settings)) {

                _writer.WriteStartDocument();
                _writer.WriteStartElement("IPC-2581", "http://webstds.ipc.org/2581");
                _writer.WriteAttributeString("revision", "C");

                WriteSection_Content();

                if (_bomDataEnabled)
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
        private void WriteSection_Content() {

            _writer.WriteStartElement("Content");

            // Escriu la seccio 'FunctionMode'
            //
            _writer.WriteStartElement("FunctionMode");
            string modeText = "USERDEF";
            string keyText = "ABCDEFGIKLMOPRSUXY";
            switch (_mode) {
                case IPCFunctionMode.Assembly:
                    modeText = "ASSEMBLY";
                    keyText = "BCAULRO";
                    break;

                case IPCFunctionMode.Fabrication:
                    modeText = "FABRICATION";
                    keyText = "KSUMLROIEY";
                    break;

                case IPCFunctionMode.Stackup:
                    modeText = "STACKUP";
                    keyText = "SOY";
                    break;

                case IPCFunctionMode.Stencil:
                    modeText = "STENCIL";
                    keyText = "UP";
                    break;

                case IPCFunctionMode.Bom:
                    modeText = "BOM";
                    keyText = "B";
                    break;
            }

            _writer.WriteAttributeString("mode", modeText);
            _writer.WriteAttributeInteger("level", 1);
            _writer.WriteAttributeString("sectionKey", keyText);
            _writer.WriteEndElement(); // FunctionMode

            // Escriu la seccio 'StepRef'
            //
            _writer.WriteStartElement("StepRef");
            _writer.WriteAttributeString("name", "board_0");
            _writer.WriteEndElement(); // StepRef

            // Escriu la seccion 'LayerRef'
            //
            foreach (var layer in _layers) {
                _writer.WriteStartElement("LayerRef");
                _writer.WriteAttributeString("name", layer.Name);
                _writer.WriteEndElement(); // LayerRef
            }

            // Escriu la seccio 'BomRef'
            //
            if (_bomDataEnabled) {
                _writer.WriteStartElement("BomRef");
                _writer.WriteAttributeString("name", "board_0_bom");
                _writer.WriteEndElement(); // BomRef
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
            _writer.WriteEndElement(); // DictionaryLineDesc

            // Escriu la seccio 'DictionaryFillDesc'
            //
            _writer.WriteStartElement("DictionaryFillDesc");
            _writer.WriteAttributeString("units", "MILLIMETER");
            foreach (var entry in _dataCache.Entries.OfType<FillDescEntry>()) {
                _writer.WriteStartElement("EntryFillDesc");
                _writer.WriteAttributeInteger("id", entry.Id);
                _writer.WriteStartElement("FillDesc");
                _writer.WriteAttributeString("fillProperty", entry.Fill ? "FILL" : "HOLLOW");
                _writer.WriteEndElement();
                _writer.WriteEndElement();
            }
            _writer.WriteEndElement(); // DictionaryFillDesc

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
            foreach (var entry in _dataCache.Entries.OfType<RectEntry>()) {
                _writer.WriteStartElement("EntryStandard");
                _writer.WriteAttributeInteger("id", entry.Id);
                if (entry.Flat) {
                    _writer.WriteStartElement("RectCham");
                    _writer.WriteAttributeDouble("width", entry.Size.Width / _scale);
                    _writer.WriteAttributeDouble("height", entry.Size.Height / _scale);
                    _writer.WriteAttributeDouble("chamfer", entry.Radius / _scale);
                    _writer.WriteEndElement();
                }
                else {
                    _writer.WriteStartElement("RectRound");
                    _writer.WriteAttributeDouble("width", entry.Size.Width / _scale);
                    _writer.WriteAttributeDouble("height", entry.Size.Height / _scale);
                    _writer.WriteAttributeDouble("radius", entry.Radius / _scale);
                    _writer.WriteEndElement();
                }
                _writer.WriteEndElement();
            }
            _writer.WriteEndElement(); // DictionaryStandard

            _writer.WriteEndElement(); // Content
        }

        /// <summary>
        /// Escriu la seccio 'Bom'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_Bom(EdaBoard board) {

            _writer.WriteStartElement("Bom");
            _writer.WriteAttributeString("name", "board_0_bom");

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

            if ((_mode == IPCFunctionMode.All) || (_mode == IPCFunctionMode.Stackup) || (_mode == IPCFunctionMode.Fabrication)) {
                WriteSection_Layer();
                WriteSection_Stackup();
            }

            _writer.WriteStartElement("Step");
            _writer.WriteAttributeString("name", "board_0");
            _writer.WriteAttributeString("type", "BOARD");
            _writer.WriteAttributeString("stackupRef", "stackup_0");

            _writer.WritePointElement("Datum", board.Position, _scale);

            if (_profileDataEnabled)
                WriteSection_Profile(board);

            if (_componentDataEnabled) {
                WriteSection_Package(board);
                WriteSection_Component(board);
            }

            if (_logicalNetDataEnabled)
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
            if (polygon != null) {
                _writer.WriteStartElement("Profile");
                _writer.WritePolygonElement(polygon, -1, _scale);
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

                var items = board.GetConectionItems(signal);
                if (items != null) {
                    foreach (var item in items) {
                        if (item.Conectable is EdaPadBaseElement pad) {
                            var part = item.Part;
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

                    case IPCLayerFunction.SilkScreen:
                        functionName = "SILKSCREEN";
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
            if (_solderMaskDataEnabled)
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
            if (_solderPasteDataEnabled)
                foreach (var layer in _layers.Where(x => x.Function == IPCLayerFunction.SolderPaste)) {

                    _writer.WriteStartElement("LayerFeature");
                    _writer.WriteAttributeString("layerRef", layer.Name);

                    foreach (var layerId in layer.LayerSet) {
                        var visitor = new SolderPasteLayerVisitor(layerId, _dataCache, _writer);
                        visitor.Visit(board);
                    }

                    _writer.WriteEndElement();
                }

            // Procesa les capes de texts.
            //
            foreach (var layer in _layers.Where(x => x.Function == IPCLayerFunction.SilkScreen)) {

                _writer.WriteStartElement("LayerFeature");
                _writer.WriteAttributeString("layerRef", layer.Name);

                foreach (var layerId in layer.LayerSet) {
                    var visitor = new SilkscreenLayerVisitor(layerId, _dataCache, _writer);
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
                if (!part.Rotation.IsZero) {
                    _writer.WriteStartElement("Xform");
                    _writer.WriteAttributeDouble("rotation", part.Rotation.AsDegrees);
                    _writer.WriteEndElement(); // Xform
                }
                _writer.WritePointElement("Location", part.Position, _scale);
                _writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Escriu la seccio 'Package'
        /// </summary>
        /// <param name="board">La placa.</param>
        /// 
        private void WriteSection_Package(EdaBoard board) {

            var visitor = new PackageVisitor(_dataCache, _writer);
            visitor.Visit(board);
        }
    }
}
