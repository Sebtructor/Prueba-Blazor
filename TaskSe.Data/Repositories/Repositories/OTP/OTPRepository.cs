using TaskSe.Data.Repositories.Interfaces.OTP;
using TaskSe.Model.Models.OTP;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Data.Repositories.Repositories.OTP
{
    public class OTPRepository : IOTPRepository
    {
        private string ConnectionString;

        public OTPRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected SqlConnection dbConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public async Task<bool> completarEstadoOTP(string id_otp_code, string estado)
        {
            var db = dbConnection();

            var result = 0;

            var sql = @"SEG.ACTUALIZAR_ESTADO_OTP";

            var p = new DynamicParameters();

            p.Add("@ID_OTP", id_otp_code);
            p.Add("@ESTADO", estado);

            result = await db.ExecuteAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);

            return result > 0;
        }

        public async Task<OTPModel> getUltimoOTPProceso(string numero_documento, string tipo_proceso)
        {
            var db = dbConnection();

            var sql = @"SELECT TOP(1) [ID_OTP] as id_otp_code
                      ,[CODIGO_OTP] as otp_code
                      ,[FECHA_ADICION]
                      ,[FECHA_VALIDACION]
                      ,[ESTADO]
                      ,[NUMERO_DOCUMENTO_PROCESO]
                      ,[TIPO_PROCESO]
                      ,[DESCRIPCION]
                      ,[METODOS_ENVIO]
                  FROM [SEG].[REGISTRO_OTP] WHERE NUMERO_DOCUMENTO_PROCESO = @NUMDOC
                  AND TIPO_PROCESO = @TIPO_PROCESO
                  ORDER BY FECHA_ADICION DESC";

            var p = new DynamicParameters();
            p.Add("@NUMDOC", numero_documento);
            p.Add("@TIPO_PROCESO", tipo_proceso);

            return await db.QueryFirstOrDefaultAsync<OTPModel>(sql.ToString(), p);
        }

        public async Task<bool> insertarOTP(OTPModel otp_code)
        {
            var db = dbConnection();

            var result = 0;

            var sql = @"SEG.INSERTAR_OTP_CODE";

            var p = new DynamicParameters();

            p.Add("@CODIGO_OTP", otp_code.otp_code);
            p.Add("@ESTADO", otp_code.estado);
            p.Add("@NUMERO_DOCUMENTO_PROCESO", otp_code.numero_documento_proceso);
            p.Add("@TIPO_PROCESO", otp_code.tipo_proceso);
            p.Add("@DESCRIPCION", otp_code.descripcion);
            p.Add("@METODOS_ENVIO", otp_code.metodos_envio);

            result = await db.ExecuteAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);

            return result > 0;
        }
    }
}
