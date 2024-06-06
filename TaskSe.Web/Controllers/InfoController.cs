using TaskSe.Model.Models.Configuracion.Perfilamiento;
using TaskSe.Services.Interfaces.Auditoria;
using TaskSe.Services.Interfaces.Configuracion.Perfilamiento;
using TaskSe.Web.Authentication;
using TaskSe.Web.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Sockets;
using System.Net;

namespace TaskSe.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly ILogger<InfoController> _logger;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IUserService _userService;
        private readonly AuthenticationStateProvider _authStateProvider;


        public InfoController(
            ILogger<InfoController> logger,
            IAuditoriaService auditoriaService,
            IUserService userService,
            AuthenticationStateProvider authStateProvider
            )
        {
            _logger = logger;
            _auditoriaService = auditoriaService;
            _authStateProvider = authStateProvider;
            _userService = userService;
        }

        [HttpGet]
        [Route("ipaddress")]
        public async Task<string> GetIpAddress()
        {
            try
            {
                var remoteIpAddress = HttpContext.Request.HttpContext.Connection.RemoteIpAddress;
                if (remoteIpAddress != null)
                    return remoteIpAddress.ToString();
            }
            catch (Exception exe)
            {
                _logger.LogError(exe, "Error al obtener ipaddress en InfoControler");
            }
            return string.Empty;
        }

        [HttpPost]
        [Route("sendClientInfo")]
        public async Task getClientInfo(DatosClienteModel datosCliente)
        {
            try
            {
                var remoteIpAddress = HttpContext.Request.HttpContext.Connection.RemoteIpAddress;

                Usuario user = await _userService.getUsuarioByUser(datosCliente.actualUser);

                string usuarioAccion = user is null ? "" : user.usuario;
                string rolUsuario = user is null ? "" : user.id_rol;
                string idUsuario = user is null ? "" : user.id_usuario;

                string ubicacion = "Permitida";

                if (datosCliente.Ubicacion is null)
                {
                    ubicacion = "No permitida";
                    datosCliente.Ubicacion = new DatosClienteModel.UbicacionModel() { Latitud = -1, Longitud = -1 };
                }

                AuditoriaNavegacion auditoria = new AuditoriaNavegacion()
                {
                    UserAgent = datosCliente.UserAgent,
                    Navegador = datosCliente.Navegador,
                    AltoPantalla = datosCliente.ResolucionPantalla.Alto,
                    AnchoPantalla = datosCliente.ResolucionPantalla.Ancho,
                    CookiesHabilitadas = datosCliente.CookiesHabilitadas,
                    Idioma = datosCliente.Idioma,
                    idUsuario = idUsuario,
                    ip_address = remoteIpAddress.ToString(),
                    Latitud = datosCliente.Ubicacion.Latitud.ToString(),
                    Longitud = datosCliente.Ubicacion.Longitud.ToString(),
                    NombreSO = datosCliente.SistemaOperativo.Nombre,
                    PlataformaNavegador = datosCliente.PlataformaNavegador,
                    ProfundidadColor = datosCliente.ProfundidadColor,
                    rolUsuario = rolUsuario,
                    UrlActual = datosCliente.Url,
                    usuario_accion = usuarioAccion,
                    VersionNavegador = datosCliente.VersionNavegador,
                    VersionSO = datosCliente.SistemaOperativo.Version,
                    ubicacion = ubicacion
                };

                if (!await _auditoriaService.registrarAuditoriaNavegacion(auditoria))
                {
                    _logger.LogError("Ocurrió un error al guardar los datos de navegación");
                }
            }
            catch (Exception exe)
            {
                _logger.LogError(exe, "Error al procesar clientinfo");
            }
        }

    }
}
