using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Services.Interfaces.Utilidades
{
    public interface IEmailService
    {
        public Task<bool> enviarCorreo(string destinatario, string cc, string asunto, string mensaje, string pantalla, string concepto, string numero_identificacion, List<string> anexos);
        public Task<bool> enviarCorreoVariosDestinatarios(string destinatarios, string cc, string asunto, string mensaje, string pantalla, string concepto, string numero_identificacion, List<string> anexos);
    }
}
