using System;

namespace FileSecurityApp.Utils
{
    public class ValidationHelper
    {
        /// <summary>
        /// Valida que una ruta de archivo sea válida
        /// </summary>
        public static bool IsValidFilePath(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    return false;

                var fileInfo = new FileInfo(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Valida que un archivo exista
        /// </summary>
        public static bool FileExists(string filePath)
        {
            return !string.IsNullOrWhiteSpace(filePath) && File.Exists(filePath);
        }

        /// <summary>
        /// Valida que una cadena hexadecimal sea válida
        /// </summary>
        public static bool IsValidHexString(string hexString)
        {
            if (string.IsNullOrWhiteSpace(hexString))
                return false;

            hexString = hexString.Replace(" ", "").Replace("-", "");
            if (hexString.Length % 2 != 0)
                return false;

            foreach (char c in hexString)
            {
                if (!Uri.IsHexDigit(c))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Valida que una cadena sea Base64 válida
        /// </summary>
        public static bool IsValidBase64(string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
                return false;

            base64String = base64String.Trim();
            if (base64String.Length % 4 != 0)
                return false;

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene la entrada del usuario con validación
        /// </summary>
        public static string GetValidatedInput(string prompt, Func<string, bool> validator, string errorMessage)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine() ?? string.Empty;

                if (validator(input))
                    return input;

                ConsoleHelper.PrintError(errorMessage);
            }
        }
    }
}
