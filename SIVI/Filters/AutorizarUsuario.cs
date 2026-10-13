using Reexpediciones.Models;
using SIVI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Usuarios = SIVI.Models.Usuarios;

namespace SIVI.Filters
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AutorizarUsuario: AuthorizeAttribute
    {
        private Usuarios Usuario;
        private SISEPEntities db = new SISEPEntities();
        private int idOperacion;
        public AutorizarUsuario(int idOperacion = 0)
        {
            this.idOperacion = idOperacion;
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
        }
    }
}