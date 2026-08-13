using System;
using System.Text;
using FileSecurityApp.Models;

namespace FileSecurityApp.Utils
{
    public class FileHandler
    {
        /// <summary>
        /// Lee un archivo completo
        /// </summary>
        public static FileData ReadFile(string filePath)
        {
            var fileData = new FileData();

            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    throw new ArgumentException("La ruta del archivo no puede estar vacía");

                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"El archivo no existe: {filePath}");

                fileData.FilePath = filePath;
                fileData.FileName = Path.GetFileName(filePath);
                fileData.FileExtension = Path.GetExtension(filePath);
                fileData.Content = File.ReadAllBytes(filePath);
                fileData.FileSize = fileData.Content.Length;

                // Intentar leer como texto (UTF-8)
                try
                {
                    fileData.ContentAsString = Encoding.UTF8.GetString(fileData.Content);
                }
                catch
                {
                    fileData.ContentAsString = "[Contenido binario - no se puede mostrar como texto]";
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al leer archivo: {ex.Message}");
            }

            return fileData;
        }

        /// <summary>
        /// Escribe datos en un archivo
        /// </summary>
        public static bool WriteFile(string filePath, byte[] data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    throw new ArgumentException("La ruta del archivo no puede estar vacía");

                if (data == null || data.Length == 0)
                    throw new ArgumentException("No hay datos para escribir");

                string directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                File.WriteAllBytes(filePath, data);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al escribir archivo: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Escribe texto en un archivo
        /// </summary>
        public static bool WriteTextFile(string filePath, string content)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    throw new ArgumentException("La ruta del archivo no puede estar vacía");

                string directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                File.WriteAllText(filePath, content, Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al escribir archivo: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Verifica si un archivo es de texto
        /// </summary>
        public static bool IsTextFile(string filePath)
        {
            string[] textExtensions = { ".txt", ".cs", ".json", ".xml", ".csv", ".log" };
            string extension = Path.GetExtension(filePath).ToLower();
            return textExtensions.Contains(extension);
        }
    }
}
