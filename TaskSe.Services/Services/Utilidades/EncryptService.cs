using iTextSharp.text.log;
using iTextSharp.text.pdf;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using TaskSe.Services.Interfaces.Utilidades;
using TaskSe.Services.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace TaskSe.Services.Services.Utilidades
{
    public class EncryptService : IEncryptService
    {
        public EncryptService()
        {

        }

        private string Decode(string str)
        {
            byte[] decbuff = Convert.FromBase64String(str.Replace(",", "=").Replace("-", "+").Replace("/", "_"));
            return System.Text.Encoding.UTF8.GetString(decbuff);
        }

        private string Encode(string input)
        {
            byte[] encbuff = Encoding.UTF8.GetBytes(input ?? "");
            return Convert.ToBase64String(encbuff).Replace("=", ",").Replace("+", "-").Replace("_", "/");
        }

        public string encriptarParametros(Dictionary<string, string> parametros)
        {
            string rutaClavePrivada = Directory.GetCurrentDirectory() + "\\RSAKeys\\private_key.pem";
            string rutaClavePublica = Directory.GetCurrentDirectory() + "\\RSAKeys\\public_key.pem";

            // Cargar clave privada desde archivo
            string clavePrivadaPEM = File.ReadAllText(rutaClavePrivada);
            TextReader privateKeyTextReader = new StringReader(clavePrivadaPEM);
            RsaPrivateCrtKeyParameters clavePrivada = (RsaPrivateCrtKeyParameters)new PemReader(privateKeyTextReader).ReadObject();

            // Cargar clave pública desde archivo
            string clavePublicaPEM = File.ReadAllText(rutaClavePublica);
            TextReader publicKeyTextReader = new StringReader(clavePublicaPEM);
            AsymmetricKeyParameter clavePublica = (AsymmetricKeyParameter)new PemReader(publicKeyTextReader).ReadObject();

            // Convertir los parámetros a JSON
            string parametrosJson = JsonConvert.SerializeObject(parametros);

            // Convertir el JSON en un arreglo de bytes
            byte[] parametrosBytes = Encoding.UTF8.GetBytes(parametrosJson);

            // Encriptar los parámetros utilizando la clave pública
            IBufferedCipher encriptador = CipherUtilities.GetCipher("RSA/ECB/OAEPWithSHA256AndMGF1Padding");
            encriptador.Init(true, clavePublica);
            byte[] parametrosEncriptados = encriptador.DoFinal(parametrosBytes);

            // Convertir el resultado en una cadena base64 para enviar en la URL
            string parametrosEncriptadosBase64 = Convert.ToBase64String(parametrosEncriptados);
            parametrosEncriptadosBase64 = Encode(parametrosEncriptadosBase64);

            return parametrosEncriptadosBase64;
        }

        public Dictionary<string, string> desencriptarParametros(string parametrosEncriptadosBase64)
        {
            parametrosEncriptadosBase64 = Decode(parametrosEncriptadosBase64);

            string rutaClavePrivada = Directory.GetCurrentDirectory() + "\\RSAKeys\\private_key.pem";
            string rutaClavePublica = Directory.GetCurrentDirectory() + "\\RSAKeys\\public_key.pem";

            // Cargar clave privada desde archivo
            string clavePrivadaPEM = File.ReadAllText(rutaClavePrivada);
            TextReader privateKeyTextReader = new StringReader(clavePrivadaPEM);
            RsaPrivateCrtKeyParameters clavePrivada = (RsaPrivateCrtKeyParameters)new PemReader(privateKeyTextReader).ReadObject();

            // Cargar clave pública desde archivo
            string clavePublicaPEM = File.ReadAllText(rutaClavePublica);
            TextReader publicKeyTextReader = new StringReader(clavePublicaPEM);
            AsymmetricKeyParameter clavePublica = (AsymmetricKeyParameter)new PemReader(publicKeyTextReader).ReadObject();

            // Decodificar los parámetros encriptados desde la cadena base64
            byte[] parametrosEncriptadosDecodificados = Convert.FromBase64String(parametrosEncriptadosBase64);

            // Desencriptar los parámetros utilizando la clave privada
            IBufferedCipher desencriptador = CipherUtilities.GetCipher("RSA/ECB/OAEPWithSHA256AndMGF1Padding");
            desencriptador.Init(false, clavePrivada);
            byte[] parametrosDesencriptados = desencriptador.DoFinal(parametrosEncriptadosDecodificados);

            // Convertir el resultado desencriptado a una cadena
            string parametrosDesencriptadosJson = Encoding.UTF8.GetString(parametrosDesencriptados);

            // Convertir la cadena JSON a objetos
            Dictionary<string, string> parametros = JsonConvert.DeserializeObject<Dictionary<string, string>>(parametrosDesencriptadosJson);

            return parametros;
        }

        public string desEncrypCadena(string cadena, string key)
        {
            Encriptador objEncriptacion = new Encriptador();

            return objEncriptacion.Desencriptar3DES(cadena, key);
        }

        public bool encriptarPDF(string ruta_archivo_origen, string ruta_archivo_final, string contraseña)
        {
            using (var input = new FileStream(ruta_archivo_origen, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var output = new FileStream(ruta_archivo_final, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var reader = new PdfReader(input);
                PdfEncryptor.Encrypt(reader, output, true, contraseña, contraseña, PdfWriter.ALLOW_PRINTING);
            }

            if (File.Exists(ruta_archivo_final)) return true;

            return false;
        }

        public string encrypCadena(string cadena, string key)
        {
            Encriptador objEncriptacion = new Encriptador();

            return objEncriptacion.Encriptar3DES(cadena, key);
        }

        public string encrypCadenaRSA(string cadena)
        {
            string rutaClavePrivada = Directory.GetCurrentDirectory() + "\\RSAKeys\\private_key.pem";
            string rutaClavePublica = Directory.GetCurrentDirectory() + "\\RSAKeys\\public_key.pem";

            // Cargar clave privada desde archivo
            string clavePrivadaPEM = File.ReadAllText(rutaClavePrivada);
            TextReader privateKeyTextReader = new StringReader(clavePrivadaPEM);
            RsaPrivateCrtKeyParameters clavePrivada = (RsaPrivateCrtKeyParameters)new PemReader(privateKeyTextReader).ReadObject();

            // Cargar clave pública desde archivo
            string clavePublicaPEM = File.ReadAllText(rutaClavePublica);
            TextReader publicKeyTextReader = new StringReader(clavePublicaPEM);
            AsymmetricKeyParameter clavePublica = (AsymmetricKeyParameter)new PemReader(publicKeyTextReader).ReadObject();

            byte[] parametrosBytes = Encoding.UTF8.GetBytes(cadena);

            // Encriptar los parámetros utilizando la clave pública
            IBufferedCipher encriptador = CipherUtilities.GetCipher("RSA/ECB/OAEPWithSHA256AndMGF1Padding");
            encriptador.Init(true, clavePublica);
            byte[] parametrosEncriptados = encriptador.DoFinal(parametrosBytes);

            // Convertir el resultado en una cadena base64 para enviar en la URL
            string parametrosEncriptadosBase64 = Convert.ToBase64String(parametrosEncriptados);
            parametrosEncriptadosBase64 = Encode(parametrosEncriptadosBase64);

            return parametrosEncriptadosBase64;
        }

        public string desEncrypCadenaRSA(string cadena_encriptada)
        {
            cadena_encriptada = Decode(cadena_encriptada);

            string rutaClavePrivada = Directory.GetCurrentDirectory() + "\\RSAKeys\\private_key.pem";
            string rutaClavePublica = Directory.GetCurrentDirectory() + "\\RSAKeys\\public_key.pem";

            // Cargar clave privada desde archivo
            string clavePrivadaPEM = File.ReadAllText(rutaClavePrivada);
            TextReader privateKeyTextReader = new StringReader(clavePrivadaPEM);
            RsaPrivateCrtKeyParameters clavePrivada = (RsaPrivateCrtKeyParameters)new PemReader(privateKeyTextReader).ReadObject();

            // Cargar clave pública desde archivo
            string clavePublicaPEM = File.ReadAllText(rutaClavePublica);
            TextReader publicKeyTextReader = new StringReader(clavePublicaPEM);
            AsymmetricKeyParameter clavePublica = (AsymmetricKeyParameter)new PemReader(publicKeyTextReader).ReadObject();

            // Decodificar los parámetros encriptados desde la cadena base64
            byte[] parametrosEncriptadosDecodificados = Convert.FromBase64String(cadena_encriptada);

            // Desencriptar los parámetros utilizando la clave privada
            IBufferedCipher desencriptador = CipherUtilities.GetCipher("RSA/ECB/OAEPWithSHA256AndMGF1Padding");
            desencriptador.Init(false, clavePrivada);
            byte[] parametrosDesencriptados = desencriptador.DoFinal(parametrosEncriptadosDecodificados);

            return Encoding.UTF8.GetString(parametrosDesencriptados);

        }
    }
}
