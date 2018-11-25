namespace MikroPic.EdaTools.v1.HDCCompiler {

    using System.IO;
    using MikroPic.EdaTools.v1.Hdc.Compiler;

    class Program {
        static void Main(string[] args) {

            Parser parser = new Parser();

            using (TextReader reader = new StreamReader(@"..\..\Data\circuit.txt"))
                parser.Parse(reader);
        }
    }
}
