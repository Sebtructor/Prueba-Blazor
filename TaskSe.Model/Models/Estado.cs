using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Model.Models
{
    public class Estado
    {
        public string id_estado { get; set; }
        public string descripcion { get; set; }


        public static IEnumerable<Estado> estados = new List<Estado>() {
            new Estado() { 
                id_estado = "1", descripcion = "Activo"
            },
            new Estado() {
                id_estado = "2", descripcion = "Inactivo"
            }
        };
    }
}
