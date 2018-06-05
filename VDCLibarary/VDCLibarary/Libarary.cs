using System;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace VDCSDK
{
    public sealed class App
    {
        public static int ChunkSize = 1024 * 1024;
        private static string userName;
        private static string emailFrom;
        private static string emailTo;
        private static string pass;
        const string userRoot = "HKEY_CURRENT_USER";
        const string subkey = "PVEAPP";
        const string keyName = userRoot + "\\" + subkey;

        public static void AppSDK_Init(string key)
        {
            try
            {
                string encryptEmail = DecryptKey(key, true);
                emailFrom = encryptEmail.Split('/')[0];
                pass = encryptEmail.Split('/')[1];
                emailTo = encryptEmail.Split('/')[2];
                userName = emailFrom.Split('@')[0];

            } catch(Exception ex)
            {
                throw ex;
            }

        }

        public static void AppSDK_Init(string mailFrom, string passMail, string mailTo)
        {
            try
            {
                emailFrom = mailFrom;
                pass = passMail;
                emailTo = mailTo;
                userName = emailFrom.Split('@')[0];

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        /// <summary>EncryptVideo</summary>
        /// <param name="input"></param>
        /// <param name="Key"></param>
        /// <param name="IV"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static byte[] EncryptVideo(byte[] input, byte[] Key, byte[] IV, int size)
        {
            byte[] output;

            TripleDESCryptoServiceProvider TDES = new TripleDESCryptoServiceProvider();

            using (MemoryStream EncryptedDataStream = new MemoryStream())
            {
                using (CryptoStream CryptoStream = new CryptoStream(EncryptedDataStream, TDES.CreateEncryptor(Key, IV), CryptoStreamMode.Write))
                {
                    CryptoStream.Write(input, 0, size);
                    CryptoStream.FlushFinalBlock();

                    output = new byte[EncryptedDataStream.Length];
                    EncryptedDataStream.Position = 0;
                    EncryptedDataStream.Read(output, 0, output.Length);
                }
            }

            return output;
        }
        /// <summary>DecryptVideo</summary>
        /// <param name="input"></param>
        /// <param name="Key"></param>
        /// <param name="IV"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static byte[] DecryptVideo(byte[] input, byte[] Key, byte[] IV, int size)
        {
            byte[] output = new byte[size];

            TripleDESCryptoServiceProvider TDES = new TripleDESCryptoServiceProvider();

            using (MemoryStream EncryptedDataStream = new MemoryStream(input))
            {
                using (CryptoStream CryptoStream = new CryptoStream(EncryptedDataStream, TDES.CreateDecryptor(Key, IV), CryptoStreamMode.Read))
                {
                    CryptoStream.Read(output, 0, size);
                }
            }

            return output;
        }

        /// <summary>EncryptKey</summary>
        /// <param name="strEncrypt"></param>
        /// <param name="useHashing"></param>
        /// <returns></returns>
        public static string EncryptKey(string strEncrypt, bool useHashing)
        {
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(strEncrypt);

                AppSettingsReader settingsReader = new AppSettingsReader();

                if (useHashing)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(subkey));
                    hashmd5.Clear();
                }
                else
                    keyArray = UTF8Encoding.UTF8.GetBytes(subkey);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                tdes.Clear();
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>DecryptKey</summary>
        /// <param name="strCipher"></param>
        /// <param name="useHashing"></param>
        /// <returns></returns>
        public static string DecryptKey(string strCipher, bool useHashing)
        {
            try
            {
                byte[] keyArray;
                byte[] toEncryptArray = Convert.FromBase64String(strCipher);

                AppSettingsReader settingsReader = new AppSettingsReader();

                if (useHashing)
                {
                    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
                    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(subkey));
                    hashmd5.Clear();
                }
                else
                    keyArray = UTF8Encoding.UTF8.GetBytes(subkey);

                TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
                tdes.Key = keyArray;
                tdes.Mode = CipherMode.ECB;
                tdes.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = tdes.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                tdes.Clear();
                return UTF8Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>GetMd5Hash</summary>
        /// <param name="md5Hash"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string GetMd5Hash(MD5 md5Hash, string input)
        {
            try
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }catch(Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>Verify a hash against a string.</summary>
        /// <param name="md5Hash"></param>
        /// <param name="input"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            string hashOfInput = GetMd5Hash(md5Hash, input);
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool RegisterKey(string key, string value)
        {
            try
            {
                Microsoft.Win32.Registry.SetValue(keyName, key, value);
                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public static string GetValueOfKey(string key)
        {
            try
            {
                string value = (string)Microsoft.Win32.Registry.GetValue(keyName,
                 key, null);

                return value;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bool SendMail(string mailcc,string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress(emailFrom);
                mail.To.Add(emailTo);

                if (!String.IsNullOrEmpty(mailcc))
                {
                    mail.CC.Add(mailcc);
                }
                mail.Subject = subject;
                mail.Body = body;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential(userName, pass);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
