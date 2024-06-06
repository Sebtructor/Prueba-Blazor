using TaskSe.Data.Connection;
using TaskSe.Data.Repositories.Interfaces.Configuracion.Perfilamiento;
using TaskSe.Data.Repositories.Repositories.Configuracion.Perfilamiento;
using TaskSe.Model.Models.Configuracion.Perfilamiento;
using TaskSe.Services.Interfaces.Configuracion.Perfilamiento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Services.Services.Configuracion.Perfilamiento
{
    public class ModuloService : IModuloService
    {
        private readonly IModuloRepository _moduloRepository;

        public ModuloService(SqlConfiguration sqlConfiguration)
        {
            _moduloRepository = new ModuloRepository(sqlConfiguration.ConnectionString);
        }

        public async Task<IEnumerable<Modulo>> getModulos()
        {
            return await _moduloRepository.getModulos();
        }

        public async Task<IEnumerable<Modulo>> getModulosRol(string id_rol)
        {
            return await _moduloRepository.getModulosRol(id_rol);
        }

        public IEnumerable<Modulo> getModulosRolSync(string id_rol)
        {
            return _moduloRepository.getModulosRolSync(id_rol);
        }

        public IEnumerable<Modulo> getModulosSync()
        {
            return _moduloRepository.getModulosSync();
        }

        public async Task<IEnumerable<Modulo>> getModulosUsuario(string usuario)
        {
            return await _moduloRepository.getModulosUsuario(usuario);
        }

        public async Task<bool> insertarAsignacionRolModulo(string id_rol, string id_modulo, string usuario)
        {
            return await _moduloRepository.insertarAsignacionRolModulo(id_rol, id_modulo, usuario);
        }

        public async Task<bool> limpiarAsignacionRolModulo(string id_rol)
        {
            return await _moduloRepository.limpiarAsignacionRolModulo(id_rol);
        }
    }
}
