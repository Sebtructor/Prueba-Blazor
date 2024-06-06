using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Services.Interfaces.Utilidades
{
    public interface IEncryptService
    {
        public string encrypCadena(string cadena, string key);
        public string desEncrypCadena(string cadena, string key);
        public bool encriptarPDF(string ruta_archivo_origen, string ruta_archivo_final, string contraseña);
        public string encriptarParametros(Dictionary<string, string> parametros);
        public Dictionary<string, string> desencriptarParametros(string parametrosEncriptadosBase64);
        public string encrypCadenaRSA(string cadena);
        public string desEncrypCadenaRSA(string cadena_encriptada);
    }
}
