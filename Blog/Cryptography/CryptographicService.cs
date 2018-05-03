using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Blog.Cryptography
{
    public class CryptographicService : ICryptographicService
    {

        private const string Key = "KIhVazAM9PcciXgyN2an/Vkp4bytF+B/INSbBrQSTmY=";
        private const int Iterations = 50000;

        /// <summary>
        /// A2 - Broken Authentication -  This takes in a password and a salt and "hashes" the result.
        /// There are two private methods in this class one will correctly hash, the other actually uses the Hash as a key 
        /// (sad panda) and then stores the password using reversible encryption.
        /// </summary>
        /// <param name="password">The password as a string, no special encoding needed.</param>
        /// <param name="salt">The salt value as any arbitrary string. No special encoding needed.</param>
        /// <returns></returns>
        public string HashPassword(string password, string salt)
        {
            // this isn't safe!
            return UnsafeStorage(password, salt);

            // this is!
            // return SafeStorage(password, salt, iterations);
        }

        /// <summary>
        /// A2 - Broken Authentication - This is a demonstration on how NOT to store passwords -- with reversible encryption.
        /// NEVER USE THIS in production! This uses the "salt" improperly as the initialization vector, uses a hardcoded
        /// key in constants and stores the password with AES - a reversible encryption.
        /// </summary>
        /// <param name="password">The user's password to hash.</param>
        /// <param name="salt">The "salt" which this method will improperly use as the Initialization Vector</param>
        /// <returns>A hash of the user's password.</returns>
        private static string UnsafeStorage(string password, string salt)
        {
            // turn our salt into a base64 encoded string
            // create crypto.
            using (var aes = Aes.Create())
            {
                aes.IV = System.Text.Encoding.UTF8.GetBytes(salt).Take(16).ToArray();
                aes.KeySize = 256;
                aes.Key = Convert.FromBase64String(Key);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(password);
                        }
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        /// <summary>
        /// A2 - Broken Authentication - This correctly hashes and salts a password with 50,000 iterations.
        /// </summary>
        /// <param name="password">The value to hash.</param>
        /// <param name="salt">The salt to use.</param>
        /// <param name="iterations">The number of RF2898 iterations to run through. Suggested minimum is 20k.</param>
        /// <returns></returns>
        private static string SafeStorage(string password, string salt, int iterations)
        {
            // convert our salt into a byte array.
            byte[] saltBytes = Convert.FromBase64String(salt);

            // create our hash
            using (var hash = new Rfc2898DeriveBytes(password, saltBytes, iterations))
            {
                // return as base 64, only first 160 bits are used.
                return Convert.ToBase64String(hash.GetBytes(20));
            }

        }
    }
}