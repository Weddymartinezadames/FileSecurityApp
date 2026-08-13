using System;
using System.Security.Cryptography;
using System.Text;
using FileSecurityApp.Models;

namespace FileSecurityApp.Services
{
    public class CryptographyService
    {
        /// <summary>
        /// Genera una clave AES-256 (32 bytes)
        /// </summary>
        public static byte[] GenerateKey()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] key = new byte[32];
                rng.GetBytes(key);
                return key;
            }
        }

        /// <summary>
        /// Genera un IV (16 bytes)
        /// </summary>
        public static byte[] GenerateIV()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] iv = new byte[16];
                rng.GetBytes(iv);
                return iv;
            }
        }

        /// <summary>
        /// Cifra datos usando AES-256-CBC
        /// </summary>
        public static SecurityResult Encrypt(byte[] data, byte[] key, byte[] iv, bool prependIV = true)
        {
            var result = new SecurityResult
            {
                OperationType = "AES Encryption"
            };

            try
            {
                if (data == null || data.Length == 0)
                    throw new ArgumentException("Los datos a cifrar no pueden estar vacíos");

                if (key == null || key.Length != 32)
                    throw new ArgumentException("La clave debe tener exactamente 32 bytes (256 bits)");

                if (iv == null || iv.Length != 16)
                    throw new ArgumentException("El IV debe tener exactamente 16 bytes");

                result.OriginalData = data;
                result.OriginalContent = Encoding.UTF8.GetString(data);

                using (var aes = new AesCryptoServiceProvider())
                {
                    aes.Key = key;
                    aes.IV = iv;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    using (var ms = new MemoryStream())
                    {
                        // Prepend IV si se solicita
                        if (prependIV)
                            ms.Write(iv, 0, iv.Length);

                        using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                        {
                            cs.Write(data, 0, data.Length);
                            cs.FlushFinalBlock();
                        }

                        result.ProcessedData = ms.ToArray();
                        result.ProcessedContent = Convert.ToBase64String(result.ProcessedData);
                        result.Success = true;
                        result.Message = $"Cifrado exitoso ({result.ProcessedData.Length} bytes)";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error al cifrar: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// Descifra datos usando AES-256-CBC
        /// </summary>
        public static SecurityResult Decrypt(byte[] encryptedData, byte[] key, byte[] iv = null, bool ivIsPrepended = true)
        {
            var result = new SecurityResult
            {
                OperationType = "AES Decryption"
            };

            try
            {
                if (encryptedData == null || encryptedData.Length == 0)
                    throw new ArgumentException("Los datos a descifrar no pueden estar vacíos");

                if (key == null || key.Length != 32)
                    throw new ArgumentException("La clave debe tener exactamente 32 bytes (256 bits)");

                byte[] actualIV = iv;
                byte[] dataToDecrypt = encryptedData;

                // Extraer IV del inicio si está prepended
                if (ivIsPrepended && encryptedData.Length > 16)
                {
                    actualIV = new byte[16];
                    Array.Copy(encryptedData, 0, actualIV, 0, 16);
                    dataToDecrypt = new byte[encryptedData.Length - 16];
                    Array.Copy(encryptedData, 16, dataToDecrypt, 0, encryptedData.Length - 16);
                }

                if (actualIV == null || actualIV.Length != 16)
                    throw new ArgumentException("El IV debe tener exactamente 16 bytes");

                result.OriginalData = encryptedData;
                result.OriginalContent = Convert.ToBase64String(encryptedData);

                using (var aes = new AesCryptoServiceProvider())
                {
                    aes.Key = key;
                    aes.IV = actualIV;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    using (var ms = new MemoryStream(dataToDecrypt))
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (var sr = new StreamReader(cs, Encoding.UTF8))
                    {
                        result.ProcessedContent = sr.ReadToEnd();
                        result.ProcessedData = Encoding.UTF8.GetBytes(result.ProcessedContent);
                        result.Success = true;
                        result.Message = "Descifrado exitoso";
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Error al descifrar: {ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// Convierte una cadena hexadecimal a bytes
        /// </summary>
        public static byte[] HexStringToByteArray(string hex)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hex))
                    throw new ArgumentException("La cadena hexadecimal no puede estar vacía");

                hex = hex.Replace(" ", "").Replace("-", "");
                if (hex.Length % 2 != 0)
                    throw new ArgumentException("La cadena hexadecimal debe tener una longitud par");

                byte[] result = new byte[hex.Length / 2];
                for (int i = 0; i < hex.Length; i += 2)
                {
                    result[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Error al convertir hexadecimal: {ex.Message}");
            }
        }

        /// <summary>
        /// Convierte bytes a cadena hexadecimal
        /// </summary>
        public static string ByteArrayToHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }
    }
}
