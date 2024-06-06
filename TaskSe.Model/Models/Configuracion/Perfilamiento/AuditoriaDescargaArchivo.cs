using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Model.Models.Configuracion.Perfilamiento
{
    public class AuditoriaDescargaArchivo
    {
        public string ruta_original { get; set; } = "";
        public string ruta_descargada { get; set; } = "";
        public string nombre_archivo { get; set; } = "";
        public string extension_archivo { get; set; } = "";
        public string peso_archivo { get; set; } = "";
        public string usuario { get; set; } = "";
        public string ip_address { get; set; } = "";
    }
}
