using TaskSe.Data.Connection;
using TaskSe.Data.Repositories.Repositories.ANI;
using TaskSe.Data.Repositories.Repositories.FirmaDigital;
using TaskSe.Data.Repositories.Interfaces.ANI;
using TaskSe.Data.Repositories.Interfaces.FirmaDigital;
using TaskSe.Model.Models.ANI;
using TaskSe.Services.Interfaces.ANI;
using iTextSharp.text.log;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Services.Services.ANI
{
    public class ANIService : IANIService
    {
        private readonly IANIRepository _aniRepository;
        private readonly ILogger<ANIService> _logger;
        public ANIService(SqlConfiguration sqlConfiguration,
            ILogger<ANIService> logger)
        {
            _aniRepository = new ANIRepository(sqlConfiguration.ConnectionString);
            _logger = logger;
        }

        public async Task<AniDTO> consultarRegistroById(string id_consulta)
        {
            return await _aniRepository.consultarRegistroById(id_consulta);
        }

        public async Task<AniDTO> consultarRegistroByNumDoc(string numero_documento)
        {
            return await _aniRepository.consultarRegistroByNumDoc(numero_documento);
        }

        public async Task<IEnumerable<AniDTO>> consultarValidacionesANI()
        {
            return await _aniRepository.consultarValidacionesANI();
        }

        #region Consumo API portal servicios

        private string getToken()
        {
            string token = "";

            try
            {

                var client = new RestClient("https://valid.excellentiam.co:10012");
                client.Timeout = -1;
                var request = new RestRequest("/api/Auth/login", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                var body = @"{
" + "\n" +
                @"  ""nombreUsuario"": ""ConsultaPSESE"",
" + "\n" +
                @"  ""contraseña"": ""ConsultaPSESE_23*!""
" + "\n" +
                @"}";
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                int StatusCode = (int)response.StatusCode;

                if (StatusCode == 200)
                {
                    dynamic JsonResponse = JsonConvert.DeserializeObject(response.Content);

                    token = JsonResponse;
                }
            }
            catch (Exception exe)
            {

            }

            return token;
        }

        private AniDTO consultarRegistroANIPortalServicios(string token, string num_doc)
        {
            AniDTO ani = null;

            try
            {
                var client = new RestClient("https://valid.excellentiam.co:10012");
                client.Timeout = -1;
                var request = new RestRequest($"/api/ANI/consultarRegistroBaseANI/{num_doc}", Method.GET);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", $"Bearer {token}");

                IRestResponse response = client.Execute(request);
                int StatusCode = (int)response.StatusCode;

                if (StatusCode == 200)
                {
                    ani = new AniDTO();
                    dynamic JsonResponse = JsonConvert.DeserializeObject(response.Content);

                    ani.departamento_expedicion = JsonResponse.dEPARTAMENTO_EXPEDICION;
                    ani.fecha_expedicion = JsonResponse.fECHA_EXPEDICION;
                    ani.municipio_expedicion = JsonResponse.mUNICIPIO_EXPEDICION;
                    ani.nuip = JsonResponse.nIUP;
                    ani.primer_apellido = JsonResponse.pRIMER_APELLIDO;
                    ani.segundo_apellido = JsonResponse.sEGUNDO_APELLIDO;
                    ani.primer_nombre = JsonResponse.pRIMER_NOMBRE;
                    ani.segundo_nombre = JsonResponse.sEGUNDO_NOMBRE;
                    ani.año_resolucion = JsonResponse.aNIO_RESOLUCION;
                    ani.particula = JsonResponse.pARTICULA;
                    ani.codigo_error_cedula = JsonResponse.cODIGO_ERROR_CEDULA;
                    ani.estado_cedula = JsonResponse.eSTADO_CEDULA;
                }
            }
            catch (Exception exe)
            {

            }

            return ani;

        }

        private bool insertarRegistroANIPortalServicios(string token, AniDTO infoANI)
        {
            try
            {
                var client = new RestClient("https://valid.excellentiam.co:10012");
                client.Timeout = -1;
                var request = new RestRequest("/api/ANI/registrarANI", Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", $"Bearer {token}");
                var body = @"{
" + "\n" +
                @"  ""cEDULA"": """ + infoANI.cedula + @""",
" + "\n" +
                @"  ""dEPARTAMENTO_EXPEDICION"": """ + infoANI.departamento_expedicion + @""",
" + "\n" +
                @"  ""fECHA_EXPEDICION"": """ + infoANI.fecha_expedicion + @""",
" + "\n" +
                @"  ""mUNICIPIO_EXPEDICION"": """ + infoANI.municipio_expedicion + @""",
" + "\n" +
                @"  ""nIUP"": """",
" + "\n" +
                @"  ""pRIMER_APELLIDO"": """ + infoANI.primer_apellido + @""",
" + "\n" +
                @"  ""sEGUNDO_APELLIDO"": """ + infoANI.segundo_apellido + @""",
" + "\n" +
                @"  ""pRIMER_NOMBRE"": """ + infoANI.primer_nombre + @""",
" + "\n" +
                @"  ""sEGUNDO_NOMBRE"": """ + infoANI.segundo_nombre + @""",
" + "\n" +
                @"  ""aNIO_RESOLUCION"": """",
" + "\n" +
                @"  ""pARTICULA"": """",
" + "\n" +
                @"  ""eSTADO_CEDULA"": """ + infoANI.estado_cedula + @""",
" + "\n" +
                @"  ""cODIGO_ERROR_CEDULA"": """ + infoANI.codigo_error_cedula + @""",
" + "\n" +
                @"  ""nUMERO_RESOLUCION"": """",
" + "\n" +
                @"  ""sTATUS_CODE"": ""200"",
" + "\n" +
                @"  ""uSUARIO"": """",
" + "\n" +
                @"  ""iD_AUDITORIA"": """",
" + "\n" +
                @"  ""concepto"": """ + "SBS Bicicletas" + @"""
" + "\n" +
                @"}";
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                int StatusCode = (int)response.StatusCode;

                if (StatusCode == 200)
                {
                    return true;
                }
            }
            catch (Exception exe)
            {

            }

            return false;
        }

        #endregion

        public async Task<AniDTO> consultarANIOlimpia(string numero_documento, string ipAccion, string usuarioAccion)
        {
            AniDTO consulta = null;

            try
            {
                string AnioResolucion = "", CodigoErrorDatosCedula = "",
                    DepartamentoExpedicion = "", EstadoCedula = "", FechaExpedicion = "",
                    MunicipioExpedicion = "", NUIP = "", NumeroResolucion = "", Particula = "",
                    PrimerApellido = "", SegundoApellido = "", PrimerNombre = "", SegundoNombre = "",
                    DescripcionRespuesta = "";

                var client = new RestClient("https://recani.olimpiait.com:8083/api/ValidacionAni");
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Basic dXN1X2V4Y2VsbGVudGlhbTprViR1NUtqejJtQVZ0cXlR");
                var body = @"{" + "\n" +
               @"	""CodigoAplicacion"":""4FB1F7A7-4CE5-4F32-AAA6-77AEA695F259""," + "\n" +
                @"	""Documento"":""" + numero_documento + @"""," + "\n" +
                @"	""TipoDocumento"":""1""" + "\n" +
                @"} ";
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                int StatusCode = (int)response.StatusCode;

                string id_consulta = await _aniRepository.insertarAuditoriaConsultaANI(numero_documento, StatusCode.ToString(), ipAccion, usuarioAccion);

                if (StatusCode == 200)
                {
                    consulta = new AniDTO();
                    consulta.id_consulta = id_consulta;
                    dynamic JsonResponse = JsonConvert.DeserializeObject(response.Content);

                    try
                    {
                        CodigoErrorDatosCedula = JsonResponse.Respuesta.CodigoErrorDatosCedula;
                        switch (CodigoErrorDatosCedula)
                        {
                            case "0":
                                CodigoErrorDatosCedula = "OK – Candidato encontrado";
                                break;
                            case "1":
                                CodigoErrorDatosCedula = "Candidato no encontrado";
                                break;
                            case "2":
                                CodigoErrorDatosCedula = "Campos de entrada con formato erróneo";
                                break;
                            case "3":
                                CodigoErrorDatosCedula = "Error interno del servicio";
                                break;
                        }
                    }
                    catch (Exception ex)
                    {

                    }

                    try
                    {
                        EstadoCedula = JsonResponse.Respuesta.EstadoCedula;
                        switch (EstadoCedula)
                        {
                            case "0":
                                EstadoCedula = "Vigente";
                                break;
                            case "1":
                                EstadoCedula = "Vigente";
                                break;
                            case "12":
                                EstadoCedula = "Baja por Pérdida o Suspensión de los Derechos Políticos";
                                break;
                            case "14":
                                EstadoCedula = "Baja por Interdicción Judicial por Demencia";
                                break;
                            case "21":
                                EstadoCedula = "Cancelada por Muerte";
                                break;
                            case "22":
                                EstadoCedula = "Cancelada por Doble Cedulación";
                                break;
                            case "23":
                                EstadoCedula = "Cancelada por Suplantación o Falsa Identidad";
                                break;
                            case "24":
                                EstadoCedula = "Cancelada por Menoría de Edad";
                                break;
                            case "25":
                                EstadoCedula = "Cancelada por Extranjería";
                                break;
                            case "26":
                                EstadoCedula = "Cancelada por Mala Elaboración";
                                break;
                            case "27":
                                EstadoCedula = "Cancelada por Reasignación o cambio de sexo";
                                break;
                            case "51":
                                EstadoCedula = "Cancelada por Muerte Facultad Ley 1365 2009";
                                break;
                            case "52":
                                EstadoCedula = "Cancelada por Intento de Doble Cedulación NO Expedida";
                                break;
                            case "53":
                                EstadoCedula = "Cancelada por Falsa Identidad o Suplantación NO Expedida";
                                break;
                            case "54":
                                EstadoCedula = "Cancelada por Menoría de Edad NO Expedida";
                                break;
                            case "55":
                                EstadoCedula = "Cancelada por Extranjería NO Expedida";
                                break;
                            case "56":
                                EstadoCedula = "Cancelada por Mala Elaboración No Expedida";
                                break;

                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    try
                    {
                        FechaExpedicion = JsonResponse.Respuesta.FechaExpedicion;
                        MunicipioExpedicion = JsonResponse.Respuesta.MunicipioExpedicion;
                        DepartamentoExpedicion = JsonResponse.Respuesta.DepartamentoExpedicion;
                        NUIP = JsonResponse.Respuesta.NUIP;
                        AnioResolucion = JsonResponse.Respuesta.AnioResolucion;
                        NumeroResolucion = JsonResponse.Respuesta.NumeroResolucion;
                        Particula = JsonResponse.Respuesta.Particula;
                        PrimerApellido = JsonResponse.Respuesta.PrimerApellido;
                        SegundoApellido = JsonResponse.Respuesta.SegundoApellido;
                        PrimerNombre = JsonResponse.Respuesta.PrimerNombre;
                        SegundoNombre = JsonResponse.Respuesta.SegundoNombre;
                        DescripcionRespuesta = JsonResponse.DescripcionRespuesta;


                        consulta.estado_cedula = EstadoCedula;
                        consulta.departamento_expedicion = DepartamentoExpedicion;
                        consulta.fecha_expedicion = FechaExpedicion;
                        consulta.municipio_expedicion = MunicipioExpedicion;
                        consulta.primer_nombre = JsonResponse.Respuesta.PrimerNombre.ToString();
                        consulta.segundo_nombre = JsonResponse.Respuesta.SegundoNombre.ToString();
                        consulta.primer_apellido = JsonResponse.Respuesta.PrimerApellido.ToString();
                        consulta.segundo_apellido = JsonResponse.Respuesta.SegundoApellido.ToString();
                        consulta.codigo_error_cedula = CodigoErrorDatosCedula;
                        consulta.nuip = NUIP;
                        consulta.año_resolucion = AnioResolucion;
                        consulta.numero_resolucion = NumeroResolucion;
                        consulta.particula = Particula;


                    }
                    catch (Exception ex)
                    {

                    }

                    if (string.IsNullOrEmpty(CodigoErrorDatosCedula))
                    {
                        consulta.codigo_error_cedula = "Error - Candidato no encontrado y/o Invalido";
                    }

                    await _aniRepository.actualizarAuditoriaConsultaANI(consulta);
                }
                else
                {
                    return new AniDTO()
                    {
                        codigo_error_cedula = "No es posible consulta esta cédula"
                    };
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error al consultar datos de cedula {numero_documento} en ANI");
                return new AniDTO()
                {
                    codigo_error_cedula = "No es posible consulta esta cédula"
                };
            }

            return consulta;
        }

        public async Task<AniDTO> validarIdentidad(string numero_documento, string ipAccion, string usuarioAccion)
        {
            string token = getToken();
            AniDTO infoANI = consultarRegistroANIPortalServicios(token, numero_documento);

            if (infoANI is not null) return infoANI;

            infoANI = await consultarANIOlimpia(numero_documento, ipAccion, usuarioAccion);

            insertarRegistroANIPortalServicios(token, infoANI);

            return infoANI;

        }
    }
}
