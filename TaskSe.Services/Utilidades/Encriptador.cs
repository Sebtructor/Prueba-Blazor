

using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Services.Utilidades
{
    public class Encriptador
    {
        //Encriptacion si llave
        public string Encriptar(string Cadena)
        {
            byte[] BtClearBytes;
            BtClearBytes = new UnicodeEncoding().GetBytes(Cadena);
            byte[] BitHashedBytes = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(BtClearBytes);
            string HashedText = BitConverter.ToString(BitHashedBytes);

            return HashedText;
        }

        //Encriptacion con llave
        public string Encriptar3DES(string Cadena, string Key)
        {
            try
            {
                TripleDESCryptoServiceProvider des;
                MD5CryptoServiceProvider hasmd5;

                byte[] keyHash, buff;
                string Encripted;

                hasmd5 = new MD5CryptoServiceProvider();
                keyHash = hasmd5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(Key));

                hasmd5 = null;
                des = new TripleDESCryptoServiceProvider();

                des.Key = keyHash;
                des.Mode = CipherMode.ECB;

                buff = ASCIIEncoding.ASCII.GetBytes(Cadena);
                Encripted = Convert.ToBase64String(des.CreateEncryptor().TransformFinalBlock(buff, 0, buff.Length));

                return Encripted;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error al encriptar cadena {Cadena}");
                throw ex;
            }
        }

        //Desencriptacion con llave
        public string Desencriptar3DES(string Cadena, string Key)
        {
            try
            {

                TripleDESCryptoServiceProvider des;
                MD5CryptoServiceProvider hasmd5;

                byte[] keyHash, buff;
                string Desencripted;

                hasmd5 = new MD5CryptoServiceProvider();
                keyHash = hasmd5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(Key));

                hasmd5 = null;
                des = new TripleDESCryptoServiceProvider();

                des.Key = keyHash;
                des.Mode = CipherMode.ECB;

                buff = Convert.FromBase64String(Cadena);
                Desencripted = ASCIIEncoding.ASCII.GetString(des.CreateDecryptor().TransformFinalBlock(buff, 0, buff.Length));

                return Desencripted;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error al desencriptar cadena {Cadena}");
                throw ex;
            }
        }
    }
}
