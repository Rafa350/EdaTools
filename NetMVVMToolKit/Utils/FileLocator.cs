namespace MikroPic.NetMVVMToolkit.v1.Utils {

    using System;
    using System.Globalization;
    using System.IO;

    public static class FileLocator {

        public static string LocateFile(string folderName, string fileName, CultureInfo ci = null) {

            if (String.IsNullOrEmpty(folderName))
                throw new ArgumentNullException("folderName");

            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");

            string path;

            if (Path.IsPathRooted(folderName)) {
                path = Path.Combine(folderName, fileName);
                return File.Exists(path) ? path : null;
            }

            path = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, 
                folderName, 
                ci == null ? CultureInfo.CurrentUICulture.Name : ci.Name,
                fileName);
            if (File.Exists(path))
                return path;

            path = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                folderName,
                fileName);
            if (File.Exists(path))
                return path;

            return null;
        }
    }
}
