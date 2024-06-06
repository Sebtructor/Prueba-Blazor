using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Model.Models.CargueMasivo
{
    public class CargueMasivoDTO
    {
        public int id_cargue { get; set; }
        public string descripcion { get; set; } = string.Empty;
        public string estado { get; set; } = string.Empty;
        public int total_registros { get; set; } = 0;
        public int total_registros_procesados { get; set; } = 0;
        public int total_registros_no_procesados { get; set; } = 0;
    }
}
