using TaskSe.Model.DTO;
using TaskSe.Model.DTO.Configuracion.Perfilamiento;
using TaskSe.Model.Models.Configuracion.Perfilamiento;
using TaskSe.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Services.Interfaces.Configuracion.Perfilamiento
{
    public interface IUserService
    {
        public Task<IEnumerable<Usuario>> getUsuarios();
        public Task<ResponseDTO> loginUsuario(UserDTO usuario, string ipAddress);
        public Task<bool> registrarAuditoriaCierreSesion(string id_auditoria, string usuario, string ip, string motivo);
        public Task<bool> insertarUsuario(Usuario usuario, string ipAddress, string usuarioAccion);
        public Task<Usuario> getUsuario(string id_usuario);
        public Task<Usuario> getUsuarioByUser(string usuario);
        public Task<ResponseDTO> gestionarSolicitudPass(string nombreUsuario, string ipAddress);
        public Task<SolicitudContraseña> getSolicitudContraseña(string id_solicitud);
        public Task<ResponseDTO> completarSolicitudContraseña(string id_solicitud, string nuevaPass);
        public Task<ResponseDTO> enviarCodigoOTPLogin(string nombre_usuario, string ipAccion);
        public Task<ResponseDTO> validarCodigoOTP(OTP_DTO otp_code, string nombre_usuario, string ipAccion);
        public Task procesarIngreso(string usuario, string ip_address, string descripcion);
        public Task<Secret2FA> generarTOTP2FA(string usuario, string ip_address);
        public Task<ResponseDTO> habilitarTOTP2FA(Secret2FA secret, string ip_address, string codigo_inicial);
        public Task<ResponseDTO> deshabilitarTOTP2FA(string usuario, string ip_address);
        public Task<ResponseDTO> validarTOTP2FA(string usuario, string ip_address, string codigo);
    }
}
