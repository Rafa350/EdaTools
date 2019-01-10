namespace MikroPic.EdaTools.v1.Base.IO {

    using System;
    using System.Collections.Generic;
    using System.IO;

    public class FileStreamLocator : IStreamLocator {

        private readonly List<string> folders = new List<string>();

        /// <summary>
        /// Borra totes les carpetes de la llista.
        /// </summary>
        /// 
        public void ClearFolders() {

            folders.Clear();
        }

        /// <summary>
        /// Afegeix una carpeta.
        /// </summary>
        /// <param name="folder">La carpeta.</param>
        /// 
        public void AddFolder(string folder) {

            if (String.IsNullOrEmpty(folder))
                throw new ArgumentNullException("folder");

            if (!folders.Contains(folder))
                folders.Add(folder);
        }

        /// <summary>
        /// Obre la ruta del fitxer especificat.
        /// </summary>
        /// <param name="fileName">El fitxer.</param>
        /// <returns>El stream.</returns>
        /// 
        public string GetPath(string fileName) {

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            if (File.Exists(fileName))
                return fileName;

            else {
                foreach (var folder in folders) {
                    string path = Path.Combine(folder, fileName);
                    if (File.Exists(path))
                        return path;
                }

                throw new Exception(
                    String.Format("No se encontro la ruta al fichero '{0}'.", fileName));
            }
        }

        /// <summary>
        /// Obte el stream del fitxer especificat.
        /// </summary>
        /// <param name="fileName">El nom del fitxer.</param>
        /// <returns>El stream.</returns>
        /// 
        public Stream GetStream(string fileName) {

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            return new FileStream(GetPath(fileName), FileMode.Open, FileAccess.Read, FileShare.Read);
        }
    }
}
