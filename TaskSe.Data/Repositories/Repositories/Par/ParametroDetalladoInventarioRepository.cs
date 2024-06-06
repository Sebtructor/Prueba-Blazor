using TaskSe.Data.Repositories.Interfaces.Par;
using TaskSe.Model.Models.Par;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Data.Repositories.Repositories.Par
{
    public class ParametroDetalladoRepository : IParametroDetalladoRepository
    {
        private string ConnectionString;

        public ParametroDetalladoRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }
        protected SqlConnection dbConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public async Task<ParametroDetallado> consultarParametroDetalladoById(string id_parametro_detallado)
        {
            var db = dbConnection();
            var sql = @"SELECT 
                        [ID_PARAMETRO_DETALLADO]
                        ,[NOMBRE_PARAMETRO_DETALLADO]
                        ,[FECHA_ADICION]
                        ,[USUARIO_ADICIONO]
                        ,[ID_PARAMETRO_GENERAL]
                        FROM [PAR].[PARAMETRO_DETALLADO]
                        WHERE ID_PARAMETRO_DETALLADO = @ID";
            var p = new DynamicParameters();
            p.Add("@ID", id_parametro_detallado);

            return await db.QueryFirstOrDefaultAsync<ParametroDetallado>(sql.ToString(), p);
        }

        public async Task<IEnumerable<ParametroDetallado>> consultarParametrosDetalladosByParametroGeneral(string id_parametro_general)
        {
            var db = dbConnection();
            var sql = @"SELECT 
                        [ID_PARAMETRO_DETALLADO]
                        ,[NOMBRE_PARAMETRO_DETALLADO]
                        ,[FECHA_ADICION]
                        ,[USUARIO_ADICIONO]
                        ,[ID_PARAMETRO_GENERAL]
                        FROM [PAR].[PARAMETRO_DETALLADO]
                        WHERE ID_PARAMETRO_GENERAL = @ID";
            var p = new DynamicParameters();
            p.Add("@ID", id_parametro_general);

            return await db.QueryAsync<ParametroDetallado>(sql.ToString(), p);
        }

        public async Task<bool> eliminarParametroDetallado(string id_parametro_detallado)
        {
            var result = 0;
            var db = dbConnection();
            var sql = @"PAR.ELIMINAR_PARAMETRO_DETALLADO";
            var p = new DynamicParameters();
            p.Add("@ID_PARAMETRO_DETALLADO", id_parametro_detallado);
            result = await db.ExecuteAsync(sql.ToString(), p, commandType:
           System.Data.CommandType.StoredProcedure);
            return result > 0;
        }

        public async Task<bool> insertarInformacionParametroDetallado(ParametroDetallado parametroDetallado, string id_parametro_general, string usuario_accion)
        {
            var result = 0;
            var db = dbConnection();
            var sql = @"PAR.INSERTAR_INFORMACION_PARAMETRO_DETALLADO";
            var p = new DynamicParameters();
            if (!string.IsNullOrEmpty(parametroDetallado.id_parametro_detallado))
                p.Add("@ID_PARAMETRO_DETALLADO", parametroDetallado.id_parametro_detallado);
            p.Add("@NOMBRE_PARAMETRO_DETALLADO", parametroDetallado.nombre_parametro_detallado);
            p.Add("@USUARIO_ADICIONO", usuario_accion);
            p.Add("@ID_PARAMETRO_GENERAL", id_parametro_general);
            result = await db.ExecuteAsync(sql.ToString(), p, commandType:
           System.Data.CommandType.StoredProcedure);
            return result > 0;
        }
    }
}
