using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Model.Models.Configuracion.Perfilamiento
{
    public class Rol
    {
        public string id_rol { get; set; } = string.Empty;
        public string nombre_rol { get; set; } = string.Empty;
        public string estado { get; set; } = string.Empty;

        public IEnumerable<Modulo> modulos { get; set; }
    }
}
