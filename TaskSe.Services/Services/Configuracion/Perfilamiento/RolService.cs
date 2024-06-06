using TaskSe.Data.Connection;
using TaskSe.Data.Repositories.Repositories.Auditoria;
using TaskSe.Data.Repositories.Repositories.Configuracion.Perfilamiento;
using TaskSe.Data.Repositories.Interfaces.Auditoria;
using TaskSe.Data.Repositories.Interfaces.Configuracion.Perfilamiento;
using TaskSe.Model.Models.Configuracion.Perfilamiento;
using TaskSe.Services.Interfaces.Auditoria;
using TaskSe.Services.Interfaces.Configuracion.Perfilamiento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Services.Services.Configuracion.Perfilamiento
{
    public class RolService : IRolService
    {
        private readonly IRolRepository _rolRepository;
        private readonly IAuditoriaService _auditoriaService;
        private readonly IModuloService _moduloService;
        private readonly SqlConfiguration _sqlConfiguration;

        public RolService(SqlConfiguration sqlConfiguration, IModuloService moduloService, IAuditoriaService auditoriaService)
        {
            _sqlConfiguration = sqlConfiguration;
            _rolRepository = new RolRepository(_sqlConfiguration.ConnectionString);
            _moduloService = moduloService;
            _auditoriaService = auditoriaService;
        }

        public async Task<Rol> getRol(string id_rol)
        {
            Rol rol = await _rolRepository.getRol(id_rol);
            rol.modulos = await _moduloService.getModulosRol(id_rol);
            return rol;
        }

        public async Task<IEnumerable<Rol>> getRoles()
        {
            IEnumerable <Rol> roles = await _rolRepository.getRoles();

            foreach(var rol in roles)
            {
                rol.modulos = await _moduloService.getModulosRol(rol.id_rol);
            }

            return roles;
        }

        public IEnumerable<Rol> getRolesSync()
        {
            IEnumerable<Rol> roles = _rolRepository.getRolesSync();

            foreach (var rol in roles)
            {
                rol.modulos = _moduloService.getModulosRolSync(rol.id_rol);
            }

            return roles;
        }

        public async Task<bool> insertarRol(Rol rol, string usuario_accion, string ipAddress)
        {
            bool nuevo_rol = false;

            if (!string.IsNullOrEmpty(rol.id_rol))
            {
                await _moduloService.limpiarAsignacionRolModulo(rol.id_rol);
            }
            else
            {
                nuevo_rol = true;
            }

            string resultado = await _rolRepository.insertarRol(rol, usuario_accion);

            if (!resultado.Equals("") && !resultado.Equals("0"))
            {
                List<Task> tareas = new List<Task>();
                foreach (var modulo in rol.modulos)
                {
                    tareas.Add(_moduloService.insertarAsignacionRolModulo(resultado, modulo.id_modulo, usuario_accion));
                }

                await Task.WhenAll(tareas);

                if (nuevo_rol)
                {
                    await _auditoriaService.mtdAuditoriaEventos("Registro de rol", "Se ha registrado un nuevo rol", ipAddress, usuario_accion, "", resultado);
                }
                else
                {
                    await _auditoriaService.mtdAuditoriaEventos("Actualización de rol", "Se ha actualizado la información del rol", ipAddress, usuario_accion, "", resultado);
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
