using TaskSe.Data.Repositories.Interfaces.Par;
using TaskSe.Model.Models.Par;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Data.Repositories.Repositories.Par
{
    public class ParametroGeneralRepository : IParametroGeneralRepository
    {
        private string ConnectionString;

        public ParametroGeneralRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }
        protected SqlConnection dbConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public async Task<ParametroGeneral> consultarParametroGeneral(string id_parametro_general)
        {
            var db = dbConnection();
            var sql = @"SELECT 
                        [ID_PARAMETRO_GENERAL]
                        ,[NOMBRE_PARAMETRO_GENERAL]
                        FROM [PAR].[PARAMETRO_GENERAL]
                        WHERE ID_PARAMETRO_GENERAL = @ID";
            var p = new DynamicParameters();
            p.Add("@ID", id_parametro_general);

            return await db.QueryFirstOrDefaultAsync<ParametroGeneral>(sql.ToString(), p);
        }

        public async Task<IEnumerable<ParametroGeneral>> consultarParametrosGenerales()
        {
            var db = dbConnection();
            var sql = @"SELECT 
                        [ID_PARAMETRO_GENERAL]
                        ,[NOMBRE_PARAMETRO_GENERAL]
                        FROM [PAR].[PARAMETRO_GENERAL]";

            return await db.QueryAsync<ParametroGeneral>(sql.ToString());
        }

        public async Task<bool> eliminarParametroGeneral(string id_parametro_general)
        {
            var result = 0;
            var db = dbConnection();
            var sql = @"PAR.ELIMINAR_PARAMETRO_GENERAL";
            var p = new DynamicParameters();
            p.Add("@ID_PARAMETRO_GENERAL", id_parametro_general);
            result = await db.ExecuteAsync(sql.ToString(), p, commandType:
           System.Data.CommandType.StoredProcedure);
            return result > 0;

        }

        public async Task<string> insertarInfoParametroGeneral(ParametroGeneral parametroGeneral, string usuario_adiciono)
        {

            var db = dbConnection();
            var sql = @"PAR.INSERTAR_INFORMACION_PARAMETRO_GENERAL";
            var p = new DynamicParameters();
            if (!string.IsNullOrEmpty(parametroGeneral.id_parametro_general))
                p.Add("@ID_PARAMETRO_GENERAL", parametroGeneral.id_parametro_general);
            p.Add("@NOMBRE_PARAMETRO_GENERAL", parametroGeneral.nombre_parametro_general);
            p.Add("@USUARIO_ADICIONO ", usuario_adiciono);
            var result = await db.ExecuteScalarAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);
            return result.ToString();

        }
    }
}
