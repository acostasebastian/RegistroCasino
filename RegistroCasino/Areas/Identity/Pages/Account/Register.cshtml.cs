// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using RegistroCasino.Areas.Admin.Controllers;
using RegistroCasino.Models;
using RegistroCasino.Utilities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RegistroCasino.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Administrador,Secretaria")]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<CajeroUser> _signInManager;
        private readonly UserManager<CajeroUser> _userManager;
        private readonly IUserStore<CajeroUser> _userStore;
        private readonly IUserEmailStore<CajeroUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<CajeroUser> userManager,
            IUserStore<CajeroUser> userStore,
            SignInManager<CajeroUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "El Email es obligatorio")]
            [EmailAddress(ErrorMessage = "El campo Email no es una dirección de correo electrónico válida.")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required(ErrorMessage = "La Contraseña es obligatoria")]
            [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} carácteres y como máximo {1} carácteres.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirmar Contraseña")]
            [Compare("Password", ErrorMessage = "Las contraseñas no son iguales.")]
            public string ConfirmPassword { get; set; }

            //Campos personalizados de cajeros 
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
                get { return Nombre + ", " + Apellido; }
            }

            public string PhoneNumber { get; set; }

        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                //agregar campos personalizados a la base de datos
                user.Nombre = Input.Nombre;
                user.Apellido = Input.Apellido;
                user.DNI = Input.DNI;
                user.PorcentajeComision = Input.PorcentajeComision;
                user.FichasCargar = Input.FichasCargar;
                user.PhoneNumber = Input.PhoneNumber;
                user.Nombre = Input.Nombre;
                


                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                string message = "";
                try
                {
                    var result = await _userManager.CreateAsync(user, Input.Password);

                   

                    if (result.Succeeded)
                    {
                        //user.UserName = user.NombreCompleto;

                        //Aquí validamos si los roles existen sino se crean
                        if (!await _roleManager.RoleExistsAsync(CNT.Administrador))
                        {
                            await _roleManager.CreateAsync(new IdentityRole(CNT.Administrador));
                            await _roleManager.CreateAsync(new IdentityRole(CNT.Secretaria));
                            await _roleManager.CreateAsync(new IdentityRole(CNT.Cajero));
                        }

                        //Obtenemos el rol seleccionado
                        string rol = Request.Form["radUsuarioRole"].ToString();

                        //Validamos si el rol seleccionado es Admin y si lo es lo agregamos
                        if (rol == CNT.Administrador)
                        {
                            await _userManager.AddToRoleAsync(user, CNT.Administrador);
                        }
                        else
                        {
                            if (rol == CNT.Secretaria)
                            {
                                await _userManager.AddToRoleAsync(user, CNT.Secretaria);
                            }
                            else
                            {
                                await _userManager.AddToRoleAsync(user, CNT.Cajero);
                            }
                        }



                        _logger.LogInformation("User created a new account with password.");

                        //var userId = await _userManager.GetUserIdAsync(user);
                        //var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        //code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        //var callbackUrl = Url.Page(
                        //    "/Account/ConfirmEmail",
                        //    pageHandler: null,
                        //    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        //    protocol: Request.Scheme);

                        //await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);

                            // returnUrl = Url.Content("~/Areas/Cajeros/Views/Index.cshtml");// "/Areas/Cajeros/Views/Index.cshtml";

                            //return LocalRedirect(returnUrl);

                            return RedirectToAction("Index", "Home", new { area = "Cajero" });

                            //return RedirectToRoute(nameof(CajerosController) + nameof(AccountController.Login));
                        }
                    }
                    foreach (var error in result.Errors)
                    {
                       

                        if (error.Description.Contains("least one non alphanumeric character"))
                        {
                            message = "Las contraseñas deben tener al menos un carácter no alfanumérico.";
                        }

                        else if (error.Description.Contains("least one lowercase ('a'-'z')"))
                        {
                            message = "Las contraseñas deben tener al menos una minúscula('a' - 'z').";
                        }

                        else if (error.Description.Contains("least one uppercase ('A'-'Z')."))
                        {
                            message = "Las contraseñas deben tener al menos una mayúscula ('A'-'Z').";
                        }

                        else if (error.Description.Contains("already taken"))
                        {
                            message = "El Email ya se encuentra en uso.";
                        }
                       

                        else
                        {
                            message = error.Description;
                        }
                        ModelState.AddModelError(string.Empty, message);
                    }
                }
                catch (Exception ex)
                {


                    if (ex.InnerException != null &&
                       ex.InnerException != null &&
                       ex.InnerException.Message.Contains("IX_Cajeros_DNI"))
                    {
                        message = "El DNI ingresado ya se encuentra registrado.";
                    }
                    else
                    {
                        message = "Contacte con el administrador >> Error: " + ex.Message;
                    }
                  
                    ModelState.AddModelError(string.Empty, message);
                }
               

                
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private CajeroUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<CajeroUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(CajeroUser)}'. " +
                    $"Ensure that '{nameof(CajeroUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<CajeroUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<CajeroUser>)_userStore;
        }
    }
}
