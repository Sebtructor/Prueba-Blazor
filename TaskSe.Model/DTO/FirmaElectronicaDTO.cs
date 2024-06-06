using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Model.DTO
{
    public class FirmaElectronicaDTO
    {
        public string ruta_original { get; set; } = string.Empty;
        public string ruta_final { get; set; } = string.Empty;
        public string numero_documento_proceso { get; set; } = string.Empty;
        public string descripcion { get; set; } = string.Empty;
        public string usuario { get; set; } = string.Empty;
        public string ip_accion { get; set; } = string.Empty;
        public bool estado_firma { get; set; } = false;
    }
}
