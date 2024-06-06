using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Model.Models.OTP
{
    public class OTPModel
    {
        public string id_otp_code {get; set;} = string.Empty;
        public string otp_code {get; set;} = string.Empty;
        public string estado {get; set;} = string.Empty;
        public string numero_documento_proceso {get; set;} = string.Empty;
        public string tipo_proceso {get; set;} = string.Empty;
        public string descripcion { get; set; } = string.Empty;
        public string metodos_envio { get; set; } = string.Empty;
        public DateTime fecha_adicion { get; set; }
    }
}
