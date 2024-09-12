using RegistroCasino.Data;
using RegistroCasino.DataAccess.Data.Repository.IRepository;
using RegistroCasino.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegistroCasino.DataAccess.Data.Repository
{
    public class CajeroRepository : Repository<Cajeros>, ICajeroRepository
    {

        private readonly ApplicationDbContext _db;

        public CajeroRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        //Busqueda por string, ID Cajeros por ejemplo (AspNetUsers)
        public Cajeros GetString(string id)
        {
            return dbSet.Find(id);
        }

        public void BloquearCajero(Cajeros cajeroDesdeBd)
        {         

            //le agrego al campo de la tabla AspUseres una fecha hasta la cual estará bloqueado
            //haciendo que el cajero quede bloqueado
            cajeroDesdeBd.LockoutEnd = DateTime.Now.AddYears(1000);
        
        }

        public void DesloquearCajero(Cajeros cajeroDesdeBd)
        {
          
            //le agrego al campo de la tabla AspUseres la fecha actual
            //haciendo que el cajero quede desbloqueado
            cajeroDesdeBd.LockoutEnd = DateTime.Now;
         
        }
      
    }
}
