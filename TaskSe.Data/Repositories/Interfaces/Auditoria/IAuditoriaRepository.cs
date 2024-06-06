using TaskSe.Model.Models.Configuracion.Perfilamiento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Data.Repositories.Interfaces.Auditoria
{
    public interface IAuditoriaRepository
    {
        public Task<bool> mtdAuditoriaEnvioCorreo(string EMAIL_DESTINATARIO, string EMAIL_EMISOR, string MENSAJE, string ENVIADO,
               string ERROR, string PANTALLA, string DESCRIPCION, string NUMERO_IDENTIFICACION, string USUARIO);

        public Task<bool> mtdAuditoriaTransaccionalPN(string Accion, string Pantalla, string Descripcion, string cedula, string idUsuario, string ipAddress);
        public Task<bool> mtdAuditoriaTransaccionalPJ(string Accion, string Pantalla, string Descripcion, string cedula, string idUsuario, string ipAddress);

        public Task<bool> mtdAuditoriaEventos(string ACCION, string DESCRIPCION, string IP_ACCION, string USUARIO_ACCION, string ID_USUARIO, string ID_ROL);

        public Task<bool> registrarAuditoriaNavegacion(AuditoriaNavegacion auditoriaNavegacion);
        public Task<bool> registrarAuditoriaDescargaArchivo(AuditoriaDescargaArchivo auditoriaDescargaArchivo);
    }
}
