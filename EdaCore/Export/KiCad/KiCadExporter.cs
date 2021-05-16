using System;
using System.Globalization;
using System.IO;
using System.Text;
using MikroPic.EdaTools.v1.Base.Geometry.Utils;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;

namespace MikroPic.EdaTools.v1.Core.Export.KiCad {

    public sealed class KiCadExporter: IExporter {

        private sealed class Visitor: DefaultBoardVisitor {

            private const double _scale = 1000000.0;
            private readonly TextWriter _writer;
            private readonly Matrix2D _m = Matrix2D.CreateScale(1 / _scale, -1 / _scale);
            private Component _currentComponent;

            /// <summary>
            /// Contructor
            /// </summary>
            /// <param name="writer">El objecte per escriure en el stream.</param>
            /// 
            public Visitor(TextWriter writer) {

                _writer = writer;
            }

            /// <summary>
            /// Visita un component
            /// </summary>
            /// <param name="component">El component</param>
            /// 
            public override void Visit(Component component) {

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
            /// Visita una linia
            /// </summary>
            /// <param name="line">La linia</param>
            /// 
            public override void Visit(LineElement line) {

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
            /// Visita un rectangle
            /// </summary>
            /// <param name="rectangle">El rectangle</param>
            /// 
            public override void Visit(RectangleElement rectangle) {
                
                var start = new Vector2D(
                    rectangle.Position.X - (rectangle.Size.Width / 2),
                    rectangle.Position.Y - (rectangle.Size.Height / 2)) * _m;
                var end = new Vector2D(
                    rectangle.Position.X + rectangle.Size.Width,
                    rectangle.Position.Y + rectangle.Size.Height) * _m;

                var sb = new StringBuilder()
                    .Append("  (fp_poly ")
                    .AppendFormat(CultureInfo.InvariantCulture, "(pts (xy {0} {1}) ", start.X, start.Y)
                    .AppendFormat(CultureInfo.InvariantCulture, "(xy {0} {1}) ", end.X, start.Y)
                    .AppendFormat(CultureInfo.InvariantCulture, "(xy {0} {1}) ", end.X, end.Y)
                    .AppendFormat(CultureInfo.InvariantCulture, "(xy {0} {1})) ", start.X, end.Y)
                    .AppendFormat("(layer {0}) ", GetLayerNames(rectangle.LayerSet))
                    .AppendFormat(CultureInfo.InvariantCulture, "(width {0}))", rectangle.Thickness / _scale);

                _writer.WriteLine(sb.ToString());
            }

            /// <summary>
            /// Visita un cercle
            /// </summary>
            /// <param name="circle">El cercle</param>
            /// 
            public override void Visit(CircleElement circle) {

                var center = new Vector2D(
                    circle.Position.X,
                    circle.Position.Y) * _m;
                var end = new Vector2D(
                    circle.Position.X + circle.Radius,
                    circle.Position.Y) * _m;

                var sb = new StringBuilder()
                    .AppendFormat("  (fp_circle ")
                    .AppendFormat(CultureInfo.InvariantCulture, "(center {0} {1}) ", center.X, center.Y)
                    .AppendFormat(CultureInfo.InvariantCulture, "(end {0} {1}) ", end.X, end.Y)
                    .AppendFormat(CultureInfo.InvariantCulture, "(width {0})) ", circle.Thickness / _scale);

                _writer.WriteLine(sb.ToString());
            }

            /// <summary>
            /// Visita un text
            /// </summary>
            /// <param name="text">El text</param>
            /// 
            public override void Visit(TextElement text) {

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
                    .AppendFormat(CultureInfo.InvariantCulture, "(at {0} {1} {2}) ", text.Position.X / _scale, -text.Position.Y / _scale, text.Rotation)
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
            /// <param name="pad">El pad</param>
            /// 
            public override void Visit(SmdPadElement pad) {

                var position = new Vector2D(pad.Position.X, pad.Position.Y) * _m;

                var sb = new StringBuilder()
                    .AppendFormat("  (pad {0} smd roundrect ", pad.Name)
                    .AppendFormat(CultureInfo.InvariantCulture, "(at {0} {1} {2}) ", position.X, position.Y, pad.Rotation.ToDegrees)
                    .AppendFormat(CultureInfo.InvariantCulture, "(size {0} {1}) ", pad.Size.Width / _scale, pad.Size.Height / _scale)
                    .AppendFormat("(layers {0}) ", GetLayerNames(pad.LayerSet))
                    .AppendFormat(CultureInfo.InvariantCulture, "(roundrect_rratio {0}))", pad.Roundness.Value / 2000.0);

                _writer.WriteLine(sb);
            }

            /// <summary>
            /// Visita un pad thruhole
            /// </summary>
            /// <param name="pad">El pad</param>
            /// 
            public override void Visit(ThPadElement pad) {

                var position = new Vector2D(pad.Position.X, pad.Position.Y) * _m;

                var sb = new StringBuilder()
                    .AppendFormat("  (pad {0} thru_hole circle ", pad.Name)
                    .AppendFormat(CultureInfo.InvariantCulture, "(at {0} {1} {2}) ", position.X, position.Y, pad.Rotation.ToDegrees)
                    .AppendFormat(CultureInfo.InvariantCulture, "(size {0} {1}) ", pad.TopSize / _scale, pad.TopSize / _scale)
                    .AppendFormat(CultureInfo.InvariantCulture, "(drill {0}) ", pad.Drill / _scale)
                    .Append("(layers *.Cu *.Mask))");

                _writer.WriteLine(sb);
            }

            public override void Visit(HoleElement hole) {

                var position = new Vector2D(hole.Position.X, hole.Position.Y) * _m;

                var sb = new StringBuilder()
                    .Append("  (pad \"\" np_thru_hole circle ")
                    .AppendFormat(CultureInfo.InvariantCulture, "(at {0} {1}) ", position.X, position.Y)
                    .AppendFormat(CultureInfo.InvariantCulture, "(size {0} {0}) ", hole.Drill / _scale)
                    .AppendFormat(CultureInfo.InvariantCulture, "(drill {0}) ", hole.Drill / _scale)
                    .Append("(layers *.Cu *.Mask))");

                _writer.WriteLine(sb);
            }

            /// <summary>
            /// Obte el nom de les capes
            /// </summary>
            /// <param name="layerSet">Conjunt de capes</param>
            /// <returns>Els noms de les capes</returns>
            /// 
            private string GetLayerNames(LayerSet layerSet) {

                var sb = new StringBuilder();
                foreach (var layerName in layerSet.ToString().Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)) {
                    var newLayerName = layerName;
                    switch (layerName) {
                        case "Top.Copper": 
                            newLayerName = "F.Cu"; 
                            break;

                        case "Bottom.Copper":
                            newLayerName = "B.Cu";
                            break;

                        case "Top.Stop":
                            newLayerName = "F.Mask";
                            break;

                        case "Bottom.Stop":
                            newLayerName = "B.Mask";
                            break;

                        case "Top.Cream":
                            newLayerName = "F.Paste";
                            break;

                        case "Bottom.Cream":
                            newLayerName = "B.Paste";
                            break;

                        case "Top.Glue":
                            newLayerName = "F.Adhes";
                            break;

                        case "Bottom.Glue":
                            newLayerName = "B.Adhes";
                            break;

                        case "Top.Names":
                            newLayerName = "F.SilkS";
                            break;

                        case "Bottom.Names":
                            newLayerName = "B.SilkS";
                            break;

                        case "Top.Values":
                            newLayerName = "F.Fab";
                            break;

                        case "Bottom.Values":
                            newLayerName = "B.Fab";
                            break;

                        case "Top.Place":
                            newLayerName = "F.SilkS";
                            break;

                        case "Bottom.Place":
                            newLayerName = "B.SilkS";
                            break;

                        case "Top.Keepout":
                            newLayerName = "F.CrtYd";
                            break;

                        case "Bottom.Keepout":
                            newLayerName = "B.CrtYd";
                            break;

                        case "Top.Document":
                            newLayerName = "F.Fab";
                            break;

                        case "Bottom.Document":
                            newLayerName = "B.Fab";
                            break;
                    }
                    sb.Append(newLayerName);
                    sb.Append(' ');
                }

                return sb.ToString();
            }
        }


        /// <inheritdoc/>
        /// 
        public void WriteLibrary(string targetPath, Library library) {

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
