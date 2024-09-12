using RegistroCasino.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistroCasino.DataAccess.Data.Repository.IRepository
{
    public interface IPlataformaRepository : IRepository<Plataforma>
    {
        void Update(Plataforma plataforma);
    }
}
