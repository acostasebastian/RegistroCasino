using Microsoft.EntityFrameworkCore;
using RegistroCasino.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistroCasino.DataAccess.Data.Repository.IRepository
{
    public interface ICajeroRepository : IRepository<Cajeros>
    {

        public Cajeros GetString(string id);

        void BloquearCajero(Cajeros cajeroDesdeBd);
        void DesloquearCajero(Cajeros cajeroDesdeBd);
   
    }
}
