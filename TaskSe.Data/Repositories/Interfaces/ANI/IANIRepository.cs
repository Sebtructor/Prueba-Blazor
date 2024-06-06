using TaskSe.Model.Models.ANI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Data.Repositories.Interfaces.ANI
{
    public interface IANIRepository
    {
        public Task<string> insertarAuditoriaConsultaANI(string numero_documento, string response, string ipAccion, string usuarioAccion);

        public Task<bool> actualizarAuditoriaConsultaANI(AniDTO consulta);

        public Task<IEnumerable<AniDTO>> consultarValidacionesANI();
        public Task<AniDTO> consultarRegistroById(string id_consulta);
        public Task<AniDTO> consultarRegistroByNumDoc(string numero_documento);
    }
}
