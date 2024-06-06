using TaskSe.Model.DTO.Configuracion.Perfilamiento;
using TaskSe.Model.Models.Configuracion.Perfilamiento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Data.Repositories.Interfaces.Configuracion.Perfilamiento
{
    public interface IRolRepository
    {
        public Task<string> insertarRol(Rol rol, string usuario);
        public Task<Rol> getRol(string id_rol);
        public Task<IEnumerable<Rol>> getRoles();
        public IEnumerable<Rol> getRolesSync();
    }
}
