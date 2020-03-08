namespace MikroPic.EdaTools.v1.Base.IO {

    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Localitzador de fitxers.
    /// </summary>
    /// 
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
                throw new ArgumentNullException(nameof(folder));

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
                throw new ArgumentNullException(nameof(fileName));

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
    }
}
