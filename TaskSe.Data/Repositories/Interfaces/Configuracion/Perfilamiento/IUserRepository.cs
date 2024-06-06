using TaskSe.Model.Models.Configuracion.Perfilamiento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Data.Repositories.Interfaces.Configuracion.Perfilamiento
{
    public interface IUserRepository
    {
        public Task<IEnumerable<Usuario>> getUsers();
        public Task<string> registrarAuditoriaLogin(string usuario, string descripcion, string ipAddress);
        public Task<bool> registrarAuditoriaCierreSesion(string id_auditoria, string usuario, string ip, string motivo);
        public Task<string> insertarUsuario(Usuario usuario, string usuario_accion);
        public Task<Usuario> getUsuarioByUser(string usuario);
        public Task<Usuario> getUsuarioById(string id_usuario);
        public Task<string> insertarSolicitudPass(string usuario, string ipAddress);
        public Task<SolicitudContraseña> getSolicitudContraseña(string id_solicitud);
        public Task<bool> completarSolicitudContraseña(string id_solicitud, string nuevaPass);
        public Task<string> cantidadIntentosFallidoPorUsuarioUltimos5Minutos(string nombre_usuario);
        public Task<string> consultarDiasDesdeUltimoCambioContraseña(string nombre_usuario);
        public Task<bool> insertarSecreto(Secret2FA secret, string ip_address);
        public Task<bool> actualizarEstado2FA(string usuario, bool estado, string ip_address);
        public Task<bool> insertarAuditoriaTOTP(string codigo, string usuario, string descripcion, string ip_address);
        public Task<Secret2FA> getSecret(string usuario);
    }
}
