using TaskSe.Model.Models.Configuracion.Perfilamiento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Services.Interfaces.Configuracion.Perfilamiento
{
    public interface IModuloService
    {
        public Task<IEnumerable<Modulo>> getModulos();
        public IEnumerable<Modulo> getModulosSync();
        public Task<IEnumerable<Modulo>> getModulosRol(string id_rol);
        public IEnumerable<Modulo> getModulosRolSync(string id_rol);
        public Task<IEnumerable<Modulo>> getModulosUsuario(string usuario);
        public Task<bool> limpiarAsignacionRolModulo(string id_rol);
        public Task<bool> insertarAsignacionRolModulo(string id_rol, string id_modulo,string usuario);
    }
}
