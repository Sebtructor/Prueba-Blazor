using TaskSe.Model.Models.Mensajes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Services.Interfaces.Mensajes
{
    public interface IMensajesService
    {
        public Task<bool> enviarMensaje(Mensaje mensaje);
        public Task<IEnumerable<Mensaje>> getMensajes();
    }
}
