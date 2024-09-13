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
    public class CajeroRepository : Repository<CajeroUser>, ICajeroRepository
    {

        private readonly ApplicationDbContext _db;

        public CajeroRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }


        //Busqueda por string, ID Cajeros por ejemplo (AspNetUsers)
        public CajeroUser GetString(string id)
        {
            return dbSet.Find(id);
        }

        public void BloquearCajero(CajeroUser cajeroDesdeBd)
        {         

            //le agrego al campo de la tabla AspUseres una fecha hasta la cual estará bloqueado
            //haciendo que el cajero quede bloqueado
            cajeroDesdeBd.LockoutEnd = DateTime.Now.AddYears(1000);
        
        }

        public void DesloquearCajero(CajeroUser cajeroDesdeBd)
        {
          
            //le agrego al campo de la tabla AspUseres la fecha actual
            //haciendo que el cajero quede desbloqueado
            cajeroDesdeBd.LockoutEnd = DateTime.Now;
         
        }

        public void Update(CajeroUser cajero)
        {
            //NO SE PUEDEN CAMBIAR EL CORREO NI LA CONTRASEÑA..
            //LA CONTRASEÑA LO PUEDE HACER EL CAJERO DESDE EL PERFIL, EL CORREO NO ESTÁ HABILITADO
            var objDesdeDb = _db.Cajero.FirstOrDefault(s => s.Id == cajero.Id);           
            objDesdeDb.Nombre = cajero.Nombre;
            objDesdeDb.Apellido = cajero.Apellido;
            objDesdeDb.DNI = cajero.DNI;
            objDesdeDb.PorcentajeComision = cajero.PorcentajeComision;
            objDesdeDb.FichasCargar = cajero.FichasCargar;
            objDesdeDb.PhoneNumber = cajero.PhoneNumber;
           

        }
    }
}
