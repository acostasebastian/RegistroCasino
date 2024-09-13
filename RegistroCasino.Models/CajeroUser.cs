using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RegistroCasino.Models
{
    [Index("DNI", IsUnique = true, Name = "IX_Cajeros_DNI")]
    public class CajeroUser : IdentityUser
    {
        [Required(ErrorMessage = "El Nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El Apellido es obligatorio")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio")]
        public string DNI { get; set; }
      

        [Required(ErrorMessage = "Las fichas a cargar es obligatorio")]
        [Display(Name = "Fichas a cargar")]        
        public int FichasCargar { get; set; }

        [Required(ErrorMessage = "El Porcentaje de Comisión es obligatorio")]
        [Display(Name = "Porcentaje de Comisión")]
        public double PorcentajeComision { get; set; }

        [Display(Name = "Nombre Completo")]
        public string NombreCompleto
        {
            get { return Nombre + " " + Apellido; }
        }
    }
}
