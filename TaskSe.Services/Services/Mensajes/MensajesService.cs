using TaskSe.Data.Connection;
using TaskSe.Data.Repositories.Repositories.Mensajes;
using TaskSe.Data.Repositories.Interfaces.Mensajes;
using TaskSe.Model.Models;
using TaskSe.Model.Models.Mensajes;
using TaskSe.Services.Interfaces.Mensajes;
using TaskSe.Services.Utilidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Services.Services.Mensajes
{
    public class MensajesService : IMensajesService
    {
        private readonly IMensajesRepository _mensajeRepository;

        public MensajesService(SqlConfiguration sqlConfiguration)
        {
            _mensajeRepository = new MensajesRepository(sqlConfiguration.ConnectionString);
        }
        public async Task<IEnumerable<Mensaje>> getMensajes()
        {
            return await _mensajeRepository.getMensajes();
        }

        public async Task<bool> enviarMensaje(Mensaje mensaje)
        {
            string resultado = "";

            if (mensaje.tipo_mensaje == 1)
            {
                resultado = UtilidadesOTP.enviarSMS(mensaje.celular_destino, mensaje.contenido_mensaje);
                mensaje.estado = resultado;
                await _mensajeRepository.insertarMensajeSms(mensaje);
            }
                
            if (mensaje.tipo_mensaje == 2)
            {
                resultado = UtilidadesOTP.enviarWPP(mensaje.celular_destino, mensaje.contenido_mensaje);
                mensaje.estado = resultado;
                await _mensajeRepository.insertarMensajeWpp(mensaje);
            }


            if (resultado.Equals("OK") || resultado.Equals("Accepted")) return true;
            else return false;
        }
    }
}
