using TaskSe.Data.Repositories.Interfaces.ANI;
using TaskSe.Model.Models.ANI;
using TaskSe.Model.Models.Configuracion.Perfilamiento;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Data.Repositories.Repositories.ANI
{
    public class ANIRepository : IANIRepository
    {
        private string ConnectionString;

        public ANIRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected SqlConnection dbConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public async Task<bool> actualizarAuditoriaConsultaANI(AniDTO consulta)
        {
            var result = 0;
            var db = dbConnection();

            var sql = @"[SEG].[ACTUALIZAR_AUDITORIA_CONSULTA_ANI]";

            var p = new DynamicParameters();
            p.Add("@idAudANY", consulta.id_consulta);
            p.Add("@DEPARTAMENTO_EXP_REGISTRADURIA", consulta.departamento_expedicion);
            p.Add("@FECHA_EXP_REGISTRADURIA", consulta.fecha_expedicion);
            p.Add("@MUNICIPIO_EXP_REGISTRADURIA", consulta.municipio_expedicion);
            p.Add("@PRIMER_NOMBRE_REGISTRADURIA", consulta.primer_nombre);
            p.Add("@SEGUNDO_NOMBRE_REGISTRADURIA", consulta.segundo_nombre);
            p.Add("@PRIMER_APELLIDO_REGISTRADURIA", consulta.primer_apellido);
            p.Add("@SEGUNDO_APELLIDO_REGISTRADURIA", consulta.segundo_apellido);
            p.Add("@CODIGO_ERROR_CEDULA", consulta.codigo_error_cedula);
            p.Add("@ESTADO_CEDULA", consulta.estado_cedula);

            result = await db.ExecuteAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);

            return result > 0;

        }

        public async Task<AniDTO> consultarRegistroByNumDoc(string numero_documento)
        {
            var db = dbConnection();

            var sql = @"SELECT [ID_AUDI_CONSULTA_ANI] as id_consulta
                          ,[CEDULA] as cedula
                          ,[RESPONSE] as status_code
                          ,[FECHA] as fecha_consulta
                          ,[CODIGO_ERROR_CEDULA] as codigo_error_cedula
                          ,[ESTADO_CEDULA] as estado_cedula
                          ,[DEPARTAMENTO_EXP_REGISTRADURIA] as departamento_expedicion
                          ,[FECHA_EXP_REGISTRADURIA] as fecha_expedicion
                          ,[MUNICIPIO_EXP_REGISTRADURIA] as municipio_expedicion
                          ,[PRIMER_NOMBRE_REGISTRADURIA] as primer_nombre
                          ,[SEGUNDO_NOMBRE_REGISTRADURIA]  as segundo_nombre
                          ,[PRIMER_APELLIDO_REGISTRADURIA] as primer_apellido
                          ,[SEGUNDO_APELLIDO_REGISTRADURIA] as segundo_apellido
                      FROM [SEG].[SEG_AUDITORIA_CONSULTA_ANI]
                        where CEDULA = @ID";

            var p = new DynamicParameters();
            p.Add("@ID", numero_documento);



            return await db.QueryFirstOrDefaultAsync<AniDTO>(sql.ToString(), p);

        }

        public async Task<string> insertarAuditoriaConsultaANI(string numero_documento, string response, string ipAccion, string usuarioAccion)
        {
            var db = dbConnection();



            var sql = @"[SEG].[INSERTAR_AUDITORIA_CONSULTA_ANI]";

            var p = new DynamicParameters();

            p.Add("@NUM_DOC", numero_documento);
            p.Add("@RESPONSE", response);
            p.Add("@IP", ipAccion);
            p.Add("@HOST", "");
            p.Add("@ID_USUARIO", usuarioAccion);

            var result = await db.ExecuteScalarAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);
            return result.ToString();

        }

        public async Task<AniDTO> consultarRegistroById(string id_consulta)
        {
            var db = dbConnection();

            var sql = @"SELECT [ID_AUDI_CONSULTA_ANI] as id_consulta
                          ,[CEDULA] as cedula
                          ,[RESPONSE] as status_code
                          ,[FECHA] as fecha_consulta
                          ,[CODIGO_ERROR_CEDULA] as codigo_error_cedula
                          ,[ESTADO_CEDULA] as estado_cedula
                          ,[DEPARTAMENTO_EXP_REGISTRADURIA] as departamento_expedicion
                          ,[FECHA_EXP_REGISTRADURIA] as fecha_expedicion
                          ,[MUNICIPIO_EXP_REGISTRADURIA] as municipio_expedicion
                          ,[PRIMER_NOMBRE_REGISTRADURIA] as primer_nombre
                          ,[SEGUNDO_NOMBRE_REGISTRADURIA]  as segundo_nombre
                          ,[PRIMER_APELLIDO_REGISTRADURIA] as primer_apellido
                          ,[SEGUNDO_APELLIDO_REGISTRADURIA] as segundo_apellido
                      FROM [SEG].[SEG_AUDITORIA_CONSULTA_ANI]
                        where ID_AUDI_CONSULTA_ANI = @ID";

            var p = new DynamicParameters();
            p.Add("@ID", id_consulta);



            return await db.QueryFirstOrDefaultAsync<AniDTO>(sql.ToString(), p);
        }

        public async Task<IEnumerable<AniDTO>> consultarValidacionesANI()
        {
            var db = dbConnection();

            var sql = @"SELECT [ID_AUDI_CONSULTA_ANI] as id_consulta
                          ,[CEDULA] as cedula
                          ,[RESPONSE] as status_code
                          ,[FECHA] as fecha_consulta
                          ,[CODIGO_ERROR_CEDULA] as codigo_error_cedula
                          ,[ESTADO_CEDULA] as estado_cedula
                          ,[DEPARTAMENTO_EXP_REGISTRADURIA] as departamento_expedicion
                          ,[FECHA_EXP_REGISTRADURIA] as fecha_expedicion
                          ,[MUNICIPIO_EXP_REGISTRADURIA] as municipio_expedicion
                          ,[PRIMER_NOMBRE_REGISTRADURIA] as primer_nombre
                          ,[SEGUNDO_NOMBRE_REGISTRADURIA]  as segundo_nombre
                          ,[PRIMER_APELLIDO_REGISTRADURIA] as primer_apellido
                          ,[SEGUNDO_APELLIDO_REGISTRADURIA] as segundo_apellido
                      FROM [SEG].[SEG_AUDITORIA_CONSULTA_ANI]";

            return await db.QueryAsync<AniDTO>(sql.ToString());
        }
    }
}
