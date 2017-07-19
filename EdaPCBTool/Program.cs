namespace Eda.PCBTool {

    using Eda.PCB.Model;
    using Eda.PCB.Model.IO;

    class Program {
       
        static void Main(string[] args) {

            Package package = PackageBuilder.DualInLine(
                "SO8", 
                "8-Lead Plastic Small Outline (SN) - Narrow, 3.90 mm body [SOIC]", 
                8,                       // pins
                0.6, 1.55, 0.3,          // pad size, roundness
                1.27, 5.4,               // pitch, row space
                4.90, 3.50);             // package size

            PackageWriter writer = new PackageWriter();
            writer.Write(package, @"..\..\..\Data\provesEDA.xml");

            PackageReader reader = new PackageReader();
            reader.Read(@"..\..\..\Data\provesEDA.xml");
        }
    }
}
