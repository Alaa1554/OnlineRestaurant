using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography;
using System.Text;

namespace OnlineRestaurant.Helpers
{
    public class Encryption
    {
        //public static byte[] GenerateRandomKey()    //{
        //    // Create a byte array to hold the key    //    byte[] key = new byte[8];
        //    // Generate random bytes to fill the key array
        //    using (var rng = new RNGCryptoServiceProvider())    //    {
        //        rng.GetBytes(key);    //    }
        //    return key;
        //}    // Method to encrypt the password using DES algorithm
        public static string Encrypt(string plainText)
        {
            byte[] key = Convert.FromBase64String("VGVzdERhdGEgS2V5");
            Array.Resize(ref key, 8);
            byte[] IV = Encoding.UTF8.GetBytes("YourInitializationVector"); 
            byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write))
                    {
                        cs.Write(inputBytes, 0, inputBytes.Length);
                        cs.FlushFinalBlock(); return Convert.ToBase64String(ms.ToArray());
                    }
                }
            }
        }
        public static string Decrypt(string encryptedText)
        {
            // Convert the key from Base64 encoded string to bytes
            byte[] key = Convert.FromBase64String("VGVzdERhdGEgS2V5");
            // Ensure the key is exactly 8 bytes (64 bits)
            Array.Resize(ref key, 8);

            byte[] IV = Encoding.UTF8.GetBytes("YourInitializationVector"); // Replace with the same IV used for encryption

            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write))
                    {
                        cs.Write(encryptedBytes, 0, encryptedBytes.Length);
                        cs.FlushFinalBlock();
                        return Encoding.UTF8.GetString(ms.ToArray());
                    }
                }
            }
        }
    }
}
