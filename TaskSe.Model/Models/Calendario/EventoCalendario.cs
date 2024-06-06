using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Model.Models.Calendario
{
    public class EventoCalendario
    {
        public string id_evento { get; set; } = "";
        public DateTime? fecha_inicio { get; set; }
        public DateTime? fecha_fin { get; set; }
        public string descripcion { get; set; } = "";
    }
}
