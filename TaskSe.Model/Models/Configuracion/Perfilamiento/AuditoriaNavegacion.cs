using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Model.Models.Configuracion.Perfilamiento
{
    public class AuditoriaNavegacion
    {
        public string UserAgent { get; set; } = "";
        public string Navegador { get; set; } = "";
        public string VersionNavegador { get; set; } = "";
        public string PlataformaNavegador { get; set; } = "";
        public string UrlActual { get; set; } = "";
        public string Idioma { get; set; } = "";
        public bool CookiesHabilitadas { get; set; }
        public int AnchoPantalla { get; set; }
        public int AltoPantalla { get; set; }
        public int ProfundidadColor { get; set; }
        public string NombreSO { get; set; } = "";
        public string VersionSO { get; set; } = "";
        public string ubicacion { get; set; } = "";
        public string Latitud { get; set; } = "";
        public string Longitud { get; set; } = "";
        public string ip_address { get; set; } = "";
        public string usuario_accion { get; set; } = "";
        public string rolUsuario { get; set; } = "";
        public string idUsuario { get; set; } = "";
        public string UBICACION_IP { get; set; } = "";
    }
}
