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
    public class PlataformaRepository : Repository<Plataforma>, IPlataformaRepository
    {
        private readonly ApplicationDbContext _db;

        public PlataformaRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(Plataforma plataforma)
        {
            var objDesdeDb = _db.Plataforma.FirstOrDefault(s => s.Id == plataforma.Id);
            objDesdeDb.URL = plataforma.URL;
            objDesdeDb.Descripcion = plataforma.Descripcion;
         
        }
    }
}
