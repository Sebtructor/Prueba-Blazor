namespace TaskSe.Web.Controllers
{
    public class DatosClienteModel
    {
        public string UserAgent { get; set; } = "";
        public string Navegador { get; set; } = "";
        public string VersionNavegador { get; set; } = "";
        public string PlataformaNavegador { get; set; } = "";
        public string Url { get; set; } = "";
        public string Idioma { get; set; } = "";
        public string Sesion { get; set; } = "";
        public string actualUser { get; set; } = "";
        public bool CookiesHabilitadas { get; set; }
        public ResolucionPantallaModel ResolucionPantalla { get; set; } = new ResolucionPantallaModel();
        public int ProfundidadColor { get; set; }
        public SistemaOperativoModel SistemaOperativo { get; set; } = new SistemaOperativoModel();
        public UbicacionModel? Ubicacion { get; set; }

        public class ResolucionPantallaModel
        {
            public int Ancho { get; set; }
            public int Alto { get; set; }
        }

        public class SistemaOperativoModel
        {
            public string Nombre { get; set; } = "";
            public string Version { get; set; } = "";
        }

        public class UbicacionModel
        {
            public double Latitud { get; set; }
            public double Longitud { get; set; }
        }
    }
}
