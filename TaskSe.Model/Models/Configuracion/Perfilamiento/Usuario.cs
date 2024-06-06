using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Model.Models.Configuracion.Perfilamiento
{
    public class Usuario
    {
        public string id_usuario { get; set; } = "";
        public string nombres { get; set; } = "";
        public string apellidos { get; set; } = "";
        public string usuario { get; set; } = "";
        public string clave { get; set; } = "";
        public string email { get; set; } = "";
        public string email_dos { get; set; } = "";
        public string id_rol { get; set; } = "";
        public string descripcion_rol { get; set; } = "";
        public string estado { get; set; } = "";
        public string celular { get; set; } = "";
        public bool TWOFA_ENABLED { get; set; } = false;
    }
}
