using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Model.Models.Mensajes
{
    public class Mensaje
    {
        public string id_mensaje { get; set; } = string.Empty;
        public string celular_destino { get; set; } = string.Empty;
        public string celular_emisor { get; set; } = string.Empty;
        public string contenido_mensaje { get; set; } = string.Empty;
        public DateTime fecha_envio { get; set; }
        public string estado { get; set; } = string.Empty;
        public int tipo_mensaje { get; set; } = 0;
        public string numero_identificacion { get; set; } = string.Empty;
        public string concepto { get; set; } = string.Empty;
    }
}
