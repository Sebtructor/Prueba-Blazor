using TaskSe.Data.Connection;
using TaskSe.Data.Repositories.Repositories.Auditoria;
using TaskSe.Data.Repositories.Interfaces.Auditoria;
using TaskSe.Data.Utilidades;
using TaskSe.Services.Interfaces.Auditoria;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskSe.Model.Models.Configuracion.Perfilamiento;

namespace TaskSe.Services.Services.Auditoria
{
    public class AuditoriaService : IAuditoriaService
    {
        private readonly IAuditoriaRepository _auditoriaRepository;
        private readonly DataTableQuery _dataTableQuery;
        public AuditoriaService(SqlConfiguration sqlConfiguration)
        {
            _auditoriaRepository = new AuditoriaRepository(sqlConfiguration.ConnectionString);
            _dataTableQuery = new DataTableQuery(sqlConfiguration.ConnectionString);
        }

        public async Task<bool> registrarAuditoriaNavegacion(AuditoriaNavegacion auditoriaNavegacion)
        {
            return await _auditoriaRepository.registrarAuditoriaNavegacion(auditoriaNavegacion);
        }

        public async Task<bool> mtdAuditoriaEnvioCorreo(string EMAIL_DESTINATARIO, string EMAIL_EMISOR, string MENSAJE, string ENVIADO, string ERROR, string PANTALLA, string DESCRIPCION, string NUMERO_IDENTIFICACION, string USUARIO)
        {
            return await _auditoriaRepository.mtdAuditoriaEnvioCorreo(EMAIL_DESTINATARIO, EMAIL_EMISOR,MENSAJE,ENVIADO,ERROR,PANTALLA,DESCRIPCION,NUMERO_IDENTIFICACION,USUARIO);
        }

        public async Task<bool> mtdAuditoriaEventos(string ACCION, string DESCRIPCION, string IP_ACCION, string USUARIO_ACCION, string ID_USUARIO, string ID_ROL)
        {
            return await _auditoriaRepository.mtdAuditoriaEventos(ACCION,DESCRIPCION,IP_ACCION,USUARIO_ACCION,ID_USUARIO,ID_ROL);    
        }

        public async Task<bool> mtdAuditoriaTransaccionalPJ(string Accion, string Pantalla, string Descripcion, string cedula, string idUsuario, string ipAddress)
        {
            return await _auditoriaRepository.mtdAuditoriaTransaccionalPJ(Accion, Pantalla, Descripcion, cedula, idUsuario, ipAddress);
        }
        public async Task<bool> mtdAuditoriaTransaccionalPN(string Accion, string Pantalla, string Descripcion, string cedula, string idUsuario, string ipAddress)
        {
            return await _auditoriaRepository.mtdAuditoriaTransaccionalPN(Accion, Pantalla, Descripcion, cedula, idUsuario, ipAddress);
        }

        public async Task<string> reporteANI(string fecha_inicio, string fecha_fin)
        {
            string ruta_base = Directory.GetCurrentDirectory() + "\\Reportes";
            if (!Directory.Exists(ruta_base)) Directory.CreateDirectory(ruta_base);

            string nombre_archivo = $"ReporteANI {DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss tt")}.xlsx";
            string ruta_reporte = ruta_base + $"\\{nombre_archivo}";

            XLWorkbook wb = new XLWorkbook();

            string query = @"SELECT [CEDULA]
                              ,[RESPONSE]
                              ,[IP]
                              ,[HOST]
                              ,[ID_USUARIO]
                              ,[FECHA]
                              ,[CODIGO_ERROR_CEDULA]
                              ,[ESTADO_CEDULA]
                              ,[DEPARTAMENTO_EXP_REGISTRADURIA]
                              ,[FECHA_EXP_REGISTRADURIA]
                              ,[MUNICIPIO_EXP_REGISTRADURIA]
                              ,[PRIMER_NOMBRE_REGISTRADURIA]
                              ,[SEGUNDO_NOMBRE_REGISTRADURIA]
                              ,[PRIMER_APELLIDO_REGISTRADURIA]
                              ,[SEGUNDO_APELLIDO_REGISTRADURIA]
                          FROM [SEG].[SEG_AUDITORIA_CONSULTA_ANI]";

            if (!string.IsNullOrEmpty(fecha_inicio) && !string.IsNullOrEmpty(fecha_fin))
            {
                query += " WHERE CONVERT(DATE,FECHA) BETWEEN CONVERT(DATE,@FECHA_INICIO) AND CONVERT(DATE,@FECHA_FIN)";
            }

            Dictionary<string,string> parametros = new Dictionary<string, string>()
            {
                { "@FECHA_INICIO", fecha_inicio },
                { "@FECHA_FIN", fecha_fin },
            };

            wb.Worksheets.Add(_dataTableQuery.getDataTableQuery(query,parametros), "Consumo ANI Olimpia");

            using (FileStream fs = new FileStream(ruta_reporte, FileMode.Create, FileAccess.Write))
            {
                wb.SaveAs(fs);
            }

            wb.Dispose();

            return ruta_reporte;
        }

        public async Task<string> reporteAuditoriaPJ(string fecha_inicio, string fecha_fin)
        {
            string ruta_base = Directory.GetCurrentDirectory() + "\\Reportes";
            if (!Directory.Exists(ruta_base)) Directory.CreateDirectory(ruta_base);

            string nombre_archivo = $"ReporteAuditoriaPJ {DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss tt")}.xlsx";
            string ruta_reporte = ruta_base + $"\\{nombre_archivo}";

            XLWorkbook wb = new XLWorkbook();

            string query = @"SELECT 
                    [NumeroIdentificacion_LogTransaccional]	as NumeroDocumento
                    ,[Accion_LogTransaccional] as Accion
                    ,[FechaCreacion_LogTransaccional] as Fecha
                    ,[Descripcion_LogTransaccional] as Descripcion
                    ,[Pantalla_LogTransaccional] as Pantalla
                    ,[IP_LogTransaccional] as IpAccion
                    ,[IdUsuario_LogTransaccional] as UsuarioAccion
                    FROM [AUD].[LogTransaccionalEventos_PJ]";

            if (!string.IsNullOrEmpty(fecha_inicio) && !string.IsNullOrEmpty(fecha_fin))
            {
                query += " WHERE CONVERT(DATE,FechaCreacion_LogTransaccional) BETWEEN CONVERT(DATE,@FECHA_INICIO) AND CONVERT(DATE,@FECHA_FIN)";
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>()
            {
                { "@FECHA_INICIO", fecha_inicio },
                { "@FECHA_FIN", fecha_fin },
            };

            wb.Worksheets.Add(_dataTableQuery.getDataTableQuery(query, parametros), "Auditoría PJ");

            using (FileStream fs = new FileStream(ruta_reporte, FileMode.Create, FileAccess.Write))
            {
                wb.SaveAs(fs);
            }

            wb.Dispose();

            return ruta_reporte;
        }

        public async Task<string> reporteAuditoriaPN(string fecha_inicio, string fecha_fin)
        {
            string ruta_base = Directory.GetCurrentDirectory() + "\\Reportes";
            if (!Directory.Exists(ruta_base)) Directory.CreateDirectory(ruta_base);

            string nombre_archivo = $"ReporteAuditoriaPN {DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss tt")}.xlsx";
            string ruta_reporte = ruta_base + $"\\{nombre_archivo}";

            XLWorkbook wb = new XLWorkbook();

            string query = @"SELECT 
                    [NumeroIdentificacion_LogTransaccional]	as NumeroDocumento
                    ,[Accion_LogTransaccional] as Accion
                    ,[FechaCreacion_LogTransaccional] as Fecha
                    ,[Descripcion_LogTransaccional] as Descripcion
                    ,[Pantalla_LogTransaccional] as Pantalla
                    ,[IP_LogTransaccional] as IpAccion
                    ,[IdUsuario_LogTransaccional] as UsuarioAccion
                    FROM [AUD].[LogTransaccionalEventos]";

            if (!string.IsNullOrEmpty(fecha_inicio) && !string.IsNullOrEmpty(fecha_fin))
            {
                query += " WHERE CONVERT(DATE,FechaCreacion_LogTransaccional) BETWEEN CONVERT(DATE,@FECHA_INICIO) AND CONVERT(DATE,@FECHA_FIN)";
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>()
            {
                { "@FECHA_INICIO", fecha_inicio },
                { "@FECHA_FIN", fecha_fin },
            };

            wb.Worksheets.Add(_dataTableQuery.getDataTableQuery(query, parametros), "Auditoría PN");

            using (FileStream fs = new FileStream(ruta_reporte, FileMode.Create, FileAccess.Write))
            {
                wb.SaveAs(fs);
            }

            wb.Dispose();

            return ruta_reporte;
        }

        public async Task<string> reporteEmail(string fecha_inicio, string fecha_fin)
        {
            string ruta_base = Directory.GetCurrentDirectory() + "\\Reportes";
            if (!Directory.Exists(ruta_base)) Directory.CreateDirectory(ruta_base);

            string nombre_archivo = $"ReporteEmail {DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss tt")}.xlsx";
            string ruta_reporte = ruta_base + $"\\{nombre_archivo}";

            XLWorkbook wb = new XLWorkbook();

            string query = @"SELECT [EMAIL_DESTINATARIO]
                              ,[FECHA_ENVIO]
                              ,[ENVIADO]
                              ,[ERROR]
                              ,[USUARIO]
                              ,[NUMERO_IDENTIFICACION_TERCERO]
                              ,[PANTALLA]
                              ,[DESCRIPCION]
                          FROM [SEG].[AUDITORIA_ENVIO_EMAIL]";

            if (!string.IsNullOrEmpty(fecha_inicio) && !string.IsNullOrEmpty(fecha_fin))
            {
                query += " WHERE CONVERT(DATE,FECHA_ENVIO) BETWEEN CONVERT(DATE,@FECHA_INICIO) AND CONVERT(DATE,@FECHA_FIN)";
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>()
            {
                { "@FECHA_INICIO", fecha_inicio },
                { "@FECHA_FIN", fecha_fin },
            };

            wb.Worksheets.Add(_dataTableQuery.getDataTableQuery(query,parametros), "Envíos Email");

            using (FileStream fs = new FileStream(ruta_reporte, FileMode.Create, FileAccess.Write))
            {
                wb.SaveAs(fs);
            }

            wb.Dispose();

            return ruta_reporte;
        }

        public async Task<string> reporteFirma(string fecha_inicio, string fecha_fin)
        {
            string ruta_base = Directory.GetCurrentDirectory() + "\\Reportes";
            if (!Directory.Exists(ruta_base)) Directory.CreateDirectory(ruta_base);

            string nombre_archivo = $"ReporteFirmaDocumentos {DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss tt")}.xlsx";
            string ruta_reporte = ruta_base + $"\\{nombre_archivo}";

            XLWorkbook wb = new XLWorkbook();

            string query = @"SELECT [IDENTIFICACION_CLIENTE]
                              ,[NOMBRE_ARCHIVO]
                              ,[DIRECCION_IP]
                              ,[DESCRIPCION]
                              ,[USUARIO_ACCION]
                              ,[FECHA_ADICION]
                          FROM [SEG].[SEG_AUDITORIA_FIRMA_DOC]";

            if (!string.IsNullOrEmpty(fecha_inicio) && !string.IsNullOrEmpty(fecha_fin))
            {
                query += " WHERE CONVERT(DATE,FECHA_ADICION) BETWEEN CONVERT(DATE,@FECHA_INICIO) AND CONVERT(DATE,@FECHA_FIN)";
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>()
            {
                { "@FECHA_INICIO", fecha_inicio },
                { "@FECHA_FIN", fecha_fin },
            };

            wb.Worksheets.Add(_dataTableQuery.getDataTableQuery(query, parametros), "Auditoría PJ");

        using (FileStream fs = new FileStream(ruta_reporte, FileMode.Create, FileAccess.Write))
            {
                wb.SaveAs(fs);
            }

            wb.Dispose();

            return ruta_reporte;
        }

        public async Task<string> reporteLogin(string fecha_inicio, string fecha_fin)
        {
            string ruta_base = Directory.GetCurrentDirectory() + "\\Reportes";
            if (!Directory.Exists(ruta_base)) Directory.CreateDirectory(ruta_base);

            string nombre_archivo = $"ReporteAuditoriaLogin {DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss tt")}.xlsx";
            string ruta_reporte = ruta_base + $"\\{nombre_archivo}";

            XLWorkbook wb = new XLWorkbook();

            string query = @"  SELECT [USUARIO]
                                  ,[FECHA]
                                  ,[DESCRIPCION]
                                  ,[IP_LOGIN]
                              FROM [SEG].[AUDITORIA_LOGIN_USUARIO]";

            if (!string.IsNullOrEmpty(fecha_inicio) && !string.IsNullOrEmpty(fecha_fin))
            {
                query += " WHERE CONVERT(DATE,FECHA) BETWEEN CONVERT(DATE,@FECHA_INICIO) AND CONVERT(DATE,@FECHA_FIN)";
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>()
            {
                { "@FECHA_INICIO", fecha_inicio },
                { "@FECHA_FIN", fecha_fin },
            };

            wb.Worksheets.Add(_dataTableQuery.getDataTableQuery(query, parametros), "Auditoría Login");

        using (FileStream fs = new FileStream(ruta_reporte, FileMode.Create, FileAccess.Write))
            {
                wb.SaveAs(fs);
            }

            wb.Dispose();

            return ruta_reporte;
        }

        public async Task<string> reporteOtp(string fecha_inicio, string fecha_fin)
        {
            string ruta_base = Directory.GetCurrentDirectory() + "\\Reportes";
            if (!Directory.Exists(ruta_base)) Directory.CreateDirectory(ruta_base);

            string nombre_archivo = $"ReporteOTP {DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss tt")}.xlsx";
            string ruta_reporte = ruta_base + $"\\{nombre_archivo}";

            XLWorkbook wb = new XLWorkbook();

            string query = @"    SELECT [CODIGO_OTP]
                                  ,[FECHA_ADICION]
                                  ,[FECHA_VALIDACION]
                                  ,[ESTADO]
                                  ,[NUMERO_DOCUMENTO_PROCESO]
                                  ,[TIPO_PROCESO]
                                  ,[DESCRIPCION]
                                  ,[METODOS_ENVIO]
                              FROM [SEG].[REGISTRO_OTP]";

            if (!string.IsNullOrEmpty(fecha_inicio) && !string.IsNullOrEmpty(fecha_fin))
            {
                query += " WHERE CONVERT(DATE,FECHA_ADICION) BETWEEN CONVERT(DATE,@FECHA_INICIO) AND CONVERT(DATE,@FECHA_FIN)";
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>()
            {
                { "@FECHA_INICIO", fecha_inicio },
                { "@FECHA_FIN", fecha_fin },
            };

            wb.Worksheets.Add(_dataTableQuery.getDataTableQuery(query, parametros), "Envíos OTP");

        using (FileStream fs = new FileStream(ruta_reporte, FileMode.Create, FileAccess.Write))
            {
                wb.SaveAs(fs);
            }

            wb.Dispose();

            return ruta_reporte;
        }

        public async Task<string> reporteReestablecerContraseña(string fecha_inicio, string fecha_fin)
        {
            string ruta_base = Directory.GetCurrentDirectory() + "\\Reportes";
            if (!Directory.Exists(ruta_base)) Directory.CreateDirectory(ruta_base);

            string nombre_archivo = $"ReporteRecuperacionContraseña {DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss tt")}.xlsx";
            string ruta_reporte = ruta_base + $"\\{nombre_archivo}";

            XLWorkbook wb = new XLWorkbook();

            string query = @"  SELECT [USUARIO]
                                  ,[FECHA_CREACION]
                                  ,[ESTADO]
                                  ,[FECHA_FINALIZACION]
                                  ,[IP_HOST]
                              FROM [SEG].[SOLICITUD_RECUPERACION_CLAVE]";

            if (!string.IsNullOrEmpty(fecha_inicio) && !string.IsNullOrEmpty(fecha_fin))
            {
                query += " WHERE CONVERT(DATE,FECHA_CREACION) BETWEEN CONVERT(DATE,@FECHA_INICIO) AND CONVERT(DATE,@FECHA_FIN)";
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>()
            {
                { "@FECHA_INICIO", fecha_inicio },
                { "@FECHA_FIN", fecha_fin },
            };

            wb.Worksheets.Add(_dataTableQuery.getDataTableQuery(query, parametros), "Reestablecer Contraseña");

        using (FileStream fs = new FileStream(ruta_reporte, FileMode.Create, FileAccess.Write))
            {
                wb.SaveAs(fs);
            }

            wb.Dispose();

            return ruta_reporte;
        }

        public async Task<string> reporteSMS(string fecha_inicio, string fecha_fin)
        {
            string ruta_base = Directory.GetCurrentDirectory() + "\\Reportes";
            if (!Directory.Exists(ruta_base)) Directory.CreateDirectory(ruta_base);

            string nombre_archivo = $"ReporteEnviosSMS {DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss tt")}.xlsx";
            string ruta_reporte = ruta_base + $"\\{nombre_archivo}";

            XLWorkbook wb = new XLWorkbook();

            string query = @"  SELECT [CELULAR_DESTINATARIO]
                              ,[MENSAJE]
                              ,[FECHA_ENVIO]
                              ,[CODIGO_RESPUESTA]
                              ,[USUARIO]
                              ,[NUMERO_IDENTIFICACION_TERCERO]
                              ,[PANTALLA]
                          FROM [SEG].[AUDITORIA_ENVIO_SMS]";

            if (!string.IsNullOrEmpty(fecha_inicio) && !string.IsNullOrEmpty(fecha_fin))
            {
                query += " WHERE CONVERT(DATE,FECHA_ENVIO) BETWEEN CONVERT(DATE,@FECHA_INICIO) AND CONVERT(DATE,@FECHA_FIN)";
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>()
            {
                { "@FECHA_INICIO", fecha_inicio },
                { "@FECHA_FIN", fecha_fin },
            };

            wb.Worksheets.Add(_dataTableQuery.getDataTableQuery(query, parametros), "Envíos SMS");

        using (FileStream fs = new FileStream(ruta_reporte, FileMode.Create, FileAccess.Write))
            {
                wb.SaveAs(fs);
            }

            wb.Dispose();

            return ruta_reporte;
        }

        public async Task<string> reporteWPP(string fecha_inicio, string fecha_fin)
        {
            string ruta_base = Directory.GetCurrentDirectory() + "\\Reportes";
            if (!Directory.Exists(ruta_base)) Directory.CreateDirectory(ruta_base);

            string nombre_archivo = $"ReporteEnviosWPP {DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss tt")}.xlsx";
            string ruta_reporte = ruta_base + $"\\{nombre_archivo}";

            XLWorkbook wb = new XLWorkbook();

            string query = @"SELECT [CELULAR_DESTINATARIO]
                              ,[MENSAJE]
                              ,[FECHA_ENVIO]
                              ,[CODIGO_RESPUESTA]
                              ,[USUARIO]
                              ,[NUMERO_IDENTIFICACION_TERCERO]
                              ,[PANTALLA]
                          FROM [SEG].[AUDITORIA_ENVIO_WPP]";

            if (!string.IsNullOrEmpty(fecha_inicio) && !string.IsNullOrEmpty(fecha_fin))
            {
                query += " WHERE CONVERT(DATE,FECHA_ENVIO) BETWEEN CONVERT(DATE,@FECHA_INICIO) AND CONVERT(DATE,@FECHA_FIN)";
            }

            Dictionary<string, string> parametros = new Dictionary<string, string>()
            {
                { "@FECHA_INICIO", fecha_inicio },
                { "@FECHA_FIN", fecha_fin },
            };

            wb.Worksheets.Add(_dataTableQuery.getDataTableQuery(query, parametros), "Envíos WPP");

        using (FileStream fs = new FileStream(ruta_reporte, FileMode.Create, FileAccess.Write))
            {
                wb.SaveAs(fs);
            }

            wb.Dispose();

            return ruta_reporte;
        }

        public async Task<bool> registrarAuditoriaDescargaArchivos(AuditoriaDescargaArchivo auditoriaDescargaArchivo)
        {
            return await _auditoriaRepository.registrarAuditoriaDescargaArchivo(auditoriaDescargaArchivo);
        }
    }
}
