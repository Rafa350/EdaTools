using MikroPic.EdaTools.v1.Base.Geometry;
using MikroPic.EdaTools.v1.Base.Geometry.Fonts;
using MikroPic.EdaTools.v1.Base.Xml;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.IO;
using System.Collections.Generic;
using System.IO;

namespace MikroPic.EdaTools.v1.Core.Model.Board.IO {

    public static class ElementParser {

        /// <summary>
        /// Obte un element linia 
        /// </summary>
        /// <param name="rd">El objecte per lleigir del stream</param>
        /// <returns>L'element obtingut.</returns>
        /// 
        public static EdaLineElement Line(XmlReaderAdapter rd) {

            if (!rd.IsStartTag("line"))
                throw new InvalidDataException("Se esperaba <line>");

            EdaLayerId layerId = EdaLayerId.Parse(rd.AttributeAsString("layer"));
            var layerSet = new EdaLayerSet(layerId);
            EdaPoint startPosition = EdaParser.ParsePoint(rd.AttributeAsString("startPosition"));
            EdaPoint endPosition = EdaParser.ParsePoint(rd.AttributeAsString("endPosition"));
            int thickness = EdaParser.ParseScalar(rd.AttributeAsString("thickness", "0"));
            EdaLineElement.CapStyle lineCap = rd.AttributeAsEnum<EdaLineElement.CapStyle>("lineCap", EdaLineElement.CapStyle.Round);

            rd.NextTag();
            if (!rd.IsEndTag("line"))
                throw new InvalidDataException("Se esperaba </line>");

            return new EdaLineElement {
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
        public static EdaArcElement Arc(XmlReaderAdapter rd) {

            if (!rd.IsStartTag("arc"))
                throw new InvalidDataException("Se esperaba <arc>");

            EdaLayerId layerId = EdaLayerId.Parse(rd.AttributeAsString("layer"));
            var layerSet = new EdaLayerSet(layerId);
            EdaPoint startPosition = EdaParser.ParsePoint(rd.AttributeAsString("startPosition"));
            EdaPoint endPosition = EdaParser.ParsePoint(rd.AttributeAsString("endPosition"));
            int thickness = EdaParser.ParseScalar(rd.AttributeAsString("thickness"));
            EdaAngle angle = EdaParser.ParseAngle(rd.AttributeAsString("angle"));
            EdaLineElement.CapStyle lineCap = rd.AttributeAsEnum<EdaLineElement.CapStyle>("lineCap", EdaLineElement.CapStyle.Round);

            rd.NextTag();
            if (!rd.IsEndTag("arc"))
                throw new InvalidDataException("Se esperaba </arc>");

            return new EdaArcElement {
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
        public static EdaCircleElement Circle(XmlReaderAdapter rd) {

            if (!rd.IsStartTag("circle"))
                throw new InvalidDataException("Se esperaba <circle>");

            EdaLayerId layerId = EdaLayerId.Parse(rd.AttributeAsString("layer"));
            var layerSet = new EdaLayerSet(layerId);
            EdaPoint position = EdaParser.ParsePoint(rd.AttributeAsString("position"));
            int radius = EdaParser.ParseScalar(rd.AttributeAsString("radius"));
            int thickness = EdaParser.ParseScalar(rd.AttributeAsString("thickness", "0"));
            bool filled = rd.AttributeAsBoolean("filled", thickness == 0);

            rd.NextTag();
            if (!rd.IsEndTag("circle"))
                throw new InvalidDataException("Se esperaba </circle>");

            return new EdaCircleElement {
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
        public static EdaRectangleElement Rectangle(XmlReaderAdapter rd) {

            if (!rd.IsStartTag("rectangle"))
                throw new InvalidDataException("Se esperaba <rectangle>");

            EdaLayerId layerId = EdaLayerId.Parse(rd.AttributeAsString("layer"));
            var layerSet = new EdaLayerSet(layerId);
            EdaPoint position = EdaParser.ParsePoint(rd.AttributeAsString("position"));
            EdaSize size = EdaParser.ParseSize(rd.AttributeAsString("size"));
            EdaAngle rotation = EdaParser.ParseAngle(rd.AttributeAsString("rotation", "0"));
            int thickness = EdaParser.ParseScalar(rd.AttributeAsString("thickness", "0"));
            EdaRatio roundness = EdaParser.ParseRatio(rd.AttributeAsString("roundness", "0"));
            bool filled = rd.AttributeAsBoolean("filled", thickness == 0);

            rd.NextTag();
            if (!rd.IsEndTag("rectangle"))
                throw new InvalidDataException("Se esperaba </rectangle>");

            return new EdaRectangleElement {
                LayerSet = layerSet,
                Position = position,
                Size = size,
                CornerRatio = roundness,
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
        public static EdaSmdPadElement SPad(XmlReaderAdapter rd) {

            if (!rd.IsStartTag("spad"))
                throw new InvalidDataException("Se esperaba <spad>");

            string name = rd.AttributeAsString("name");
            var layerSet = EdaParser.ParseLayerSet(rd.AttributeAsString("layers"));
            EdaPoint position = EdaParser.ParsePoint(rd.AttributeAsString("position"));
            EdaSize size = EdaParser.ParseSize(rd.AttributeAsString("size"));
            EdaAngle rotation = EdaParser.ParseAngle(rd.AttributeAsString("rotation", "0"));
            EdaRatio cornerRatio = EdaParser.ParseRatio(rd.AttributeAsString("cornerRatio", "0"));
            var cornerShape = rd.AttributeAsEnum("cornerShape", EdaSmdPadElement.SmdPadCornerShape.Round);

            rd.NextTag();
            if (!rd.IsEndTag("spad"))
                throw new InvalidDataException("Se esperaba </spad>");

            return new EdaSmdPadElement {
                Name = name,
                LayerSet = layerSet,
                Position = position,
                Size = size,
                Rotation = rotation,
                CornerRatio = cornerRatio,
                CornerShape = cornerShape
            };
        }

        /// <summary>
        /// Obte un element tpad
        /// </summary>
        /// <param name="rd">El objecte per lleigir el srtream</param>
        /// <returns>L'element obtinguc</returns>
        /// 
        public static EdaThPadElement TPad(XmlReaderAdapter rd) {

            if (!rd.IsStartTag("tpad"))
                throw new InvalidDataException("Se esperaba <tpad>");

            string name = rd.AttributeAsString("name");
            EdaLayerId layerId = EdaLayerId.Parse(rd.AttributeAsString("layers"));
            var layerSet = new EdaLayerSet();
            layerSet.Add(layerId);
            EdaPoint position = EdaParser.ParsePoint(rd.AttributeAsString("position"));
            EdaSize size = EdaParser.ParseSize(rd.AttributeAsString("topSize"));
            EdaAngle rotation = EdaParser.ParseAngle(rd.AttributeAsString("rotation", "0"));
            int drill = EdaParser.ParseScalar(rd.AttributeAsString("drill"));

            rd.NextTag();
            if (!rd.IsEndTag("tpad"))
                throw new InvalidDataException("Se esperaba </tpad>");

            return new EdaThPadElement {
                Name = name,
                LayerSet = layerSet,
                Position = position,
                Rotation = rotation,
                TopSize = size,
                InnerSize = size,
                BottomSize = size,
                Drill = drill
            };
        }

        /// <summary>
        /// Obte un element text
        /// </summary>
        /// <returns>L'element obtingut.</returns>
        /// 
        public static EdaTextElement Text(XmlReaderAdapter rd) {

            if (!rd.IsStartTag("text"))
                throw new InvalidDataException("Se esperaba <text>");

            EdaLayerId layerId = EdaLayerId.Parse(rd.AttributeAsString("layer"));
            var layerSet = new EdaLayerSet(layerId);
            EdaPoint position = EdaParser.ParsePoint(rd.AttributeAsString("position"));
            EdaAngle rotation = EdaParser.ParseAngle(rd.AttributeAsString("rotation", "0"));
            int height = EdaParser.ParseScalar(rd.AttributeAsString("height"));
            HorizontalTextAlign horizontalAlign = rd.AttributeAsEnum("horizontalAlign", HorizontalTextAlign.Left);
            VerticalTextAlign verticalAlign = rd.AttributeAsEnum("verticalAlign", VerticalTextAlign.Bottom);
            int thickness = EdaParser.ParseScalar(rd.AttributeAsString("thickness"));
            string value = rd.AttributeAsString("value");

            rd.NextTag();
            if (!rd.IsEndTag("text"))
                throw new InvalidDataException("Se esperaba </text>");

            return new EdaTextElement {
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
        /// Obte un element regio
        /// </summary>
        /// <returns>L'element obtingut.</returns>
        /// 
        public static EdaRegionElement Region(XmlReaderAdapter rd) {

            if (!rd.IsStartTag("region"))
                throw new InvalidDataException("Se esperaba <region>");

            EdaLayerId layerId = EdaLayerId.Parse(rd.AttributeAsString("layer"));
            var layerSet = new EdaLayerSet(layerId);
            int thickness = rd.AttributeExists("thickness") ?
                EdaParser.ParseScalar(rd.AttributeAsString("thickness")) :
                0;
            bool filled = rd.AttributeAsBoolean("filled", thickness == 0);
            int clearance = EdaParser.ParseScalar(rd.AttributeAsString("clearance", "0"));

            var region = new EdaRegionElement {
                LayerSet = layerSet,
                Thickness = thickness,
                Filled = filled,
                Clearance = clearance
            };

            rd.NextTag();

            var segments = new List<EdaArcPoint>();
            while (rd.IsStartTag("segment")) {

                EdaPoint position = EdaParser.ParsePoint(rd.AttributeAsString("position"));
                EdaAngle angle = EdaParser.ParseAngle(rd.AttributeAsString("angle", "0"));

                rd.NextTag();
                if (!rd.IsEndTag("segment"))
                    throw new InvalidDataException("Se esperaba </segment>");

                segments.Add(new EdaArcPoint(position, angle));
                rd.NextTag();
            }
            region.Segments = segments;

            if (!rd.IsEndTag("region"))
                throw new InvalidDataException("Se esperaba </region>");

            return region;
        }
    }
}
