using EdaComponentGenerator.Generators;

namespace EdaComponentGenerator {

    class Program {

        static void Main(string[] args) {

            var generator = new DilGenerator();
            var componment = generator.Generate();
        }
    }
}
