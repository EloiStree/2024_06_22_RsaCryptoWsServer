using System.Security.Cryptography;

namespace Rsa1024DLL
{
    public class RsaLib
    {

        public static byte[] Encrypt(byte[] data, string publicKey)
        {
            RSA rsa = RSA.Create();
            rsa.KeySize = 1024;
            rsa.FromXmlString(publicKey);
            return rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
        }

        public static byte[] Decrypt(byte[] data, string privateKey)
        {
            RSA rsa = RSA.Create();
            rsa.KeySize = 1024;
            rsa.FromXmlString(privateKey);
            return rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
        }
        
        public static string GeneratePrivateKeyXml()
        {
            RSA rsa = RSA.Create();
            rsa.KeySize = 1024;
            return rsa.ToXmlString(true);
        }
        public static string GetPublicKeyXmlFromPrivate(string privateKeyXml) {
            RSA rsa = RSA.Create();
            rsa.KeySize = 1024;
            rsa.FromXmlString(privateKeyXml);
            return rsa.ToXmlString(false);
        }

        public static void GenerateKeyPair(out string publicKeyXml, out string privateKeyXml)
        {
            RSA rsa = RSA.Create();
            rsa.KeySize = 1024;
            publicKeyXml = rsa.ToXmlString(false);
            privateKeyXml = rsa.ToXmlString(true);
        }



        public static byte[] SignDataFromTextUTF8(string textUTF8, string privateRsaXml) { 
        
            return SignData(System.Text.Encoding.UTF8.GetBytes(textUTF8), privateRsaXml);
        }
        public static byte[] SignData(byte[] data, string privateRsaXml )
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.FromXmlString(privateRsaXml);
                rsa.ImportParameters(rsa.ExportParameters(true));
                return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }

        public static string ParseBytesToB64(byte[] bytesToTurnInB64) {
            return System.Convert.ToBase64String(bytesToTurnInB64);
        }
        public static byte[] ParseB64ToBytes(string b64Text) {
            return System.Convert.FromBase64String(b64Text);
        }

        public static bool VerifySignatureFromTextUTF8(string textUTF8, byte[] signature, string publicKeyXml) {
            return VerifySignature(System.Text.Encoding.UTF8.GetBytes(textUTF8), signature, publicKeyXml);
        }
        public static bool VerifySignature(byte[] data, byte[] signature, string publicKeyXml)
        {
            using (RSA rsa = RSA.Create())
            {
                rsa.FromXmlString(publicKeyXml);
                rsa.ImportParameters(rsa.ExportParameters(false));
                return rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }
    }
}
