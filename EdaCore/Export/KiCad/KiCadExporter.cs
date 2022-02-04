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
            /// Visita un objecte 'EdaComponent'
            /// </summary>
            /// <param name="component">L'objecte a visitar.</param>
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
            /// Visita un objecte 'EdaLineElement'
            /// </summary>
            /// <param name="line">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaLineElement line) {

                var start = new Vector2D(line.StartPosition.X, line.StartPosition.Y) * _m;
                var end = new Vector2D(line.EndPosition.X, line.EndPosition.Y) * _m;

                var sb = new StringBuilder()
                    .Append("  (fp_line ")
                    .AppendFormat(CultureInfo.InvariantCulture, "(start {0} {1}) ", start.X, start.Y)
                    .AppendFormat(CultureInfo.InvariantCulture, "(end {0} {1}) ", end.X, end.Y)
                    .AppendFormat("(layer {0}) ", GetLayerNames(line.LayerSet))
                    .AppendFormat(CultureInfo.InvariantCulture, "(width {0}))", line.Thickness / _scale);

                _writer.WriteLine(sb.ToString());
            }

            /// <summary>
            /// Visita un objecte 'EdaRectangleElement'
            /// </summary>
            /// <param name="rectangle">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaRectangleElement rectangle) {

                var start = new Vector2D(
                    rectangle.Position.X - (rectangle.Size.Width / 2),
                    rectangle.Position.Y - (rectangle.Size.Height / 2)) * _m;
                var end = new Vector2D(
                    rectangle.Position.X + rectangle.Size.Width / 2,
                    rectangle.Position.Y + rectangle.Size.Height / 2) * _m;

                var sb = new StringBuilder()
                    .Append("  (fp_rect ")
                    .AppendFormat(CultureInfo.InvariantCulture, "(start {0} {1}) ", start.X, start.Y)
                    .AppendFormat(CultureInfo.InvariantCulture, "(end {0} {1}) ", end.X, end.Y)
                    .AppendFormat("(layer {0}) ", GetLayerNames(rectangle.LayerSet))
                    .AppendFormat(CultureInfo.InvariantCulture, "(width {0}))", rectangle.Thickness / _scale);

                _writer.WriteLine(sb.ToString());
            }

            /// <summary>
            /// Visita un cercle
            /// </summary>
            /// <param name="circle">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaCircleElement circle) {

                var center = new Vector2D(
                    circle.Position.X,
                    circle.Position.Y) * _m;
                var end = new Vector2D(
                    circle.Position.X + circle.Radius,
                    circle.Position.Y) * _m;

                var sb = new StringBuilder();
                if (circle.IsOnLayer(EdaLayerId.Unplatted)) {
                    sb
                        .Append("  (pad \"\" np_thru_hole circle ")
                        .AppendFormat(CultureInfo.InvariantCulture, "(at {0} {1}) ", center.X, center.Y)
                        .AppendFormat(CultureInfo.InvariantCulture, "(size {0} {0}) ", circle.Diameter / _scale)
                        .AppendFormat(CultureInfo.InvariantCulture, "(drill {0}) ", circle.Diameter / _scale)
                        .Append("(layers *.Cu *.Mask))");
                }
                else {
                    sb
                        .AppendFormat("  (fp_circle ")
                        .AppendFormat(CultureInfo.InvariantCulture, "(center {0} {1}) ", center.X, center.Y)
                        .AppendFormat(CultureInfo.InvariantCulture, "(end {0} {1}) ", end.X, end.Y)
                        .AppendFormat(CultureInfo.InvariantCulture, "(width {0})) ", circle.Thickness / _scale);
                }
                _writer.WriteLine(sb.ToString());
            }

            /// <summary>
            /// Visita un text
            /// </summary>
            /// <param name="text">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaTextElement text) {

                var type = "value";
                var value = text.Value;

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
                    .AppendFormat(CultureInfo.InvariantCulture, "(at {0} {1} {2}) ", text.Position.X / _scale, -text.Position.Y / _scale, text.Rotation.AsDegrees)
                    .AppendFormat("(layer {0}) ", GetLayerNames(text.LayerSet))
                    .AppendLine()
                    .Append("    (effects ")
                    .Append("(font ")
                    .AppendFormat("(size {0} {1}) ", 1, 1)
                    .AppendFormat(CultureInfo.InvariantCulture, "(thickness {0})) ", text.Thickness / _scale)
                    .AppendFormat("(justify {0} {1})", "left", "bottom")
                    .AppendLine(")")
                    .Append("  )");
                _writer.WriteLine(sb);
            }

            /// <summary>
            /// Visita un pad smd
            /// </summary>
            /// <param name="pad">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaSmdPadElement pad) {

                var sb = new StringBuilder()
                    .AppendFormat("  (pad {0} smd roundrect ", pad.Name)
                    .AppendFormat(CultureInfo.InvariantCulture, "(at {0} {1} {2}) ", 
                        pad.Position.X / _scale, 
                        pad.Position.Y / -_scale, 
                        pad.Rotation.AsDegrees)
                    .AppendFormat(CultureInfo.InvariantCulture, "(size {0} {1}) ", pad.Size.Width / _scale, pad.Size.Height / _scale)
                    .AppendFormat("(layers {0}) ", GetLayerNames(pad.LayerSet))
                    .AppendFormat(CultureInfo.InvariantCulture, "(roundrect_rratio {0}))", pad.CornerRatio.Value / 2000.0);

                _writer.WriteLine(sb);
            }

            /// <summary>
            /// Visita un pad thruhole
            /// </summary>
            /// <param name="pad">L'objecte a visitar.</param>
            /// 
            public override void Visit(EdaThPadElement pad) {

                var sb = new StringBuilder()
                    .AppendFormat("  (pad {0} thru_hole circle ", pad.Name)
                    .AppendFormat(CultureInfo.InvariantCulture, "(at {0} {1} {2}) ", 
                        pad.Position.X / _scale, 
                        pad.Position.Y / -_scale, 
                        pad.Rotation.AsDegrees)
                    .AppendFormat(CultureInfo.InvariantCulture, "(size {0} {1}) ", pad.TopSize.Width / _scale, pad.TopSize.Height / _scale)
                    .AppendFormat(CultureInfo.InvariantCulture, "(drill {0}) ", pad.Drill / _scale)
                    .Append("(layers *.Cu *.Mask))");

                _writer.WriteLine(sb);
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
