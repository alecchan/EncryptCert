using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EncryptCert
{
    public static class AppSettings
    {
        public static string ConnectionString
        {
            get
            {
                var cs = ConfigurationManager.ConnectionStrings["WingtipToys"].ConnectionString;

                var builder = new SqlConnectionStringBuilder(cs);
                string password = builder.Password;

                var encrypt = new ProtectString();               

                builder.Password = encrypt.Decrypt(password);

                return builder.ConnectionString;
            }
        }
    }

    public class ProtectString
    {
        //creating the certificate
        //makecert -r -pe -n "CN=WWW.ALECCHAN.COM" -b 01/01/2005 -e 01/01/2020 -sky exchange -ss my
        private X509Certificate2 _x509 = null;

        private static X509Certificate2 LoadCertificate(StoreName storeName, string certificateName)
        {
            X509Store store = new X509Store(storeName);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certCollection = store.Certificates;
            X509Certificate2 x509 = null;
            foreach (X509Certificate2 c in certCollection)
            {
                if (c.Subject == certificateName)
                {
                    x509 = c;
                    break;
                }
            }
            if (x509 == null)
                Console.WriteLine("A x509 certificate for " + certificateName + " was not found");

            store.Close();
            return x509;
        }

        private X509Certificate2 Certificate
        {
            get
            {
                if (_x509 == null)
                {
                    var name = ConfigurationManager.AppSettings["CertificateName"];
                    if (string.IsNullOrEmpty(name))
                        throw new ApplicationException("Certificate name was not supplied");

                    _x509 = LoadCertificate(StoreName.My, name);
                }

                return _x509;
            }
        }

        public string Encrypt(string str)
        {
            if (Certificate == null || string.IsNullOrEmpty(str))
                throw new Exception("A x509 certificate and string for encryption must be provided");

            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)Certificate.PublicKey.Key;
            byte[] bytestoEncrypt = ASCIIEncoding.ASCII.GetBytes(str);
            byte[] encryptedBytes = rsa.Encrypt(bytestoEncrypt, false);
            return Convert.ToBase64String(encryptedBytes);
        }

        public string Decrypt(string encrypted)
        {
            if (Certificate == null || string.IsNullOrEmpty(encrypted))
                throw new Exception("A x509 certificate and string for decryption must be provided");

            if (!Certificate.HasPrivateKey)
                throw new Exception("x509 certicate does not contain a private key for decryption");

            RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)Certificate.PrivateKey;
            byte[] bytestodecrypt = Convert.FromBase64String(encrypted);
            byte[] plainbytes = rsa.Decrypt(bytestodecrypt, false);
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetString(plainbytes);
        }
    }
}
