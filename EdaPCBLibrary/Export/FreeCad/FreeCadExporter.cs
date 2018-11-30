namespace MikroPic.EdaTools.v1.Core.Import.FreeCad {

    using System;
    using System.Globalization;
    using System.IO;
    using System.Xml;

    public class FreeCadGenerator {

        public void Generate(string fileName, XmlNode node) {

            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
                Generate(stream, node);
        }

        public void Generate(Stream stream, XmlNode node) {

            using (StreamWriter writer = new StreamWriter(stream)) {

                writer.WriteLine("import Part");
                writer.WriteLine();

/*                writer.WriteLine("err = 0.05;");
                writer.WriteLine("err2 = 2 * err;");
                writer.WriteLine();

                writer.WriteLine("module pcb_board() {");
                GenerateBoard(writer, node);
                writer.WriteLine("}");
                writer.WriteLine();

                writer.WriteLine("module pcb_components() {");
                GenerateComponents(writer, node);
                writer.WriteLine("}");
                writer.WriteLine();

                writer.WriteLine("module pcb_vias() {");
                GenerateVias(writer, node);
                writer.WriteLine("}");
                writer.WriteLine();

                writer.WriteLine("pcb_board();");*/
                writer.WriteLine("pcb_components();");
                //writer.WriteLine("pcb_vias();");

            }
        }

        private void GenerateBoard(TextWriter writer, XmlNode node) {

            // Genera la placa
            //
            bool first = true;
            foreach (XmlNode elementNode in node.SelectNodes("plain/wire")) {

                double x1 = 0;
                double y1 = 0;
                double x2 = 0;
                double y2 = 0;

                if (first) {
                    first = false;
                    x1 = Double.Parse(elementNode.Attributes["x1"].Value, CultureInfo.InvariantCulture);
                    y1 = Double.Parse(elementNode.Attributes["y1"].Value, CultureInfo.InvariantCulture);

                    writer.Write(String.Format(CultureInfo.InvariantCulture, "    p = [[{0}, {1}]", x1, y1));
                }

                x2 = Double.Parse(elementNode.Attributes["x2"].Value, CultureInfo.InvariantCulture);
                y2 = Double.Parse(elementNode.Attributes["y2"].Value, CultureInfo.InvariantCulture);

                writer.Write(String.Format(CultureInfo.InvariantCulture, ", [{0}, {1}]", x2, y2));
            }

            writer.WriteLine("];");
            writer.WriteLine("    difference() {");
            writer.WriteLine(String.Format(CultureInfo.InvariantCulture, "        board(p, {0});", "1.55"));

            // Genera els forats de la placa
            //
            foreach (XmlNode elementNode in node.SelectNodes("elements/element[@library='holes']")) {

                double x = 0;
                double y = 0;

                x = Double.Parse(elementNode.Attributes["x"].Value, CultureInfo.InvariantCulture);
                y = Double.Parse(elementNode.Attributes["y"].Value, CultureInfo.InvariantCulture);
                string package = elementNode.Attributes["package"].Value;

                writer.Write("    ");
                writer.Write(String.Format(CultureInfo.InvariantCulture, "    translate([{0}, {1}, -err]) ", x, y));
                writer.WriteLine(String.Format(CultureInfo.InvariantCulture, "cylinder(d = {0}, h = {1} + err2);", 3.6, 1.55));
            }

            // Genera les vias
            //
            foreach (XmlNode viaNode in node.SelectNodes("signals/signal/via")) {

                double x;
                double y;
                double drill = 0;

                x = Double.Parse(viaNode.Attributes["x"].Value, CultureInfo.InvariantCulture);
                y = Double.Parse(viaNode.Attributes["y"].Value, CultureInfo.InvariantCulture);
                if (viaNode.Attributes["drill"] != null)
                    drill = Double.Parse(viaNode.Attributes["drill"].Value, CultureInfo.InvariantCulture);

                writer.Write("    ");
                writer.Write(String.Format(CultureInfo.InvariantCulture, "    translate([{0}, {1}, -err]) ", x, y));
                writer.Write(String.Format(CultureInfo.InvariantCulture, "cylinder(d = {0}, h = {1} + err2);", drill, 1.55));
                writer.WriteLine();
            }

            writer.WriteLine("    }");
        }        

        private void GenerateComponents(TextWriter writer, XmlNode node) {

            foreach (XmlNode elementNode in node.SelectNodes("elements/element")) {

                double x = 0;
                double y = 0;
                double r = 0;

                x = Double.Parse(elementNode.Attributes["x"].Value, CultureInfo.InvariantCulture);
                y = Double.Parse(elementNode.Attributes["y"].Value, CultureInfo.InvariantCulture);

                if (elementNode.Attributes["rot"] != null) {
                    string rot = elementNode.Attributes["rot"].Value;
                    if (rot.StartsWith("R"))
                        r = Double.Parse(rot.Substring(1), CultureInfo.InvariantCulture);
                    else if (rot.StartsWith("MR"))
                        r = Double.Parse(rot.Substring(2), CultureInfo.InvariantCulture);
                }

                string library = elementNode.Attributes["library"].Value;
                string package = elementNode.Attributes["package"].Value;

                writer.Write("    ");
                writer.Write(String.Format(CultureInfo.InvariantCulture, "translate([{0}, {1}, 0]) ", x, y));
                if (r != 0)
                    writer.Write(String.Format(CultureInfo.InvariantCulture, "rotate([0, 0, {0}]) ", r));

                writer.Write("package(\"{0}:{1}\");", library, package);
                writer.WriteLine();
            }
        }

        private void GenerateVias(StreamWriter writer, XmlNode node) {

            foreach (XmlNode viaNode in node.SelectNodes("signals/signal/via")) {

                double x;
                double y;
                double drill = 0;
                double diameter = 0;

                x = Double.Parse(viaNode.Attributes["x"].Value, CultureInfo.InvariantCulture);
                y = Double.Parse(viaNode.Attributes["y"].Value, CultureInfo.InvariantCulture);
                if (viaNode.Attributes["drill"] != null)
                    drill  = Double.Parse(viaNode.Attributes["drill"].Value, CultureInfo.InvariantCulture);
                if (viaNode.Attributes["diameter"] != null)
                    diameter = Double.Parse(viaNode.Attributes["diameter"].Value, CultureInfo.InvariantCulture);

                writer.Write("    ");
                writer.Write(String.Format(CultureInfo.InvariantCulture, "translate([{0}, {1}, 0]) ", x, y));
                writer.Write(String.Format(CultureInfo.InvariantCulture, "via({0}, {1});", drill, diameter));
                writer.WriteLine();
            }
        }
    }
}
