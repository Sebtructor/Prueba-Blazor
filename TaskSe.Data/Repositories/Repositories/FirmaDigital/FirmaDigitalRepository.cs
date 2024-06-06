using TaskSe.Data.Repositories.Interfaces.FirmaDigital;
using TaskSe.Model.Models.Configuracion.Perfilamiento;
using TaskSe.Model.Models.FirmaDigital;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Data.Repositories.Repositories.FirmaDigital
{
    public class FirmaDigitalRepository : IFirmaDigitalRepository
    {

        private string ConnectionString;

        public FirmaDigitalRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected SqlConnection dbConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public async Task<Firma> getFirma(string id_firma)
        {
            var db = dbConnection();

            var sql = @"SELECT [ID_AUDITORIA_FIRMA_DOC] as id_firma
                      ,[IDENTIFICACION_CLIENTE] as identificacion_cliente
                      ,[NOMBRE_ARCHIVO] as nombre_archivo
                      ,[DIRECCION_IP] as direccion_ip
                      ,[DESCRIPCION] as descripcion
                      ,[USUARIO_ACCION] as usuario
                      ,[FECHA_ADICION] as fecha_firma
                  FROM [SEG].[SEG_AUDITORIA_FIRMA_DOC] WHERE ID_AUDITORIA_FIRMA_DOC = @ID";

            var p = new DynamicParameters();
            p.Add("@ID", id_firma);


            return await db.QueryFirstOrDefaultAsync<Firma>(sql.ToString(), p);

        }

        public async Task<IEnumerable<Firma>> getFirmas()
        {
            var db = dbConnection();

            var sql = @"SELECT [ID_AUDITORIA_FIRMA_DOC] as id_firma
                      ,[IDENTIFICACION_CLIENTE] as identificacion_cliente
                      ,[NOMBRE_ARCHIVO] as nombre_archivo
                      ,[DIRECCION_IP] as direccion_ip
                      ,[DESCRIPCION] as descripcion
                      ,[USUARIO_ACCION] as usuario
                      ,[FECHA_ADICION] as fecha_firma
                  FROM [SEG].[SEG_AUDITORIA_FIRMA_DOC]";

            return await db.QueryAsync<Firma>(sql.ToString());
        }

        public async Task<bool> insertarFirmaDigital(Firma firma)
        {
            var result = 0;
            var db = dbConnection();

            var sql = @"[SEG].[INSERTAR_AUDITORIA_FIRMA_DOC]";

            var p = new DynamicParameters();
            p.Add("@IDENTIFICACION_CLIENTE", firma.identificacion_cliente);
            p.Add("@NOMBRE_ARCHIVO", firma.nombre_archivo);
            p.Add("@DIRECCION_IP", firma.direccion_ip);
            p.Add("@DESCRIPCION", firma.descripcion);
            p.Add("@USUARIO_ACCION", firma.usuario);

            result = await db.ExecuteAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);

            return result > 0;

        }
    }
}
