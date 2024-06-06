using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Model.Models.Configuracion.Perfilamiento
{
    public class Secret2FA
    {
        public string ID_SECRETO { get; set; } = "";
        public string SECRETO { get; set; } = "";
        public string USUARIO { get; set; } = "";
        public string RUTA_QR { get; set; } = "";
        public DateTime FECHA_ADICION { get; set; }
    }
}
