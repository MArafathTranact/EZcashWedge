using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EZCashWedge
{
    public static class TokenEncryptDecrypt
    {
        private const int KEYSIZE = 256; // bits
        private const int DERIVATION_ITERATIONS = 5000;

        private const string PASS_PHRASE = "CWIwbXQxNvyqydGEEhL4i8drfTIjlHGBISphSkhVfhdIF8rAQaeUXiWYNVSkBmTNVyLA49jaK" +
                                           "FnbVpuS1m2rTxQibNdQv37Hxy9blCIYWaPSJH1ew6I58TdIXrEnmciyx5dXXSzW1tJy3GwNWz" +
                                           "xtlIKjwT957HD5bshB6yNu9NQXiaG3SNBFkk8CW8QrfH5I8NE3Qkt1JSE5qWIESB4F2hnxfsD" +
                                           "pBL7b1nRJYUWFRN2DTHNiwmlFp16tQEMaBdqf3SIzgflReyGkWbDrsg6U6xX3bgTtwO4BOju4" +
                                           "yQubsTxSMwpe2em77Gl7sn9RGCg2uMZehdGEud73XEwCVKdmvY16FULyxbPJBvQs8baXjIzNc" +
                                           "NjCYyNGIBqKqjrIPX3CCqU28XVGDNDCD6NPlee2MyCpKBuRLFninCPWLnFI1nDEcsWqJTzEBD" +
                                           "vLUDHTMHaa7KQjpGkKObLND24zwaV1Pm6dmLyThx7ggcuDQlT6Df1OkrE3OQX1GpCZgf9JBYfX";

        public static string Encrypt(string plainText)
        {
            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes;
            using (var password = new Rfc2898DeriveBytes(PASS_PHRASE, saltStringBytes, DERIVATION_ITERATIONS))
            {
                keyBytes = password.GetBytes(KEYSIZE / 8);
            }

            var engine = new RijndaelEngine(256);
            var blockCipher = new CbcBlockCipher(engine);
            var cipher = new PaddedBufferedBlockCipher(blockCipher, new Pkcs7Padding());
            var keyParam = new KeyParameter(keyBytes);
            var keyParamWithIV = new ParametersWithIV(keyParam, ivStringBytes, 0, 32);

            cipher.Init(true, keyParamWithIV);
            var comparisonBytes = new byte[cipher.GetOutputSize(plainTextBytes.Length)];
            var length = cipher.ProcessBytes(plainTextBytes, comparisonBytes, 0);
            cipher.DoFinal(comparisonBytes, length);

            return Convert.ToBase64String(
                saltStringBytes.Concat(ivStringBytes).Concat(comparisonBytes).ToArray()
            );
        }

        public static string Decrypt(string cipherText)
        {
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(KEYSIZE / 8).ToArray();
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(KEYSIZE / 8).Take(KEYSIZE / 8).ToArray();
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip(KEYSIZE / 8 * 2)
                                                              .Take(cipherTextBytesWithSaltAndIv.Length - KEYSIZE / 8 * 2)
                                                              .ToArray();

            byte[] keyBytes;
            using (var password = new Rfc2898DeriveBytes(PASS_PHRASE, saltStringBytes, DERIVATION_ITERATIONS))
            {
                keyBytes = password.GetBytes(KEYSIZE / 8);
            }

            var engine = new RijndaelEngine(256);
            var blockCipher = new CbcBlockCipher(engine);
            var cipher = new PaddedBufferedBlockCipher(blockCipher, new Pkcs7Padding());
            var keyParam = new KeyParameter(keyBytes);
            var keyParamWithIV = new ParametersWithIV(keyParam, ivStringBytes, 0, 32);

            cipher.Init(false, keyParamWithIV);
            var comparisonBytes = new byte[cipher.GetOutputSize(cipherTextBytes.Length)];
            var length = cipher.ProcessBytes(cipherTextBytes, comparisonBytes, 0);
            cipher.DoFinal(comparisonBytes, length);

            int nullIndex = comparisonBytes.Length - 1;
            while (nullIndex >= 0 && comparisonBytes[nullIndex] == 0)
            {
                nullIndex--;
            }
            comparisonBytes = comparisonBytes.Take(nullIndex + 1).ToArray();

            return Encoding.UTF8.GetString(comparisonBytes, 0, comparisonBytes.Length);
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes = 256 bits
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}
