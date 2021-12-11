using System.Collections.Generic;
using System.IO;

using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
using MikroPic.EdaTools.v1.Base.Xml;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.IO;

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

            EdaLayerId layerId = EdaLayerId.Parse(rd.AttributeAsString("layer"));
            var layerSet = new EdaLayerSet(layerId);
            EdaPoint startPosition = EdaPoint.Parse(rd.AttributeAsString("startPosition"));
            EdaPoint endPosition = EdaPoint.Parse(rd.AttributeAsString("endPosition"));
            int thickness = EdaParser.ParseScalar(rd.AttributeAsString("thickness", "0"));
            LineElement.CapStyle lineCap = rd.AttributeAsEnum<LineElement.CapStyle>("lineCap", LineElement.CapStyle.Round);

            rd.NextTag();
            if (!rd.IsEndTag("line"))
                throw new InvalidDataException("Se esperaba </line>");

            return new LineElement {
                LayerSet = layerSet,
                StartPosition = startPosition,
                EndPosition = endPosition,
                Thickness = thickness,
                LineCap = lineCap
            };
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

            EdaLayerId layerId = EdaLayerId.Parse(rd.AttributeAsString("layer"));
            var layerSet = new EdaLayerSet(layerId);
            EdaPoint startPosition = EdaPoint.Parse(rd.AttributeAsString("startPosition"));
            EdaPoint endPosition = EdaPoint.Parse(rd.AttributeAsString("endPosition"));
            int thickness = EdaParser.ParseScalar(rd.AttributeAsString("thickness"));
            EdaAngle angle = EdaAngle.Parse(rd.AttributeAsString("angle"));
            LineElement.CapStyle lineCap = rd.AttributeAsEnum<LineElement.CapStyle>("lineCap", LineElement.CapStyle.Round);

            rd.NextTag();
            if (!rd.IsEndTag("arc"))
                throw new InvalidDataException("Se esperaba </arc>");

            return new ArcElement {
                LayerSet = layerSet,
                StartPosition = startPosition,
                EndPosition = endPosition,
                Thickness = thickness,
                Angle = angle,
                LineCap = lineCap
            };
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

            EdaLayerId layerId = EdaLayerId.Parse(rd.AttributeAsString("layer"));
            var layerSet = new EdaLayerSet(layerId);
            EdaPoint position = EdaPoint.Parse(rd.AttributeAsString("position"));
            int radius = EdaParser.ParseScalar(rd.AttributeAsString("radius"));
            int thickness = EdaParser.ParseScalar(rd.AttributeAsString("thickness", "0"));
            bool filled = rd.AttributeAsBoolean("filled", thickness == 0);

            rd.NextTag();
            if (!rd.IsEndTag("circle"))
                throw new InvalidDataException("Se esperaba </circle>");

            return new CircleElement {
                LayerSet = layerSet,
                Position = position,
                Radius = radius,
                Thickness = thickness,
                Filled = filled
            };
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

            EdaLayerId layerId = EdaLayerId.Parse(rd.AttributeAsString("layer"));
            var layerSet = new EdaLayerSet(layerId);
            EdaPoint position = EdaPoint.Parse(rd.AttributeAsString("position"));
            EdaSize size = EdaSize.Parse(rd.AttributeAsString("size"));
            EdaAngle rotation = EdaAngle.Parse(rd.AttributeAsString("rotation", "0"));
            int thickness = EdaParser.ParseScalar(rd.AttributeAsString("thickness", "0"));
            EdaRatio roundness = EdaRatio.Parse(rd.AttributeAsString("roundness", "0"));
            bool filled = rd.AttributeAsBoolean("filled", thickness == 0);

            rd.NextTag();
            if (!rd.IsEndTag("rectangle"))
                throw new InvalidDataException("Se esperaba </rectangle>");

            return new RectangleElement {
                LayerSet = layerSet,
                Position = position,
                Size = size,
                Roundness = roundness,
                Rotation = rotation,
                Thickness = thickness,
                Filled = filled
            };
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
            EdaLayerId layerId = EdaLayerId.Parse(rd.AttributeAsString("layer"));
            var layerSet = new EdaLayerSet(layerId);
            EdaPoint position = EdaPoint.Parse(rd.AttributeAsString("position"));
            EdaSize size = EdaSize.Parse(rd.AttributeAsString("size"));
            EdaAngle rotation = EdaAngle.Parse(rd.AttributeAsString("rotation", "0"));
            EdaRatio roundness = EdaRatio.Parse(rd.AttributeAsString("roundness", "0"));

            rd.NextTag();
            if (!rd.IsEndTag("spad"))
                throw new InvalidDataException("Se esperaba </spad>");

            return new SmdPadElement {
                Name = name,
                LayerSet = layerSet,
                Position = position,
                Size = size,
                Rotation = rotation,
                Roundness = roundness
            };
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
            EdaLayerId layerId = EdaLayerId.Parse(rd.AttributeAsString("layer"));
            var layerSet = new EdaLayerSet();
            layerSet.Add(layerId);
            EdaPoint position = EdaPoint.Parse(rd.AttributeAsString("position"));
            int size = EdaParser.ParseScalar(rd.AttributeAsString("size"));
            EdaAngle rotation = EdaAngle.Parse(rd.AttributeAsString("rotation", "0"));
            int drill = EdaParser.ParseScalar(rd.AttributeAsString("drill"));
            ThPadElement.ThPadShape shape = rd.AttributeAsEnum<ThPadElement.ThPadShape>("shape", ThPadElement.ThPadShape.Circle);

            rd.NextTag();
            if (!rd.IsEndTag("tpad"))
                throw new InvalidDataException("Se esperaba </tpad>");

            return new ThPadElement {
                Name = name,
                LayerSet = layerSet,
                Position = position,
                Rotation = rotation,
                TopSize = size,
                InnerSize = drill,
                BottomSize = drill,
                Shape = shape,
                Drill = drill
            };
        }

        /// <summary>
        /// Obte un element text
        /// </summary>
        /// <returns>L'element obtingut.</returns>
        /// 
        public static TextElement Text(XmlReaderAdapter rd) {

            if (!rd.IsStartTag("text"))
                throw new InvalidDataException("Se esperaba <text>");

            EdaLayerId layerId = EdaLayerId.Parse(rd.AttributeAsString("layer"));
            var layerSet = new EdaLayerSet(layerId);
            EdaPoint position = EdaPoint.Parse(rd.AttributeAsString("position"));
            EdaAngle rotation = EdaAngle.Parse(rd.AttributeAsString("rotation", "0"));
            int height = EdaParser.ParseScalar(rd.AttributeAsString("height"));
            HorizontalTextAlign horizontalAlign = rd.AttributeAsEnum("horizontalAlign", HorizontalTextAlign.Left);
            VerticalTextAlign verticalAlign = rd.AttributeAsEnum("verticalAlign", VerticalTextAlign.Bottom);
            int thickness = EdaParser.ParseScalar(rd.AttributeAsString("thickness"));
            string value = rd.AttributeAsString("value");

            rd.NextTag();
            if (!rd.IsEndTag("text"))
                throw new InvalidDataException("Se esperaba </text>");

            return new TextElement {
                LayerSet = layerSet,
                Position = position,
                Rotation = rotation,
                Height = height,
                Thickness = thickness,
                HorizontalAlign = horizontalAlign,
                VerticalAlign = verticalAlign,
                Value = value
            };
        }

        /// <summary>
        /// Obte un element Hole
        /// </summary>
        /// <returns>L'element obtigut.</returns>
        /// 
        public static HoleElement Hole(XmlReaderAdapter rd) {

            if (!rd.IsStartTag("hole"))
                throw new InvalidDataException("Se esperaba <hole>");

            var position = EdaPoint.Parse(rd.AttributeAsString("position"));
            var drill = EdaParser.ParseScalar(rd.AttributeAsString("drill"));

            rd.NextTag();
            if (!rd.IsEndTag("hole"))
                throw new InvalidDataException("Se esperaba </hole>");

            return new HoleElement {
                LayerSet = new EdaLayerSet(EdaLayerId.Holes),
                Position = position,
                Drill = drill
            };
        }

        /// <summary>
        /// Obte un element regio
        /// </summary>
        /// <returns>L'element obtingut.</returns>
        /// 
        public static RegionElement Region(XmlReaderAdapter rd) {

            if (!rd.IsStartTag("region"))
                throw new InvalidDataException("Se esperaba <region>");

            EdaLayerId layerId = EdaLayerId.Parse(rd.AttributeAsString("layers"));
            var layerSet = new EdaLayerSet(layerId);
            int thickness = rd.AttributeExists("thickness") ?
                EdaParser.ParseScalar(rd.AttributeAsString("thickness")) :
                0;
            bool filled = rd.AttributeAsBoolean("filled", thickness == 0);
            int clearance = EdaParser.ParseScalar(rd.AttributeAsString("clearance", "0"));

            var region = new RegionElement {
                LayerSet = layerSet,
                Thickness = thickness,
                Filled = filled,
                Clearance = clearance
            };

            rd.NextTag();

            var segments = new List<EdaArcPoint>();
            while (rd.IsStartTag("segment")) {

                EdaPoint position = EdaPoint.Parse(rd.AttributeAsString("position"));
                EdaAngle angle = EdaAngle.Parse(rd.AttributeAsString("angle", "0"));

                rd.NextTag();
                if (!rd.IsEndTag("segment"))
                    throw new InvalidDataException("Se esperaba </segment>");

                segments.Add(new EdaArcPoint(position, angle));
                rd.NextTag();
            }
            region.AddSegments(segments);

            if (!rd.IsEndTag("region"))
                throw new InvalidDataException("Se esperaba </region>");

            return region;
        }
    }
}
