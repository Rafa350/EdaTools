namespace MikroPic.EdaTools.v1.Cam {

    using MikroPic.EdaTools.v1.Cam.Gerber;
    using MikroPic.EdaTools.v1.Cam.Ipcd356;
    using MikroPic.EdaTools.v1.Pcb.Model;
    using System;
    using System.Text;
    using System.IO;
    using System.Collections.Generic;

    public sealed class CAMGenerator {

        /// <summary>
        /// Genera els fitxers CAM
        /// </summary>
        /// <param name="board">La placa.</param>
        /// <param name="folder">La carpeta on deixar el resultat.</param>
        /// <param name="name">El nom base del fitxer.</param>
        /// 
        public void Generate(Board board, string folder, string name) {

            if (board == null)
                throw new ArgumentNullException("board");

            GenerateImageGerbers(board, folder, name);
            GenerateDrillGerbers(board, folder, name);
            GenerateIPCD(board, folder, name);
        }

        /// <summary>
        /// Genera els fitxers gerber d'imatges.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// <param name="folder">La carpeta on deixar el resultat.</param>
        /// <param name="name">El nom base del fitxer.</param>
        /// 
        private void GenerateImageGerbers(Board board, string folder, string name) {

            string prefix = Path.Combine(folder, name);
            string fileName;

            IReadOnlyList<Layer> signalLayers = board.GetSignalLayers();
            List<Layer> layers = new List<Layer>();

            GerberImageGenerator imageGenerator = new GerberImageGenerator(board);

            // Capes de senyal
            //
            for (int i = 0; i < signalLayers.Count; i++) {
                layers.Clear();
                layers.Add(signalLayers[i]);
                fileName = imageGenerator.GenerateFileName(prefix, GerberImageGenerator.ImageType.Copper, i + 1);
                using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                    using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                        imageGenerator.GenerateContent(writer, layers, GerberImageGenerator.ImageType.Copper, i + 1);
                    }
                }
            }

            // Mascara de soldadura superior
            //
            layers.Clear();
            layers.Add(board.GetLayer(Layer.TopStopName));
            fileName = imageGenerator.GenerateFileName(prefix, GerberImageGenerator.ImageType.TopSolderMask);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.GenerateContent(writer, layers, GerberImageGenerator.ImageType.TopSolderMask);
                }
            }

            // Mascara de soldadura inferior
            //
            layers.Clear();
            layers.Add(board.GetLayer(Layer.BottomStopName));
            fileName = imageGenerator.GenerateFileName(prefix, GerberImageGenerator.ImageType.BottomSolderMask);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.GenerateContent(writer, layers, GerberImageGenerator.ImageType.BottomSolderMask);
                }
            }

            // Texts i imatges de la cara superior
            //
            layers.Clear();
            layers.Add(board.GetLayer(Layer.TopPlaceName));
            layers.Add(board.GetLayer(Layer.TopNamesName));
            fileName = imageGenerator.GenerateFileName(prefix, GerberImageGenerator.ImageType.TopLegend);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.GenerateContent(writer, layers, GerberImageGenerator.ImageType.TopLegend);
                }
            }

            // Texts i imatges de la cara inferior
            //
            layers.Clear();
            layers.Add(board.GetLayer(Layer.BottomPlaceName));
            layers.Add(board.GetLayer(Layer.BottomNamesName));
            fileName = imageGenerator.GenerateFileName(prefix, GerberImageGenerator.ImageType.BottomLegend);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.GenerateContent(writer, layers, GerberImageGenerator.ImageType.BottomLegend);
                }
            }

            // Perfil exterior de la placa
            //
            layers.Clear();
            layers.Add(board.GetLayer(Layer.ProfileName));
            fileName = imageGenerator.GenerateFileName(prefix, GerberImageGenerator.ImageType.Profile);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    imageGenerator.GenerateContent(writer, layers, GerberImageGenerator.ImageType.Profile);
                }
            }
        }

        /// <summary>
        /// Genera els fitxers gerber dels forats.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// <param name="folder">La carpeta on deixar el resultat.</param>
        /// <param name="name">El nom base del fitxer.</param>
        /// 
        private void GenerateDrillGerbers(Board board, string folder, string name) {

            string prefix = Path.Combine(folder, name);
            string fileName;

            IReadOnlyList<Layer> signalLayers = board.GetSignalLayers();
            List<Layer> layers = new List<Layer>();

            GerberDrillGenerator drillGenerator = new GerberDrillGenerator(board);

            // Forats platejats
            //
            layers.Clear();
            layers.Add(board.GetLayer(Layer.DrillsName));
            fileName = drillGenerator.GenerateFileName(prefix, GerberDrillGenerator.DrillType.PlatedDrill, 1, signalLayers.Count);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    drillGenerator.GenerateContent(writer, layers, GerberDrillGenerator.DrillType.PlatedDrill, 1, signalLayers.Count);
                }
            }

            // Forats sense platejar
            //
            layers.Clear();
            layers.Add(board.GetLayer(Layer.HolesName));
            fileName = drillGenerator.GenerateFileName(prefix, GerberDrillGenerator.DrillType.NonPlatedDrill, 1, signalLayers.Count);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    drillGenerator.GenerateContent(writer, layers, GerberDrillGenerator.DrillType.NonPlatedDrill, 1, signalLayers.Count);
                }
            }
        }

        /// <summary>
        /// Genera el fitxer IPCD.
        /// </summary>
        /// <param name="board">La placa.</param>
        /// <param name="folder">La carpeta on deixar el resultat.</param>
        /// <param name="name">El nom base del fitxer.</param>
        /// 
        private void GenerateIPCD(Board board, string folder, string name) {

            string prefix = Path.Combine(folder, name);
            string fileName;

            // Arxiu de conexions per verificacio
            //
            Ipcd356Generator ipcd356Generator = new Ipcd356Generator(board);
            fileName = ipcd356Generator.GenerateFileName(prefix);
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None)) {
                using (TextWriter writer = new StreamWriter(stream, Encoding.ASCII)) {
                    ipcd356Generator.GenerateContent(writer);
                }
            }
        }
    }
}

