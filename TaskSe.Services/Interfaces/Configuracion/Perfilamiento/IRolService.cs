using TaskSe.Model.Models.Configuracion.Perfilamiento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Services.Interfaces.Configuracion.Perfilamiento
{
    public interface IRolService
    {
        public Task<IEnumerable<Rol>> getRoles();
        public IEnumerable<Rol> getRolesSync();
        public Task<Rol> getRol(string id_rol);
        public Task<bool> insertarRol(Rol rol, string usuario_accion, string ipAddress);
    }
}
