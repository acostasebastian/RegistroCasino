using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RegistroCasino.DataAccess.Data.Repository.IRepository;
using RegistroCasino.Models;
using System.Security.Claims;

namespace RegistroCasino.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrador,Secretaria")]
    [Area("Admin")]
    public class CajerosController : Controller
    {
        private readonly IContenedorTrabajo _contenedorTrabajo;

        public CajerosController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            //Opción 1: Obtener todos los usuario
            //return View(_contenedorTrabajo.Cajero.GetAll());

            //Opción 2: Obtener todos los usuarios menos el que esté logueado, para no bloquearse el mismo
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var usuarioActual = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return View(_contenedorTrabajo.Cajero.GetAll(u => u.Id != usuarioActual.Value));
        }


        [HttpGet]
        public IActionResult Edit(string id)
        {
            CajeroUser cajero = new CajeroUser();
            cajero = _contenedorTrabajo.Cajero.GetString(id);
            if (cajero == null)
            {
                return NotFound();
            }

            return View(cajero);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CajeroUser cajero)
        {
            if (ModelState.IsValid)
            {
                //Logica para actualizar en BD
                _contenedorTrabajo.Cajero.Update(cajero);

                try
                {
                    _contenedorTrabajo.Save();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {

                    if (ex.InnerException != null &&
                      ex.InnerException != null &&
                      ex.InnerException.Message.Contains("IX_Cajeros_DNI"))
                    {
                        ModelState.AddModelError(string.Empty, "El DNI ingresado ya se encuentra registrado.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.Message);
                    }
                }


            }

            return View(cajero);
        }



        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll()
        {
                return Json(new { data = _contenedorTrabajo.Cajero.GetAll() });                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                           return Json(new { data = _contenedorTrabajo.Cajero.GetAll() });
        }



        public IActionResult BloquearDesloquearCajero(string id)
        {
            var objFromDb = _contenedorTrabajo.Cajero.GetString(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error bloqueando el cajero." });
            }          

            //en un solo metodo, bloqueo o desbloqueo el usuario segun corresponda

            if (objFromDb.LockoutEnd == null || objFromDb.LockoutEnd < DateTime.Now)
            {
               
                _contenedorTrabajo.Cajero.BloquearCajero(objFromDb);
                _contenedorTrabajo.Save();
                return Json(new { success = true, message = "Cajero Bloqueado Correctamente" });
            }

            else
            {
         
                _contenedorTrabajo.Cajero.DesloquearCajero(objFromDb);
                _contenedorTrabajo.Save();
                return Json(new { success = true, message = "Cajero DesBloqueado Correctamente" });
            }
    
            

        }

        #endregion
    }
}
