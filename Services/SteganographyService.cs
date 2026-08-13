using System;
using System.Text;
using FileSecurityApp.Models;

namespace FileSecurityApp.Services
{
    public class SteganographyService
    {
        private const int BitsPerByte = 8;

        /// <summary>
        /// Oculta texto en un archivo binario usando LSB (Least Significant Bit)
        /// </summary>
        public static SecurityResult HideTextInData(byte[] carrierData, string textToHide)
        {
            var result = new SecurityResult
            {
                OperationType = "Steganography - Hide Text"
            };

            try
            {
                if (carrierData == null || carrierData.Length == 0)
                    throw new ArgumentException("Los datos portadores no pueden estar vacíos");

                if (string.IsNullOrEmpty(textToHide))
                    throw new ArgumentException("El texto a ocultar no puede estar vacío");

                byte[] textBytes = Encoding.UTF8.GetBytes(textToHide);
                int requiredBits = textBytes.Length * BitsPerByte + 32; // +32 para la longitud

                if (requiredBits > carrierData.Length * BitsPerByte)
                    throw new ArgumentException(
                        $"El texto es demasiado grande. Se requieren al menos {requiredBits / 8} bytes de datos portadores");

                result.OriginalData = carrierData;
                result.OriginalContent = $"Datos portadores ({carrierData.Length} bytes)";

                byte[] output = (byte[])carrierData.Clone();
                int bitPosition = 0;

                // Codificar la longitud del texto (4 bytes / 32 bits)
                int textLength = textBytes.Length;
                for (int i = 0; i < 32; i++)
                {
                    int byteIndex = bitPosition / BitsPerByte;
                    int bitOffset = bitPosition % BitsPerByte;
                    int bit = (textLength >> i) & 1;
                    output[byteIndex] = (byte)((output[byteIndex] & ~(1 << bitOffset)) | (bit << bitOffset));
                    bitPosition++;
                }

                // Codificar el texto
                foreach (byte textByte in textBytes)
                {
                    for (int i = 0; i < BitsPerByte; i++)
                    {
                        int byteIndex = bitPosition / BitsPerByte;
                        int bitOffset = bitPosition % BitsPerByte;
                        int bit = (textByte >> i) & 1;
                        output[byteIndex] = (byte)((output[byteIndex] & ~(1 << bitOffset)) | (bit << bitOffset));
                        bitPosition++;
                    }
                }

                result.ProcessedData = output;
                result.ProcessedContent = $"Datos con texto oculto ({output.Length} bytes)";
                result.Success = true;
                result.Message = $"Texto oculto exitosamente ({textBytes.Length} bytes)";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error al ocultar texto: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// Extrae texto oculto de datos usando LSB
        /// </summary>
        public static SecurityResult ExtractHiddenText(byte[] stegoData)
        {
            var result = new SecurityResult
            {
                OperationType = "Steganography - Extract Text"
            };

            try
            {
                if (stegoData == null || stegoData.Length < 4)
                    throw new ArgumentException("Los datos esteganografiados son inválidos o muy pequeños");

                result.OriginalData = stegoData;
                result.OriginalContent = $"Datos esteganografiados ({stegoData.Length} bytes)";

                int bitPosition = 0;

                // Decodificar la longitud del texto (32 bits)
                int textLength = 0;
                for (int i = 0; i < 32; i++)
                {
                    int byteIndex = bitPosition / BitsPerByte;
                    int bitOffset = bitPosition % BitsPerByte;
                    int bit = (stegoData[byteIndex] >> bitOffset) & 1;
                    textLength |= (bit << i);
                    bitPosition++;
                }

                // Validar longitud
                if (textLength <= 0 || textLength > stegoData.Length)
                    throw new ArgumentException("Longitud de texto inválida en los datos esteganografiados");

                // Decodificar el texto
                byte[] extractedText = new byte[textLength];
                for (int i = 0; i < textLength; i++)
                {
                    byte textByte = 0;
                    for (int j = 0; j < BitsPerByte; j++)
                    {
                        int byteIndex = bitPosition / BitsPerByte;
                        int bitOffset = bitPosition % BitsPerByte;
                        int bit = (stegoData[byteIndex] >> bitOffset) & 1;
                        textByte |= (byte)(bit << j);
                        bitPosition++;
                    }
                    extractedText[i] = textByte;
                }

                result.ProcessedContent = Encoding.UTF8.GetString(extractedText);
                result.ProcessedData = extractedText;
                result.Success = true;
                result.Message = $"Texto extraído exitosamente ({textLength} bytes)";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error al extraer texto: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// Calcula la capacidad máxima de datos que se pueden ocultar
        /// </summary>
        public static int GetMaxHiddenCapacity(byte[] carrierData)
        {
            if (carrierData == null || carrierData.Length == 0)
                return 0;

            // 32 bits para la longitud + bits para el contenido
            return (carrierData.Length * BitsPerByte - 32) / BitsPerByte;
        }
    }
}
