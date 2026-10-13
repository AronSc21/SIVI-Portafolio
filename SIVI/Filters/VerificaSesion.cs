using SIVI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SIVI.Filters
{
    public class VerificaSesion:ActionFilterAttribute
    {
        private Models.Usuarios usuario;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                usuario = (Models.Usuarios)HttpContext.Current.Session["User"];
                if (usuario == null)
                {
                    if (filterContext.Controller is HomeController == false)
                        filterContext.HttpContext.Response.Redirect("~/Home/Login");
                }
                base.OnActionExecuting(filterContext);
            }
            catch (Exception ex) {
                filterContext.HttpContext.Response.Redirect("~/Home/Login");
            }
        }
    }
}