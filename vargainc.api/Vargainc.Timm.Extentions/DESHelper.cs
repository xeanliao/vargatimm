using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Vargainc.Timm.Extentions
{
    public static class DESHelper
    {

        public static string DesEncrypt(this string plainText)
        {
            var passphrase = ConfigurationManager.AppSettings["Passphrase"];
            if (string.IsNullOrWhiteSpace(passphrase))
            {
                throw new ArgumentException("can not found passphrase in web.config");
            }
            byte[] result;
            MD5CryptoServiceProvider hashProvider = new MD5CryptoServiceProvider();
            byte[] tripleDESKey = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(passphrase));
            TripleDESCryptoServiceProvider tripleDESAlgorithm = new TripleDESCryptoServiceProvider();
            tripleDESAlgorithm.Key = tripleDESKey;
            tripleDESAlgorithm.Mode = CipherMode.ECB;
            tripleDESAlgorithm.Padding = PaddingMode.PKCS7;
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(plainText);
            try
            {
                ICryptoTransform encryptor = tripleDESAlgorithm.CreateEncryptor();
                result = encryptor.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
            }
            finally
            {
                tripleDESAlgorithm.Clear();
                hashProvider.Clear();
            }
            return HttpServerUtility.UrlTokenEncode(result);
        }

        public static string DesDecrypt(this string message)
        {
            var passphrase = ConfigurationManager.AppSettings["Passphrase"];
            if (string.IsNullOrWhiteSpace(passphrase))
            {
                throw new ArgumentException("can not found passphrase in web.config");
            }

            byte[] result;
            MD5CryptoServiceProvider hashProvider = new MD5CryptoServiceProvider();
            byte[] tripleDESKey = hashProvider.ComputeHash(Encoding.UTF8.GetBytes(passphrase));
            TripleDESCryptoServiceProvider tripleDESAlgorithm = new TripleDESCryptoServiceProvider();
            tripleDESAlgorithm.Key = tripleDESKey;
            tripleDESAlgorithm.Mode = CipherMode.ECB;
            tripleDESAlgorithm.Padding = PaddingMode.PKCS7;
            byte[] dataToDecrypt = HttpServerUtility.UrlTokenDecode(message);
            try
            {
                ICryptoTransform decryptor = tripleDESAlgorithm.CreateDecryptor();
                result = decryptor.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
            }
            finally
            {
                tripleDESAlgorithm.Clear();
                hashProvider.Clear();
            }
            return Encoding.UTF8.GetString(result);
        }
    }
}
