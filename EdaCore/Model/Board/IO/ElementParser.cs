using System.IO;
using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
using MikroPic.EdaTools.v1.Base.Xml;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;

namespace MikroPic.EdaTools.v1.Core.Model.Board.IO {

    public static class ElementParser {

        /// <summary>
        /// Obte un element linia 
        /// </summary>
        /// <param name="rd">El objecte per lleigir del stream</param>
        /// <returns>L'element obtingut.</returns>
        /// 
        public static LineElement Line(XmlReaderAdapter rd) {

            if (!rd.IsStartTag("line"))
                throw new InvalidDataException("Se esperaba <line>");

            LayerId layerId = LayerId.Parse(rd.AttributeAsString("layer"));
            var layerSet = new LayerSet(layerId);
            Point startPosition = XmlTypeParser.ParsePoint(rd.AttributeAsString("startPosition"));
            Point endPosition = XmlTypeParser.ParsePoint(rd.AttributeAsString("endPosition"));
            int thickness = XmlTypeParser.ParseNumber(rd.AttributeAsString("thickness", "0"));
            LineElement.CapStyle lineCap = rd.AttributeAsEnum<LineElement.CapStyle>("lineCap", LineElement.CapStyle.Round);

            rd.NextTag();
            if (!rd.IsEndTag("line"))
                throw new InvalidDataException("Se esperaba </line>");

            return new LineElement(layerSet, startPosition, endPosition, thickness, lineCap);
        }

        /// <summary>
        /// Obte un element arc
        /// </summary>
        /// <param name="rd">El objecte per lleigir del stream.</param>
        /// <returns>L'element obtingut</returns>
        /// 
        public static ArcElement Arc(XmlReaderAdapter rd) {

            if (!rd.IsStartTag("arc"))
                throw new InvalidDataException("Se esperaba <arc>");

            LayerId layerId = LayerId.Parse(rd.AttributeAsString("layer"));
            var layerSet = new LayerSet(layerId);
            Point startPosition = XmlTypeParser.ParsePoint(rd.AttributeAsString("startPosition"));
            Point endPosition = XmlTypeParser.ParsePoint(rd.AttributeAsString("endPosition"));
            int thickness = XmlTypeParser.ParseNumber(rd.AttributeAsString("thickness"));
            Angle angle = XmlTypeParser.ParseAngle(rd.AttributeAsString("angle"));
            LineElement.CapStyle lineCap = rd.AttributeAsEnum<LineElement.CapStyle>("lineCap", LineElement.CapStyle.Round);

            rd.NextTag();
            if (!rd.IsEndTag("arc"))
                throw new InvalidDataException("Se esperaba </arc>");

            return new ArcElement(layerSet, startPosition, endPosition, thickness, angle, lineCap);
        }

        /// <summary>
        /// Obte un element cercle
        /// </summary>
        /// <param name="rd">El objecte per lleigir del stream</param>
        /// <returns>L'element obtingut.</returns>
        /// 
        public static CircleElement Circle(XmlReaderAdapter rd) {

            if (!rd.IsStartTag("circle"))
                throw new InvalidDataException("Se esperaba <circle>");

            LayerId layerId = LayerId.Parse(rd.AttributeAsString("layer"));
            var layerSet = new LayerSet(layerId);
            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            int radius = XmlTypeParser.ParseNumber(rd.AttributeAsString("radius"));
            int thickness = XmlTypeParser.ParseNumber(rd.AttributeAsString("thickness", "0"));
            bool filled = rd.AttributeAsBoolean("filled", thickness == 0);

            rd.NextTag();
            if (!rd.IsEndTag("circle"))
                throw new InvalidDataException("Se esperaba </circle>");

            return new CircleElement(layerSet, position, radius, thickness, filled);
        }

        /// <summary>
        /// Obte un element rectangle
        /// </summary>
        /// <param name="rd">El objecte per lleigir el srtream</param>
        /// <returns>L'element obtingut</returns>
        /// 
        public static RectangleElement Rectangle(XmlReaderAdapter rd) {

            if (!rd.IsStartTag("rectangle"))
                throw new InvalidDataException("Se esperaba <rectangle>");

            LayerId layerId = LayerId.Parse(rd.AttributeAsString("layer"));
            var layerSet = new LayerSet(layerId);
            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            Size size = XmlTypeParser.ParseSize(rd.AttributeAsString("size"));
            Angle rotation = XmlTypeParser.ParseAngle(rd.AttributeAsString("rotation", "0"));
            int thickness = XmlTypeParser.ParseNumber(rd.AttributeAsString("thickness", "0"));
            Ratio roundness = XmlTypeParser.ParseRatio(rd.AttributeAsString("roundness", "0"));
            bool filled = rd.AttributeAsBoolean("filled", thickness == 0);

            rd.NextTag();
            if (!rd.IsEndTag("rectangle"))
                throw new InvalidDataException("Se esperaba </rectangle>");

            return new RectangleElement(layerSet, position, size, roundness, rotation, thickness, filled);
        }

        /// <summary>
        /// Obte un element spad
        /// </summary>
        /// <param name="rd">El objecte per lleigir el srtream</param>
        /// <returns>L'element obtingut.</returns>
        /// 
        public static SmdPadElement SPad(XmlReaderAdapter rd) {

            if (!rd.IsStartTag("spad"))
                throw new InvalidDataException("Se esperaba <spad>");

            string name = rd.AttributeAsString("name");
            LayerId layerId = LayerId.Parse(rd.AttributeAsString("layer"));
            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            Size size = XmlTypeParser.ParseSize(rd.AttributeAsString("size"));
            Angle rotation = XmlTypeParser.ParseAngle(rd.AttributeAsString("rotation", "0"));
            Ratio roundness = XmlTypeParser.ParseRatio(rd.AttributeAsString("roundness", "0"));

            rd.NextTag();
            if (!rd.IsEndTag("spad"))
                throw new InvalidDataException("Se esperaba </spad>");

            var layerSet = new LayerSet();
            layerSet.Add(layerId);

            return new SmdPadElement(name, layerSet, position, size, rotation, roundness);
        }

        /// <summary>
        /// Obte un element tpad
        /// </summary>
        /// <param name="rd">El objecte per lleigir el srtream</param>
        /// <returns>L'element obtinguc</returns>
        /// 
        public static ThPadElement TPad(XmlReaderAdapter rd) {

            if (!rd.IsStartTag("tpad"))
                throw new InvalidDataException("Se esperaba <tpad>");

            string name = rd.AttributeAsString("name");
            LayerId layerId = LayerId.Parse(rd.AttributeAsString("layer"));
            var layerSet = new LayerSet();
            layerSet.Add(layerId);
            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            int size = XmlTypeParser.ParseNumber(rd.AttributeAsString("size"));
            Angle rotation = XmlTypeParser.ParseAngle(rd.AttributeAsString("rotation", "0"));
            int drill = XmlTypeParser.ParseNumber(rd.AttributeAsString("drill"));
            ThPadElement.ThPadShape shape = rd.AttributeAsEnum<ThPadElement.ThPadShape>("shape", ThPadElement.ThPadShape.Circle);

            rd.NextTag();
            if (!rd.IsEndTag("tpad"))
                throw new InvalidDataException("Se esperaba </tpad>");

            return new ThPadElement(name, layerSet, position, rotation, size, shape, drill);
        }

        /// <summary>
        /// Obte un element text
        /// </summary>
        /// <returns>L'element obtingut.</returns>
        /// 
        public static TextElement Text(XmlReaderAdapter rd) {

            if (!rd.IsStartTag("text"))
                throw new InvalidDataException("Se esperaba <text>");

            LayerId layerId = LayerId.Parse(rd.AttributeAsString("layer"));
            var layerSet = new LayerSet(layerId);
            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            Angle rotation = XmlTypeParser.ParseAngle(rd.AttributeAsString("rotation", "0"));
            int height = XmlTypeParser.ParseNumber(rd.AttributeAsString("height"));
            HorizontalTextAlign horizontalAlign = rd.AttributeAsEnum("horizontalAlign", HorizontalTextAlign.Left);
            VerticalTextAlign verticalAlign = rd.AttributeAsEnum("verticalAlign", VerticalTextAlign.Bottom);
            int thickness = XmlTypeParser.ParseNumber(rd.AttributeAsString("thickness"));
            string value = rd.AttributeAsString("value");

            rd.NextTag();
            if (!rd.IsEndTag("text"))
                throw new InvalidDataException("Se esperaba </text>");

            return new TextElement(layerSet, position, rotation, height, thickness, horizontalAlign, verticalAlign, value);
        }

        /// <summary>
        /// Obte un element Hole
        /// </summary>
        /// <returns>L'element obtigut.</returns>
        /// 
        public static HoleElement Hole(XmlReaderAdapter rd) {

            if (!rd.IsStartTag("hole"))
                throw new InvalidDataException("Se esperaba <hole>");

            Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
            int drill = XmlTypeParser.ParseNumber(rd.AttributeAsString("drill"));

            rd.NextTag();
            if (!rd.IsEndTag("hole"))
                throw new InvalidDataException("Se esperaba </hole>");

            return new HoleElement(new LayerSet(LayerId.Holes), position, drill);
        }

        /// <summary>
        /// Obte un element regio
        /// </summary>
        /// <returns>L'element obtingut.</returns>
        /// 
        public static  RegionElement Region(XmlReaderAdapter rd) {

            if (!rd.IsStartTag("region"))
                throw new InvalidDataException("Se esperaba <region>");

            LayerId layerId = LayerId.Parse(rd.AttributeAsString("layers"));
            var layerSet = new LayerSet(layerId);
            int thickness = rd.AttributeExists("thickness") ?
                XmlTypeParser.ParseNumber(rd.AttributeAsString("thickness")) :
                0;
            bool filled = rd.AttributeAsBoolean("filled", thickness == 0);
            int clearance = XmlTypeParser.ParseNumber(rd.AttributeAsString("clearance", "0"));

            var region = new RegionElement(layerSet, thickness, filled, clearance);

            rd.NextTag();
            while (rd.IsStartTag("segment")) {

                Point position = XmlTypeParser.ParsePoint(rd.AttributeAsString("position"));
                Angle angle = XmlTypeParser.ParseAngle(rd.AttributeAsString("angle", "0"));

                rd.NextTag();
                if (!rd.IsEndTag("segment"))
                    throw new InvalidDataException("Se esperaba </segment>");

                var segment = new RegionElement.Segment(position, angle);
                region.Add(segment);
                rd.NextTag();
            }

            if (!rd.IsEndTag("region"))
                throw new InvalidDataException("Se esperaba </region>");

            return region;
        }
    }
}
