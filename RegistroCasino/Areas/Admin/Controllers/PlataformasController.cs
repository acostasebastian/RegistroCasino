using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RegistroCasino.DataAccess.Data.Repository.IRepository;
using RegistroCasino.Models;
using System.Net;

namespace RegistroCasino.Areas.Admin.Controllers
{
    [Authorize(Roles = "Administrador,Secretaria")]
    [Area("Admin")]
    public class PlataformasController : Controller
    {

        private readonly IContenedorTrabajo _contenedorTrabajo;

        public PlataformasController(IContenedorTrabajo contenedorTrabajo)
        {
            _contenedorTrabajo = contenedorTrabajo;
        }

        [HttpGet]
        public IActionResult Index()
        {                     
            return View();       


        }

        //[AllowAnonymous]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Plataforma plataforma)
        {
            if (ModelState.IsValid)
            {
                //Logica para guardar en BD
                _contenedorTrabajo.Plataforma.Add(plataforma);
               


                try
                {
                    _contenedorTrabajo.Save();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {

                    if (ex.InnerException != null &&
                       ex.InnerException != null &&
                       ex.InnerException.Message.Contains("IX_Plataformas_URL"))
                    {
                        ModelState.AddModelError(string.Empty, "Esta Plataforma ya existe");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.Message);
                    }
                }
            }

            return View(plataforma);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Plataforma plataforma = new Plataforma();
            plataforma = _contenedorTrabajo.Plataforma.Get(id);
            if (plataforma == null)
            {
                return NotFound();
            }

            return View(plataforma);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Plataforma plataforma)
        {
            if (ModelState.IsValid)
            {
                //Logica para actualizar en BD
                _contenedorTrabajo.Plataforma.Update(plataforma);

                try
                {
                    _contenedorTrabajo.Save();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {

                    if (ex.InnerException != null &&
                      ex.InnerException != null &&
                      ex.InnerException.Message.Contains("IX_Plataformas_URL"))
                    {
                        ModelState.AddModelError(string.Empty, "Esta Plataforma ya existe");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Contacte con el administrador >> Error: " + ex.Message);
                    }
                }

              
            }

            return View(plataforma);
        }



        #region Llamadas a la API
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _contenedorTrabajo.Plataforma.GetAll() });
        }


        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _contenedorTrabajo.Plataforma.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error borrando la plataforma" });
            }

            _contenedorTrabajo.Plataforma.Remove(objFromDb);
            _contenedorTrabajo.Save();
            return Json(new { success = true, message = "Plataforma Borrada Correctamente" });
        }

        #endregion
    }
}
