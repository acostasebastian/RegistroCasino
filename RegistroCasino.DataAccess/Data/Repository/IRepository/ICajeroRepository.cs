using Microsoft.EntityFrameworkCore;
using RegistroCasino.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistroCasino.DataAccess.Data.Repository.IRepository
{
    public interface ICajeroRepository : IRepository<CajeroUser>
    {

        public CajeroUser GetString(string id);

        void BloquearCajero(CajeroUser cajeroDesdeBd);
        void DesloquearCajero(CajeroUser cajeroDesdeBd);

        void Update(CajeroUser cajero);

    }
}
