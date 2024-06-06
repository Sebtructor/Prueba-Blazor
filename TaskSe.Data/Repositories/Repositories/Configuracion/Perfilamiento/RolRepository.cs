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
    public class RolRepository : IRolRepository
    {
        private string ConnectionString;

        public RolRepository(string connectionString)
        {
            ConnectionString = connectionString;
        }
        protected SqlConnection dbConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public async Task<Rol> getRol(string id_rol)
        {
            var db = dbConnection();

            var sql = @"SELECT [Id_Rol]
                            ,[Nombre_Rol]
                            ,estado
                        FROM [PAR].[ROL] WHERE id_rol = @ID";

            var p = new DynamicParameters();
            p.Add("@ID", id_rol);



            return await db.QueryFirstOrDefaultAsync<Rol>(sql.ToString(), p);
        }

        public IEnumerable<Rol> getRolesSync()
        {
            var db = dbConnection();

            var sql = @"SELECT [Id_Rol]
                        ,[Nombre_Rol]
                        ,estado
                    FROM [PAR].[ROL]";

            return db.Query<Rol>(sql.ToString());
        }
        public async Task<IEnumerable<Rol>> getRoles()
        {
            var db = dbConnection();

            var sql = @"SELECT [Id_Rol]
                        ,[Nombre_Rol]
                        ,estado
                    FROM [PAR].[ROL]";

            return await db.QueryAsync<Rol>(sql.ToString());
        }

        public async Task<string> insertarRol(Rol rol, string usuario)
        {

            var db = dbConnection();

            var sql = @"PAR.INSERTAR_ROL";

            var p = new DynamicParameters();

            if (!string.IsNullOrEmpty(rol.id_rol))
                p.Add("@Id_Rol", rol.id_rol);
            p.Add("@Nombre_Rol", rol.nombre_rol);
            p.Add("@Usuario", usuario);
            p.Add("@Estado ", rol.estado);

            var result = await db.ExecuteScalarAsync(sql.ToString(), p, commandType: System.Data.CommandType.StoredProcedure);
            return result.ToString();

        }
    }
}
