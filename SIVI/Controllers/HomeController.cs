using Microsoft.Ajax.Utilities;
using SIVI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIVI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string usuario, string pass)
        {
            try
            {
                using (SISEPEntities db = new SISEPEntities())
                {
                    var user = (from u in db.Usuarios
                                join p in db.Pers_perm on u.Noempleado equals p.NoEmpleado
                                where u.Noempleado == usuario && u.password == pass && p.Vigente == true
                                select u).FirstOrDefault();
                    if (user == null)
                    {
                        ViewBag.Error = "Usuario o contraseña inválida";
                        return View();
                    }
                    Session["User"] = user;
                    ViewBag.Usuario = user.Pers_perm.Nombre + " " + user.Pers_perm.Apellidos;
                    ViewBag.id = user.Noempleado;
                    ViewBag.idArea = user.Pers_perm.AreaId;
                    ViewBag.usuarioCaptura = user.Noempleado;
                    if (user.Noempleado == "090" || user.Pers_perm.AreaId == 10)
                    {
                        return RedirectToAction("Buscardor", "Comprobacion");
                    }
                    return RedirectToAction("solicitud", "Comprobacion");
                }

            }
            catch (Exception ex) {
                ViewBag.Error = ex.Message;
                return View();
            }
        }
        public void permisos(Usuarios usuario)
        {

        }
    }
}