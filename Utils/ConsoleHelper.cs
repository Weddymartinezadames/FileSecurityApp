using System;

namespace FileSecurityApp.Utils
{
    public class ConsoleHelper
    {
        public static void PrintHeader(string title)
        {
            Console.Clear();
            Console.WriteLine(new string('=', 60));
            Console.WriteLine($"  {title}".PadRight(60));
            Console.WriteLine(new string('=', 60));
            Console.WriteLine();
        }

        public static void PrintSection(string title)
        {
            Console.WriteLine($"\n--- {title} ---");
        }

        public static void PrintSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ {message}");
            Console.ResetColor();
        }

        public static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ {message}");
            Console.ResetColor();
        }

        public static void PrintWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"⚠ {message}");
            Console.ResetColor();
        }

        public static void PrintInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"ℹ {message}");
            Console.ResetColor();
        }

        public static string GetInput(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine() ?? string.Empty;
        }

        public static int GetMenuChoice(int maxOptions)
        {
            while (true)
            {
                string input = GetInput("\nSeleccione una opción: ");
                if (int.TryParse(input, out int choice) && choice >= 1 && choice <= maxOptions)
                    return choice;
                PrintError("Opción inválida. Intente de nuevo.");
            }
        }

        public static void PrintDivider()
        {
            Console.WriteLine(new string('-', 60));
        }

        public static void PrintContentComparison(string original, string processed, int maxLength = 100)
        {
            PrintSection("Comparación de Contenido");
            
            Console.WriteLine("\nContenido Original:");
            PrintContent(original, maxLength);
            
            Console.WriteLine("\nContenido Procesado:");
            PrintContent(processed, maxLength);
        }

        private static void PrintContent(string content, int maxLength)
        {
            if (string.IsNullOrEmpty(content))
            {
                Console.WriteLine("[Vacío]");
                return;
            }

            if (content.Length > maxLength)
            {
                Console.WriteLine(content.Substring(0, maxLength));
                Console.WriteLine($"... [{content.Length - maxLength} caracteres más]");
            }
            else
            {
                Console.WriteLine(content);
            }
        }

        public static void WaitForKey()
        {
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey(true);
        }
    }
}
