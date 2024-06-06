using TaskSe.Data.Connection;
using TaskSe.Data.Repositories.Interfaces.Configuracion.Perfilamiento;
using TaskSe.Model.Models.Configuracion.Perfilamiento;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Data.Repositories.Repositories.Configuracion.Perfilamiento
{
    public class ModuloRepository : IModuloRepository
    {
        private string ConnectionString;

        public ModuloRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }
        protected SqlConnection dbConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public async Task<IEnumerable<Modulo>> getModulos()
        {
            var db = dbConnection();

            var sql = @"SELECT [Id_Modulo] as id_modulo
                          ,[Nombre_Modulo] as nombre_modulo
                          ,[Tipo_Modulo] as tipo_modulo
                          ,[Nivel] as nivel
                      FROM [PAR].[MODULO]
                      order by NIVEL";

            var p = new DynamicParameters();

            return await db.QueryAsync<Modulo>(sql.ToString());
        }

        public async Task<IEnumerable<Modulo>> getModulosRol(string id_rol)
        {
            var db = dbConnection();

            var sql = @"SELECT [Id_Modulo] as id_modulo
                        ,[Nombre_Modulo] as nombre_modulo
                        ,[Tipo_Modulo] as tipo_modulo
                        ,[Nivel] as nivel
                        FROM [PAR].[MODULO]
                        where Id_Modulo in
                        (
	                        SELECT [id_modulo]
	                        FROM [PAR].[ROL_MODULO]
	                        where id_rol = @ID
                        )
                        order by NIVEL";

            var p = new DynamicParameters();
            p.Add("@ID", id_rol);

            return await db.QueryAsync<Modulo>(sql.ToString(), p);
        }

        public async Task<IEnumerable<Modulo>> getModulosUsuario(string usuario)
        {
            var db = dbConnection();

            var sql = @"SELECT [Id_Modulo] as id_modulo
                        ,[Nombre_Modulo] as nombre_modulo
                        ,[Tipo_Modulo] as tipo_modulo
                        ,[Nivel] as nivel
                        FROM [PAR].[MODULO]
                        where Id_Modulo in
                        (
	                        SELECT distinct [id_modulo]
                            FROM [PAR].[ROL_MODULO] RM
                            INNER JOIN PAR.ROL R ON R.Id_Rol = RM.id_rol
                            INNER JOIN SEG.SEG_USUARIO U ON U.ID_ROL = R.Id_Rol
                            WHERE U.USUARIO = @ID
                        )
                        order by NIVEL";

            var p = new DynamicParameters();
            p.Add("@ID", usuario);

            return await db.QueryAsync<Modulo>(sql.ToString(), p);
        }

        public async Task<bool> limpiarAsignacionRolModulo(string id_rol)
        {
            var result = 0;
            var db = dbConnection();

            var sql = @"PAR.LIMPIAR_ASIGNACION_ROL_MODULO";

            var p = new DynamicParameters();
            p.Add("@ID_ROL", id_rol);


            result = await db.ExecuteAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);

            return result > 0;
        }

        public async Task<bool> insertarAsignacionRolModulo(string id_rol, string id_modulo, string usuario)
        {
            var result = 0;
            var db = dbConnection();

            var sql = @"PAR.INSERTAR_ASIGNACION_ROL_MODULO";

            var p = new DynamicParameters();
            p.Add("@ID_ROL", id_rol);
            p.Add("@ID_MODULO", id_modulo);
            p.Add("@USUARIO", usuario);

            result = await db.ExecuteAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);

            return result > 0;
        }

        public IEnumerable<Modulo> getModulosSync()
        {
            var db = dbConnection();

            var sql = @"SELECT [Id_Modulo] as id_modulo
                          ,[Nombre_Modulo] as nombre_modulo
                          ,[Tipo_Modulo] as tipo_modulo
                          ,[Nivel] as nivel
                      FROM [PAR].[MODULO]
                      order by NIVEL";

            var p = new DynamicParameters();

            return db.Query<Modulo>(sql.ToString());
        }

        public IEnumerable<Modulo> getModulosRolSync(string id_rol)
        {
            var db = dbConnection();

            var sql = @"SELECT [Id_Modulo] as id_modulo
                        ,[Nombre_Modulo] as nombre_modulo
                        ,[Tipo_Modulo] as tipo_modulo
                        ,[Nivel] as nivel
                        FROM [PAR].[MODULO]
                        where Id_Modulo in
                        (
	                        SELECT [id_modulo]
	                        FROM [PAR].[ROL_MODULO]
	                        where id_rol = @ID
                        )
                        order by NIVEL";

            var p = new DynamicParameters();
            p.Add("@ID", id_rol);

            return db.Query<Modulo>(sql.ToString(), p);
        }
    }
}
