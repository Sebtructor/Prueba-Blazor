using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Model.Models.FirmaDigital
{
    public class Firma
    {
        public string id_firma { get; set; } = string.Empty;
        public string identificacion_cliente { get; set; } = string.Empty;
        public string nombre_archivo { get; set; } = string.Empty;
        public string direccion_ip { get; set; } = string.Empty;
        public string descripcion { get; set; } = string.Empty;
        public string usuario { get; set; } = string.Empty;
        public DateTime fecha_firma { get; set; }
        public int tipo_firma { get; set; } = 0;

    }
}
