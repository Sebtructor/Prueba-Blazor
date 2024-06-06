using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Model.Models.ANI
{
    public class AniDTO
    {
        public string id_consulta{get; set;} = string.Empty;
        public string cedula{get; set;} = string.Empty;
        public string departamento_expedicion{get; set;} = string.Empty;
        public string fecha_expedicion{get; set;} = string.Empty;
        public string municipio_expedicion{get; set;} = string.Empty;
        public string nuip{get; set;} = string.Empty;
        public string primer_apellido{get; set;} = string.Empty;
        public string segundo_apellido{get; set;} = string.Empty;
        public string primer_nombre{get; set;} = string.Empty;
        public string segundo_nombre{get; set;} = string.Empty;
        public string año_resolucion{get; set;} = string.Empty;
        public string particula{get; set;} = string.Empty;
        public string estado_cedula{get; set;} = string.Empty;
        public string codigo_error_cedula{get; set;} = string.Empty;
        public string numero_resolucion{get; set;} = string.Empty;
        public string status_code{get; set;} = string.Empty;
        public DateTime fecha_consulta { get; set; }
       
    }
}
