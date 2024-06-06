using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Model.Models.Configuracion.Perfilamiento
{
    public class SolicitudContraseña
    {
        public string id_solicitud { get; set; } = string.Empty;
        public string estado { get; set; } = string.Empty;
        public string usuario { get; set; } = string.Empty;
        public DateTime fecha { get; set; }
    }
}
