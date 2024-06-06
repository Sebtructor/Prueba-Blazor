using TaskSe.Model.Models.ANI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Services.Interfaces.ANI
{
    public interface IANIService
    {
        public Task<AniDTO> validarIdentidad(string numero_documento, string ipAccion, string usuarioAccion);
        public Task<AniDTO> consultarRegistroById(string id_consulta);
        public Task<AniDTO> consultarRegistroByNumDoc(string numero_documento);
        public Task<IEnumerable<AniDTO>> consultarValidacionesANI();
    }
}
