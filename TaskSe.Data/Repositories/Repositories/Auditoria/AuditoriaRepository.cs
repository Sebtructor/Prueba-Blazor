using TaskSe.Data.Repositories.Interfaces.Auditoria;
using TaskSe.Model.Models.Configuracion.Perfilamiento;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Data.Repositories.Repositories.Auditoria
{
    public class AuditoriaRepository : IAuditoriaRepository
    {
        private string ConnectionString;

        public AuditoriaRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected SqlConnection dbConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public async Task<bool> mtdAuditoriaEnvioCorreo(string EMAIL_DESTINATARIO, string EMAIL_EMISOR, string MENSAJE, string ENVIADO,
               string ERROR, string PANTALLA, string DESCRIPCION, string NUMERO_IDENTIFICACION, string USUARIO)
        {

            var result = 0;
            var db = dbConnection();

            var sql = @"[SEG].[INSERTAR_AUDITORIA_ENVIO_EMAIL]";

            var p = new DynamicParameters();
            p.Add("@EMAIL_DESTINATARIO", EMAIL_DESTINATARIO);
            p.Add("@EMAIL_EMISOR", EMAIL_EMISOR);
            p.Add("@MENSAJE", MENSAJE);
            p.Add("@ENVIADO", ENVIADO);
            p.Add("@USUARIO", USUARIO);
            p.Add("@PANTALLA", PANTALLA);
            p.Add("@DESCRIPCION", DESCRIPCION);
            p.Add("@ERROR", ERROR);
            p.Add("@NUMERO_IDENTIFICACION_TERCERO", NUMERO_IDENTIFICACION);


            result = await db.ExecuteAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);

            return result > 0;
        }

        public async Task<bool> mtdAuditoriaTransaccionalPN(string Accion, string Pantalla, string Descripcion, string cedula, string idUsuario, string ipAddress)
        {
            var result = 0;
            var db = dbConnection();

            var sql = @"[AUD].[InsertarLogTransaccional]";

            var p = new DynamicParameters();
            p.Add("@Accion_LogTransaccional", Accion);
            p.Add("@Descripcion_LogTransaccional", Descripcion);
            p.Add("@IdUsuario_LogTransaccional", idUsuario);
            p.Add("@Pantalla_LogTransaccional", Pantalla);
            p.Add("@NumeroIdentificacion_LogTransaccional", cedula);
            p.Add("@IP_LogTransaccional", ipAddress);

            result = await db.ExecuteAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);

            return result > 0;
        }

        public async Task<bool> mtdAuditoriaTransaccionalPJ(string Accion, string Pantalla, string Descripcion, string cedula, string idUsuario, string ipAddress)
        {
            var result = 0;
            var db = dbConnection();

            var sql = @"[AUD].[InsertarLogTransaccional_PJ]";

            var p = new DynamicParameters();
            p.Add("@Accion_LogTransaccional", Accion);
            p.Add("@Descripcion_LogTransaccional", Descripcion);
            p.Add("@IdUsuario_LogTransaccional", idUsuario);
            p.Add("@Pantalla_LogTransaccional", Pantalla);
            p.Add("@NumeroIdentificacion_LogTransaccional", cedula);
            p.Add("@IP_LogTransaccional", ipAddress);

            result = await db.ExecuteAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);

            return result > 0;
        }
        public async Task<bool> mtdAuditoriaEventos(string ACCION, string DESCRIPCION, string IP_ACCION, string USUARIO_ACCION, string ID_USUARIO, string ID_ROL)
        {
            var result = 0;
            var db = dbConnection();

            var sql = @"AUD.INSERTAR_AUDITORIA_EVENTOS";

            var p = new DynamicParameters();
            p.Add("@ACCION", ACCION);
            p.Add("@DESCRIPCION", DESCRIPCION);
            p.Add("@IP_ACCION", IP_ACCION);
            p.Add("@USUARIO_ACCION", USUARIO_ACCION);
            p.Add("@ID_USUARIO", ID_USUARIO);
            p.Add("@ID_ROL", ID_ROL);

            result = await db.ExecuteAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);

            return result > 0;
        }

        public async Task<bool> registrarAuditoriaNavegacion(AuditoriaNavegacion auditoriaNavegacion)
        {
            var result = 0;
            var db = dbConnection();

            var sql = @"SEG.INSERTAR_AUDITORIA_NAVEGACION";

            var p = new DynamicParameters();
            p.Add("@UserAgent", auditoriaNavegacion.UserAgent);
            p.Add("@Navegador", auditoriaNavegacion.Navegador);
            p.Add("@VersionNavegador", auditoriaNavegacion.VersionNavegador);
            p.Add("@PlataformaNavegador", auditoriaNavegacion.PlataformaNavegador);
            p.Add("@UrlActual", auditoriaNavegacion.UrlActual);
            p.Add("@Idioma", auditoriaNavegacion.Idioma);
            p.Add("@CookiesHabilitadas", auditoriaNavegacion.CookiesHabilitadas);
            p.Add("@AnchoPantalla", auditoriaNavegacion.AnchoPantalla);
            p.Add("@AltoPantalla", auditoriaNavegacion.AltoPantalla);
            p.Add("@ProfundidadColor", auditoriaNavegacion.ProfundidadColor);
            p.Add("@NombreSO", auditoriaNavegacion.NombreSO);
            p.Add("@VersionSO", auditoriaNavegacion.VersionSO);
            p.Add("@Latitud", auditoriaNavegacion.Latitud);
            p.Add("@Longitud", auditoriaNavegacion.Longitud);
            p.Add("@ip_address", auditoriaNavegacion.ip_address);
            p.Add("@usuario_accion", auditoriaNavegacion.usuario_accion);
            p.Add("@rolUsuario", auditoriaNavegacion.rolUsuario);
            p.Add("@idUsuario", auditoriaNavegacion.idUsuario);
            p.Add("@ubicacion", auditoriaNavegacion.ubicacion);
            p.Add("@UBICACION_IP", auditoriaNavegacion.UBICACION_IP);

            result = await db.ExecuteAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);

            return result > 0;
        }

        public async Task<bool> registrarAuditoriaDescargaArchivo(AuditoriaDescargaArchivo auditoriaDescargaArchivo)
        {
            var result = 0;
            var db = dbConnection();

            var sql = @"SEG.INSERTAR_AUDITORIA_DESCARGA_ARCHIVOS";

            var p = new DynamicParameters();
            p.Add("@ruta_original", auditoriaDescargaArchivo.ruta_original);
            p.Add("@ruta_descargada", auditoriaDescargaArchivo.ruta_descargada);
            p.Add("@nombre_archivo", auditoriaDescargaArchivo.nombre_archivo);
            p.Add("@extension_archivo", auditoriaDescargaArchivo.extension_archivo);
            p.Add("@peso_archivo", auditoriaDescargaArchivo.peso_archivo);
            p.Add("@usuario", auditoriaDescargaArchivo.usuario);
            p.Add("@ip_address", auditoriaDescargaArchivo.ip_address);

            result = await db.ExecuteAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);

            return result > 0;
        }
    }
}
