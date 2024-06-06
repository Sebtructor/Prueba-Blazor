using TaskSe.Data.Repositories.Interfaces.Par;
using TaskSe.Model.Models.Mensajes;
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
    public class ParRepository : IParRepository
    {
        private string ConnectionString;

        public ParRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected SqlConnection dbConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public async Task<IEnumerable<TipoDocumento>> GetTipoDocumentos()
        {
            var db = dbConnection();

            var sql = @"SELECT [ID_TIPO_DOCUMENTO] 
                          ,[DESCRIPCION_TIPO_DOCUMENTO]
                      FROM [PAR].[TIPO_DOCUMENTO]";

            return await db.QueryAsync<TipoDocumento>(sql.ToString());
        }

        public async Task<IEnumerable<Ciiu>> GetCodigosCiiu()
        {
            var db = dbConnection();

            var sql = @"SELECT [CODIGO]
                        ,[DESCRIPCION]
                        ,[PESOPROFESION] as peso
                        FROM [PAR].[CIIU]";

            return await db.QueryAsync<Ciiu>(sql.ToString());
        }

        public async Task<IEnumerable<Ciudad>> GetCiudades()
        {
            var db = dbConnection();

            var sql = @"SELECT [ID_CIUDAD]
                        ,[NOMBRE] as nombre_ciudad
                        ,[DEPARTAMENTO_ID_DEPARTAMENTO] as id_departamento
                        ,[PESO]
                        FROM [PAR].[CIUDAD] ORDER BY NOMBRE";

            return await db.QueryAsync<Ciudad>(sql.ToString());
        }

        public async Task<IEnumerable<Ciudad>> GetCiudadesByDepartamento(string id_departamento)
        {
            var db = dbConnection();

            var sql = @"SELECT [ID_CIUDAD]
                        ,[NOMBRE] as nombre_ciudad
                        ,[DEPARTAMENTO_ID_DEPARTAMENTO] as id_departamento
                        ,[PESO]
                        FROM [PAR].[CIUDAD] where DEPARTAMENTO_ID_DEPARTAMENTO = @ID_DEPARTAMENTO
                        ORDER BY NOMBRE";

            var p = new DynamicParameters();

            p.Add("@ID_DEPARTAMENTO", id_departamento);

            return await db.QueryAsync<Ciudad>(sql.ToString(), p);
        }

        public async Task<IEnumerable<Departamento>> GetDepartamentos()
        {
            var db = dbConnection();

            var sql = @"SELECT [ID_DEPARTAMENTO]
                    ,[NOMBRE] as nombre_departamento
                    FROM [PAR].[DEPARTAMENTO]
                    ORDER BY NOMBRE";

            return await db.QueryAsync<Departamento>(sql.ToString());
        }

        public async Task<IEnumerable<ClaseVinculacion>> GetClasesVinculacion()
        {
            var db = dbConnection();

            var sql = @"SELECT [ID_CLASE_VINCULACION] as id_clase_vinculacion
                        ,[DESCRIPCION_CLASE_VINCULACION] as descripcion_clase_vinculacion
                        FROM [PAR].[CLASE_VINCULACION]";

            return await db.QueryAsync<ClaseVinculacion>(sql.ToString());
        }

        public async Task<IEnumerable<OrigenRecursos>> GetOrigenesRecursos()
        {
            var db = dbConnection();

            var sql = @"SELECT [ID_ORIGEN_RECURSOS]
                        ,[DESCRIPCION_ORIGEN_RECURSOS]
                        FROM [PAR].[ORIGEN_RECURSOS]";

            return await db.QueryAsync<OrigenRecursos>(sql.ToString());
        }

        public async Task<IEnumerable<Pais>> GetPaises()
        {
            var db = dbConnection();

            var sql = @"SELECT [Id_Pais]
                        ,[Nombre_Pais] as descripcion_pais
                        ,[NivelRiesgo] as nivel_riesgo
                        FROM [PAR].[Pais] ORDER BY Nombre_Pais";

            return await db.QueryAsync<Pais>(sql.ToString());
        }

        public async Task<IEnumerable<Profesion>> GetProfesiones()
        {
            var db = dbConnection();

            var sql = @"SELECT [ID_PROFESION]
                        ,[DESCRIPCION_PROFESION]
                        FROM [PAR].[PROFESION]";

            return await db.QueryAsync<Profesion>(sql.ToString());
        }

        public async Task<IEnumerable<SiNo>> GetRespuestasSiNo()
        {
            var db = dbConnection();

            var sql = @"SELECT [ID_RESPUESTA]
                        ,[DESCRIPCION_RESPUESTA]
                        FROM [PAR].[RESPUESTA_SI_NO]";

            return await db.QueryAsync<SiNo>(sql.ToString());
        }

        public async Task<IEnumerable<Sexo>> GetSexos()
        {
            var db = dbConnection();

            var sql = @"SELECT [ID_SEXO]
                        ,[DESCRIPCION_SEXO]
                        FROM [PAR].[SEXO]";

            return await db.QueryAsync<Sexo>(sql.ToString());
        }

        public async Task<IEnumerable<SituacionLaboral>> GetSituacionesLaborales()
        {
            var db = dbConnection();

            var sql = @"SELECT [Id_SituacionLaboral] as id_situacion_laboral
                        ,[Nombre] as descripcion_situacion_laboral
                        FROM [PAR].[SituacionLaboral]";

            return await db.QueryAsync<SituacionLaboral>(sql.ToString());
        }

        public async Task<IEnumerable<TipoPep>> GetTiposPep()
        {
            var db = dbConnection();

            var sql = @"SELECT [ID_TIPO_PEP]
                            ,[DESCRIPCION_TIPO_PEP]
                            FROM [PAR].[TIPO_PEP]";

            return await db.QueryAsync<TipoPep>(sql.ToString());
        }

        public async Task<IEnumerable<TipoSolicitud>> GetTiposSolicitud()
        {
            var db = dbConnection();

            var sql = @"SELECT [ID_TIPO_SOLICITUD]
                        ,[DESCRIPCION_TIPO_SOLICITUD]
                        FROM [PAR].[TIPO_SOLICITUD]";

            return await db.QueryAsync<TipoSolicitud>(sql.ToString());
        }

        public async Task<IEnumerable<TipoEmpresa>> GetTiposEmpresas()
        {
            var db = dbConnection();

            var sql = @"SELECT [ID_TIPO_EMPRESA]
                          ,[DESCRIPCION] as descripcion_tipo_empresa
                      FROM [PAR].[TIPO_EMPRESA]";

            return await db.QueryAsync<TipoEmpresa>(sql.ToString());
        }

        public async Task<IEnumerable<TipoRepresentante>> GetTiposRepresentantesLegales()
        {
            var db = dbConnection();

            var sql = @"SELECT [ID_TIPO_REPRESENTANTE]
                          ,[DESCRIPCION_TIPO_REPRESENTANTE]
                      FROM [PAR].[TIPO_REPRESENTANTE]";

            return await db.QueryAsync<TipoRepresentante>(sql.ToString());
        }
    }
}
