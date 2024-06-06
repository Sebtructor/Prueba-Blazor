using TaskSe.Data.Repositories.Interfaces.Configuracion.Perfilamiento;
using TaskSe.Model.Models.Configuracion.Perfilamiento;
using Dapper;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Data.Repositories.Repositories.Configuracion.Perfilamiento
{
    public class UserRepository : IUserRepository
    {
        private string ConnectionString;

        public UserRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected SqlConnection dbConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public async Task<IEnumerable<Usuario>> getUsers()
        {
            var db = dbConnection();

            var sql = @"SELECT [ID_USUARIO] as id_usuario
                          ,[NOMBRES] as nombres
                          ,[APELLIDOS] as apellidos
                          ,[USUARIO] as usuario
                          ,[CLAVE] as clave
                          ,[EMAIL] as email
                          ,[EMAIL_DOS] as email_dos
                          ,[ID_ROL] as id_rol
                          ,(SELECT NOMBRE_ROL FROM PAR.ROL R WHERE R.ID_ROL = U.ID_ROL) as descripcion_rol
                          ,[ESTADO] as estado
                          ,[CELULAR] as celular
                          ,TWOFA_ENABLED
                      FROM [SEG].[SEG_USUARIO] U";

            var p = new DynamicParameters();

            return await db.QueryAsync<Usuario>(sql.ToString());
        }

        public async Task<string> registrarAuditoriaLogin(string usuario, string descripcion, string ipAddress)
        {
            var db = dbConnection();

            var sql = @"[SEG].[INSERTAR_AUDITORIA_LOGIN_USUARIO]";

            var p = new DynamicParameters();
            p.Add("@USUARIO", usuario);
            p.Add("@DESCRIPCION", descripcion);
            p.Add("@IP_HOST", ipAddress);

            var result = await db.ExecuteScalarAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);

            return result.ToString();
        }

        public async Task<string> insertarUsuario(Usuario usuario, string usuario_accion)
        {
            var db = dbConnection();



            var sql = @"SEG.INSERTAR_USUARIO";

            var p = new DynamicParameters();

            if (!string.IsNullOrEmpty(usuario.id_usuario))
                p.Add("@ID_USUARIO", usuario.id_usuario);
            p.Add("@NOMBRES", usuario.nombres);
            p.Add("@APELLIDOS", usuario.apellidos);
            p.Add("@USUARIO", usuario.usuario);
            p.Add("@CLAVE", usuario.clave);
            p.Add("@EMAIL", usuario.email);
            p.Add("@EMAIL_DOS", usuario.email_dos);
            p.Add("@ID_ROL", usuario.id_rol);
            p.Add("@USUARIO_ADICIONO", usuario_accion);
            p.Add("@ESTADO", usuario.estado);
            p.Add("@CELULAR", usuario.celular);

            var result = await db.ExecuteScalarAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);
            return result.ToString();
        }

        public async Task<Usuario> getUsuarioByUser(string usuario)
        {
            var db = dbConnection();

            var sql = @"SELECT [ID_USUARIO] as id_usuario
                          ,[NOMBRES] as nombres
                          ,[APELLIDOS] as apellidos
                          ,[USUARIO] as usuario
                          ,[CLAVE] as clave
                          ,[EMAIL] as email
                          ,[EMAIL_DOS] as email_dos
                          ,[ID_ROL] as id_rol
                          ,[ESTADO] as estado
                          ,[CELULAR] as celular
                          ,TWOFA_ENABLED
                      FROM [SEG].[SEG_USUARIO] WHERE USUARIO = @USER";

            var p = new DynamicParameters();
            p.Add("@USER", usuario);

            return await db.QueryFirstOrDefaultAsync<Usuario>(sql.ToString(), p);
        }

        public async Task<Usuario> getUsuarioById(string id_usuario)
        {
            var db = dbConnection();

            var sql = @"SELECT [ID_USUARIO] as id_usuario
                          ,[NOMBRES] as nombres
                          ,[APELLIDOS] as apellidos
                          ,[USUARIO] as usuario
                          ,[CLAVE] as clave
                          ,[EMAIL] as email
                          ,[EMAIL_DOS] as email_dos
                          ,[ID_ROL] as id_rol
                          ,[ESTADO] as estado
                          ,[CELULAR] as celular
                          ,TWOFA_ENABLED
                      FROM [SEG].[SEG_USUARIO] WHERE ID_USUARIO = @USER";

            var p = new DynamicParameters();
            p.Add("@USER", id_usuario);

            return await db.QueryFirstOrDefaultAsync<Usuario>(sql.ToString(), p);
        }

        public async Task<string> insertarSolicitudPass(string usuario, string ipAddress)
        {

            var db = dbConnection();

            var sql = @"SEG.INSERTAR_SOLICITUD_RECUPERACION_CLAVE";

            var p = new DynamicParameters();
            p.Add("@USUARIO", usuario);
            p.Add("@IP", ipAddress);

            var result = (decimal)await db.ExecuteScalarAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);
            return result.ToString();
        }

        public async Task<SolicitudContraseña> getSolicitudContraseña(string id_solicitud)
        {
            var db = dbConnection();

            var sql = @"SELECT [ID_SOLICITUD_RECUPERACION] as id_solicitud
                              ,[FECHA_CREACION] as fecha
                              ,[ESTADO] as estado
                              ,USUARIO
                          FROM [SEG].[SOLICITUD_RECUPERACION_CLAVE] WHERE ID_SOLICITUD_RECUPERACION = @ID";

            var p = new DynamicParameters();
            p.Add("@ID", id_solicitud);

            return await db.QueryFirstOrDefaultAsync<SolicitudContraseña>(sql.ToString(), p);
        }

        public async Task<bool> completarSolicitudContraseña(string id_solicitud, string nuevaPass)
        {
            var result = 0;

            var db = dbConnection();

            var sql = @"SEG.COMPLETAR_SOLICITUD_RECUPERACION_CLAVE";

            var p = new DynamicParameters();
            p.Add("@ID", id_solicitud);
            p.Add("@NUEVA_PASS", nuevaPass);

            result = await db.ExecuteAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);

            return result > 0;
        }

        public async Task<bool> registrarAuditoriaCierreSesion(string id_auditoria, string usuario, string ip, string motivo)
        {
            var result = 0;

            var db = dbConnection();

            var sql = @"SEG.REGISTRAR_INFO_CIERRE_SESION";

            var p = new DynamicParameters();
            p.Add("@ID_AUDITORIA", id_auditoria);
            p.Add("@USUARIO_CIERRE_SESION", usuario);
            p.Add("@IP_CIERRE_SESION", ip);
            p.Add("@MOTIVO_CIERRE_SESION", motivo);

            result = await db.ExecuteAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);

            return result > 0;
        }

        public async Task<string> cantidadIntentosFallidoPorUsuarioUltimos5Minutos(string nombre_usuario)
        {
            var db = dbConnection();

            var sql = @"SELECT 
                        COUNT(*) AS INTENTOS_FALLIDO
                        FROM [SEG].[AUDITORIA_LOGIN_USUARIO]
                        WHERE 
                        DATEDIFF(MINUTE,FECHA,GETDATE())<=5
                        AND DESCRIPCION = 'No exitoso, credenciales incorrectas'
                        AND USUARIO = @USUARIO";

            var p = new DynamicParameters();
            p.Add("@USUARIO", nombre_usuario);

            return await db.QueryFirstOrDefaultAsync<string>(sql.ToString(), p);
        }

        public async Task<string> consultarDiasDesdeUltimoCambioContraseña(string nombre_usuario)
        {
            var db = dbConnection();

            var sql = @"SELECT 
                        DATEDIFF(DAY,[FECHA_CAMBIO_CONTRASENA],GETDATE()) AS DIAS_ULTIMO_CAMBIO
                        FROM [SEG].[SEG_USUARIO]
                        WHERE USUARIO = @USUARIO";

            var p = new DynamicParameters();
            p.Add("@USUARIO", nombre_usuario);

            return await db.QueryFirstOrDefaultAsync<string>(sql.ToString(), p);
        }

        public async Task<bool> insertarSecreto(Secret2FA secret, string ip_address)
        {
            var result = 0;

            var db = dbConnection();

            var sql = @"SEG.INSERTAR_SECRETO_2FA";

            var p = new DynamicParameters();
            p.Add("@SECRETO", secret.SECRETO);
            p.Add("@USUARIO", secret.USUARIO);
            p.Add("@IP_ACCION", ip_address);

            result = await db.ExecuteAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);

            return result > 0;
        }

        public async Task<Secret2FA> getSecret(string usuario)
        {
            var db = dbConnection();

            var sql = @"
                        SELECT TOP(1) [ID_SECRETO]
                        ,[SECRETO]
                        ,[FECHA_ADICION]
                        ,[USUARIO]
                        FROM [SEG].[SECRETOS_2FA]
                        WHERE USUARIO = @USUARIO
                        ORDER BY FECHA_ADICION DESC
                        ";

            var p = new DynamicParameters();
            p.Add("@USUARIO", usuario);

            return await db.QueryFirstOrDefaultAsync<Secret2FA>(sql.ToString(), p);
        }

        public async Task<bool> actualizarEstado2FA(string usuario, bool estado, string ip_address)
        {
            var result = 0;

            var db = dbConnection();

            var sql = @"SEG.ACTUALIZAR_2FA_USUARIO";

            var p = new DynamicParameters();
            p.Add("@ENABLED", estado);
            p.Add("@USUARIO", usuario);

            result = await db.ExecuteAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);

            return result > 0;
        }

        public async Task<bool> insertarAuditoriaTOTP(string codigo, string usuario, string descripcion, string ip_address)
        {
            var result = 0;

            var db = dbConnection();

            var sql = @"SEG.INSERTAR_AUDITORIA_TOTP_2FA";

            var p = new DynamicParameters();
            p.Add("@CODIGO_TOTP", codigo);
            p.Add("@USUARIO", usuario);
            p.Add("@DESCRIPCION", descripcion);
            p.Add("@IP_ACCION", ip_address);

            result = await db.ExecuteAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);

            return result > 0;
        }
    }

}
