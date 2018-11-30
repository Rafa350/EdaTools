namespace MikroPic.EdaTools.v1.HDCCompiler {

    using System.IO;
    using System.Collections.Generic;
    using MikroPic.EdaTools.v1.Hdc.Ast;
    using MikroPic.EdaTools.v1.Hdc.Compiler;

    class Program {
        static void Main(string[] args) {


            Parser parser = new Parser();
            using (TextReader reader = new StreamReader(@"..\..\Data\circuit.txt")) {
                IEnumerable<EntityNode> entities = parser.Parse(reader);
                Compiler compiler = new Compiler();
                compiler.Compile(entities);
            }
        }
    }
}
