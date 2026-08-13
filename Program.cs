using System;
using System.Text;
using FileSecurityApp.Services;
using FileSecurityApp.Utils;
using FileSecurityApp.Models;

namespace FileSecurityApp
{
    class Program
    {
        static void Main(string[] args)
        {
            bool running = true;
            while (running)
            {
                ConsoleHelper.PrintHeader("FileSecurityApp - Herramienta de Seguridad de Archivos");
                Console.WriteLine("Seleccione una categoría:\n");
                Console.WriteLine("1. Criptografía (AES-256-CBC)");
                Console.WriteLine("2. Esteganografía (LSB)");
                Console.WriteLine("3. Ofuscación (Base64, ROT13, Inversión)");
                Console.WriteLine("4. Salir");

                int choice = ConsoleHelper.GetMenuChoice(4);

                switch (choice)
                {
                    case 1:
                        RunCryptographyMenu();
                        break;
                    case 2:
                        RunSteganographyMenu();
                        break;
                    case 3:
                        RunObfuscationMenu();
                        break;
                    case 4:
                        running = false;
                        ConsoleHelper.PrintSuccess("Gracias por usar FileSecurityApp. ¡Adiós!");
                        break;
                }
            }
        }

        static void RunCryptographyMenu()
        {
            bool cryptoRunning = true;
            while (cryptoRunning)
            {
                ConsoleHelper.PrintHeader("Criptografía - AES-256-CBC");
                Console.WriteLine("1. Cifrar archivo");
                Console.WriteLine("2. Descifrar archivo");
                Console.WriteLine("3. Generar clave y IV");
                Console.WriteLine("4. Volver al menú principal");

                int choice = ConsoleHelper.GetMenuChoice(4);

                switch (choice)
                {
                    case 1:
                        EncryptFile();
                        break;
                    case 2:
                        DecryptFile();
                        break;
                    case 3:
                        GenerateKeyAndIV();
                        break;
                    case 4:
                        cryptoRunning = false;
                        break;
                }
            }
        }

        static void EncryptFile()
        {
            ConsoleHelper.PrintHeader("Cifrar Archivo");

            try
            {
                // Obtener archivo
                string filePath = ConsoleHelper.GetInput("Ingrese la ruta del archivo: ");
                if (!ValidationHelper.FileExists(filePath))
                {
                    ConsoleHelper.PrintError("El archivo no existe");
                    ConsoleHelper.WaitForKey();
                    return;
                }

                // Leer archivo
                FileData fileData = FileHandler.ReadFile(filePath);
                ConsoleHelper.PrintSuccess($"Archivo leído: {fileData.FileName} ({fileData.FileSize} bytes)");

                // Obtener clave
                ConsoleHelper.PrintInfo("Ingrese la clave en hexadecimal (64 caracteres para 256 bits)");
                string keyHex = ConsoleHelper.GetInput("Clave (hex): ");
                if (!ValidationHelper.IsValidHexString(keyHex) || keyHex.Replace(" ", "").Replace("-", "").Length != 64)
                {
                    ConsoleHelper.PrintError("La clave debe tener exactamente 64 caracteres hexadecimales");
                    ConsoleHelper.WaitForKey();
                    return;
                }
                byte[] key = CryptographyService.HexStringToByteArray(keyHex);

                // Obtener IV
                ConsoleHelper.PrintInfo("Ingrese el IV en hexadecimal (32 caracteres para 128 bits)");
                string ivHex = ConsoleHelper.GetInput("IV (hex): ");
                if (!ValidationHelper.IsValidHexString(ivHex) || ivHex.Replace(" ", "").Replace("-", "").Length != 32)
                {
                    ConsoleHelper.PrintError("El IV debe tener exactamente 32 caracteres hexadecimales");
                    ConsoleHelper.WaitForKey();
                    return;
                }
                byte[] iv = CryptographyService.HexStringToByteArray(ivHex);

                // Opción de prepend IV
                string prependChoice = ConsoleHelper.GetInput("¿Desea incluir el IV al inicio del resultado cifrado? (s/n): ").ToLower();
                bool prependIV = prependChoice == "s" || prependChoice == "si";

                // Cifrar
                SecurityResult result = CryptographyService.Encrypt(fileData.Content, key, iv, prependIV);

                if (result.Success)
                {
                    ConsoleHelper.PrintSuccess(result.Message);
                    ConsoleHelper.PrintContentComparison(result.OriginalContent, result.ProcessedContent, 50);

                    // Guardar resultado
                    string outputPath = ConsoleHelper.GetInput("\nRuta de salida (incluyendo nombre de archivo): ");
                    if (FileHandler.WriteFile(outputPath, result.ProcessedData))
                    {
                        ConsoleHelper.PrintSuccess($"Archivo cifrado guardado en: {outputPath}");
                    }
                }
                else
                {
                    ConsoleHelper.PrintError(result.Message);
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Error: {ex.Message}");
            }

            ConsoleHelper.WaitForKey();
        }

        static void DecryptFile()
        {
            ConsoleHelper.PrintHeader("Descifrar Archivo");

            try
            {
                // Obtener archivo
                string filePath = ConsoleHelper.GetInput("Ingrese la ruta del archivo cifrado: ");
                if (!ValidationHelper.FileExists(filePath))
                {
                    ConsoleHelper.PrintError("El archivo no existe");
                    ConsoleHelper.WaitForKey();
                    return;
                }

                // Leer archivo
                FileData fileData = FileHandler.ReadFile(filePath);
                ConsoleHelper.PrintSuccess($"Archivo leído: {fileData.FileName} ({fileData.FileSize} bytes)");

                // Obtener clave
                ConsoleHelper.PrintInfo("Ingrese la clave en hexadecimal (64 caracteres para 256 bits)");
                string keyHex = ConsoleHelper.GetInput("Clave (hex): ");
                if (!ValidationHelper.IsValidHexString(keyHex) || keyHex.Replace(" ", "").Replace("-", "").Length != 64)
                {
                    ConsoleHelper.PrintError("La clave debe tener exactamente 64 caracteres hexadecimales");
                    ConsoleHelper.WaitForKey();
                    return;
                }
                byte[] key = CryptographyService.HexStringToByteArray(keyHex);

                // Preguntar si IV está incluido
                string ivIncludedChoice = ConsoleHelper.GetInput("¿Está el IV incluido al inicio del archivo? (s/n): ").ToLower();
                bool ivIsPrepended = ivIncludedChoice == "s" || ivIncludedChoice == "si";

                byte[] iv = null;
                if (!ivIsPrepended)
                {
                    ConsoleHelper.PrintInfo("Ingrese el IV en hexadecimal (32 caracteres para 128 bits)");
                    string ivHex = ConsoleHelper.GetInput("IV (hex): ");
                    if (!ValidationHelper.IsValidHexString(ivHex) || ivHex.Replace(" ", "").Replace("-", "").Length != 32)
                    {
                        ConsoleHelper.PrintError("El IV debe tener exactamente 32 caracteres hexadecimales");
                        ConsoleHelper.WaitForKey();
                        return;
                    }
                    iv = CryptographyService.HexStringToByteArray(ivHex);
                }

                // Descifrar
                SecurityResult result = CryptographyService.Decrypt(fileData.Content, key, iv, ivIsPrepended);

                if (result.Success)
                {
                    ConsoleHelper.PrintSuccess(result.Message);
                    ConsoleHelper.PrintContentComparison(result.OriginalContent, result.ProcessedContent, 50);

                    // Guardar resultado
                    string outputPath = ConsoleHelper.GetInput("\nRuta de salida (incluyendo nombre de archivo): ");
                    if (FileHandler.WriteFile(outputPath, result.ProcessedData))
                    {
                        ConsoleHelper.PrintSuccess($"Archivo descifrado guardado en: {outputPath}");
                    }
                }
                else
                {
                    ConsoleHelper.PrintError(result.Message);
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Error: {ex.Message}");
            }

            ConsoleHelper.WaitForKey();
        }

        static void GenerateKeyAndIV()
        {
            ConsoleHelper.PrintHeader("Generar Clave y IV");

            try
            {
                byte[] key = CryptographyService.GenerateKey();
                byte[] iv = CryptographyService.GenerateIV();

                string keyHex = CryptographyService.ByteArrayToHexString(key);
                string ivHex = CryptographyService.ByteArrayToHexString(iv);

                ConsoleHelper.PrintSuccess("Clave y IV generados exitosamente");
                ConsoleHelper.PrintSection("Clave (256 bits / 32 bytes)");
                Console.WriteLine(keyHex);
                ConsoleHelper.PrintSection("IV (128 bits / 16 bytes)");
                Console.WriteLine(ivHex);

                string saveChoice = ConsoleHelper.GetInput("\n¿Desea guardar la clave y IV en un archivo? (s/n): ").ToLower();
                if (saveChoice == "s" || saveChoice == "si")
                {
                    string outputPath = ConsoleHelper.GetInput("Ruta del archivo (incluyendo nombre): ");
                    string content = $"Clave: {keyHex}\nIV: {ivHex}";
                    if (FileHandler.WriteTextFile(outputPath, content))
                    {
                        ConsoleHelper.PrintSuccess($"Clave e IV guardados en: {outputPath}");
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Error: {ex.Message}");
            }

            ConsoleHelper.WaitForKey();
        }

        static void RunSteganographyMenu()
        {
            bool stegoRunning = true;
            while (stegoRunning)
            {
                ConsoleHelper.PrintHeader("Esteganografía - LSB (Least Significant Bit)");
                Console.WriteLine("1. Ocultar texto en un archivo");
                Console.WriteLine("2. Extraer texto oculto de un archivo");
                Console.WriteLine("3. Volver al menú principal");

                int choice = ConsoleHelper.GetMenuChoice(3);

                switch (choice)
                {
                    case 1:
                        HideTextInFile();
                        break;
                    case 2:
                        ExtractTextFromFile();
                        break;
                    case 3:
                        stegoRunning = false;
                        break;
                }
            }
        }

        static void HideTextInFile()
        {
            ConsoleHelper.PrintHeader("Ocultar Texto en Archivo");

            try
            {
                // Obtener archivo portador
                string filePath = ConsoleHelper.GetInput("Ingrese la ruta del archivo portador: ");
                if (!ValidationHelper.FileExists(filePath))
                {
                    ConsoleHelper.PrintError("El archivo no existe");
                    ConsoleHelper.WaitForKey();
                    return;
                }

                FileData fileData = FileHandler.ReadFile(filePath);
                ConsoleHelper.PrintSuccess($"Archivo leído: {fileData.FileName} ({fileData.FileSize} bytes)");

                // Obtener máxima capacidad
                int maxCapacity = SteganographyService.GetMaxHiddenCapacity(fileData.Content);
                ConsoleHelper.PrintInfo($"Capacidad máxima: {maxCapacity} caracteres");

                // Obtener texto a ocultar
                string textToHide = ConsoleHelper.GetInput("Ingrese el texto a ocultar: ");
                if (textToHide.Length > maxCapacity)
                {
                    ConsoleHelper.PrintError($"El texto es muy largo. Máximo: {maxCapacity} caracteres");
                    ConsoleHelper.WaitForKey();
                    return;
                }

                // Ocultar texto
                SecurityResult result = SteganographyService.HideTextInData(fileData.Content, textToHide);

                if (result.Success)
                {
                    ConsoleHelper.PrintSuccess(result.Message);
                    ConsoleHelper.PrintSection("Información");
                    Console.WriteLine($"Archivo original: {result.OriginalContent}");
                    Console.WriteLine($"Archivo con texto oculto: {result.ProcessedContent}");

                    // Guardar resultado
                    string outputPath = ConsoleHelper.GetInput("\nRuta de salida (incluyendo nombre de archivo): ");
                    if (FileHandler.WriteFile(outputPath, result.ProcessedData))
                    {
                        ConsoleHelper.PrintSuccess($"Archivo guardado en: {outputPath}");
                    }
                }
                else
                {
                    ConsoleHelper.PrintError(result.Message);
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Error: {ex.Message}");
            }

            ConsoleHelper.WaitForKey();
        }

        static void ExtractTextFromFile()
        {
            ConsoleHelper.PrintHeader("Extraer Texto Oculto de Archivo");

            try
            {
                // Obtener archivo
                string filePath = ConsoleHelper.GetInput("Ingrese la ruta del archivo con texto oculto: ");
                if (!ValidationHelper.FileExists(filePath))
                {
                    ConsoleHelper.PrintError("El archivo no existe");
                    ConsoleHelper.WaitForKey();
                    return;
                }

                FileData fileData = FileHandler.ReadFile(filePath);
                ConsoleHelper.PrintSuccess($"Archivo leído: {fileData.FileName} ({fileData.FileSize} bytes)");

                // Extraer texto
                SecurityResult result = SteganographyService.ExtractHiddenText(fileData.Content);

                if (result.Success)
                {
                    ConsoleHelper.PrintSuccess(result.Message);
                    ConsoleHelper.PrintContentComparison(result.OriginalContent, result.ProcessedContent);

                    // Guardar resultado
                    string saveChoice = ConsoleHelper.GetInput("\n¿Desea guardar el texto extraído en un archivo? (s/n): ").ToLower();
                    if (saveChoice == "s" || saveChoice == "si")
                    {
                        string outputPath = ConsoleHelper.GetInput("Ruta de salida (incluyendo nombre de archivo): ");
                        if (FileHandler.WriteTextFile(outputPath, result.ProcessedContent))
                        {
                            ConsoleHelper.PrintSuccess($"Texto guardado en: {outputPath}");
                        }
                    }
                }
                else
                {
                    ConsoleHelper.PrintError(result.Message);
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Error: {ex.Message}");
            }

            ConsoleHelper.WaitForKey();
        }

        static void RunObfuscationMenu()
        {
            bool obfuscationRunning = true;
            while (obfuscationRunning)
            {
                ConsoleHelper.PrintHeader("Ofuscación");
                Console.WriteLine("1. Base64 - Codificar");
                Console.WriteLine("2. Base64 - Decodificar");
                Console.WriteLine("3. ROT13");
                Console.WriteLine("4. Invertir Texto");
                Console.WriteLine("5. Volver al menú principal");

                int choice = ConsoleHelper.GetMenuChoice(5);

                switch (choice)
                {
                    case 1:
                        EncodeBase64();
                        break;
                    case 2:
                        DecodeBase64();
                        break;
                    case 3:
                        ApplyROT13();
                        break;
                    case 4:
                        ReverseText();
                        break;
                    case 5:
                        obfuscationRunning = false;
                        break;
                }
            }
        }

        static void EncodeBase64()
        {
            ConsoleHelper.PrintHeader("Base64 - Codificar");

            try
            {
                string input = ConsoleHelper.GetInput("Ingrese el texto a codificar: ");
                if (string.IsNullOrEmpty(input))
                {
                    ConsoleHelper.PrintWarning("El entrada no puede estar vacía");
                    ConsoleHelper.WaitForKey();
                    return;
                }

                SecurityResult result = ObfuscationService.EncodeBase64(input);

                if (result.Success)
                {
                    ConsoleHelper.PrintSuccess(result.Message);
                    ConsoleHelper.PrintContentComparison(result.OriginalContent, result.ProcessedContent, 80);

                    SaveResultOption(result.ProcessedContent);
                }
                else
                {
                    ConsoleHelper.PrintError(result.Message);
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Error: {ex.Message}");
            }

            ConsoleHelper.WaitForKey();
        }

        static void DecodeBase64()
        {
            ConsoleHelper.PrintHeader("Base64 - Decodificar");

            try
            {
                string input = ConsoleHelper.GetInput("Ingrese el texto codificado en Base64: ");
                if (string.IsNullOrEmpty(input))
                {
                    ConsoleHelper.PrintWarning("La entrada no puede estar vacía");
                    ConsoleHelper.WaitForKey();
                    return;
                }

                if (!ValidationHelper.IsValidBase64(input))
                {
                    ConsoleHelper.PrintError("El texto no es Base64 válido");
                    ConsoleHelper.WaitForKey();
                    return;
                }

                SecurityResult result = ObfuscationService.DecodeBase64(input);

                if (result.Success)
                {
                    ConsoleHelper.PrintSuccess(result.Message);
                    ConsoleHelper.PrintContentComparison(result.OriginalContent, result.ProcessedContent, 80);

                    SaveResultOption(result.ProcessedContent);
                }
                else
                {
                    ConsoleHelper.PrintError(result.Message);
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Error: {ex.Message}");
            }

            ConsoleHelper.WaitForKey();
        }

        static void ApplyROT13()
        {
            ConsoleHelper.PrintHeader("ROT13");

            try
            {
                string input = ConsoleHelper.GetInput("Ingrese el texto a transformar: ");
                if (string.IsNullOrEmpty(input))
                {
                    ConsoleHelper.PrintWarning("La entrada no puede estar vacía");
                    ConsoleHelper.WaitForKey();
                    return;
                }

                SecurityResult result = ObfuscationService.ApplyROT13(input);

                if (result.Success)
                {
                    ConsoleHelper.PrintSuccess(result.Message);
                    ConsoleHelper.PrintContentComparison(result.OriginalContent, result.ProcessedContent, 80);

                    SaveResultOption(result.ProcessedContent);
                }
                else
                {
                    ConsoleHelper.PrintError(result.Message);
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Error: {ex.Message}");
            }

            ConsoleHelper.WaitForKey();
        }

        static void ReverseText()
        {
            ConsoleHelper.PrintHeader("Invertir Texto");

            try
            {
                string input = ConsoleHelper.GetInput("Ingrese el texto a invertir: ");
                if (string.IsNullOrEmpty(input))
                {
                    ConsoleHelper.PrintWarning("La entrada no puede estar vacía");
                    ConsoleHelper.WaitForKey();
                    return;
                }

                SecurityResult result = ObfuscationService.ReverseText(input);

                if (result.Success)
                {
                    ConsoleHelper.PrintSuccess(result.Message);
                    ConsoleHelper.PrintContentComparison(result.OriginalContent, result.ProcessedContent, 80);

                    SaveResultOption(result.ProcessedContent);
                }
                else
                {
                    ConsoleHelper.PrintError(result.Message);
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.PrintError($"Error: {ex.Message}");
            }

            ConsoleHelper.WaitForKey();
        }

        static void SaveResultOption(string content)
        {
            string saveChoice = ConsoleHelper.GetInput("\n¿Desea guardar el resultado en un archivo? (s/n): ").ToLower();
            if (saveChoice == "s" || saveChoice == "si")
            {
                string outputPath = ConsoleHelper.GetInput("Ruta de salida (incluyendo nombre de archivo): ");
                if (FileHandler.WriteTextFile(outputPath, content))
                {
                    ConsoleHelper.PrintSuccess($"Resultado guardado en: {outputPath}");
                }
            }
        }
    }
}
