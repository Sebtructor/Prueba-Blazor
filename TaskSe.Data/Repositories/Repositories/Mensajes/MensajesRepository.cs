using TaskSe.Data.Repositories.Interfaces.Mensajes;
using TaskSe.Model.Models.Configuracion.Perfilamiento;
using TaskSe.Model.Models.Mensajes;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Data.Repositories.Repositories.Mensajes
{
    public class MensajesRepository : IMensajesRepository
    {
        private string ConnectionString;

        public MensajesRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected SqlConnection dbConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public async Task<IEnumerable<Mensaje>> getMensajes()
        {
            var db = dbConnection();

            var sql = @"
                        SELECT [ID_AUDITORIA] as id_mensaje
                              ,[CELULAR_DESTINATARIO] as celular_destino
                              ,[MENSAJE] as contenido_mensaje
                              ,[FECHA_ENVIO] as fecha_envio
                              ,[CODIGO_RESPUESTA] as estado
                              ,'1' AS tipo_mensaje
                              ,[PANTALLA] as pantalla
                          FROM [SEG].[AUDITORIA_ENVIO_SMS]

                          UNION ALL


                        SELECT [ID_AUDITORIA] as id_mensaje
                              ,[CELULAR_DESTINATARIO] as celular_destino
                              ,[MENSAJE] as contenido_mensaje
                              ,[FECHA_ENVIO] as fecha_envio
                              ,[CODIGO_RESPUESTA] as estado
                              ,'2' AS tipo_mensaje
                              ,[PANTALLA] as pantalla
                          FROM [SEG].[AUDITORIA_ENVIO_WPP]";

            return await db.QueryAsync<Mensaje>(sql.ToString());
        }

        public async Task<bool> insertarMensajeSms(Mensaje mensaje)
        {
            var result = 0;
            var db = dbConnection();

            var sql = @"[SEG].[INSERTAR_AUDITORIA_ENVIO_SMS]";

            var p = new DynamicParameters();

            p.Add("@CELULAR_DESTINATARIO", mensaje.celular_destino);
            p.Add("@MENSAJE", mensaje.contenido_mensaje);
            p.Add("@CODIGO_RESPUESTA", mensaje.estado);
            p.Add("@USUARIO", "");
            p.Add("@NUMERO_IDENTIFICACION_TERCERO", mensaje.numero_identificacion);
            p.Add("@PANTALLA", mensaje.concepto);


            result = await db.ExecuteAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);

            return result > 0;
        }

        public async Task<bool> insertarMensajeWpp(Mensaje mensaje)
        {
            var result = 0;
            var db = dbConnection();

            var sql = @"[SEG].[INSERTAR_AUDITORIA_ENVIO_WPP]";

            var p = new DynamicParameters();

            p.Add("@CELULAR_EMISOR", "");
            p.Add("@CELULAR_DESTINATARIO", mensaje.celular_destino);
            p.Add("@MENSAJE", mensaje.contenido_mensaje);
            p.Add("@CODIGO_RESPUESTA", mensaje.estado);
            p.Add("@USUARIO", "");
            p.Add("@NUMERO_IDENTIFICACION_TERCERO", mensaje.numero_identificacion);
            p.Add("@PANTALLA", mensaje.concepto);


            result = await db.ExecuteAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);

            return result > 0;
        }
    }
}
