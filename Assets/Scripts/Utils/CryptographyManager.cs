using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using UnityEngine.InputSystem;
using System.IO;
using System;
using System.Text;
using System.Linq;

public class CryptographyManager : MonoBehaviour
{
    //Genera N byte casuali
    public static byte[] GenerateRandomBytes(int byteLength) {
        var rand = new RNGCryptoServiceProvider();
        var byteArray = new byte[byteLength];
        rand.GetNonZeroBytes(byteArray);
        return byteArray;
    }

    //Genera un UUID casuale
    public static string GenerateRandomUUID()
    {
        Guid uuid = Guid.NewGuid();
        return uuid.ToString();
    }

    //Cifratura AES
    public static byte[] AesEncrypt(byte[] plaintext, byte[] key, byte[] IV) {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = IV;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plaintext, 0, plaintext.Length);
                    }
                }

                return memoryStream.ToArray();
            }
        }
    }

    //Decifratura AES
    public static byte[] AesDecrypt(byte[] ciphertext, byte[] key, byte[] IV) {
        using (Aes aes = Aes.Create())
        {
            aes.Key = key;
            aes.IV = IV;

            using (MemoryStream memoryStream = new MemoryStream(ciphertext))
            {
                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader(cryptoStream))
                        {
                            return Encoding.UTF8.GetBytes(streamReader.ReadToEnd());
                        }
                    }
                }
            }
        }
    }

    //Calcola l'hash SHA256 di un array di byte
    public static byte[] Sha256Hash(byte[] data)
    {
        SHA256 sha256 = SHA256.Create();
        return sha256.ComputeHash(data);
    }

    //Trasforma una stringa esadecimale in un array di byte (per decifrare successivamente i messaggi)
    public static byte[] HexStringToByteArray(string hex)
    {
        return Enumerable.Range(0, hex.Length)
                         .Where(x => x % 2 == 0)
                         .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                         .ToArray();
    }

    //Trasforma un array di byte in una stringa esadecimale (per inviare successivamente i messaggi cifrati)
    public static string ByteArrayToHexString(byte[] byteArray)
    {
        return BitConverter.ToString(byteArray).Replace("-", "");
    }
}
