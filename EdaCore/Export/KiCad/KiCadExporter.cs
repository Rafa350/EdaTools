using MikroPic.EdaTools.v1.Base.Geometry.Utils;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;
using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace MikroPic.EdaTools.v1.Core.Export.KiCad {

    public sealed class KiCadExporter: IEdaExporter {

        private sealed class Visitor: EdaDefaultBoardVisitor {

            private const double _scale = 1000000.0;
            private readonly TextWriter _writer;
            private readonly Matrix2D _m = Matrix2D.CreateScale(1 / _scale, -1 / _scale);
            private EdaComponent _currentComponent;

            /// <summary>
            /// Contructor
            /// </summary>
            /// <param name="writer">El objecte per escriure en el stream.</param>
            /// 
            public Visitor(TextWriter writer) {

                _writer = writer;
            }

            /// <summary>
            /// Visita un component.
            /// </summary>
            /// <param name="component">El component.</param>
            /// 
            public override void Visit(EdaComponent component) {

                var sb = new StringBuilder()
                    .AppendFormat("(module \"{0}\" (layer F.Cu) (tedit 0) ", component.Name)
                    .AppendLine()
                    .AppendFormat("  (descr \"{0}\")", component.Description);
                _writer.WriteLine(sb.ToString());

                if (component.HasElements) {
                    _currentComponent = component;
                    foreach (var element in component.Elements)
                        element.AcceptVisitor(this);
                }

                _writer.WriteLine(')');
            }

            /// <summary>
            /// Visita un element.
            /// </summary>
            /// <param name="element">L'element.</param>
            /// 
            public override void Visit(EdaLineElement element) {

                var start = new Vector2D(element.StartPosition.X, element.StartPosition.Y) * _m;
                var end = new Vector2D(element.EndPosition.X, element.EndPosition.Y) * _m;

                var sb = new StringBuilder()
                    .Append("  (fp_line ")
                    .AppendFormat(CultureInfo.InvariantCulture, "(start {0} {1}) ", start.X, start.Y)
                    .AppendFormat(CultureInfo.InvariantCulture, "(end {0} {1}) ", end.X, end.Y)
                    .AppendFormat("(layer {0}) ", GetLayerNames(element.LayerSet))
                    .AppendFormat(CultureInfo.InvariantCulture, "(width {0}))", element.Thickness / _scale);

                _writer.WriteLine(sb.ToString());
            }

            /// <summary>
            /// Visita un element
            /// </summary>
            /// <param name="element">L'element.</param>
            /// 
            public override void Visit(EdaRectangleElement element) {

                var start = new Vector2D(
                    element.Position.X - (element.Size.Width / 2),
                    element.Position.Y - (element.Size.Height / 2)) * _m;
                var end = new Vector2D(
                    element.Position.X + element.Size.Width / 2,
                    element.Position.Y + element.Size.Height / 2) * _m;

                var sb = new StringBuilder()
                    .Append("  (fp_rect ")
                    .AppendFormat(CultureInfo.InvariantCulture, "(start {0} {1}) ", start.X, start.Y)
                    .AppendFormat(CultureInfo.InvariantCulture, "(end {0} {1}) ", end.X, end.Y)
                    .AppendFormat("(layer {0}) ", GetLayerNames(element.LayerSet))
                    .AppendFormat(CultureInfo.InvariantCulture, "(width {0}))", element.Thickness / _scale);

                _writer.WriteLine(sb.ToString());
            }

            /// <summary>
            /// Visita un element.
            /// </summary>
            /// <param name="element">L'element.</param>
            /// 
            public override void Visit(EdaCircleElement element) {

                var center = new Vector2D(
                    element.Position.X,
                    element.Position.Y) * _m;
                var end = new Vector2D(
                    element.Position.X + element.Radius,
                    element.Position.Y) * _m;

                var sb = new StringBuilder()
                    .AppendFormat("  (fp_circle ")
                    .AppendFormat(CultureInfo.InvariantCulture, "(center {0} {1}) ", center.X, center.Y)
                    .AppendFormat(CultureInfo.InvariantCulture, "(end {0} {1}) ", end.X, end.Y)
                    .AppendFormat("(layer {0}) ", GetLayerNames(element.LayerSet))
                    .AppendFormat(CultureInfo.InvariantCulture, "(width {0})) ", element.Thickness / _scale);
                _writer.WriteLine(sb.ToString());
            }

            /// <summary>
            /// Visita un element
            /// </summary>
            /// <param name="element">L'element.</param>
            /// 
            public override void Visit(EdaTextElement element) {

                var type = "value";
                var value = element.Value;

                if (value == "{NAME}") {
                    value = "REF**";
                    type = "reference";
                }
                else if (value == "{VALUE}") {
                    value = _currentComponent.Name;
                    type = "value";
                }

                var sb = new StringBuilder()
                    .AppendFormat("  (fp_text {0} \"{1}\" ", type, value)
                    .AppendFormat(CultureInfo.InvariantCulture, "(at {0} {1} {2}) ", element.Position.X / _scale, -element.Position.Y / _scale, element.Rotation.AsDegrees)
                    .AppendFormat("(layer {0}) ", GetLayerNames(element.LayerSet))
                    .AppendLine()
                    .Append("    (effects ")
                    .Append("(font ")
                    .AppendFormat("(size {0} {1}) ", 1, 1)
                    .AppendFormat(CultureInfo.InvariantCulture, "(thickness {0})) ", element.Thickness / _scale)
                    .AppendFormat("(justify {0} {1})", "left", "bottom")
                    .AppendLine(")")
                    .Append("  )");
                _writer.WriteLine(sb);
            }

            /// <summary>
            /// Visita un element.
            /// </summary>
            /// <param name="element">L'element.<param>
            /// 
            public override void Visit(EdaSmdPadElement element) {

                var sb = new StringBuilder()
                    .AppendFormat("  (pad {0} smd roundrect ", element.Name)
                    .AppendFormat(CultureInfo.InvariantCulture, "(at {0} {1} {2}) ",
                        element.Position.X / _scale,
                        element.Position.Y / -_scale,
                        element.Rotation.AsDegrees)
                    .AppendFormat(CultureInfo.InvariantCulture, "(size {0} {1}) ", element.Size.Width / _scale, element.Size.Height / _scale)
                    .AppendFormat("(layers {0}) ", GetLayerNames(element.LayerSet))
                    .AppendFormat(CultureInfo.InvariantCulture, "(roundrect_rratio {0}))", element.CornerRatio.Value / 2000.0);

                _writer.WriteLine(sb);
            }

            /// <summary>
            /// Visita un element.
            /// </summary>
            /// <param name="element">L'element.</param>
            /// 
            public override void Visit(EdaThPadElement element) {

                var sb = new StringBuilder()
                    .AppendFormat("  (pad {0} thru_hole {1} ", 
                        element.Name,
                        element.CornerRatio.IsZero ? "rect" : "roundrect")
                    .AppendFormat(CultureInfo.InvariantCulture, "(at {0} {1} {2}) ",
                        element.Position.X / _scale,
                        element.Position.Y / -_scale,
                        element.Rotation.AsDegrees);

                if (!element.CornerRatio.IsZero) {
                    if (element.CornerShape == EdaThPadElement.ThPadCornerShape.Round)
                        sb.AppendFormat(CultureInfo.InvariantCulture, "(roundrect_rratio {0}) ",
                            element.CornerRatio.AsPercent / 2.0);
                    else
                        sb.AppendFormat(CultureInfo.InvariantCulture, "(roundrect_rratio 0) (chamfer_ratio {0}) (chamfer top_left top_right bottom_left bottom_right) ",
                            element.CornerRatio.AsPercent / 2.0);
                }

                sb
                    .AppendFormat(CultureInfo.InvariantCulture, "(size {0} {1}) ", element.TopSize.Width / _scale, element.TopSize.Height / _scale)
                    .AppendFormat(CultureInfo.InvariantCulture, "(drill {0}) ", element.DrillDiameter / _scale)
                    .Append("(layers *.Cu *.Mask))");

                _writer.WriteLine(sb);
            }

            /// <summary>
            /// Visita un element.
            /// </summary>
            /// <param name="element">L'element.</param>
            /// 
            public override void Visit(EdaCircleHoleElement element) {

                var center = new Vector2D(
                    element.Position.X,
                    element.Position.Y) * _m;

                var sb = new StringBuilder()
                    .Append("  (pad \"\" np_thru_hole circle ")
                    .AppendFormat(CultureInfo.InvariantCulture, "(at {0} {1}) ", center.X, center.Y)
                    .AppendFormat(CultureInfo.InvariantCulture, "(size {0} {0}) ", element.Diameter / _scale)
                    .AppendFormat(CultureInfo.InvariantCulture, "(drill {0}) ", element.Diameter / _scale)
                    .Append("(layers *.Cu *.Mask))");
                _writer.WriteLine(sb.ToString());
            }

            /// <summary>
            /// Visita un element.
            /// </summary>
            /// <param name="element">L'element.</param>
            /// 
            public override void Visit(EdaLineHoleElement element) {
            }

            private static string GetLayerNames(EdaLayerSet layerSet) {

                var sb = new StringBuilder();

                bool first = true;
                foreach (EdaLayerId layerId in layerSet.Items) {
                    if (first)
                        first = false;
                    else
                        sb.Append(' ');
                    sb.Append(GetLayerName(layerId));
                }

                return sb.ToString();
            }

            /// <summary>
            /// Obte el nom de les capes
            /// </summary>
            /// <param name="layerId">La capa.</param>
            /// <returns>El nom de les capa.</returns>
            /// 
            private static string GetLayerName(EdaLayerId layerId) {

                var layerName = layerId.ToString();
                switch (layerName) {
                    case "Top.Copper":
                        layerName = "F.Cu";
                        break;

                    case "Bottom.Copper":
                        layerName = "B.Cu";
                        break;

                    case "Top.Stop":
                        layerName = "F.Mask";
                        break;

                    case "Bottom.Stop":
                        layerName = "B.Mask";
                        break;

                    case "Top.Cream":
                        layerName = "F.Paste";
                        break;

                    case "Bottom.Cream":
                        layerName = "B.Paste";
                        break;

                    case "Top.Glue":
                        layerName = "F.Adhes";
                        break;

                    case "Bottom.Glue":
                        layerName = "B.Adhes";
                        break;

                    case "Top.Names":
                        layerName = "F.SilkS";
                        break;

                    case "Bottom.Names":
                        layerName = "B.SilkS";
                        break;

                    case "Top.Values":
                        layerName = "F.Fab";
                        break;

                    case "Bottom.Values":
                        layerName = "B.Fab";
                        break;

                    case "Top.Place":
                        layerName = "F.SilkS";
                        break;

                    case "Bottom.Place":
                        layerName = "B.SilkS";
                        break;

                    case "Top.Keepout":
                        layerName = "F.CrtYd";
                        break;

                    case "Bottom.Keepout":
                        layerName = "B.CrtYd";
                        break;

                    case "Top.Document":
                        layerName = "F.Fab";
                        break;

                    case "Bottom.Document":
                        layerName = "B.Fab";
                        break;
                }

                return layerName;
            }
        }


        /// <inheritdoc/>
        /// 
        public void WriteLibrary(string targetPath, EdaLibrary library) {

            if (!Directory.Exists(targetPath))
                Directory.CreateDirectory(targetPath);

            foreach (var component in library.Components) {

                var fileName = Path.Combine(targetPath, String.Format("{0}.kicad_mod", component.Name));

                using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                    using (var writer = new StreamWriter(stream)) {
                        var visitor = new Visitor(writer);
                        visitor.Visit(component);
                    }
                }
            }
        }
    }
}
