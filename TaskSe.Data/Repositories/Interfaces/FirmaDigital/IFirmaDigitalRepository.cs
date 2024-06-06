using TaskSe.Model.Models.FirmaDigital;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskSe.Data.Repositories.Interfaces.FirmaDigital
{
    public interface IFirmaDigitalRepository
    {
        public Task<bool> insertarFirmaDigital(Firma firma);

        public Task<Firma> getFirma(string id_firma);
        public Task<IEnumerable<Firma>> getFirmas();
    }
}
