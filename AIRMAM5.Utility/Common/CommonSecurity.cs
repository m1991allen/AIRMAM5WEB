using System;
using System.Configuration;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace AIRMAM5.Utility.Common//AIRMAM5.Utility.Encryption
{
    public class CommonSecurity
    {
        /// <summary>
        /// 加密(for 密碼使用)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string Encrypt(string input)
        {
            return STD_MAM3.Security.Encrypt.EncryptString(input);
        }

        /// <summary>
        /// Cookie 加密
        /// </summary>
        /// <param name="cookieValue"></param>
        /// <returns></returns>
        public static string CookieEncrypt(string cookieValue)
        {
            string encKey = ConfigurationManager.AppSettings["fsENC_KEY"].ToString();
            return Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(cookieValue), encKey));
        }

        /// <summary>
        /// Cookie 解密
        /// </summary>
        /// <param name="encryptValue"></param>
        /// <returns></returns>
        public static string CookieDecrypt(string encryptValue)
        {
            string encKey = ConfigurationManager.AppSettings["fsENC_KEY"].ToString();
            return Encoding.UTF8.GetString(MachineKey.Unprotect(Convert.FromBase64String(encryptValue), encKey));
        }

        /// <summary>
        /// Encrypt a file.
        /// </summary>
        /// <param name="sourceFile">原始檔案(路徑字串) </param>
        /// <param name="encryptFile">加密後檔案(路徑字串) </param>
        public static void AddFileEncryption(string sourceFile, string encryptFile)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            string _fileEncKey = ConfigurationManager.AppSettings["fsFILE_ENC_KEY"].ToString()
                , _fileEncIV = ConfigurationManager.AppSettings["fsFILE_ENC_IV"].ToString();
            byte[] key = Encoding.ASCII.GetBytes(_fileEncKey);
            byte[] iv = Encoding.ASCII.GetBytes(_fileEncIV);

            des.Key = key;
            des.IV = iv;

            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
            using (FileStream encryptStream = new FileStream(encryptFile, FileMode.Create, FileAccess.Write))
            {
                //檔案加密
                byte[] dataByteArray = new byte[sourceStream.Length];
                sourceStream.Read(dataByteArray, 0, dataByteArray.Length);

                using (CryptoStream cs = new CryptoStream(encryptStream, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(dataByteArray, 0, dataByteArray.Length);
                    cs.FlushFinalBlock();
                }
            }
        }

        /// <summary>
        /// Decrypt a file.
        /// </summary>
        /// <param name="encryptFile">加密過檔案(路徑字串)</param>
        /// <param name="decryptFile">解密後檔案(路徑字串)</param>
        public static void RemoveFileEncryption(string encryptFile, string decryptFile)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            string _fileEncKey = ConfigurationManager.AppSettings["fsFILE_ENC_KEY"].ToString()
                , _fileEncIV = ConfigurationManager.AppSettings["fsFILE_ENC_IV"].ToString();
            byte[] key = Encoding.ASCII.GetBytes(_fileEncKey);
            byte[] iv = Encoding.ASCII.GetBytes(_fileEncIV);

            des.Key = key;
            des.IV = iv;

            using (FileStream encryptStream = new FileStream(encryptFile, FileMode.Open, FileAccess.Read))
            using (FileStream decryptStream = new FileStream(decryptFile, FileMode.Create, FileAccess.Write))
            {
                byte[] dataByteArray = new byte[encryptStream.Length];
                encryptStream.Read(dataByteArray, 0, dataByteArray.Length);
                using (CryptoStream cs = new CryptoStream(decryptStream, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(dataByteArray, 0, dataByteArray.Length);
                    cs.FlushFinalBlock();
                }
            }
        }

    }
}
