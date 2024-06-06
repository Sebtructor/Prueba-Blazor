using TaskSe.Data.Connection;
using TaskSe.Data.Repositories.Repositories.Auditoria;
using TaskSe.Data.Repositories.Repositories.Configuracion.Perfilamiento;
using TaskSe.Data.Repositories.Interfaces.Auditoria;
using TaskSe.Data.Repositories.Interfaces.Configuracion.Perfilamiento;
using TaskSe.Model.DTO.Configuracion.Perfilamiento;
using TaskSe.Model.Models.Configuracion.Perfilamiento;
using TaskSe.Model.Response;
using TaskSe.Services.Interfaces.Auditoria;
using TaskSe.Services.Interfaces.Configuracion.Perfilamiento;
using TaskSe.Services.Interfaces.Utilidades;
using TaskSe.Services.Services.Auditoria;
using TaskSe.Services.Utilidades;
using iTextSharp.text.log;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TaskSe.Services.Services.Utilidades;
using TaskSe.Model.Models.Mensajes;
using TaskSe.Model.Models.OTP;
using TaskSe.Data.Repositories.Interfaces.OTP;
using TaskSe.Data.Repositories.Repositories.OTP;
using TaskSe.Model.DTO;
using TaskSe.Services.Interfaces.Mensajes;
using DocumentFormat.OpenXml.Wordprocessing;
using Org.BouncyCastle.Utilities.Net;
using DocumentFormat.OpenXml.Drawing.Charts;
using OtpNet;
using DocumentFormat.OpenXml.Spreadsheet;
using NPOI.SS.Formula.Functions;
using Microsoft.AspNetCore.DataProtection;
using System.Net.Sockets;

namespace TaskSe.Services.Services.Configuracion.Perfilamiento
{
    public class UserService : IUserService
    {
        private readonly SqlConfiguration _sqlConfiguration;
        private readonly IUserRepository _userRepository;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailSender;
        private readonly IEncryptService _encryptService;
        private readonly NavigationManager _navigationManager;
        private readonly IOTPRepository _OTPRepository;
        private readonly IMensajesService _mensajesService;
        private readonly ILogger<UserService> _logger;

        public UserService(SqlConfiguration sqlConfiguration, 
            IConfiguration configuration, 
            IEncryptService encrypt, 
            IEmailService emailService, 
            NavigationManager navigationManager, 
            IAuditoriaService auditoriaService,
            IEncryptService encryptService,
            IMensajesService mensajesService,
            ILogger<UserService> logger)
        {
            _sqlConfiguration = sqlConfiguration;
            _userRepository = new UserRepository(_sqlConfiguration.ConnectionString);
            _OTPRepository = new OTPRepository(_sqlConfiguration.ConnectionString);
            _auditoriaService = auditoriaService;
            _configuration = configuration;
            _emailSender = emailService;
            _navigationManager = navigationManager;
            _encryptService = encryptService;
            _mensajesService = mensajesService;
            _logger = logger;
        }

        public async Task<ResponseDTO> loginUsuario(UserDTO usuario, string ipAddress)
        {
            try
            {
                Usuario user = await _userRepository.getUsuarioByUser(usuario.usuario);

                if (user != null)
                {
                    if (user.estado.Equals("1")) // ACTIVO
                    {
                        if (PasswordHasher.VerifyPassword(usuario.pass, user.clave))
                        {
                            string dias_desde_ultimo_cambio = await _userRepository.consultarDiasDesdeUltimoCambioContraseña(usuario.usuario);

                            if (!string.IsNullOrEmpty(dias_desde_ultimo_cambio))
                            {
                                //Dias que han pasado desde el último cambio de contraseña
                                Int32 dias = Int32.Parse(dias_desde_ultimo_cambio);

                                if (dias >= 72) //Si supera 72 días, se debe hacer el cambio de contraseña
                                {
                                    await _userRepository.registrarAuditoriaLogin(usuario.usuario, $"Exitoso, pero el último cambio de contraseña fue hace {dias} días, por lo que se debe hacer cambio de contraseña", ipAddress);
                                    return new ResponseDTO() { estado = "WARNING", descripcion = "Debido a políticas de seguridad, debes realizar el cambio de tu contraseña para continuar. Puedes realizarlo desde el enlace 'Olvidé mi contraseña'" };
                                }
                            }


                            _logger.LogInformation($"Login exitoso para usuario {usuario.usuario}");
                            string id_auditoria = await _userRepository.registrarAuditoriaLogin(usuario.usuario, "Exitoso", ipAddress);
                            return new ResponseDTO() { estado = "OK", descripcion = id_auditoria };
                        }
                        else
                        {
                            _logger.LogWarning($"Login incorrecto para usuario {usuario.usuario}; credenciales incorrectas");
                            await _userRepository.registrarAuditoriaLogin(usuario.usuario, "No exitoso, credenciales incorrectas", ipAddress);

                            string intentos_fallidos = await _userRepository.cantidadIntentosFallidoPorUsuarioUltimos5Minutos(user.usuario); //Intentos fallidos en los últimos 5 minutos

                            if (!string.IsNullOrEmpty(intentos_fallidos))
                            {
                                Int32 intentos = Int32.Parse(intentos_fallidos);

                                if (intentos >= 3) //Si llega a 3 intentos fallidos en los últimos 5 minutos, se inactivará el usuario
                                {
                                    //Se inactiva el usuario
                                    user.estado = "2";
                                    await _userRepository.insertarUsuario(user,"Administrador");
                                    await _userRepository.registrarAuditoriaLogin(usuario.usuario, "Se inactiva el usuario por realizar 3 o más intentos fallidos de ingreso en menos de 5 minutos", ipAddress);
                                    return new ResponseDTO() { estado = "ERROR", descripcion = "Credenciales incorrectas. El usuario ha sido inactivado" };
                                }
                            }

                            return new ResponseDTO() { estado = "ERROR", descripcion = "Credenciales incorrectas" };
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Login incorrecto para usuario {usuario.usuario}; el usuario está inactivo");
                        return new ResponseDTO()
                        {
                            estado = "ERROR",
                            descripcion = "El usuario se encuentra inactivo",
                        };
                    }
                }
                else
                {
                    _logger.LogWarning($"Login incorrecto para usuario {usuario.usuario}; el usuario no existe");
                    return new ResponseDTO(){
                        estado = "ERROR",
                        descripcion = "El usuario no existe",
                    };
                }
            }
            catch(Exception exe)
            {
                _logger.LogError(exe, $"Error al realizar login para usuario {usuario.usuario}");
                return new ResponseDTO()
                {
                    estado = "ERROR",
                    descripcion = "Ocurrió un error al realizar el login",
                };
            }
        }

        public async Task<IEnumerable<Usuario>> getUsuarios()
        {
            return await _userRepository.getUsers();
        }

        public async Task<bool> insertarUsuario(Usuario usuario, string ipAddress, string usuarioAccion)
        {
            Usuario user = await _userRepository.getUsuarioByUser(usuario.usuario);

            bool usuarioNuevo = false;

            if (user != null)
            {
                usuario.id_usuario = user.id_usuario;
            }
            else
            {
                usuarioNuevo = true;
            }

            if (!string.IsNullOrEmpty(usuario.clave))
            {
                usuario.clave = PasswordHasher.HashPassword(usuario.clave);
            }

            string resultado = await _userRepository.insertarUsuario(usuario, usuarioAccion);

            if (!string.IsNullOrEmpty(resultado) && !resultado.Equals("0"))
            {
                usuario.id_usuario = resultado;
                if (usuarioNuevo)
                {
                    await _auditoriaService.mtdAuditoriaEventos("Registro de información de usuario", "Se ha registrado un nuevo usuario", ipAddress, usuarioAccion, usuario.id_usuario, "");

                    string nombre_cliente = _configuration.GetSection("DatosAplicativo:NombreCliente").Value;
                    string nombre_aplicativo = _configuration.GetSection("DatosAplicativo:NombreAplicativo").Value;
                    string link_logo = _configuration.GetSection("DatosAplicativo:LinkLogoCorreos").Value;

                    #region Correo bienvenida

                    string correoBienvenida = @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional //EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
                                                    <html xmlns=""http://www.w3.org/1999/xhtml"" xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:o=""urn:schemas-microsoft-com:office:office"">
                                                    <head>
                                                    <!--[if gte mso 9]>
                                                    <xml>
                                                      <o:OfficeDocumentSettings>
                                                        <o:AllowPNG/>
                                                        <o:PixelsPerInch>96</o:PixelsPerInch>
                                                      </o:OfficeDocumentSettings>
                                                    </xml>
                                                    <![endif]-->
                                                      <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"">
                                                      <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                                      <meta name=""x-apple-disable-message-reformatting"">
                                                      <!--[if !mso]><!--><meta http-equiv=""X-UA-Compatible"" content=""IE=edge""><!--<![endif]-->
                                                      <title></title>
  
                                                        <style type=""text/css"">
                                                          @media only screen and (min-width: 570px) {
                                                      .u-row {
                                                        width: 550px !important;
                                                      }
                                                      .u-row .u-col {
                                                        vertical-align: top;
                                                      }

                                                      .u-row .u-col-50 {
                                                        width: 275px !important;
                                                      }

                                                      .u-row .u-col-100 {
                                                        width: 550px !important;
                                                      }

                                                    }

                                                    @media (max-width: 570px) {
                                                      .u-row-container {
                                                        max-width: 100% !important;
                                                        padding-left: 0px !important;
                                                        padding-right: 0px !important;
                                                      }
                                                      .u-row .u-col {
                                                        min-width: 320px !important;
                                                        max-width: 100% !important;
                                                        display: block !important;
                                                      }
                                                      .u-row {
                                                        width: calc(100% - 40px) !important;
                                                      }
                                                      .u-col {
                                                        width: 100% !important;
                                                      }
                                                      .u-col > div {
                                                        margin: 0 auto;
                                                      }
                                                    }
                                                    body {
                                                      margin: 0;
                                                      padding: 0;
                                                    }

                                                    table,
                                                    tr,
                                                    td {
                                                      vertical-align: top;
                                                      border-collapse: collapse;
                                                    }

                                                    p {
                                                      margin: 0;
                                                    }

                                                    .ie-container table,
                                                    .mso-container table {
                                                      table-layout: fixed;
                                                    }

                                                    * {
                                                      line-height: inherit;
                                                    }

                                                    a[x-apple-data-detectors='true'] {
                                                      color: inherit !important;
                                                      text-decoration: none !important;
                                                    }

                                                    table, td { color: #000000; } @media (max-width: 480px) { #u_content_image_1 .v-container-padding-padding { padding: 30px 10px 10px !important; } #u_content_image_1 .v-src-width { width: auto !important; } #u_content_image_1 .v-src-max-width { max-width: 66% !important; } #u_content_image_3 .v-src-width { width: auto !important; } #u_content_image_3 .v-src-max-width { max-width: 35% !important; } #u_content_image_2 .v-src-width { width: auto !important; } #u_content_image_2 .v-src-max-width { max-width: 55% !important; } #u_content_menu_1 .v-container-padding-padding { padding: 10px !important; } }
                                                        </style>
  
  

                                                    <!--[if !mso]><!--><link href=""https://fonts.googleapis.com/css?family=Cabin:400,700&display=swap"" rel=""stylesheet"" type=""text/css""><link href=""https://fonts.googleapis.com/css?family=Lato:400,700&display=swap"" rel=""stylesheet"" type=""text/css""><!--<![endif]-->

                                                    </head>

                                                    <body class=""clean-body u_body"" style=""margin: 0;padding: 0;-webkit-text-size-adjust: 100%;background-color: #e0e5eb;color: #000000"">
                                                      <!--[if IE]><div class=""ie-container""><![endif]-->
                                                      <!--[if mso]><div class=""mso-container""><![endif]-->
                                                      <table style=""border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;min-width: 320px;Margin: 0 auto;background-color: #e0e5eb;width:100%"" cellpadding=""0"" cellspacing=""0"">
                                                      <tbody>
                                                      <tr style=""vertical-align: top"">
                                                        <td style=""word-break: break-word;border-collapse: collapse !important;vertical-align: top"">
                                                        <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td align=""center"" style=""background-color: #e0e5eb;""><![endif]-->
    

                                                    <div class=""u-row-container"" style=""padding: 0px;background-color: transparent"">
                                                      <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 550px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;"">
                                                        <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
                                                          <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 0px;background-color: transparent;"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:550px;""><tr style=""background-color: transparent;""><![endif]-->
      
                                                    <!--[if (mso)|(IE)]><td align=""center"" width=""550"" style=""width: 550px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
                                                    <div class=""u-col u-col-100"" style=""max-width: 320px;min-width: 550px;display: table-cell;vertical-align: top;"">
                                                      <div style=""height: 100%;width: 100% !important;"">
                                                      <!--[if (!mso)&(!IE)]><!--><div style=""height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
                                                    <table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
                                                      <tbody>
                                                        <tr>
                                                          <td class=""v-container-padding-padding"" style=""overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:'Lato',sans-serif;"" align=""left"">
        
                                                      <table height=""0px"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;border-top: 0px solid #BBBBBB;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%"">
                                                        <tbody>
                                                          <tr style=""vertical-align: top"">
                                                            <td style=""word-break: break-word;border-collapse: collapse !important;vertical-align: top;font-size: 0px;line-height: 0px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%"">
                                                              <span>&#160;</span>
                                                            </td>
                                                          </tr>
                                                        </tbody>
                                                      </table>

                                                          </td>
                                                        </tr>
                                                      </tbody>
                                                    </table>

                                                      <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
                                                      </div>
                                                    </div>
                                                    <!--[if (mso)|(IE)]></td><![endif]-->
                                                          <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                                                        </div>
                                                      </div>
                                                    </div>



                                                    <div class=""u-row-container"" style=""padding: 0px;background-color: transparent"">
                                                      <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 550px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #ffffff;"">
                                                        <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
                                                          <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 0px;background-color: transparent;"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:550px;""><tr style=""background-color: #ffffff;""><![endif]-->
      
                                                    <!--[if (mso)|(IE)]><td align=""center"" width=""275"" style=""width: 275px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
                                                    <div class=""u-col u-col-50"" style=""max-width: 320px;min-width: 275px;display: table-cell;vertical-align: top;"">
                                                      <div style=""height: 100%;width: 100% !important;"">
                                                      <!--[if (!mso)&(!IE)]><!--><div style=""height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
                                                    <table id=""u_content_image_1"" style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
                                                      <tbody>
                                                        <tr>
                                                          <td class=""v-container-padding-padding"" style=""overflow-wrap:break-word;word-break:break-word;padding:30px 10px;font-family:'Lato',sans-serif;"" align=""left"">
        
                                                    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                                                      <tr>
                                                        <td style=""padding-right: 0px;padding-left: 0px;"" align=""center"">
      
                                                          <img align=""center"" border=""0"" src="""+link_logo+@""" alt=""Image"" title=""Image"" style=""outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 74%;max-width: 188.7px;"" width=""188.7"" class=""v-src-width v-src-max-width""/>
      
                                                        </td>
                                                      </tr>
                                                    </table>

                                                          </td>
                                                        </tr>
                                                      </tbody>
                                                    </table>

                                                      <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
                                                      </div>
                                                    </div>
                                                    <!--[if (mso)|(IE)]></td><![endif]-->
                                                    <!--[if (mso)|(IE)]><td align=""center"" width=""275"" style=""width: 275px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
                                                    <div class=""u-col u-col-50"" style=""max-width: 320px;min-width: 275px;display: table-cell;vertical-align: top;"">
                                                      <div style=""height: 100%;width: 100% !important;"">
                                                      <!--[if (!mso)&(!IE)]><!--><div style=""height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
                                                      <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
                                                      </div>
                                                    </div>
                                                    <!--[if (mso)|(IE)]></td><![endif]-->
                                                          <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                                                        </div>
                                                      </div>
                                                    </div>



                                                    <div class=""u-row-container"" style=""padding: 0px;background-color: transparent"">
                                                      <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 550px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #f8f9fa;"">
                                                        <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
                                                          <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 0px;background-color: transparent;"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:550px;""><tr style=""background-color: #d5827c;""><![endif]-->
      
                                                    <!--[if (mso)|(IE)]><td align=""center"" width=""550"" style=""width: 550px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
                                                    <div class=""u-col u-col-100"" style=""max-width: 320px;min-width: 550px;display: table-cell;vertical-align: top;"">
                                                      <div style=""height: 100%;width: 100% !important;"">
                                                      <!--[if (!mso)&(!IE)]><!--><div style=""height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
                                                    <table id=""u_content_image_3"" style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
                                                      <tbody>
                                                        <tr>
                                                          <td class=""v-container-padding-padding"" style=""overflow-wrap:break-word;word-break:break-word;padding:30px 10px 10px;font-family:'Lato',sans-serif;"" align=""left"">
        
                                                    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                                                      <tr>
                                                        <td style=""padding-right: 0px;padding-left: 0px;"" align=""center"">
      
                                                          <img align=""center"" border=""0"" src=""https://i.ibb.co/K24pPrz/user.png"" alt=""Image"" title=""Image"" style=""outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 26%;max-width: 137.8px;"" width=""137.8"" class=""v-src-width v-src-max-width""/>
      
                                                        </td>
                                                      </tr>
                                                    </table>

                                                          </td>
                                                        </tr>
                                                      </tbody>
                                                    </table>

                                                      <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
                                                      </div>
                                                    </div>
                                                    <!--[if (mso)|(IE)]></td><![endif]-->
                                                          <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                                                        </div>
                                                      </div>
                                                    </div>



                                                    <div class=""u-row-container"" style=""padding: 0px;background-color: transparent"">
                                                      <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 550px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #d5827c;"">
                                                        <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
                                                          <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 0px;background-color: transparent;"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:550px;""><tr style=""background-color: #d5827c;""><![endif]-->
      
                                                    <!--[if (mso)|(IE)]><td align=""center"" width=""550"" style=""width: 550px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
                                                    <div class=""u-col u-col-100"" style=""max-width: 320px;min-width: 550px;display: table-cell;vertical-align: top;"">
                                                      <div style=""height: 100%;width: 100% !important;"">
                                                      <!--[if (!mso)&(!IE)]><!--><div style=""height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
                                                    <table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
                                                      <tbody>
                                                        <tr>
                                                          <td class=""v-container-padding-padding"" style=""overflow-wrap:break-word;word-break:break-word;padding:15px 10px 10px;font-family:'Lato',sans-serif;"" align=""left"">
        
                                                      <h1 style=""margin: 0px; color: #ffffff; line-height: 140%; text-align: center; word-wrap: break-word; font-weight: normal; font-family: book antiqua,palatino; font-size: 35px;"">
                                                        <div>Bienvenido, " + usuario.usuario + @"</div>
                                                      </h1>

                                                          </td>
                                                        </tr>
                                                      </tbody>
                                                    </table>

                                                    <table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
                                                      <tbody>
                                                        <tr>
                                                          <td class=""v-container-padding-padding"" style=""overflow-wrap:break-word;word-break:break-word;padding:0px 10px 10px;font-family:'Lato',sans-serif;"" align=""left"">
        
                                                      <h3 style=""margin: 0px; color: #ffffff; line-height: 140%; text-align: center; word-wrap: break-word; font-weight: normal; font-family: book antiqua,palatino; font-size: 18px;"">
                                                        <div>tu cuenta está lista para usarse</div>
                                                      </h3>

                                                          </td>
                                                        </tr>
                                                      </tbody>
                                                    </table>

                                                      <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
                                                      </div>
                                                    </div>
                                                    <!--[if (mso)|(IE)]></td><![endif]-->
                                                          <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                                                        </div>
                                                      </div>
                                                    </div>



                                                    <div class=""u-row-container"" style=""padding: 0px;background-color: transparent"">
                                                      <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 550px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #d5827c;"">
                                                        <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
                                                          <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 0px;background-color: transparent;"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:550px;""><tr style=""background-color: #d5827c;""><![endif]-->
      
                                                    <!--[if (mso)|(IE)]><td align=""center"" width=""550"" style=""width: 550px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
                                                    <div class=""u-col u-col-100"" style=""max-width: 320px;min-width: 550px;display: table-cell;vertical-align: top;"">
                                                      <div style=""height: 100%;width: 100% !important;"">
                                                      <!--[if (!mso)&(!IE)]><!--><div style=""height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
                                                    <table id=""u_content_image_2"" style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
                                                      <tbody>
                                                        <tr>
                                                          <td class=""v-container-padding-padding"" style=""overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:'Lato',sans-serif;"" align=""left"">
        
                                                    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                                                      <tr>
                                                        <td style=""padding-right: 0px;padding-left: 0px;"" align=""center"">
      
                                                          <img align=""center"" border=""0"" src=""https://i.ibb.co/2vfkNTr/image-1.png"" alt=""Image"" title=""Image"" style=""outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 39%;max-width: 206.7px;"" width=""206.7"" class=""v-src-width v-src-max-width""/>
      
                                                        </td>
                                                      </tr>
                                                    </table>

                                                          </td>
                                                        </tr>
                                                      </tbody>
                                                    </table>

                                                    <table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
                                                      <tbody>
                                                        <tr>
                                                          <td class=""v-container-padding-padding"" style=""overflow-wrap:break-word;word-break:break-word;padding:10px 40px 20px;font-family:'Lato',sans-serif;"" align=""left"">
        
                                                      <div style=""color: #4b4a4a; line-height: 140%; text-align: center; word-wrap: break-word;"">
                                                        <p style=""font-size: 14px; line-height: 140%;""><span style=""font-size: 12px; line-height: 16.8px;"">Tu nuevo usuario para el aplicativo "  +nombre_aplicativo+ @" está listo para ser usado.</span></p>
                                                      </div>

                                                          </td>
                                                        </tr>
                                                      </tbody>
                                                    </table>

                                                      <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
                                                      </div>
                                                    </div>
                                                    <!--[if (mso)|(IE)]></td><![endif]-->
                                                          <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                                                        </div>
                                                      </div>
                                                    </div>



                                                    <div class=""u-row-container"" style=""padding: 0px;background-color: transparent"">
                                                      <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 550px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #ffffff;"">
                                                        <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
                                                          <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 0px;background-color: transparent;"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:550px;""><tr style=""background-color: #ffffff;""><![endif]-->
      
                                                    <!--[if (mso)|(IE)]><td align=""center"" width=""550"" style=""width: 550px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
                                                    <div class=""u-col u-col-100"" style=""max-width: 320px;min-width: 550px;display: table-cell;vertical-align: top;"">
                                                      <div style=""height: 100%;width: 100% !important;"">
                                                      <!--[if (!mso)&(!IE)]><!--><div style=""height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
                                                    <table id=""u_content_menu_1"" style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
                                                      <tbody>
                                                        <tr>
                                                          <td class=""v-container-padding-padding"" style=""overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:'Lato',sans-serif;"" align=""left"">
        
                                                    <div class=""menu"" style=""text-align:center"">
                                                    <!--[if (mso)|(IE)]><table role=""presentation"" border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center""><tr><![endif]-->

                                                    <!--[if (mso)|(IE)]></tr></table><![endif]-->
                                                    </div>

                                                          </td>
                                                        </tr>
                                                      </tbody>
                                                    </table>

                                                    <table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
                                                      <tbody>
                                                        <tr>
                                                          <td class=""v-container-padding-padding"" style=""overflow-wrap:break-word;word-break:break-word;padding:10px 10px 15px;font-family:'Lato',sans-serif;"" align=""left"">
        
                                                      <div style=""color: #7e8c8d; line-height: 140%; text-align: center; word-wrap: break-word;"">
                                                        <p style=""font-size: 14px; line-height: 140%;"">© "+DateTime.Now.Year+@" "+nombre_cliente+@". Todos los derechos reservados.</p>
                                                      </div>

                                                          </td>
                                                        </tr>
                                                      </tbody>
                                                    </table>

                                                      <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
                                                      </div>
                                                    </div>
                                                    <!--[if (mso)|(IE)]></td><![endif]-->
                                                          <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                                                        </div>
                                                      </div>
                                                    </div>



                                                    <div class=""u-row-container"" style=""padding: 0px;background-color: transparent"">
                                                      <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 550px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: transparent;"">
                                                        <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
                                                          <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 0px;background-color: transparent;"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:550px;""><tr style=""background-color: transparent;""><![endif]-->
      
                                                    <!--[if (mso)|(IE)]><td align=""center"" width=""550"" style=""width: 550px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
                                                    <div class=""u-col u-col-100"" style=""max-width: 320px;min-width: 550px;display: table-cell;vertical-align: top;"">
                                                      <div style=""height: 100%;width: 100% !important;"">
                                                      <!--[if (!mso)&(!IE)]><!--><div style=""height: 100%; padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
                                                    <table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
                                                      <tbody>
                                                        <tr>
                                                          <td class=""v-container-padding-padding"" style=""overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:'Lato',sans-serif;"" align=""left"">
        
                                                      <table height=""0px"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;border-top: 0px solid #BBBBBB;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%"">
                                                        <tbody>
                                                          <tr style=""vertical-align: top"">
                                                            <td style=""word-break: break-word;border-collapse: collapse !important;vertical-align: top;font-size: 0px;line-height: 0px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%"">
                                                              <span>&#160;</span>
                                                            </td>
                                                          </tr>
                                                        </tbody>
                                                      </table>

                                                          </td>
                                                        </tr>
                                                      </tbody>
                                                    </table>

                                                      <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
                                                      </div>
                                                    </div>
                                                    <!--[if (mso)|(IE)]></td><![endif]-->
                                                          <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                                                        </div>
                                                      </div>
                                                    </div>


                                                        <!--[if (mso)|(IE)]></td></tr></table><![endif]-->
                                                        </td>
                                                      </tr>
                                                      </tbody>
                                                      </table>
                                                      <!--[if mso]></div><![endif]-->
                                                      <!--[if IE]></div><![endif]-->
                                                    </body>

                                                    </html>
                                                    ";

                    #endregion

                    await _emailSender.enviarCorreo(usuario.email, "", "Confirmación de nuevo usuario", correoBienvenida, "Agregar Usuario", "Correo de creación de nuevo usuario", usuario.usuario, null);
                }
                else
                {
                    await _auditoriaService.mtdAuditoriaEventos("Actualización de información de usuario", "Se ha actualizado la información del usuario", ipAddress, usuarioAccion, usuario.id_usuario, "");
                }
                return true;
            }

            return false;
        }

        public async Task<Usuario> getUsuario(string id_usuario)
        {
            Usuario usuario = await _userRepository.getUsuarioById(id_usuario);

            usuario.clave = "";

            return usuario;
        }

        public async Task<ResponseDTO> gestionarSolicitudPass(string nombreUsuario, string ipAddress)
        {
            try
            {
                Usuario usuario = await _userRepository.getUsuarioByUser(nombreUsuario);

                if (usuario != null)
                {
                    string idSolicitud = await _userRepository.insertarSolicitudPass(nombreUsuario, ipAddress);

                    if (!string.IsNullOrEmpty(idSolicitud) && !idSolicitud.Equals("0"))
                    {
                        string base_url = _navigationManager.BaseUri;

                        Dictionary<string,string> parametros = new Dictionary<string, string>()
                        {
                            {"idSolicitud", idSolicitud.ToString()}
                        };

                        string parametrosEncriptados = _encryptService.encriptarParametros(parametros);

                        string url = base_url + $"CambioPass/{parametrosEncriptados}";

                        string nombre_cliente = _configuration.GetSection("DatosAplicativo:NombreCliente").Value;
                        string nombre_aplicativo = _configuration.GetSection("DatosAplicativo:NombreAplicativo").Value;
                        string link_logo = _configuration.GetSection("DatosAplicativo:LinkLogoCorreos").Value;

                        #region mensaje correo

                        string mensajeCorreo = @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional //EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
                                <html xmlns=""http://www.w3.org/1999/xhtml"" xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:o=""urn:schemas-microsoft-com:office:office"">
                                <head>
                                <!--[if gte mso 9]>
                                <xml>
                                  <o:OfficeDocumentSettings>
                                    <o:AllowPNG/>
                                    <o:PixelsPerInch>96</o:PixelsPerInch>
                                  </o:OfficeDocumentSettings>
                                </xml>
                                <![endif]-->
                                  <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"">
                                  <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                  <meta name=""x-apple-disable-message-reformatting"">
                                  <!--[if !mso]><!--><meta http-equiv=""X-UA-Compatible"" content=""IE=edge""><!--<![endif]-->
                                  <title></title>
  
                                    <style type=""text/css"">
                                      @media only screen and (min-width: 620px) {
                                  .u-row {
                                    width: 600px !important;
                                  }
                                  .u-row .u-col {
                                    vertical-align: top;
                                  }

                                  .u-row .u-col-50 {
                                    width: 300px !important;
                                  }

                                  .u-row .u-col-100 {
                                    width: 600px !important;
                                  }

                                }

                                @media (max-width: 620px) {
                                  .u-row-container {
                                    max-width: 100% !important;
                                    padding-left: 0px !important;
                                    padding-right: 0px !important;
                                  }
                                  .u-row .u-col {
                                    min-width: 320px !important;
                                    max-width: 100% !important;
                                    display: block !important;
                                  }
                                  .u-row {
                                    width: calc(100% - 40px) !important;
                                  }
                                  .u-col {
                                    width: 100% !important;
                                  }
                                  .u-col > div {
                                    margin: 0 auto;
                                  }
                                }
                                body {
                                  margin: 0;
                                  padding: 0;
                                }

                                table,
                                tr,
                                td {
                                  vertical-align: top;
                                  border-collapse: collapse;
                                }

                                p {
                                  margin: 0;
                                }

                                .ie-container table,
                                .mso-container table {
                                  table-layout: fixed;
                                }

                                * {
                                  line-height: inherit;
                                }

                                a[x-apple-data-detectors='true'] {
                                  color: inherit !important;
                                  text-decoration: none !important;
                                }

                                table, td { color: #000000; } #u_body a { color: #161a39; text-decoration: underline; }
                                    </style>
  
  

                                <!--[if !mso]><!--><link href=""https://fonts.googleapis.com/css?family=Lato:400,700&display=swap"" rel=""stylesheet"" type=""text/css""><!--<![endif]-->

                                </head>

                                <body class=""clean-body u_body"" style=""margin: 0;padding: 0;-webkit-text-size-adjust: 100%;background-color: #f9f9f9;color: #000000"">
                                  <!--[if IE]><div class=""ie-container""><![endif]-->
                                  <!--[if mso]><div class=""mso-container""><![endif]-->
                                  <table id=""u_body"" style=""border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;min-width: 320px;Margin: 0 auto;background-color: #f9f9f9;width:100%"" cellpadding=""0"" cellspacing=""0"">
                                  <tbody>
                                  <tr style=""vertical-align: top"">
                                    <td style=""word-break: break-word;border-collapse: collapse !important;vertical-align: top"">
                                    <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td align=""center"" style=""background-color: #f9f9f9;""><![endif]-->
    

                                <div class=""u-row-container"" style=""padding: 0px;background-color: #f9f9f9"">
                                  <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #f9f9f9;"">
                                    <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
                                      <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 0px;background-color: #f9f9f9;"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:600px;""><tr style=""background-color: #f9f9f9;""><![endif]-->
      
                                <!--[if (mso)|(IE)]><td align=""center"" width=""600"" style=""width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
                                <div class=""u-col u-col-100"" style=""max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;"">
                                  <div style=""height: 100%;width: 100% !important;"">
                                  <!--[if (!mso)&(!IE)]><!--><div style=""padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
                                <table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
                                  <tbody>
                                    <tr>
                                      <td style=""overflow-wrap:break-word;word-break:break-word;padding:15px;font-family:'Lato',sans-serif;"" align=""left"">
        
                                  <table height=""0px"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;border-top: 1px solid #f9f9f9;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%"">
                                    <tbody>
                                      <tr style=""vertical-align: top"">
                                        <td style=""word-break: break-word;border-collapse: collapse !important;vertical-align: top;font-size: 0px;line-height: 0px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%"">
                                          <span>&#160;</span>
                                        </td>
                                      </tr>
                                    </tbody>
                                  </table>

                                      </td>
                                    </tr>
                                  </tbody>
                                </table>

                                  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
                                  </div>
                                </div>
                                <!--[if (mso)|(IE)]></td><![endif]-->
                                      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                                    </div>
                                  </div>
                                </div>



                                <div class=""u-row-container"" style=""padding: 0px;background-color: transparent"">
                                  <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #ffffff;"">
                                    <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
                                      <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 0px;background-color: transparent;"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:600px;""><tr style=""background-color: #ffffff;""><![endif]-->
      
                                <!--[if (mso)|(IE)]><td align=""center"" width=""600"" style=""width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
                                <div class=""u-col u-col-100"" style=""max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;"">
                                  <div style=""height: 100%;width: 100% !important;"">
                                  <!--[if (!mso)&(!IE)]><!--><div style=""padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
                                <table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
                                  <tbody>
                                    <tr>
                                      <td style=""overflow-wrap:break-word;word-break:break-word;padding:25px 10px;font-family:'Lato',sans-serif;"" align=""left"">
        
                                <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                                  <tr>
                                    <td style=""padding-right: 0px;padding-left: 0px;"" align=""center"">
      
                                      <img align=""center"" border=""0"" src="""+ link_logo + @""" alt=""Image"" title=""Image"" style=""outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 29%;max-width: 168.2px;"" width=""168.2""/>
      
                                    </td>
                                  </tr>
                                </table>

                                      </td>
                                    </tr>
                                  </tbody>
                                </table>

                                  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
                                  </div>
                                </div>
                                <!--[if (mso)|(IE)]></td><![endif]-->
                                      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                                    </div>
                                  </div>
                                </div>



                                <div class=""u-row-container"" style=""padding: 0px;background-color: transparent"">
                                  <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #161a39;"">
                                    <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
                                      <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 0px;background-color: transparent;"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:600px;""><tr style=""background-color: #161a39;""><![endif]-->
      
                                <!--[if (mso)|(IE)]><td align=""center"" width=""600"" style=""width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
                                <div class=""u-col u-col-100"" style=""max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;"">
                                  <div style=""height: 100%;width: 100% !important;"">
                                  <!--[if (!mso)&(!IE)]><!--><div style=""padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
                                <table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
                                  <tbody>
                                    <tr>
                                      <td style=""overflow-wrap:break-word;word-break:break-word;padding:35px 10px 10px;font-family:'Lato',sans-serif;"" align=""left"">
        
                                <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                                  <tr>
                                    <td style=""padding-right: 0px;padding-left: 0px;"" align=""center"">
      
                                      <img align=""center"" border=""0"" src=""https://i.ibb.co/kgRHHgF/image-6.png"" alt=""Image"" title=""Image"" style=""outline: none;text-decoration: none;-ms-interpolation-mode: bicubic;clear: both;display: inline-block !important;border: none;height: auto;float: none;width: 10%;max-width: 58px;"" width=""58""/>
      
                                    </td>
                                  </tr>
                                </table>

                                      </td>
                                    </tr>
                                  </tbody>
                                </table>

                                <table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
                                  <tbody>
                                    <tr>
                                      <td style=""overflow-wrap:break-word;word-break:break-word;padding:0px 10px 30px;font-family:'Lato',sans-serif;"" align=""left"">
        
                                  <div style=""line-height: 140%; text-align: left; word-wrap: break-word;"">
                                    <p style=""font-size: 14px; line-height: 140%; text-align: center;""><span style=""font-size: 28px; line-height: 39.2px; color: #ffffff; font-family: Lato, sans-serif;"">Reestablece tu contraseña </span></p>
                                  </div>

                                      </td>
                                    </tr>
                                  </tbody>
                                </table>

                                  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
                                  </div>
                                </div>
                                <!--[if (mso)|(IE)]></td><![endif]-->
                                      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                                    </div>
                                  </div>
                                </div>



                                <div class=""u-row-container"" style=""padding: 0px;background-color: transparent"">
                                  <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #ffffff;"">
                                    <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
                                      <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 0px;background-color: transparent;"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:600px;""><tr style=""background-color: #ffffff;""><![endif]-->
      
                                <!--[if (mso)|(IE)]><td align=""center"" width=""600"" style=""width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
                                <div class=""u-col u-col-100"" style=""max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;"">
                                  <div style=""height: 100%;width: 100% !important;"">
                                  <!--[if (!mso)&(!IE)]><!--><div style=""padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
                                <table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
                                  <tbody>
                                    <tr>
                                      <td style=""overflow-wrap:break-word;word-break:break-word;padding:40px 40px 30px;font-family:'Lato',sans-serif;"" align=""left"">
        
                                  <div style=""line-height: 140%; text-align: left; word-wrap: break-word;"">
                                    <p style=""font-size: 14px; line-height: 140%;""><span style=""font-size: 18px; line-height: 25.2px; color: #666666;"">Hola, " + usuario.nombres + @"</span></p>
                                <p style=""font-size: 14px; line-height: 140%;"">&nbsp;</p>
                                <p style=""font-size: 14px; line-height: 140%;text-align: justify;""><span style=""font-size: 18px; line-height: 25.2px; color: #666666;"">Te hemos envíado este correo en respuesta a tu solicitud de reestablecimiento de contraseña para el aplicativo "+nombre_aplicativo+@".</span></p>
                                <p style=""font-size: 14px; line-height: 140%;"">&nbsp;</p>
                                <p style=""font-size: 14px; line-height: 140%;text-align: justify;""><span style=""font-size: 18px; line-height: 25.2px; color: #666666;"">Para reestablecer tu contraseña, por favor da click en el siguiente enlance: </span></p>
                                  </div>

                                      </td>
                                    </tr>
                                  </tbody>
                                </table>

                                <table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
                                  <tbody>
                                    <tr>
                                      <td style=""overflow-wrap:break-word;word-break:break-word;padding:0px 40px;font-family:'Lato',sans-serif;"" align=""left"">
        
                                <div align=""left"">
                                  <!--[if mso]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""border-spacing: 0; border-collapse: collapse; mso-table-lspace:0pt; mso-table-rspace:0pt;font-family:'Lato',sans-serif;""><tr><td style=""font-family:'Lato',sans-serif;"" align=""left""><v:roundrect xmlns:v=""urn:schemas-microsoft-com:vml"" xmlns:w=""urn:schemas-microsoft-com:office:word"" href="""" style=""height:51px; v-text-anchor:middle; width:205px;"" arcsize=""2%"" stroke=""f"" fillcolor=""#18163a""><w:anchorlock/><center style=""color:#FFFFFF;font-family:'Lato',sans-serif;""><![endif]-->
                                    <a href=""" + url + @""" target=""_blank"" style=""box-sizing: border-box;display: inline-block;font-family:'Lato',sans-serif;text-decoration: none;-webkit-text-size-adjust: none;text-align: center;color: #FFFFFF; background-color: #18163a; border-radius: 1px;-webkit-border-radius: 1px; -moz-border-radius: 1px; width:auto; max-width:100%; overflow-wrap: break-word; word-break: break-word; word-wrap:break-word; mso-border-alt: none;"">
                                      <span style=""display:block;padding:15px 40px;line-height:120%;""><span style=""font-size: 18px; line-height: 21.6px;"">Reestablecer Contraseña</span></span>
                                    </a>
                                  <!--[if mso]></center></v:roundrect></td></tr></table><![endif]-->
                                </div>

                                      </td>
                                    </tr>
                                  </tbody>
                                </table>

                                <table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
                                  <tbody>
                                    <tr>
                                      <td style=""overflow-wrap:break-word;word-break:break-word;padding:40px 40px 30px;font-family:'Lato',sans-serif;"" align=""left"">
        
                                  <div style=""line-height: 140%; text-align: left; word-wrap: break-word;"">
                                    <p style=""font-size: 14px; line-height: 140%;""><span style=""color: #888888; font-size: 14px; line-height: 19.6px;""><em><span style=""font-size: 16px; line-height: 22.4px;"">Por favor ignora este email si no hiciste ninguna solicitud de reestablecimiento de contraseña.</span></em></span><br /><span style=""color: #888888; font-size: 14px; line-height: 19.6px;""><em><span style=""font-size: 16px; line-height: 22.4px;"">&nbsp;</span></em></span></p>
                                  </div>

                                      </td>
                                    </tr>
                                  </tbody>
                                </table>

                                  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
                                  </div>
                                </div>
                                <!--[if (mso)|(IE)]></td><![endif]-->
                                      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                                    </div>
                                  </div>
                                </div>



                                <div class=""u-row-container"" style=""padding: 0px;background-color: transparent"">
                                  <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #18163a;"">
                                    <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
                                      <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 0px;background-color: transparent;"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:600px;""><tr style=""background-color: #18163a;""><![endif]-->
      
                                <!--[if (mso)|(IE)]><td align=""center"" width=""300"" style=""width: 300px;padding: 20px 20px 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
                                <div class=""u-col u-col-50"" style=""max-width: 320px;min-width: 300px;display: table-cell;vertical-align: top;"">
                                  <div style=""height: 100%;width: 100% !important;"">
                                  <!--[if (!mso)&(!IE)]><!--><div style=""padding: 20px 20px 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
                                <table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
                                  <tbody>
                                    <tr>
                                      <td style=""overflow-wrap:break-word;word-break:break-word;padding:10px;font-family:'Lato',sans-serif;"" align=""left"">
        
      

                                      </td>
                                    </tr>
                                  </tbody>
                                </table>

                                  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
                                  </div>
                                </div>
                                <!--[if (mso)|(IE)]></td><![endif]-->
                                <!--[if (mso)|(IE)]><td align=""center"" width=""300"" style=""width: 300px;padding: 0px 0px 0px 20px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
                                <div class=""u-col u-col-50"" style=""max-width: 320px;min-width: 300px;display: table-cell;vertical-align: top;"">
                                  <div style=""height: 100%;width: 100% !important;"">
                                  <!--[if (!mso)&(!IE)]><!--><div style=""padding: 0px 0px 0px 20px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
                                <table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
                                  <tbody>
                                    <tr>
                                      <td style=""overflow-wrap:break-word;word-break:break-word;padding:25px 10px 10px;font-family:'Lato',sans-serif;"" align=""left"">
        
                                <div align=""left"">
                                  <div style=""display: table; max-width:187px;"">
                                  <!--[if (mso)|(IE)]><table width=""187"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""border-collapse:collapse;"" align=""left""><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""border-collapse:collapse; mso-table-lspace: 0pt;mso-table-rspace: 0pt; width:187px;""><tr><![endif]-->
  
    
    
    
                                    <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                                  </div>
                                </div>

                                      </td>
                                    </tr>
                                  </tbody>
                                </table>

                                <table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
                                  <tbody>
                                    <tr>
                                      <td style=""overflow-wrap:break-word;word-break:break-word;padding:5px 10px 10px;font-family:'Lato',sans-serif;"" align=""left"">
        
                                  <div style=""line-height: 140%; text-align: left; word-wrap: break-word;"">
                                    <p style=""line-height: 140%; font-size: 14px;""><span style=""font-size: 14px; line-height: 19.6px;""><span style=""color: #ecf0f1; font-size: 14px; line-height: 19.6px;""><span style=""line-height: 19.6px; font-size: 14px;"">"+nombre_cliente+@" &copy;&nbsp; Todos los derechos reservados </span></span></span></p>
                                  </div>

                                      </td>
                                    </tr>
                                  </tbody>
                                </table>

                                  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
                                  </div>
                                </div>
                                <!--[if (mso)|(IE)]></td><![endif]-->
                                      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                                    </div>
                                  </div>
                                </div>



                                <div class=""u-row-container"" style=""padding: 0px;background-color: #f9f9f9"">
                                  <div class=""u-row"" style=""Margin: 0 auto;min-width: 320px;max-width: 600px;overflow-wrap: break-word;word-wrap: break-word;word-break: break-word;background-color: #1c103b;"">
                                    <div style=""border-collapse: collapse;display: table;width: 100%;height: 100%;background-color: transparent;"">
                                      <!--[if (mso)|(IE)]><table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0""><tr><td style=""padding: 0px;background-color: #f9f9f9;"" align=""center""><table cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:600px;""><tr style=""background-color: #1c103b;""><![endif]-->
      
                                <!--[if (mso)|(IE)]><td align=""center"" width=""600"" style=""width: 600px;padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;"" valign=""top""><![endif]-->
                                <div class=""u-col u-col-100"" style=""max-width: 320px;min-width: 600px;display: table-cell;vertical-align: top;"">
                                  <div style=""height: 100%;width: 100% !important;"">
                                  <!--[if (!mso)&(!IE)]><!--><div style=""padding: 0px;border-top: 0px solid transparent;border-left: 0px solid transparent;border-right: 0px solid transparent;border-bottom: 0px solid transparent;""><!--<![endif]-->
  
                                <table style=""font-family:'Lato',sans-serif;"" role=""presentation"" cellpadding=""0"" cellspacing=""0"" width=""100%"" border=""0"">
                                  <tbody>
                                    <tr>
                                      <td style=""overflow-wrap:break-word;word-break:break-word;padding:15px;font-family:'Lato',sans-serif;"" align=""left"">
        
                                  <table height=""0px"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""border-collapse: collapse;table-layout: fixed;border-spacing: 0;mso-table-lspace: 0pt;mso-table-rspace: 0pt;vertical-align: top;border-top: 1px solid #1c103b;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%"">
                                    <tbody>
                                      <tr style=""vertical-align: top"">
                                        <td style=""word-break: break-word;border-collapse: collapse !important;vertical-align: top;font-size: 0px;line-height: 0px;mso-line-height-rule: exactly;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%"">
                                          <span>&#160;</span>
                                        </td>
                                      </tr>
                                    </tbody>
                                  </table>

                                      </td>
                                    </tr>
                                  </tbody>
                                </table>

                                  <!--[if (!mso)&(!IE)]><!--></div><!--<![endif]-->
                                  </div>
                                </div>
                                <!--[if (mso)|(IE)]></td><![endif]-->
                                      <!--[if (mso)|(IE)]></tr></table></td></tr></table><![endif]-->
                                    </div>
                                  </div>
                                </div>


                                    <!--[if (mso)|(IE)]></td></tr></table><![endif]-->
                                    </td>
                                  </tr>
                                  </tbody>
                                  </table>
                                  <!--[if mso]></div><![endif]-->
                                  <!--[if IE]></div><![endif]-->
                                </body>

                                </html>
                                ";

                        #endregion

                        if (await _emailSender.enviarCorreo(usuario.email, "", "Reestablecimiento de Contraseña", mensajeCorreo, "Reestablecimiento contraseña usuario", "Correo de reestablecimiento de contraseña", usuario.usuario, null))
                        {
                            _logger.LogInformation($"Correo de reestablecimiento de contraseña enviado correctamente a usuario {nombreUsuario}");
                            return new ResponseDTO() { estado = "OK", descripcion = "Proceso realizado correctamente" };
                        }
                        else
                        {
                            _logger.LogError($"Error al procesar solicitud de cambio de contraseña para usuario {nombreUsuario}: No es posible enviar el correo");
                            return new ResponseDTO() { estado = "ERROR", descripcion = "No es posible enviar el correo de reestablecimiento de contraseña" };
                        }
                    }
                    else
                    {
                        _logger.LogError($"Error al procesar solicitud de cambio de contraseña para usuario {nombreUsuario}: Solicitud no registrada en base de datos correctamente");
                        return new ResponseDTO() { estado = "ERROR", descripcion = "Error al procesar la solicitud" };
                    }
                }
                else
                {
                    _logger.LogWarning($"Error al procesar solicitud de cambio de contraseña para usuario {nombreUsuario}: El usuario no existe");
                    return new ResponseDTO() { estado = "ERROR", descripcion = "El usuario no existe" }; 
                }
            }
            catch (Exception exe)
            {
                _logger.LogError(exe,$"Error al procesar solicitud de cambio de contraseña para usuario {nombreUsuario}");
                return new ResponseDTO() { estado = "ERROR", descripcion = "Error al procesar solicitud" };
            }
        }

        public async Task<SolicitudContraseña> getSolicitudContraseña(string id_solicitud)
        {
            return await _userRepository.getSolicitudContraseña(id_solicitud);
        }

        public async Task<ResponseDTO> completarSolicitudContraseña(string id_solicitud, string nuevaPass)
        {
            SolicitudContraseña solicitud = await _userRepository.getSolicitudContraseña(id_solicitud);

            if (solicitud is null)
            {
                return new ResponseDTO() { estado = "ERROR", descripcion = "La solicitud no existe" };
            }

            Usuario user = await _userRepository.getUsuarioByUser(solicitud.usuario);

            string encriptada = PasswordHasher.HashPassword(nuevaPass);

            try
            {
                if (PasswordHasher.VerifyPassword(nuevaPass, user.clave))
                {
                    return new ResponseDTO() { estado = "ERROR", descripcion = "Debes indicar una contraseña diferente a la que tienes actualmente" };
                }
            }
            catch (Exception exe)
            {

            }

            if (!await _userRepository.completarSolicitudContraseña(id_solicitud, encriptada))
            {
                return new ResponseDTO() { estado = "ERROR", descripcion = "No se pudo completar la solicitud" };
            }

            return new ResponseDTO() { estado = "OK", descripcion = "" };
        }

        public async Task<Usuario> getUsuarioByUser(string usuario)
        {
            return await _userRepository.getUsuarioByUser(usuario);
        }

        public async Task<bool> registrarAuditoriaCierreSesion(string id_auditoria, string usuario, string ip, string motivo)
        {
            return await _userRepository.registrarAuditoriaCierreSesion(id_auditoria, usuario, ip, motivo);
        }

        public async Task<ResponseDTO> enviarCodigoOTPLogin(string nombre_usuario, string ipAccion)
        {
            try
            {
                Usuario user = await _userRepository.getUsuarioByUser(nombre_usuario);

                if(user is null)
                {
                    return new ResponseDTO() { estado = "ERROR", descripcion = "El usuario no existe" };
                }

                if(string.IsNullOrEmpty(user.email))
                {
                    return new ResponseDTO() { estado = "ERROR", descripcion = "El usuario no posee correo electrónico" };
                }

                Random R = new Random();
                int otp_code = R.Next(100000, 999999);

                string nombre_cliente = _configuration.GetSection("DatosAplicativo:NombreCliente").Value;
                string nombre_aplicativo = _configuration.GetSection("DatosAplicativo:NombreAplicativo").Value;

                #region Cuerpo correo

                string cuerpoCorreo = @$"<div style=""font-family: Helvetica,Arial,sans-serif;min-width:1000px;overflow:auto;line-height:2"">
                  <div style=""margin:50px auto;width:70%;padding:20px 0"">
                    <div style=""border-bottom:1px solid #eee"">
                      <a href="""" style=""font-size:1.4em;color: #00466a;text-decoration:none;font-weight:600"">Verificación OTP</a>
                    </div>
                    <p style=""font-size:1.1em"">Hola {user.nombres},</p>
                    <p>Usa el siguiente código OTP para completar tu inicio de sesión en el aplicativo "+ nombre_aplicativo + @$". El código es válido por 5 minutos</p>
                    <h2 style=""background: #00466a;margin: 0 auto;width: max-content;padding: 0 10px;color: #fff;border-radius: 4px;"">{otp_code}</h2>
                    <p style=""font-size:0.9em;"">Saludos,<br />"+nombre_cliente+@"</p>
                  </div>
                </div>";

                #endregion

                bool resultado = await _emailSender.enviarCorreo(user.email, "", "Código OTP - Verificación Login", cuerpoCorreo, "Verificación OTP Login", "Verificación OTP Login", user.usuario, null);


                if (!string.IsNullOrEmpty(user.celular))
                {

                    string contenido_mensaje_wpp = otp_code.ToString();

                    Mensaje mensajeWPP = new Mensaje() { tipo_mensaje = 2, celular_destino = user.celular, concepto = "Validación OTP Login", contenido_mensaje = contenido_mensaje_wpp, numero_identificacion = user.usuario };


                    await _mensajesService.enviarMensaje(mensajeWPP);
                }

                await _OTPRepository.insertarOTP(new OTPModel()
                {
                    otp_code = otp_code.ToString(),
                    descripcion = "Validación OTP Login",
                    estado = "Envíado",
                    metodos_envio = "email",
                    numero_documento_proceso = user.usuario,
                    tipo_proceso = "Login"
                });

                if (resultado)
                {
                    return new ResponseDTO() { estado = "OK", descripcion = "" };
                }
            }
            catch (Exception exe)
            {
                _logger.LogError(exe, $"Error al enviar codigo OTP login usuario {nombre_usuario}");
            }

            return new ResponseDTO() { estado = "ERROR", descripcion = "No ha sido posible completar esta operación" };
        }

        public async Task<ResponseDTO> validarCodigoOTP(OTP_DTO otp_code, string nombre_usuario, string ipAccion)
        {
            OTPModel lastOTP = await _OTPRepository.getUltimoOTPProceso(nombre_usuario, "Login");

            string OTPCliente = otp_code.C1 +
                                otp_code.C2 +
                                otp_code.C3 +
                                otp_code.C4 +
                                otp_code.C5 +
                                otp_code.C6
                                ;

            if (!lastOTP.otp_code.Equals(OTPCliente))
            {
                return new ResponseDTO() { estado = "ERROR", descripcion = "Código no válido" };
            }

            //OTP mayor a 15 minutos
            if ((DateTime.Now - lastOTP.fecha_adicion).TotalSeconds > (5 * 60))
            {
                return new ResponseDTO() { estado = "ERROR", descripcion = "El código ha expirado, por favor solicite uno nuevo" };
            }

            await _OTPRepository.completarEstadoOTP(lastOTP.id_otp_code, "Verificado");

            return new ResponseDTO() { estado = "OK", descripcion = "" };
        }

        public async Task procesarIngreso(string usuario, string ip_address, string descripcion)
        {
            await _userRepository.registrarAuditoriaLogin(usuario, descripcion, ip_address);
        }

        public async Task<Secret2FA> generarTOTP2FA(string usuario, string ip_address)
        {
            string nombre_aplicativo = _configuration.GetSection("DatosAplicativo:NombreAplicativo").Value;

            var secret = KeyGeneration.GenerateRandomKey(20); // Generar un secreto
            var totp = new Totp(secret);

            // Generar URL para el código QR
            var url = $"otpauth://totp/{nombre_aplicativo}:{usuario}?secret={Base32Encoding.ToString(secret)}";

            string ruta_QR = $"{Directory.GetCurrentDirectory()}\\QR_2FA";
            if (!Directory.Exists(ruta_QR)) Directory.CreateDirectory(ruta_QR);

            string ruta = $"{ruta_QR}\\QR_{usuario}_{DateTime.Now.ToString("ddMMyyyyhhmmss")}.png";

            if (!QRGenerator.generarQRNoLogo(url, ruta, 500))
            {
                throw new Exception("No fue posible generar el QR");
            }

            Secret2FA secret2FA = new Secret2FA()
            {
                RUTA_QR = ruta,
                SECRETO = Convert.ToBase64String(secret),
                USUARIO = usuario,
            };

            return secret2FA;
        }

        public async Task<ResponseDTO> habilitarTOTP2FA(Secret2FA secret, string ip_address, string codigo_inicial)
        {
            Usuario user = await _userRepository.getUsuarioByUser(secret.USUARIO);

            if (user is null)
            {
                return new ResponseDTO() { estado = "ERROR", descripcion = "El usuario no existe" };
            }

            var totp = new Totp(Convert.FromBase64String(secret.SECRETO));

            if (totp.VerifyTotp(codigo_inicial, out _, new VerificationWindow(2, 2)))
            {
                await _userRepository.actualizarEstado2FA(secret.USUARIO, true, ip_address);
                string secreto_encriptado = _encryptService.encrypCadenaRSA(secret.SECRETO);
                secret.SECRETO = secreto_encriptado;
                await _userRepository.insertarSecreto(secret,ip_address);
                await _userRepository.insertarAuditoriaTOTP(codigo_inicial, secret.USUARIO, "OK, verificación inicial", ip_address);

                await _auditoriaService.mtdAuditoriaEventos("Habilitación de 2FA mediante TOTP", "El usuario ha habilitado 2FA", ip_address, secret.USUARIO, "", "");

                return new ResponseDTO() { estado = "OK", descripcion = "" };
            }
            else
            {
                await _userRepository.insertarAuditoriaTOTP(codigo_inicial, secret.USUARIO, "CÓDIGO INCORRECTO, verificación inicial", ip_address);
                return new ResponseDTO() { estado = "ERROR", descripcion = "El código indicado es incorrecto"};
            }
        }

        public async Task<ResponseDTO> deshabilitarTOTP2FA(string usuario, string ip_address)
        {
            if(await _userRepository.actualizarEstado2FA(usuario, false, ip_address))
            {
                await _auditoriaService.mtdAuditoriaEventos("Deshabilitación de 2FA mediante TOTP", "El usuario ha deshabilitado 2FA", ip_address, usuario, "", "");
                return new ResponseDTO() { estado = "OK", descripcion = "" };
            }
            else
            {
                return new ResponseDTO() { estado = "ERROR", descripcion = "El código indicado es incorrecto" };
            }
        }

        public async Task<ResponseDTO> validarTOTP2FA(string usuario, string ip_address, string codigo)
        {
            Usuario user = await _userRepository.getUsuarioByUser(usuario);

            if(user is null)
            {
                return new ResponseDTO() { estado = "ERROR", descripcion = "El usuario no existe"};
            }

            if (!user.TWOFA_ENABLED)
            {
                return new ResponseDTO() { estado = "ERROR", descripcion = "Autenticación TOTP no habilitada para este usuario" };
            }

            var secretoEncriptado = await _userRepository.getSecret(usuario);

            var secretoDesencriptado = _encryptService.desEncrypCadenaRSA(secretoEncriptado.SECRETO);

            byte[] secreto = Convert.FromBase64String(secretoDesencriptado);

            var totp = new Totp(secreto);

            if (totp.VerifyTotp(codigo, out _, new VerificationWindow(2, 2)))
            {
                await _userRepository.insertarAuditoriaTOTP(codigo, usuario, "OK, ingreso", ip_address);

                return new ResponseDTO() { estado = "OK", descripcion = "" };
            }
            else
            {
                return new ResponseDTO() { estado = "ERROR", descripcion = "El código indicado es incorrecto" };
            }
        }
    }
}
