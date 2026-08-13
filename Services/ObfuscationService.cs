using System;
using System.Text;
using FileSecurityApp.Models;

namespace FileSecurityApp.Services
{
    public class ObfuscationService
    {
        /// <summary>
        /// Codifica texto en Base64
        /// </summary>
        public static SecurityResult EncodeBase64(string text)
        {
            var result = new SecurityResult
            {
                OperationType = "Obfuscation - Base64 Encode"
            };

            try
            {
                if (string.IsNullOrEmpty(text))
                    throw new ArgumentException("El texto a codificar no puede estar vacío");

                result.OriginalContent = text;
                byte[] textBytes = Encoding.UTF8.GetBytes(text);
                result.OriginalData = textBytes;

                string encoded = Convert.ToBase64String(textBytes);
                result.ProcessedContent = encoded;
                result.ProcessedData = Encoding.UTF8.GetBytes(encoded);
                result.Success = true;
                result.Message = "Codificación Base64 exitosa";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error al codificar Base64: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// Decodifica texto desde Base64
        /// </summary>
        public static SecurityResult DecodeBase64(string encodedText)
        {
            var result = new SecurityResult
            {
                OperationType = "Obfuscation - Base64 Decode"
            };

            try
            {
                if (string.IsNullOrEmpty(encodedText))
                    throw new ArgumentException("El texto a decodificar no puede estar vacío");

                result.OriginalContent = encodedText;
                result.OriginalData = Encoding.UTF8.GetBytes(encodedText);

                byte[] decodedBytes = Convert.FromBase64String(encodedText);
                string decoded = Encoding.UTF8.GetString(decodedBytes);
                result.ProcessedContent = decoded;
                result.ProcessedData = decodedBytes;
                result.Success = true;
                result.Message = "Decodificación Base64 exitosa";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error al decodificar Base64: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// Aplica ROT13 al texto
        /// </summary>
        public static SecurityResult ApplyROT13(string text)
        {
            var result = new SecurityResult
            {
                OperationType = "Obfuscation - ROT13"
            };

            try
            {
                if (string.IsNullOrEmpty(text))
                    throw new ArgumentException("El texto no puede estar vacío");

                result.OriginalContent = text;
                result.OriginalData = Encoding.UTF8.GetBytes(text);

                StringBuilder sb = new StringBuilder();
                foreach (char c in text)
                {
                    if (char.IsLetter(c))
                    {
                        char baseChar = char.IsUpper(c) ? 'A' : 'a';
                        int charIndex = char.ToUpper(c) - 'A';
                        int newIndex = (charIndex + 13) % 26;
                        sb.Append((char)(baseChar + newIndex));
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }

                result.ProcessedContent = sb.ToString();
                result.ProcessedData = Encoding.UTF8.GetBytes(result.ProcessedContent);
                result.Success = true;
                result.Message = "ROT13 aplicado exitosamente";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error al aplicar ROT13: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// Invierte el texto
        /// </summary>
        public static SecurityResult ReverseText(string text)
        {
            var result = new SecurityResult
            {
                OperationType = "Obfuscation - Text Reversal"
            };

            try
            {
                if (string.IsNullOrEmpty(text))
                    throw new ArgumentException("El texto no puede estar vacío");

                result.OriginalContent = text;
                result.OriginalData = Encoding.UTF8.GetBytes(text);

                char[] charArray = text.ToCharArray();
                Array.Reverse(charArray);
                result.ProcessedContent = new string(charArray);
                result.ProcessedData = Encoding.UTF8.GetBytes(result.ProcessedContent);
                result.Success = true;
                result.Message = "Texto invertido exitosamente";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error al invertir texto: {ex.Message}";
            }

            return result;
        }
    }
}
