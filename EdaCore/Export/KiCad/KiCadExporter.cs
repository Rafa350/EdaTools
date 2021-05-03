using System;
using System.Globalization;
using System.IO;
using System.Text;
using MikroPic.EdaTools.v1.Core.Model.Board;
using MikroPic.EdaTools.v1.Core.Model.Board.Elements;
using MikroPic.EdaTools.v1.Core.Model.Board.Visitors;

namespace MikroPic.EdaTools.v1.Core.Export.KiCad {

    public sealed class KiCadExporter: IExporter {

        private sealed class Visitor: DefaultBoardVisitor {

            private const double scale = 1000000.0;
            private readonly TextWriter _writer;

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
                    .AppendFormat("(descr \"{0}\")", component.Description);
                _writer.WriteLine(sb.ToString());

                if (component.HasElements)
                    foreach (var element in component.Elements)
                        element.AcceptVisitor(this);

                _writer.WriteLine(')');
            }

            /// <summary>
            /// Visita una linia
            /// </summary>
            /// <param name="line">La linia</param>
            /// 
            public override void Visit(LineElement line) {

                var sb = new StringBuilder()
                    .Append("(fp_line ")
                    .AppendFormat(CultureInfo.InvariantCulture, "(start {0} {1}) ", line.StartPosition.X / scale, line.StartPosition.Y / scale)
                    .AppendFormat(CultureInfo.InvariantCulture, "(end {0} {1}) ", line.EndPosition.X / scale, line.EndPosition.Y / scale)
                    .AppendFormat("(layer {0}) ", GetLayerNames(line.LayerSet))
                    .AppendFormat(CultureInfo.InvariantCulture, "(width {0}))", line.Thickness / scale);
                _writer.WriteLine(sb.ToString());
            }

            /// <summary>
            /// Visita un rectangle
            /// </summary>
            /// <param name="rectangle">El rectangle</param>
            /// 
            public override void Visit(RectangleElement rectangle) {
                
                int x1 = rectangle.Position.X - (rectangle.Size.Width / 2);
                int y1 = rectangle.Position.Y - (rectangle.Size.Height / 2);
                int x2 = x1 + rectangle.Size.Width;
                int y2 = y1 + rectangle.Size.Height;

                var sb = new StringBuilder()
                    .Append("(fp_poly ")
                    .AppendFormat(CultureInfo.InvariantCulture, "(pts (xy {0} {1}) ", x1 / scale, y1 / scale)
                    .AppendFormat(CultureInfo.InvariantCulture, "(xy {0} {1}) ", x2 / scale, y1 / scale)
                    .AppendFormat(CultureInfo.InvariantCulture, "(xy {0} {1}) ", x2 / scale, y2 / scale)
                    .AppendFormat(CultureInfo.InvariantCulture, "(xy {0} {1})) ", x1 / scale, y2 / scale)
                    .AppendFormat("(layer {0}) ", GetLayerNames(rectangle.LayerSet))
                    .AppendFormat(CultureInfo.InvariantCulture, "(width {0}))", rectangle.Thickness / scale);
                _writer.WriteLine(sb.ToString());
            }

            /// <summary>
            /// Visita un cercle
            /// </summary>
            /// <param name="circle">El cercle</param>
            /// 
            public override void Visit(CircleElement circle) {

                var sb = new StringBuilder()
                    .AppendFormat("(fp_circle ")
                    .AppendFormat(CultureInfo.InvariantCulture, "(center {0} {1}) ", circle.Position.X / scale, -circle.Position.Y / scale)
                    .AppendFormat(CultureInfo.InvariantCulture, "(end {0} {1}) ", (circle.Position.X + circle.Radius) / scale, -circle.Position.Y / scale)
                    .AppendFormat(CultureInfo.InvariantCulture, "(width {0})) ", circle.Thickness / scale);
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

                if (value == ">NAME") {
                    value = "REF**";
                    type = "reference";
                }

                var sb = new StringBuilder()
                    .AppendFormat("(fp_text {0} \"{1}\" ", type, value)
                    .AppendFormat(CultureInfo.InvariantCulture, "(at {0} {1} {2}) ", text.Position.X / scale, -text.Position.Y / scale, text.Rotation)
                    .AppendFormat("(layer {0}) ", GetLayerNames(text.LayerSet))
                    .AppendLine()
                    .Append("  (effects ")
                    .Append("(font ")
                    .AppendFormat("(size {0} {1}) ", 1, 1)
                    .AppendFormat(CultureInfo.InvariantCulture, "(thickness {0})) ", text.Thickness / scale)
                    .AppendFormat("(justify {0} {1})", "left", "bottom")
                    .AppendLine(")")
                    .Append(")");
                _writer.WriteLine(sb);
            }

            /// <summary>
            /// Visita un pad smd
            /// </summary>
            /// <param name="pad">El pad</param>
            /// 
            public override void Visit(SmdPadElement pad) {

                var sb = new StringBuilder()
                    .AppendFormat("(pad {0} smd roundrect ", pad.Name)
                    .AppendFormat(CultureInfo.InvariantCulture, "(at {0} {1} {2}) ", pad.Position.X / scale, pad.Position.Y / scale, pad.Rotation.ToDegrees)
                    .AppendFormat(CultureInfo.InvariantCulture, "(size {0} {1}) ", pad.Size.Width / scale, pad.Size.Height / scale)
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

                var sb = new StringBuilder()
                    .AppendFormat("(pad {0} thru_hole circle ", pad.Name)
                    .AppendFormat(CultureInfo.InvariantCulture, "(at {0} {1} {2}) ", pad.Position.X / scale, pad.Position.Y / scale, pad.Rotation.ToDegrees)
                    .AppendFormat(CultureInfo.InvariantCulture, "(size {0} {1}) ", pad.TopSize / scale, pad.TopSize / scale)
                    .AppendFormat(CultureInfo.InvariantCulture, "(drill {0}) ", pad.Drill / scale)
                    .AppendFormat("(layers {0})) ", GetLayerNames(pad.LayerSet));
                _writer.WriteLine(sb);
            }

            public override void Visit(HoleElement hole) {

                var sb = new StringBuilder()
                    .Append("(pad \"\" np_thru_hole circle ")
                    .AppendFormat(CultureInfo.InvariantCulture, "(at {0} {1}) ", hole.Position.X / scale, hole.Position.Y / scale)
                    .AppendFormat(CultureInfo.InvariantCulture, "(size {0} {0}) ", hole.Drill / scale)
                    .AppendFormat(CultureInfo.InvariantCulture, "(drill {0}) ", hole.Drill / scale)
                    .Append("(layers *.Cu *.SilkS *.Mask))");
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
