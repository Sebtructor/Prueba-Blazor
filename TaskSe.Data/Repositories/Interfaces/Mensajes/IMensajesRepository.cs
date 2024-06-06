using TaskSe.Model.Models.Mensajes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Data.Repositories.Interfaces.Mensajes
{
    public interface IMensajesRepository
    {
        public Task<bool> insertarMensajeWpp(Mensaje mensaje);
        public Task<bool> insertarMensajeSms(Mensaje mensaje);
        public Task<IEnumerable<Mensaje>> getMensajes();
    }
}
