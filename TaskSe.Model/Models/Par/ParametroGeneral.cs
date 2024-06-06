using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Model.Models.Par
{
    public class ParametroGeneral
    {
        public string id_parametro_general { get; set; } = "";
        public string nombre_parametro_general { get; set; } = "";
        public List<ParametroDetallado> listaParametrosDetallados { get; set; } = new List<ParametroDetallado>();
    }
}
