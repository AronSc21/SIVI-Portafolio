using Antlr.Runtime.Misc;
using Microsoft.Ajax.Utilities;
using Microsoft.Office.Interop.Excel;
using Microsoft.Reporting.WinForms;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Oware.Util;
using Reexpediciones.Models.Almacen;
using Reexpediciones.Models.Estadistica;
using SIVI.Models;
using SIVI.Models.Comprobacion;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;

namespace SIVI.Controllers
{
    public class ComprobacionController : Controller
    {
        public class ComprobacionVM
        {
            public ComisionGeneralVM comision;
            public MontoMaximoVM monto;
            public rptEmpleado empleado;
            public List<DatosXML> datos;
            public VehiculosVM vehiculo;
        }
        // GET: Comprobacion
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult solicitud(int idComision=0)
        {
            var user = (Usuarios)Session["User"];
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }

            ViewBag.idComision = idComision;
            //generarZip(61);
            //GenerarReporte(72);
            ViewBag.Usuario = user.Pers_perm.Nombre + " " + user.Pers_perm.Apellidos;
            ViewBag.id = user.Noempleado;
            ViewBag.idArea = user.Pers_perm.AreaId;
            ViewBag.usuarioCaptura = user.Noempleado;
            return View();
        }
        public ActionResult Comprobacion()
        {
            //GenerarSolicitud(2);
            var user = (Usuarios)Session["User"];
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            ViewBag.Usuario = user.Pers_perm.Nombre + " " + user.Pers_perm.Apellidos;
            ViewBag.id = user.Noempleado;
            ViewBag.idArea = user.Pers_perm.AreaId;
            ViewBag.idComision = 0;
            ViewBag.usuarioCaptura = user.Noempleado;
            return View();
        }
        public ActionResult Buscardor()
        {
            var user = (Usuarios)Session["User"];
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            ViewBag.Usuario = user.Pers_perm.Nombre + " " + user.Pers_perm.Apellidos;
            ViewBag.id = user.Noempleado;
            ViewBag.idArea = user.Pers_perm.AreaId;
            ViewBag.idComision = 0;
            ViewBag.usuarioCaptura = user.Noempleado;
            return View();
        }

        public ActionResult AnexoCombustible()
        {
            var user = (Usuarios)Session["User"];
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            ViewBag.Usuario = user.Pers_perm.Nombre + " " + user.Pers_perm.Apellidos;
            ViewBag.id = user.Noempleado;
            ViewBag.idArea = user.Pers_perm.AreaId;
            ViewBag.idComision = 0;
            ViewBag.usuarioCaptura = user.Noempleado;
            return View();
        }
        public ActionResult InfoCarpeta()
        {
            var user = (Usuarios)Session["User"];
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            ViewBag.Usuario = user.Pers_perm.Nombre + " " + user.Pers_perm.Apellidos;
            ViewBag.id = user.Noempleado;
            ViewBag.idArea = user.Pers_perm.AreaId;
            ViewBag.idComision = 0;
            ViewBag.usuarioCaptura = user.Noempleado;
            return View();
        }
        public ActionResult cerrar()
        {
            Session["User"] = null;
            return RedirectToAction("Login", "Home");
        }
        public ActionResult BuscarEmpleado(string noEmpleado)
        {
            Resultado res = new Resultado();
            Pers_permVM pers_PermVM = new Pers_permVM();
            try
            {
                using (SISEPEntities db = new SISEPEntities())
                {
                    var emp = (from e in db.Pers_perm
                               where e.NoEmpleado == noEmpleado && e.Vigente == true
                               select new Pers_permVM
                               {
                                   NoEmpleado = e.NoEmpleado,
                                   Nombre = e.Nombre,
                                   Apellidos = e.Apellidos,
                                   AreaId = e.AreaId,
                                   Tipo_Contrato = e.Tipo_Contrato,
                                   Area = e.Areas.Descripcion
                               }).FirstOrDefault();
                    if (emp != null)
                    {
                        var x = GetTabulador(noEmpleado);
                        emp.hospedaje = x.Hospedaje;
                        emp.alimentacion = x.Alimentos;
                    }
                    res.Objeto = emp;
                    res.Correcto = true;
                }
            }
            catch (Exception ex)
            {
                res.Correcto = false;
                res.Excepcion = ex;
                res.Mensaje = ex.Message;
                throw;
            }                     
            return Json(res, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ObtenerSolicitudId(int idComision)
        {
            Resultado res = new Resultado();
            contabilidadEntities db = new contabilidadEntities();
            SISEPEntities dbSISEP = new SISEPEntities();
            ComprobacionVM comprobacion = new ComprobacionVM();            
            try
            {
                ComisionGeneralVM c = (from com in db.ComisionGeneral
                                       where com.id == idComision
                                       select new ComisionGeneralVM
                                       {
                                           id = com.id,
                                           noEmpleado = com.noEmpleado,
                                           noOficio = com.noOficio,
                                           tipoGasto = com.tipoGasto,
                                           objetivoComision = com.objetivoComision,
                                           lugarComision = com.lugarComision,
                                           avion = com.avion,
                                           autobus = com.autobus,
                                           automovil = com.automovil,
                                           noPlacas = com.noPlacas,
                                           fechaInicio = com.fechaInicio,
                                           fechaFinal = com.fechaFinal,
                                           horaSalida = com.horaSalida,
                                           diasComision = com.diasComision,
                                           ctaBancaria = com.ctaBancaria,
                                           fechaPago = com.fechaPago,
                                           noOperacion = com.noOperacion,
                                           principalesAcciones = com.principalesAcciones,
                                           idUsuario = com.idUsuario,
                                           fechaAlta = com.fechaAlta,
                                           actividadesrealizadas = com.actividadesrealizadas,
                                           resultadosObtenidos = com.actividadesrealizadas,
                                           contribuciones = com.contribuciones,
                                           noPoliza = com.noPoliza,
                                           rutaOficio = com.rutaOficio,
                                           fechaOficio = com.fechaOficio,
                                           fechaComprobacion = com.fechaComprobacion,
                                           reintegro = com.reintegro,
                                           reintegrado = com.reintegrado,
                                           jefeComision = com.jefeComision,
                                           observaciones = com.observaciones,
                                           validadoRF = com.validadoRF,
                                           reposicion = com.reposicion,
                                           permitirCorrecion = com.permitirCorrecion
                                       }).FirstOrDefault();
                if (c == null)
                {
                    res.Correcto = false;
                    res.Mensaje = "No hay una comisión registrada con el número de oficio";
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                idComision = c.id;
                MontoMaximoVM m = (from mon in db.MontoMaximo
                                   where mon.idComision == idComision
                                   select new MontoMaximoVM
                                   {
                                       id = mon.id,
                                       idComision = mon.idComision,
                                       hospedaje = mon.hospedaje,
                                       alimentacion = mon.alimentacion,
                                       terrestre = mon.terrestre,
                                       combustible = mon.combustible,
                                       peaje = mon.peaje,
                                       traslados = mon.traslados,
                                       otrosGastos = mon.otrosGastos,
                                       idUsuarioAlta = mon.idUsuarioAlta,
                                       fechaAlta = mon.fechaAlta
                                   }).FirstOrDefault();
                comprobacion.comision = c;
                comprobacion.monto = m;
                res.Objeto = comprobacion;
                res.Correcto = true;
            }
            catch (Exception ex)
            {
                res.Objeto = ex;
                res.Correcto = false;
                res.Mensaje = ex.Message;
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        ///
        public ActionResult ObtenerSolicitud(string noOficio)
        {
            Resultado res = new Resultado();
            contabilidadEntities db = new contabilidadEntities();
            SISEPEntities dbSISEP = new SISEPEntities();
            ComprobacionVM comprobacion = new ComprobacionVM();
            int idComision = 0;
            try
            {
                ComisionGeneralVM c = (from com in db.ComisionGeneral
                                       where com.noOficio == noOficio
                                       select new ComisionGeneralVM
                                       {
                                           id = com.id,
                                           noEmpleado = com.noEmpleado,
                                           noOficio = com.noOficio,
                                           tipoGasto = com.tipoGasto,
                                           objetivoComision = com.objetivoComision,
                                           lugarComision = com.lugarComision,
                                           avion = com.avion,
                                           autobus = com.autobus,
                                           automovil = com.automovil,
                                           noPlacas = com.noPlacas,
                                           fechaInicio = com.fechaInicio,
                                           fechaFinal = com.fechaFinal,
                                           horaSalida = com.horaSalida,
                                           diasComision = com.diasComision,
                                           ctaBancaria = com.ctaBancaria,
                                           fechaPago = com.fechaPago,
                                           noOperacion = com.noOperacion,
                                           principalesAcciones = com.principalesAcciones,
                                           idUsuario = com.idUsuario,
                                           fechaAlta = com.fechaAlta,
                                           actividadesrealizadas = com.actividadesrealizadas,
                                           resultadosObtenidos = com.resultadosObtenidos,
                                           contribuciones = com.contribuciones,
                                           noPoliza = com.noPoliza,
                                           rutaOficio = com.rutaOficio,
                                           fechaOficio = com.fechaOficio,
                                           fechaComprobacion = com.fechaComprobacion,
                                           reintegro = com.reintegro,
                                           reintegrado = com.reintegrado,
                                           jefeComision = com.jefeComision,
                                           observaciones = com.observaciones,
                                           validadoRF = com.validadoRF,
                                           reposicion = com.reposicion,
                                           permitirCorrecion = com.permitirCorrecion
                                       }).FirstOrDefault();
                if (c == null)
                {
                    res.Correcto = false;
                    res.Mensaje = "No hay una comisión registrada con el número de oficio";
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
                idComision = c.id;
                string placa = c.noPlacas;
                VehiculosVM vehiculo = null;
                if (!string.IsNullOrEmpty(placa))
                {
                    vehiculo = (from v in db.vehiculos
                                            where v.placa == placa
                                            select new VehiculosVM
                                            {
                                                id = v.id,
                                                marca = v.marca,
                                                modelo = v.modelo,
                                                tipo = v.tipo,
                                                placa = v.placa,
                                                color = v.color
                                            }).FirstOrDefault();
                }

                MontoMaximoVM m = (from mon in db.MontoMaximo
                                   where mon.idComision == idComision
                                   select new MontoMaximoVM
                                   {
                                       id = mon.id,
                                       idComision = mon.idComision,
                                       hospedaje = mon.hospedaje,
                                       alimentacion = mon.alimentacion,
                                       terrestre = mon.terrestre,
                                       combustible = mon.combustible,
                                       peaje = mon.peaje,
                                       traslados = mon.traslados,
                                       otrosGastos = mon.otrosGastos,
                                       idUsuarioAlta = mon.idUsuarioAlta,
                                       fechaAlta = mon.fechaAlta
                                   }).FirstOrDefault();
                var lstFactura = (from f in db.GastosComprobar
                                  join fac in db.FacturaEncabezado on f.idFactura equals fac.id
                                  where f.idComision == idComision
                                  select new DatosXML
                                  {
                                      idComision = f.idComision,
                                      idTipoGasto = f.idTipoGasto,
                                      idFactura = f.idFactura,
                                      importe = fac.importe,
                                      idTipo = f.idTipo,
                                      RFCEmisor = fac.rfcEmisor,
                                      noFactura = fac.serie,
                                      fechaExpedicion = fac.fechaExpedicion,
                                      razonSocialEmisor = fac.razonSocial,
                                      gasto = f.importe,
                                      id = f.id,
                                      rutaXML = fac.rutaXML,
                                      rutaPDF = fac.rutaPDF,
                                      rutaPDFFirmado = fac.rutaPDFFirmado,
                                      rutaComprobacion = fac.rutaComrpobacion,
                                      clasificadorGasto = f.clasificadorGasto
                                  }).ToList();
                var e = dbSISEP.csp_rpt_DatosEmpleado(c.noEmpleado).ToList();
                comprobacion.comision = c;
                comprobacion.monto = m;
                comprobacion.empleado = GetEmpleados(e)[0];
                comprobacion.datos = lstFactura;
                comprobacion.vehiculo = vehiculo; 
                res.Objeto = comprobacion;
                res.Correcto = true;
            }
            catch (Exception ex) { 
                res.Objeto = ex;
                res.Correcto = false;
                res.Mensaje = ex.Message;
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        ///  /// /// /// /// /// /// /// /// /// //7
        ///  
       

        public ActionResult EliminarFactura(int id)
        {
            Resultado res = new Resultado();
            try
            {
                using(contabilidadEntities db=new contabilidadEntities())
                {
                    var fac = (from f in db.GastosComprobar
                               where f.id == id
                               select f).FirstOrDefault();
                    if( fac != null)
                    {
                        db.GastosComprobar.Remove(fac);
                        db.SaveChanges();
                        res.Correcto = true;
                    }
                }
            }
            catch(Exception ex)
            {
                res.Correcto = false;
                res.Mensaje = ex.Message;
                res.Excepcion = ex;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GuardarNoPoliza(List<DatosXML> ltsfacturas, ComisionGeneral com)
        {
            Resultado res = new Resultado();
            using(contabilidadEntities db=new contabilidadEntities())
            {
                try
                {
                    var comision = (from c in db.ComisionGeneral
                                    where c.id == com.id   
                                    select c).FirstOrDefault();
                    if (comision != null)
                    {
                        comision.noPoliza = com.noPoliza;                        
                    }
                    foreach(var lst in ltsfacturas)
                    {
                        var tmp = (from g in db.GastosComprobar
                                   where g.id == lst.id
                                   select g).FirstOrDefault();
                        if (tmp != null) {
                            tmp.clasificadorGasto = lst.clasificadorGasto;
                        }
                    }
                    db.SaveChanges();
                    res.Correcto = true;
                }
                catch (Exception ex)
                {
                    res.Correcto = false;
                    res.Mensaje = ex.Message;
                    res.Excepcion = ex;
                }
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult GuardarComprobarGasto(ComisionGeneral comision, MontoMaximo monto, int idUsuario, string usuarioCaptura)
        {
            Resultado res = new Resultado();
            int idSolicitud = 0;
            try
            {
                using (contabilidadEntities db = new contabilidadEntities())
                {
                    if (comision.id == 0)//Nuevo
                    {
                        var verif = (from c in db.ComisionGeneral
                                     where c.noOficio == comision.noOficio
                                     select c).FirstOrDefault();
                        if (verif != null)
                        {
                            res.Correcto = false;
                            res.Mensaje = "El número de oficio ya se encuentra dado de alta con otra comisión";
                            return Json(res, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            comision.idUsuario = idUsuario;
                            comision.fechaAlta = DateTime.Now;
                            comision.permitirCorrecion = true;
                            db.ComisionGeneral.Add(comision);
                            monto.idComision = comision.id;
                            monto.idUsuarioAlta = idUsuario;
                            monto.fechaAlta = DateTime.Now;
                            db.MontoMaximo.Add(monto);
                            //db.SaveChanges();
                            //idSolicitud = comision.id;
                            //res.Correcto = true;
                            //res.Objeto = idSolicitud;
                            db.SaveChanges();

                            idSolicitud = comision.id;

                            // ===============================
                            // NUEVO GUARDADO EN SISEP
                            // ===============================

                            //try
                            //{
                            //    GuardarSolicitudSISEP(comision, idUsuario, usuarioCaptura);
                            //}
                            //catch (Exception exSisep)
                            //{
                            //    // Opcional:
                            //    // puedes guardar log si quieres
                            //    // pero NO romper el flujo actual
                            //}

                            res.Correcto = true;
                            res.Objeto = idSolicitud;
                        }
                    }
                    else//Modificar
                    {
                        var com = (from c in db.ComisionGeneral
                                   where c.id == comision.id
                                   select c).FirstOrDefault();
                        if (com == null)
                        {
                            res.Correcto = false;
                            res.Mensaje = "No se encuentra la comisión";
                            return Json(res, JsonRequestBehavior.AllowGet);

                        }
                        else
                        {
                            idSolicitud = comision.id;
                            com.noEmpleado = comision.noEmpleado;
                            com.noOficio = comision.noOficio;
                            com.tipoGasto = comision.tipoGasto;
                            com.objetivoComision = comision.objetivoComision;
                            com.lugarComision = comision.lugarComision;
                            com.avion = comision.avion;
                            com.autobus = comision.autobus;
                            com.automovil = comision.automovil;
                            com.noPlacas = comision.noPlacas;
                            com.fechaInicio = comision.fechaInicio;
                            com.fechaFinal = comision.fechaFinal;
                            com.horaSalida = comision.horaSalida;
                            com.diasComision = comision.diasComision;
                            //com.ctaBancaria = comision.ctaBancaria;
                            //com.fechaPago = comision.fechaPago;
                            com.noOperacion = comision.noOperacion;
                            com.principalesAcciones = comision.principalesAcciones;
                            com.idUsuario = comision.idUsuario;
                            //com.fechaAlta = comision.fechaAlta;
                            //com.actividadesrealizadas = comision.actividadesrealizadas;
                            //com.resultadosObtenidos = comision.actividadesrealizadas;
                            //com.contribuciones = comision.contribuciones;
                            //com.noPoliza = comision.noPoliza;
                            //com.rutaOficio = comision.rutaOficio;
                            com.fechaOficio = comision.fechaOficio;
                            //com.fechaComprobacion = comision.fechaComprobacion;
                            //com.reintegro = comision.reintegro;
                            //com.reintegrado = comision.reintegrado;
                            com.jefeComision = comision.jefeComision;
                            com.observaciones = comision.observaciones;
                            com.validadoRF = comision.validadoRF;
                            com.reposicion = comision.reposicion;
                            var mon = (from m in db.MontoMaximo
                                       where m.id == monto.id
                                       select m).FirstOrDefault();
                            mon.hospedaje = monto.hospedaje;
                            mon.alimentacion = monto.alimentacion;
                            mon.terrestre = monto.terrestre;
                            mon.combustible = monto.combustible;
                            mon.peaje = monto.peaje;
                            mon.traslados = monto.traslados;
                            mon.otrosGastos = monto.otrosGastos;                            
                            db.SaveChanges();
                            res.Correcto = true;
                            res.Objeto = idSolicitud;
                        }
                    }                                                           
                }
            }
            catch (Exception ex)
            {
                res.Correcto = false;
                res.Excepcion = ex;
                res.Mensaje = ex.Message;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
            //return GenerarReporte(idSolicitud);
        }

        private void GuardarSolicitudSISEP(ComisionGeneral comision, int idUsuario, string usuarioCaptura)
        {
            using (SISEPEntities dbSisep = new SISEPEntities())
            {
                // =========================
                // OBTENER EMPLEADO
                // =========================
                var empleado = dbSisep.Pers_perm
                    .FirstOrDefault(x => x.NoEmpleado == comision.noEmpleado);

                if (empleado == null)
                {
                    throw new Exception("No se encontró el empleado en SISEP.");
                }

                // =========================
                // TABLA SOLICITUD
                // =========================
                SIVI.Models.Solicitud solicitud = new SIVI.Models.Solicitud();

                solicitud.NoEmpleado = comision.noEmpleado;

                solicitud.AreaId = Convert.ToByte(empleado.AreaId);

                solicitud.FechaSolicitud = DateTime.Now;

                solicitud.EstatusId = "01";

                solicitud.Observaciones = comision.objetivoComision;

                solicitud.FechaJefe = null;
                solicitud.FechaRH = null;

                solicitud.FechaCaptura = DateTime.Now;

                solicitud.ObservacionesJefe = null;
                solicitud.ObservacionesRH = null;

                solicitud.UsuarioCaptura = usuarioCaptura;

                solicitud.UsuarioJefe = null;
                solicitud.UsuarioRH = null;

                dbSisep.Solicitud.Add(solicitud);

                dbSisep.SaveChanges();

                // =========================
                // TABLA DETALLE SOLICITUD
                // =========================
                DetalleSolicitud detalle = new DetalleSolicitud();

                detalle.IdSolicitud = solicitud.Id;

                detalle.IdPermiso = "01";

                detalle.FechaInicio = comision.fechaInicio.Value;

                detalle.HoraInicio = comision.horaSalida.Value;

                detalle.FechaTermino = comision.fechaFinal.Value;

                detalle.HoraTermino = Convert.ToDateTime("18:00");

                detalle.FolioNumOficio = comision.noOficio;

                detalle.Fecha = comision.fechaOficio;

                dbSisep.DetalleSolicitud.Add(detalle);

                dbSisep.SaveChanges();
            }
        }
        public ActionResult GenerarSolicitud(int idSolicitud) {
            ReportViewer rptEstadistica = new ReportViewer();
            Resultado res = new Resultado();
            string mimeType = "";
            string encoding = "";
            string filenameExtension = "";
            string[] streamids = null;
            Warning[] warnings = null;
            contabilidadEntities db = new contabilidadEntities();
            SISEPEntities dbSisep = new SISEPEntities();
            try
            {

                List<csp_rpt_solicitud_Result> rptSolicitud = db.csp_rpt_solicitud(idSolicitud).ToList();
                List<csp_rpt_DatosEmpleado_Result> rptEmpleado = dbSisep.csp_rpt_DatosEmpleado(rptSolicitud[0].noEmpleado).ToList();
                rptEstadistica.LocalReport.DataSources.Clear();
                rptEstadistica.LocalReport.ReportPath = "";
                rptEstadistica.LocalReport.ReportPath = Server.MapPath("~/Reportes/rptSolicitud.rdlc");
                ReportDataSource DSAc = new ReportDataSource("contabilidadDS", GetSolicitud(rptSolicitud));
                ReportDataSource DSAu = new ReportDataSource("SISEPDS", GetEmpleados(rptEmpleado));
                rptEstadistica.LocalReport.DataSources.Add(DSAc);
                rptEstadistica.LocalReport.DataSources.Add(DSAu);
                rptEstadistica.LocalReport.Refresh();
                //rptEstadistica.DataBind();                +
                byte[] bCedula = null;
                //return Json(rptEstadistica.LocalReport.DataSources, JsonRequestBehavior.AllowGet);
                bCedula = rptEstadistica.LocalReport.Render("PDF", null, out mimeType, out encoding, out filenameExtension, out streamids, out warnings);
                String rutaCedula = "~/Reportes/" + rptSolicitud[0].noOficio.Replace("/", "_") + "_Solicitud.pdf";
                System.IO.File.WriteAllBytes(Server.MapPath(rutaCedula), bCedula);
                res.Objeto = rutaCedula.Replace("~/", "");
                res.Correcto = true;
            }
            catch (Exception ex) {
                res.Correcto = false;
                res.Excepcion = ex;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult InfoComplemento(string ruta)
        {
            Resultado res = new Resultado();
            DatosXML datosXML = new DatosXML();            
            XmlDocument doc = new XmlDocument();
            String[] carpetas = Directory.GetDirectories(ruta);
            int cont = 0;
            
            using (contabilidadEntities db = new contabilidadEntities())
            {
                var x = (from t in db.tmp_formaPago
                         select t).ToList();

                db.tmp_formaPago.RemoveRange(x);
                try
                {
                    db.SaveChanges();
                }
                catch (Exception ex)
                {

                }
                foreach (var carpeta in carpetas)
                {
                    String[] xmls = Directory.GetFiles(carpeta, "*.xml");
                    foreach (var xml in xmls)
                    {
                        try
                        {
                            doc.Load(xml);
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }                        
                        XmlNamespaceManager nsm = new XmlNamespaceManager(doc.NameTable);
                        nsm.AddNamespace("cfdi", @"http://www.sat.gob.mx/cfd/4");
                        nsm.AddNamespace("tfd", @"http://www.sat.gob.mx/TimbreFiscalDigital");
                        nsm.AddNamespace("pago20", @"http://www.sat.gob.mx/Pagos20");
                        XmlNode nodeComprobante = doc.SelectSingleNode("//cfdi:Comprobante", nsm);
                        XmlNode nodeComplemento = nodeComprobante.SelectSingleNode("cfdi:Complemento", nsm);
                        XmlNode nodetfd = nodeComplemento.SelectSingleNode("tfd:TimbreFiscalDigital", nsm);
                        XmlNode nodepago = nodeComplemento.SelectSingleNode("pago20:Pagos", nsm);                        

                        if(nodepago != null)
                        {
                            XmlNode node20 = nodepago.SelectSingleNode("pago20:Pago", nsm);
                            XmlNode nodeRelacionado = node20.SelectSingleNode("pago20:DoctoRelacionado", nsm);
                            string varSerie = nodetfd.Attributes["UUID"].Value;
                            string varFecha = nodeComprobante.Attributes["Fecha"].Value;
                            string varidRelacionado= nodeRelacionado.Attributes["IdDocumento"].Value;
                            tmp_formaPago tmp = new tmp_formaPago();
                            tmp.enc_fechaExpedicion = DateTime.Parse(varFecha);
                            tmp.UUID = varSerie;
                            tmp.enc_UUID= varidRelacionado;
                            db.tmp_formaPago.Add(tmp);
                            db.SaveChanges();
                            cont++;
                        }                        

                    }

                } 
            }            
            res.Objeto = cont;
            res.Correcto = true;
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult InfoCarpeta(string ruta, int Tipo=2)
        {
            Resultado res = new Resultado();
            DatosXML datosXML = new DatosXML();
            String[] xmls = Directory.GetFiles(ruta, "*.xml");
            XmlDocument doc = new XmlDocument();

            Microsoft.Office.Interop.Excel.Application ExApp;
            ExApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook oWBook;
            Microsoft.Office.Interop.Excel._Worksheet oSheet;
            string nomArch = "layout ASF" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".xlsx";
            string rutaOrigen = Server.MapPath("~/Resources/") + "layout ASF.xlsx";
            if(Tipo==2)
                rutaOrigen= Server.MapPath("~/Resources/") + "LAYOUTmETODOpAGO.xlsx";
            string rutaNueva = Server.MapPath("~/Layouts/") + nomArch;
            var Errores = "";
            //try
            //{
                System.IO.File.Copy(rutaOrigen, rutaNueva, true);

                oWBook = ExApp.Workbooks.Open(rutaNueva, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWBook.ActiveSheet;
                int inicio = 16;
            foreach (var xml in xmls)
            {
                String Error = xml;
                try
                {
                    doc.Load(xml);
                }
                catch (Exception ex)
                {
                    Errores += Error + ", ";
                    continue;
                }
                string metodoPago = "";
                string varSerie = "";                
                XmlNamespaceManager nsm = new XmlNamespaceManager(doc.NameTable);
                nsm.AddNamespace("cfdi", @"http://www.sat.gob.mx/cfd/4");
                nsm.AddNamespace("tfd", @"http://www.sat.gob.mx/TimbreFiscalDigital");
                XmlNode nodeComprobante = doc.SelectSingleNode("//cfdi:Comprobante", nsm);
                //datosXML.idTipo = idTipo;
                //datosXML.idComision = idComision;
                if (nodeComprobante.Attributes["Serie"] != null)
                    varSerie = nodeComprobante.Attributes["Serie"].Value + "-";
                if (nodeComprobante.Attributes["Folio"] != null)
                    varSerie += nodeComprobante.Attributes["Folio"].Value;
                datosXML.importe = decimal.Parse(nodeComprobante.Attributes["Total"].Value);
                if (nodeComprobante.Attributes["MetodoPago"] != null)
                    datosXML.metodoPago = nodeComprobante.Attributes["MetodoPago"].Value;
                string varFecha = nodeComprobante.Attributes["Fecha"].Value;
                DateTime fecha = DateTime.Parse(varFecha, CultureInfo.CreateSpecificCulture("es-MX"));
                datosXML.fechaExpedicion = fecha;

                XmlNode nodeReceptor = nodeComprobante.SelectSingleNode("cfdi:Emisor", nsm);
                datosXML.RFCEmisor = nodeReceptor.Attributes["Rfc"].Value;
                datosXML.razonSocialEmisor = nodeReceptor.Attributes["Nombre"].Value;

                XmlNode nodeComplemento = nodeComprobante.SelectSingleNode("cfdi:Complemento", nsm);
                XmlNode nodetfd = nodeComplemento.SelectSingleNode("tfd:TimbreFiscalDigital", nsm);
                //XmlNode nodetfd = nodeComplemento.FirstChild;
                if (nodeComprobante.Attributes["Folio"] == null)
                    varSerie = nodetfd.Attributes["UUID"].Value;

                //string varSerie = nodetfd.Attributes["UUID"].Value;


                oSheet.Cells[inicio, 1] = (inicio - 15).ToString();
                oSheet.Cells[inicio, 2] = datosXML.RFCEmisor;
                oSheet.Cells[inicio, 3] = datosXML.razonSocialEmisor;
                oSheet.Cells[inicio, 4] = "VIATICOS";
                oSheet.Cells[inicio, 5] = datosXML.fechaExpedicion.Year.ToString() + "-" + datosXML.fechaExpedicion.Month.ToString() + "-" + datosXML.fechaExpedicion.Day.ToString();
                oSheet.Cells[inicio, 6] = datosXML.importe.ToString();
                oSheet.Cells[inicio, 7] = "Informe de comisión";
                oSheet.Cells[inicio, 8] = datosXML.importe.ToString();
                oSheet.Cells[inicio, 9] = "";
                oSheet.Cells[inicio, 10] = varSerie;
                oSheet.Cells[inicio, 11] = "";
                if (Tipo == 2)
                {
                    var enc_folio = nodetfd.Attributes["UUID"].Value;
                    oSheet.Cells[inicio, 12] = enc_folio;
                    oSheet.Cells[inicio, 13] = datosXML.metodoPago;
                    var folios = "";
                    var fechas = "";
                    //validar si ya tiene contra parte
                    using (contabilidadEntities db = new contabilidadEntities())
                    {
                        var tmp = (from t in db.tmp_formaPago
                                   where t.enc_UUID == enc_folio
                                   select t).ToList();
                        if (tmp.Count > 0)
                        {
                            foreach(var item in tmp)
                            {
                                fechas = item.enc_fechaExpedicion?.Year.ToString() + "-" + item.enc_fechaExpedicion?.Month + "-" + item.enc_fechaExpedicion?.Day.ToString();
                                folios += item.UUID.ToString() + ", ";
                            }
                            folios = folios.Substring(0, folios.Length - 2);
                            oSheet.Cells[inicio, 15] = fechas;
                            oSheet.Cells[inicio, 14] = folios;
                        }
                    }
                }              
                
                inicio++;
            }
            if (Errores.Length > 0)

                oSheet.Cells[inicio, 1] = "*Los siguientes archivos no pudieron cargarse: " + Errores;
            
            ExApp.Visible = false;
                ExApp.UserControl = true;
                oWBook.Save();
                ExApp.ActiveWorkbook.Close(true, rutaNueva, Type.Missing);
                ExApp.Quit();
                ExApp = null;
                res.Objeto = "Layouts/" + nomArch;
                res.Correcto = true;
            res.Mensaje = Errores.Length > 0 ? "Los siguientes archivos no pudieron cargarse: " + Errores : "";
            //}
            //catch (Exception ex)
            //{
            //    res.Correcto = false;
            //    res.Objeto = ex;
            //    res.Mensaje = ex.Message;
            //}
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult InfoXML(HttpPostedFileBase xml, HttpPostedFileBase PDF, HttpPostedFileBase PDFFirm, int idTipo, int idComision)
        {
            Resultado res = new Resultado();
            DatosXML datosXML = new DatosXML();

            try
            {
                var facturaApi = LeerFacturaDesdeApi(xml);

                string pruebaRFC = facturaApi.rfcEmisor;
                string pruebaUUID = facturaApi.uuid;
                decimal pruebaTotal = facturaApi.total;

                XmlDocument doc = new XmlDocument();
                doc.Load(xml.InputStream);
                XmlNamespaceManager nsm = new XmlNamespaceManager(doc.NameTable);
                nsm.AddNamespace("cfdi", @"http://www.sat.gob.mx/cfd/4");
                nsm.AddNamespace("tfd", @"http://www.sat.gob.mx/TimbreFiscalDigital");
                XmlNode nodeComprobante = doc.SelectSingleNode("//cfdi:Comprobante", nsm);
                datosXML.idTipo = idTipo;
                datosXML.idComision = idComision;
                if (nodeComprobante.Attributes["Serie"] != null)
                    datosXML.noFactura = nodeComprobante.Attributes["Serie"].Value + "-";
                if (nodeComprobante.Attributes["Folio"] != null)
                    datosXML.noFactura += nodeComprobante.Attributes["Folio"].Value;
                datosXML.importe = decimal.Parse(nodeComprobante.Attributes["Total"].Value);
                string varFecha = nodeComprobante.Attributes["Fecha"].Value;
                DateTime fecha = DateTime.Parse(varFecha);
                datosXML.fechaExpedicion = fecha;

                XmlNode nodeReceptor = nodeComprobante.SelectSingleNode("cfdi:Emisor", nsm);
                datosXML.RFCEmisor = nodeReceptor.Attributes["Rfc"].Value;
                datosXML.razonSocialEmisor = nodeReceptor.Attributes["Nombre"].Value;

                XmlNode nodeComplemento = nodeComprobante.SelectSingleNode("cfdi:Complemento", nsm);
                XmlNode nodetfd = nodeComplemento.SelectSingleNode("tfd:TimbreFiscalDigital", nsm);
                //XmlNode nodetfd = nodeComplemento.FirstChild;
                if (nodeComprobante.Attributes["Folio"] == null)
                    datosXML.noFactura = nodetfd.Attributes["UUID"].Value;
                string varSerie = nodetfd.Attributes["UUID"].Value;


                using(contabilidadEntities db=new contabilidadEntities())
                {
                    //var factura = (from f in db.FacturaEncabezado
                    //               where f.folio == varSerie && f.rfcEmisor == datosXML.RFCEmisor
                    //               select f).FirstOrDefault();
                    //if(factura == null)
                    //{
                        var factura = new FacturaEncabezado();
                        factura.razonSocial = datosXML.razonSocialEmisor;
                        factura.rfcEmisor = datosXML.RFCEmisor;
                        factura.folio = varSerie;
                        factura.serie = datosXML.noFactura;
                        factura.fechaExpedicion = datosXML.fechaExpedicion;
                        factura.importe =datosXML.importe;
                    factura.rutaXML = "Facturas/" + idComision.ToString() + "_" + factura.rfcEmisor + "/" + varSerie + ".xml";
                        factura.rutaPDF = "Facturas/" + idComision.ToString() + "_" + factura.rfcEmisor + "/" + varSerie + ".pdf";
                        factura.rutaPDFFirmado = "Facturas/" + idComision.ToString() + "_" + factura.rfcEmisor + "/Firma_" + varSerie + ".pdf";
                        factura.rutaComrpobacion = "Facturas/" + idComision.ToString() + "_" + factura.rfcEmisor + "/Comp_" + varSerie + ".pdf";                        
                        factura.fechaAlta = DateTime.Now;
                        db.FacturaEncabezado.Add(factura);
                        db.SaveChanges();                        
                        GuardarArchivo(xml, "Facturas/" + idComision.ToString() + "_" + factura.rfcEmisor + "/", varSerie + ".xml");
                        GuardarArchivo(PDF, "Facturas/" + idComision.ToString() + "_" + factura.rfcEmisor + "/", varSerie + ".pdf");
                        GuardarArchivo(PDFFirm, "Facturas/" + idComision.ToString() + "_" + factura.rfcEmisor + "/", "Firma_" + varSerie + ".pdf");                        
                    //}
                    datosXML.idFactura = factura.id;
                }
                res.Objeto = datosXML;
                res.Correcto = true;
                
            }
            catch (Exception ex) {
                res.Correcto = false;
                res.Excepcion = ex;
                res.Mensaje = ex.Message;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        private SIVI.Models.Api.FacturaData LeerFacturaDesdeApi(HttpPostedFileBase xml)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(2);

                using (var form = new MultipartFormDataContent())
                {
                    var streamContent =
                        new StreamContent(xml.InputStream);

                    streamContent.Headers.ContentType =
                        new MediaTypeHeaderValue("application/xml");

                    form.Add(
                        streamContent,
                        "xmlFile",
                        xml.FileName);

                    var response =
                        client.PostAsync(
                            "https://localhost:7256/api/invoices/read-xml",
                            form)
                        .Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception(
                            "La API devolvió error: "
                            + response.StatusCode);
                    }

                    string json =
                        response.Content
                        .ReadAsStringAsync()
                        .Result;

                    var resultado =
                        JsonConvert.DeserializeObject<SIVI.Models.Api.FacturaApiRespuesta>(json);

                    if (resultado == null || !resultado.success)
                    {
                        throw new Exception(
                            resultado?.message ??
                            "Error desconocido");
                    }

                    return resultado.data;
                }
            }
        }

        [HttpPost]
        public JsonResult GuardarAnexoCombustible(AnexoCombustible model)
        {
            Resultado res = new Resultado();
            try
            {
                using (contabilidadEntities db = new contabilidadEntities())
                {
                    var existente = db.AnexoCombustible
                                      .FirstOrDefault(x => x.idComision == model.idComision);

                    if (existente == null)
                    {
                        db.AnexoCombustible.Add(model);
                    }
                    else
                    {
                        existente.kmDestino = model.kmDestino;
                        existente.kmRutas = model.kmRutas;
                        existente.precioCombustible = model.precioCombustible;
                        existente.montoCombustible = model.montoCombustible;
                        existente.montoPeaje = model.montoPeaje;
                        existente.numCilindros = model.numCilindros;
                    }

                    db.SaveChanges();

                    res.Correcto = true;
                    res.Mensaje = "Anexo combustible guardado correctamente.";
                }
            }
            catch (Exception ex)
            {
                res.Correcto = false;
                res.Mensaje = ex.Message;
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        ///
        [HttpGet]
        public JsonResult ObtenerAnexoCombustible(int idComision)
        {
            Resultado res = new Resultado();

            try
            {
                using (contabilidadEntities db = new contabilidadEntities())
                {
                    var datos = db.AnexoCombustible
                        .Where(x => x.idComision == idComision)
                        .Select(x => new
                        {
                            kmDestino = x.kmDestino,
                            kmRutas = x.kmRutas,
                            precioCombustible = x.precioCombustible,
                            montoCombustible = x.montoCombustible,
                            montoPeaje = x.montoPeaje,
                            numCilindros = x.numCilindros
                        })
                        .FirstOrDefault();

                    if (datos == null)
                    {
                        res.Correcto = false;
                        res.Mensaje = "Sin información del anexo combustible.";
                        return Json(res, JsonRequestBehavior.AllowGet);
                    }

                    res.Correcto = true;
                    res.Objeto = datos;
                    return Json(res, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                res.Correcto = false;
                res.Mensaje = ex.Message;
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult GuardarComprobacion(List<DatosXML> ltsfacturas, ComisionGeneral Comision)
        {
            Resultado res = new Resultado();
            int idComision = ltsfacturas[0].idComision;
            decimal? reintegro = 0;
            try
            {
                using (contabilidadEntities db = new contabilidadEntities())
                {
                    //var monto = (from m in db.MontoMaximo
                    //             where m.idComision == idComision
                    //             select m).FirstOrDefault();
                    //var montoOtorgado = monto.hospedaje + monto.alimentacion + monto.terrestre + monto.combustible + monto.peaje + monto.traslados + monto.otrosGastos;
                    //var montoComprobado = ltsfacturas.Sum(l => l.gasto);
                    
                    var comision = (from c in db.ComisionGeneral
                                    where c.id == idComision
                                    select c).FirstOrDefault();
                    comision.actividadesrealizadas = Comision.actividadesrealizadas;
                    comision.resultadosObtenidos = Comision.resultadosObtenidos;
                    comision.contribuciones = Comision.contribuciones;
                    comision.fechaComprobacion = Comision.fechaComprobacion;
                    comision.observaciones = Comision.observaciones;
                    comision.reposicion = Comision.reposicion;
                    //comision.reintegro = montoOtorgado - montoComprobado;
                    //Eliminar facturas existentes 
                    var gastosDB = (from g in db.GastosComprobar
                                  where g.idComision == idComision
                                  select g).ToList();
                    if (gastosDB.Count > 0)
                    {
                        db.GastosComprobar.RemoveRange(gastosDB);
                        db.SaveChanges();
                    }
                                
                    string ruta = "Comisiones/" + comision.noOficio.Replace("/", "_") + "/";
                    ValidarRuta(Server.MapPath("~/" + ruta));
                    foreach (var lst in ltsfacturas)
                    {
                        var fact = (from f in db.FacturaEncabezado
                                    where f.id == lst.idFactura
                                    select f).FirstOrDefault();
                        System.IO.FileInfo file = new FileInfo(Server.MapPath("~/") + fact.rutaXML);
                        System.IO.File.Copy(file.FullName, Server.MapPath("~/" + ruta) + fact.folio + comision.noOficio.Replace("/", "_") + ".xml", true);
                        file = new FileInfo(Server.MapPath("~/") + fact.rutaPDF);
                        System.IO.File.Copy(file.FullName, Server.MapPath("~/" + ruta) + fact.folio + comision.noOficio.Replace("/", "_") + ".pdf", true);
                        file = new FileInfo(Server.MapPath("~/") + fact.rutaPDFFirmado);
                        System.IO.File.Copy(file.FullName, Server.MapPath("~/" + ruta) + fact.folio + "Firma_" + comision.noOficio.Replace("/", "_") + ".pdf", true);
                        file = new FileInfo(Server.MapPath("~/") + fact.rutaPDF);
                        System.IO.File.Copy(file.FullName, Server.MapPath("~/" + ruta) + fact.folio + "Comp_" + comision.noOficio.Replace("/", "_") + ".pdf", true);
                        GastosComprobar gastos = new GastosComprobar();
                        gastos.idComision = idComision;
                        gastos.idTipoGasto = lst.idTipoGasto;
                        gastos.idFactura = lst.idFactura;
                        gastos.importe = lst.gasto;
                        gastos.idTipo = lst.idTipo;
                        switch (lst.idTipoGasto)
                        {
                            case 1://HOSPEDAJE
                            case 2://ALIMENTACIÓN
                                gastos.clasificadorGasto = "37501";
                                break;
                            case 3://AVIÓN/AUTOBUS
                            case 4://COMBUSTIBLE
                            case 5://PEAJE
                                gastos.clasificadorGasto = "37201";
                                break;
                            case 6://TRASLADO
                                gastos.clasificadorGasto = "26103";
                                break;
                            case 7://OTROS GASTOS
                                gastos.clasificadorGasto = "37901";
                                break;
                        }
                        reintegro = reintegro + gastos.importe;
                        db.GastosComprobar.Add(gastos);
                    }
                    comision.reintegro = MontoMaximoVM.MontoOtorgado(idComision) - reintegro;
                    comision.permitirCorrecion = false;
                    db.SaveChanges();
                    res.Objeto = comision.id;
                    res.Correcto = true;                        
                }                
            }
            catch (Exception ex)
            {
                res.Correcto = false;
                res.Mensaje = ex.Message;
                res.Excepcion = ex;
            }
            //return GenerarReporte(Comision.id);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ActualizarPermisoCorreccion(int idComision, bool permitirCorrecion)
        {
            try
            {
                using (var db = new contabilidadEntities())
                {
                    var comision = db.ComisionGeneral.FirstOrDefault(c => c.id == idComision);
                    if (comision == null)
                        return Json(new { Correcto = false, Mensaje = "No se encontró la comisión." });

                    comision.permitirCorrecion = permitirCorrecion;
                    db.SaveChanges();

                    return Json(new { Correcto = true });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Correcto = false, Mensaje = ex.Message });
            }
        }

        [HttpGet]
        public JsonResult ObtenerMarcaModelo(string noPlacas)
        {
            try
            {
                using (contabilidadEntities db = new contabilidadEntities())
                {
                    var vehiculo = db.vehiculos
                        .Where(x => x.placa == noPlacas)
                        .Select(x => new
                        {
                            Marca = x.marca,
                            Modelo = x.modelo
                        })
                        .FirstOrDefault();

                    if (vehiculo == null)
                    {
                        return Json(new
                        {
                            Exito = false,
                            Mensaje = "No existe información para esta placa."
                        }, JsonRequestBehavior.AllowGet);
                    }

                    return Json(new
                    {
                        Exito = true,
                        Marca = vehiculo.Marca,
                        Modelo = vehiculo.Modelo
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Exito = false,
                    Mensaje = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public ActionResult ObtenerComisionesHabilitadasPorArea(string noEmpleado)
        {
            Resultado res = new Resultado();
            try
            {
                using (SISEPEntities dbSisep = new SISEPEntities())
                using (contabilidadEntities dbConta = new contabilidadEntities())
                {
                    
                    var empleado = dbSisep.Pers_perm
                        .FirstOrDefault(e => e.NoEmpleado == noEmpleado && e.Vigente == true);

                    if (empleado == null)
                    {
                        
                        return Json(res, JsonRequestBehavior.AllowGet);
                    }

                    var idAreaEmpleado = empleado.AreaId;

                    
                    var comisionesHabilitadas = (from c in dbConta.csp_BusquedaComision()
                                                 where c.idArea == idAreaEmpleado
                                                       && c.permitirCorrecion == true
                                                 select new
                                                 {
                                                     c.idComision,
                                                     c.noOficio,
                                                     c.noEmpleado
                                                 }).ToList();

                    res.Objeto = comisionesHabilitadas;
                    res.Correcto = true;
                }
            }
            catch (Exception ex)
            {
                res.Correcto = false;
                res.Mensaje = ex.Message;
                res.Excepcion = ex;
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }





        ////////////////////////////////////////////////////////////////////////




        public ActionResult ObtenerEmpleados(int idArea, string noEmpleado = null)
        {
            Resultado res = new Resultado();
            try
            {
                using (SISEPEntities db = new SISEPEntities())
                {
                    var personas = (from p in db.Pers_perm
                                    where p.AreaId == idArea && p.Vigente == true
                                    select new Pers_permVM
                                    {
                                        NoEmpleado = p.NoEmpleado,
                                        Nombre = p.Nombre,
                                        Apellidos = p.Apellidos,
                                        ClavePuesto = p.ClavePuesto,
                                        AreaId = p.AreaId,
                                        Tipo_Contrato = p.Tipo_Contrato,
                                        Fecha_Ingreso = p.Fecha_Ingreso,
                                        Telefono = p.Telefono,
                                        Extension = p.Extension,
                                        Vigente = p.Vigente
                                    }).ToList();
                    personas = noEmpleado != null ? personas.Where(a => a.NoEmpleado == noEmpleado).ToList() : personas;
                    res.Objeto = personas;
                    res.Correcto = true;
                }
            }
            catch (Exception ex)
            {
                res.Correcto = false;
                res.Mensaje = ex.Message;
                res.Excepcion = ex;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ObtenerJefeArea(int idArea)
        {
            Resultado res=new Resultado();
            try
            {
                using (SISEPEntities db = new SISEPEntities())
                {
                    var JefeArea = (from j in db.csp_ObtenerJefeArea(idArea)
                                    select new JefeAreaVM
                                    {
                                        NoEmpleado = j.NoEmpleado,
                                        Nombre = j.Nombre
                                    }).ToList();
                    res.Objeto = JefeArea;
                    res.Correcto = true;
                }
            }
            catch (Exception ex) {
                res.Correcto = false;
                res.Mensaje = ex.Message;
                res.Excepcion = ex;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ObtenerVehiculos(int? id = null)
        {
            Resultado res= new Resultado();
            try
            {
                using (contabilidadEntities db = new contabilidadEntities())
                {
                    var vehiculo = (from v in db.vehiculos
                                    select new VehiculosVM
                                    {
                                        id = v.id,
                                        marca = v.marca,
                                        modelo = v.modelo,
                                        tipo = v.tipo,
                                        placa = v.placa,
                                        color = v.color
                                    }).ToList();
                    vehiculo = id != null ? vehiculo.Where(v => v.id == id).ToList() : vehiculo;
                    res.Objeto = vehiculo;
                    res.Correcto = true;
                }
            }
            catch (Exception ex) {
                res.Correcto = false;
                res.Mensaje = ex.Message;
                res.Excepcion = ex;
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ObtenerAreas(int? id=null)
        {
            Resultado res = new Resultado();
            try
            {
                using (SISEPEntities db = new SISEPEntities())
                {
                    var areas = (from a in db.Areas
                                 where a.Vigente == true
                                 select new AreasVM
                                 {
                                     Id = a.Id,
                                     Descripcion = a.Descripcion,
                                     PadreID = a.PadreID,
                                     Vigente = a.Vigente
                                 }).ToList();
                    areas = id != null ? areas.Where(a => a.Id == id).ToList() : areas;
                    res.Objeto = areas;
                    res.Correcto = true;
                }
            }
            catch (Exception ex) { 
                res.Correcto = false;
                res.Mensaje = ex.Message;   
                res.Excepcion = ex;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult BuscarComision(string noOficio = null, DateTime? fechaInicio = null, DateTime? fechaFin = null, int? idArea = null, string noPersonal = null)
        {
            Resultado res=new Resultado();
            if (noPersonal == "null")
            {
                noPersonal = null;
            }
            try
            {
                //using (SISEPEntities dbSisep = new SISEPEntities())
                using (contabilidadEntities db = new contabilidadEntities())
                {
                    var lstComision = (from c in db.csp_BusquedaComision()

                                       select new ReporteVM
                                       {
                                           idComision = c.idComision,
                                           noOficio = c.noOficio,
                                           noEmpleado = c.noEmpleado,
                                           objetivoComision = c.objetivoComision,
                                           lugarComision = c.lugarComision,
                                           fechaFinal = c.fechaFinal,
                                           fechaInicio = c.fechaInicio,
                                           principalesAcciones = c.principalesAcciones,
                                           actividadesrealizadas = c.actividadesrealizadas,
                                           resultadosObtenidos = c.resultadosObtenidos,
                                           contribuciones = c.contribuciones,
                                           Nombre = c.Nombre,
                                           Apellidos = c.Apellidos,
                                           idArea = c.idArea,
                                           Area = c.Area,
                                           noPoliza = c.noPoliza,
                                           rutaOficio = c.rutaOficio,
                                           montoOtorgado = c.montoOtorgado,
                                           montoComprobado = c.montoComprobado,
                                           fechacomprobacion = c.fechacomprobacion,
                                           diferencia= c.reintegro,
                                           permitirCorrecion = c.permitirCorrecion
                                       }).ToList();
                    lstComision = (noOficio != null && noOficio != "") ? lstComision.Where(c => c.noOficio == noOficio).ToList() : lstComision;
                    lstComision = idArea != null ? lstComision.Where(c => c.idArea == idArea).ToList() : lstComision;
                    lstComision = noPersonal != null ? lstComision.Where(c => c.noEmpleado == noPersonal).ToList() : lstComision;
                    if (fechaInicio != null && fechaFin != null)
                        lstComision = lstComision.Where(p => p.fechaInicio >= fechaInicio && p.fechaInicio <= fechaFin).ToList();
                    else
                        if (fechaInicio != null)
                        lstComision = lstComision.Where(p => p.fechaInicio == fechaInicio).ToList();
                    res.Correcto = true;
                    res.Objeto = lstComision;
                }
            }
            catch (Exception ex)
            {
                res.Correcto = false;
                res.Mensaje = ex.Message;
                res.Excepcion = ex;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public void escribirLog(string texto)
        {
            StreamWriter sw = System.IO.File.AppendText(Server.MapPath("~/Layouts/") + "log.txt");
            sw.WriteLine(texto);
            sw.Close();

        }



        [HttpPost]
        public ActionResult GenerarReporte(int idSolicitud)
        {
            Resultado res = new Resultado();
            Microsoft.Office.Interop.Excel.Application ExApp;
            ExApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook oWBook;
            Microsoft.Office.Interop.Excel._Worksheet oSheet;
            string nomArch = "Reporte Gatos" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".xlsx";
            string rutaOrigen = Server.MapPath("~/Resources/") + "oficio.xlsx";
            string rutaNueva = Server.MapPath("~/Layouts/") + nomArch;
            try
            {

                System.IO.File.Copy(rutaOrigen, rutaNueva, true);
                oWBook = ExApp.Workbooks.Open(rutaNueva, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                //Anexo 1
                oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWBook.Sheets[1];
                using (SISEPEntities dbSisep = new SISEPEntities())
                using (contabilidadEntities db = new contabilidadEntities())
                {
                    csp_rpt_solicitud_Result rptSolicitud = db.csp_rpt_solicitud(idSolicitud).FirstOrDefault();
                    csp_rpt_DatosEmpleado_Result rptEmpleado = dbSisep.csp_rpt_DatosEmpleado(rptSolicitud.noEmpleado).FirstOrDefault();
                    csp_rpt_DatosEmpleado_Result rptEmpleadoJefe = dbSisep.csp_rpt_DatosEmpleado(rptSolicitud.jefeComision).FirstOrDefault();
                    //Anexo 1
                    oSheet.Cells[8, 6] = (rptEmpleado.Nombre + " " + rptEmpleado.Apellidos).ToUpper();
                    oSheet.Cells[9, 6] = rptEmpleado.Tipo_Contrato.ToUpper(); 
                    oSheet.Cells[9, 17] = rptSolicitud.noOficio.ToUpper();
                    oSheet.Cells[10, 6] = rptEmpleado.Area.ToUpper();
                    oSheet.Cells[16, 7] = rptSolicitud.objetivoComision.ToUpper();
                    oSheet.Cells[20, 7] = rptSolicitud.lugarComision.ToUpper();
                    oSheet.Cells[23, 7] = rptSolicitud.fechaInicio?.Year.ToString() + "-" + rptSolicitud.fechaInicio?.Month.ToString() + "-" + rptSolicitud.fechaInicio?.Day.ToString();
                    oSheet.Cells[23, 14] = rptSolicitud.fechaFinal?.Year.ToString() + "-" + rptSolicitud.fechaFinal?.Month.ToString() + "-" + rptSolicitud.fechaFinal?.Day.ToString();
                    oSheet.Cells[27, 7] = rptSolicitud.principalesAcciones.ToUpper();
                    oSheet.Cells[37, 7] = "ATENTAMENTE\nXalapa Veracruz a " + rptSolicitud.fechaOficio?.Day.ToString() + " " + mesatexto((int)rptSolicitud.fechaOficio?.Month) + " " + rptSolicitud.fechaOficio?.Year.ToString();
                    oSheet.Cells[42, 7] = rptEmpleadoJefe.Nombre.ToUpper() + " " + rptEmpleadoJefe.Apellidos.ToUpper();
                    oSheet.Cells[43, 7] = rptEmpleadoJefe.NoEmpleado == "214"
    ? "MAGISTRADA PRESIDENTA DEL TRIBUNAL DE JUSTICIA ADMINISTRATIVA DEL ESTADO DE VERACRUZ"
    : rptEmpleadoJefe.Tipo_Contrato.ToUpper() + " DE " + rptEmpleadoJefe.Area.ToUpper() + "\nDEL TRIBUNAL DE JUSTICIA ADMINISTRATIVA DEL ESTADO DE VERACRUZ";
                    //oSheet.Cells[43, 7] = rptEmpleadoJefe.Tipo_Contrato.ToUpper() + " DE " + rptEmpleadoJefe.Area.ToUpper() + "\nDEL TRIBUNAL DE JUSTICIA ADMINISTRATIVA DEL ESTADO DE VERACRUZ";
                    //Anexo 2
                    oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWBook.Worksheets[2];
                    oSheet.Cells[10, 6] = (rptEmpleado.Nombre + " " + rptEmpleado.Apellidos).ToUpper();
                    //oSheet.Cells[10, 18]= rptSolicitud.fechaOficio?.Year.ToString() + "-" + rptSolicitud.fechaOficio?.Month.ToString() + "-" + rptSolicitud.fechaOficio?.Day.ToString();
                    oSheet.Cells[11, 6] = rptEmpleado.Area.ToUpper();
                    oSheet.Cells[11, 18] = rptSolicitud.noOficio.ToUpper();
                    oSheet.Cells[14, 3] = "X";
                    oSheet.Cells[16, 7] = rptSolicitud.objetivoComision.ToUpper();
                    oSheet.Cells[19, 7] = rptSolicitud.lugarComision.ToUpper();
                    oSheet.Cells[21, 9] = rptSolicitud.avion == true ? "X" : "";
                    oSheet.Cells[21, 13] = rptSolicitud.autobus == true ? "X" : "";
                    oSheet.Cells[21, 17] = rptSolicitud.automovil == true ? "X" : "";
                    oSheet.Cells[22, 17] = String.IsNullOrEmpty(rptSolicitud.noPlacas) ? "" : rptSolicitud.noPlacas;
                    oSheet.Cells[23, 7] = rptSolicitud.fechaInicio?.Year.ToString() + "-" + rptSolicitud.fechaInicio?.Month.ToString() + "-" + rptSolicitud.fechaInicio?.Day.ToString();
                    oSheet.Cells[23, 14] = rptSolicitud.horaSalida?.ToString("hh:mm t");
                    oSheet.Cells[23, 20] = rptSolicitud.diasComision.ToString();
                    oSheet.Cells[27, 7] = rptSolicitud.fechaFinal?.Year.ToString() + "-" + rptSolicitud.fechaFinal?.Month.ToString() + "-" + rptSolicitud.fechaFinal?.Day.ToString();
                    oSheet.Cells[23, 14] = rptSolicitud.horaSalida != null ? rptSolicitud.horaSalida?.ToString("hh:mm") : "";
                    oSheet.Cells[23, 20] = rptSolicitud.diasComision.ToString();
                    oSheet.Cells[33, 2] = rptEmpleado.NoEmpleado;
                    oSheet.Cells[33, 5] = (rptEmpleado.Nombre + " " + rptEmpleado.Apellidos).ToUpper();
                    oSheet.Cells[33, 9] = rptSolicitud.hospedaje == null ? "0" : rptSolicitud.hospedaje.ToString();
                    oSheet.Cells[33, 12] = rptSolicitud.alimentacion == null ? "0" : rptSolicitud.alimentacion.ToString();
                    oSheet.Cells[33, 15] = rptSolicitud.terrestre == null ? "0" : rptSolicitud.terrestre.ToString();
                    oSheet.Cells[33, 18] = rptSolicitud.combustible == null ? "0" : rptSolicitud.combustible.ToString();
                    oSheet.Cells[38, 12] = rptSolicitud.peaje == null ? "0" : rptSolicitud.peaje.ToString();
                    oSheet.Cells[38, 15] = rptSolicitud.traslados == null ? "0" : rptSolicitud.traslados.ToString();
                    oSheet.Cells[38, 18] = rptSolicitud.otrosGastos == null ? "0" : rptSolicitud.otrosGastos.ToString();

                    //Validar si ya se cuenta con el desglose de gastos para poder generar los demás anexos
                    var lstFactura = (from f in db.GastosComprobar
                                      join fac in db.FacturaEncabezado on f.idFactura equals fac.id
                                      where f.idComision == idSolicitud
                                      select new DatosXML
                                      {
                                          idComision = f.idComision,
                                          idTipoGasto = f.idTipoGasto,
                                          idFactura = f.idFactura,
                                          importe = f.importe,
                                          idTipo = f.idTipo,
                                          RFCEmisor = fac.rfcEmisor,
                                          noFactura = fac.serie,
                                          fechaExpedicion = fac.fechaExpedicion,
                                          razonSocialEmisor = fac.razonSocial,
                                          gasto = fac.importe,
                                          id = f.id
                                      }).ToList();
                    if (lstFactura != null && lstFactura.Count > 0)
                    {
                        //Anexo 3
                        oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWBook.Worksheets[3];
                        oSheet.Cells[6, 12] = rptSolicitud.fechaComprobacion?.Day.ToString();
                        oSheet.Cells[6, 13] = rptSolicitud.fechaComprobacion?.Month.ToString();
                        oSheet.Cells[6, 14] = rptSolicitud.fechaComprobacion?.Year.ToString();
                        oSheet.Cells[8, 6] = (rptEmpleado.Nombre + " " + rptEmpleado.Apellidos).ToUpper();                        
                        oSheet.Cells[9, 6] = rptEmpleado.Area.ToUpper();
                        oSheet.Cells[10, 6] = rptSolicitud.noOficio.ToUpper();
                        oSheet.Cells[10, 12] = rptSolicitud.fechaOficio?.Year.ToString() + "-" + rptSolicitud.fechaOficio?.Month.ToString() + "-" + rptSolicitud.fechaOficio?.Day.ToString();
                        oSheet.Cells[12, 6] = rptSolicitud.objetivoComision.ToUpper();
                        oSheet.Cells[15, 6] = rptSolicitud.lugarComision.ToUpper();
                        oSheet.Cells[17, 6] = rptSolicitud.fechaInicio?.Year.ToString() + "-" + rptSolicitud.fechaInicio?.Month.ToString() + "-" + rptSolicitud.fechaInicio?.Day.ToString();
                        oSheet.Cells[18, 6] = rptSolicitud.fechaFinal?.Year.ToString() + "-" + rptSolicitud.fechaFinal?.Month.ToString() + "-" + rptSolicitud.fechaFinal?.Day.ToString();
                        oSheet.Cells[19, 6] = rptSolicitud.principalesAcciones.ToUpper();
                        
                        int iniciorow = 0;

                        //Desglose
                        var tipos = lstFactura.OrderByDescending(p => p.idTipoGasto).Select(p => p.idTipoGasto).ToList();
                        tipos = tipos.Distinct().ToList();
                        int salto = 0;
                        foreach(var tipo in tipos)
                        {
                            iniciorow = 23 + ((int)tipo * 4);
                            int i = 0;
                            foreach (var f in lstFactura.Where(p=>p.idTipoGasto==tipo))
                            {
                                if (i >= 1)
                                {
                                    oSheet.Rows[iniciorow + i].Insert();
                                    salto++;
                                }                                    
                                oSheet.Cells[iniciorow + i, 1] = (i + 1).ToString();
                                oSheet.Range[oSheet.Cells[iniciorow + i, 2], oSheet.Cells[iniciorow + i, 3]].Merge();
                                oSheet.Cells[iniciorow + i, 2] = f.RFCEmisor;                                
                                oSheet.Cells[iniciorow + i, 4] = f.noFactura;
                                oSheet.Range[oSheet.Cells[iniciorow + i, 5], oSheet.Cells[iniciorow + i, 7]].Merge();
                                oSheet.Cells[iniciorow + i, 5] = f.fechaExpedicion.Year.ToString() + "-" + f.fechaExpedicion.Month.ToString() + "-" + f.fechaExpedicion.Day.ToString();
                                oSheet.Range[oSheet.Cells[iniciorow + i, 8], oSheet.Cells[iniciorow + i, 13]].Merge();
                                oSheet.Cells[iniciorow + i, 8] = f.razonSocialEmisor;
                                oSheet.Cells[iniciorow + i, 14] = f.importe.ToString();
                                i++;
                            }
                        }
                        if (rptSolicitud.reposicion == true)
                        {
                            oSheet.Cells[57 + salto, 1] = "REPOSICIÓN AL COMISIONADO";
                            oSheet.Cells[22, 10] = "0";
                        }
                        if (!string.IsNullOrEmpty(rptSolicitud.observaciones))
                        {
                            oSheet.Cells[59 + salto, 1] = "BAJO PROTESTA DE DECIR VERDAD QUE: " + rptSolicitud.observaciones.ToUpper();
                        }


                        //Anexo 4
                        oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWBook.Worksheets[4];
                        oSheet.Cells[6, 12] = rptSolicitud.fechaComprobacion?.Day.ToString();
                        oSheet.Cells[6, 13] = rptSolicitud.fechaComprobacion?.Month.ToString();
                        oSheet.Cells[6, 14] = rptSolicitud.fechaComprobacion?.Year.ToString();
                        oSheet.Cells[8, 6] = (rptEmpleado.Nombre + " " + rptEmpleado.Apellidos).ToUpper();
                        oSheet.Cells[9, 6] = rptEmpleado.Area.ToUpper();
                        oSheet.Cells[10, 6] = rptSolicitud.noOficio.ToUpper();
                        oSheet.Cells[12, 6] = rptSolicitud.actividadesrealizadas.ToUpper();
                        oSheet.Cells[16, 6] = rptSolicitud.resultadosObtenidos.ToUpper();
                        oSheet.Cells[20, 6] = rptSolicitud.contribuciones.ToUpper();
                        oSheet.Cells[32, 7] = rptEmpleadoJefe.Nombre.ToUpper() + " " + rptEmpleadoJefe.Apellidos.ToUpper() + "\n" + rptEmpleadoJefe.Tipo_Contrato.ToUpper() + " DE " + rptEmpleadoJefe.Area.ToUpper() + "\nDEL TRIBUNAL DE JUSTICIA ADMINISTRATIVA DEL ESTADO DE VERACRUZ" + "\n" + "( 9 )";
                    }



                    ExApp.Visible = false;
                    ExApp.UserControl = true;
                    oWBook.Save();
                    ExApp.ActiveWorkbook.Close(true, rutaNueva, Type.Missing);
                    ExApp.Quit();
                    ExApp = null;
                    Process[] excelProcesses = Process.GetProcessesByName("excel");
                    foreach (Process p in excelProcesses)
                    {
                        if (string.IsNullOrEmpty(p.MainWindowTitle)) // use MainWindowTitle to distinguish this excel process with other excel processes 
                        {
                            p.Kill();
                        }
                    }
                    res.Objeto = "Layouts/" + nomArch;
                    res.Correcto = true;
                }
            }
            catch (Exception ex)
            {
                res.Correcto = false;
                res.Mensaje = ex.Message;
                res.Excepcion = ex;
            }            


            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ExportarExcel(List<ReporteVM> lst)
        {
            Resultado res = new Resultado();
           
                escribirLog("inicio " + DateTime.Now.ToString("ddMMyyyyhhmmss"));
                Microsoft.Office.Interop.Excel.Application ExApp;
                ExApp = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel._Workbook oWBook;
                Microsoft.Office.Interop.Excel._Worksheet oSheet;
                string nomArch = "layout ASF" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".xlsx";
                string rutaOrigen = Server.MapPath("~/Resources/") + "layout ASF.xlsx";
                string rutaNueva = Server.MapPath("~/Layouts/") + nomArch;
                escribirLog("variables " + DateTime.Now.ToString("ddMMyyyyhhmmss"));
            try
            {
                System.IO.File.Copy(rutaOrigen, rutaNueva, true);
                escribirLog("copiado " + DateTime.Now.ToString("ddMMyyyyhhmmss"));

                oWBook = ExApp.Workbooks.Open(rutaNueva, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                escribirLog("abierto " + DateTime.Now.ToString("ddMMyyyyhhmmss"));
                oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWBook.ActiveSheet;
                int inicio = 16;
                using (contabilidadEntities db = new contabilidadEntities())
                {
                    foreach (var fact in lst)
                    {
                        var gastos = db.csp_Datos_ASF(fact.idComision).ToList();
                        foreach (var gasto in gastos)
                        {
                            oSheet.Cells[inicio, 1] = (inicio - 15).ToString();
                            oSheet.Cells[inicio, 2] = gasto.rfcEmisor;
                            oSheet.Cells[inicio, 3] = gasto.razonSocial;
                            oSheet.Cells[inicio, 4] = gasto.descripcion;
                            oSheet.Cells[inicio, 5] = gasto.fechaAdqui.Year.ToString() + "-" + gasto.fechaAdqui.Month.ToString() + "-" + gasto.fechaAdqui.Day.ToString();
                            oSheet.Cells[inicio, 6] = gasto.montoPagado.ToString();
                            oSheet.Cells[inicio, 7] = gasto.entregable;
                            oSheet.Cells[inicio, 8] = gasto.montoParticipaciones.ToString();
                            oSheet.Cells[inicio, 9] = gasto.clasificadorGasto;
                            oSheet.Cells[inicio, 10] = gasto.serie;
                            oSheet.Cells[inicio, 11] = gasto.noPoliza;
                            inicio++;
                        }
                    }
                }
                escribirLog("ciclo " + DateTime.Now.ToString("ddMMyyyyhhmmss"));
                ExApp.Visible = false;
                ExApp.UserControl = true;
                oWBook.Save();
                ExApp.ActiveWorkbook.Close(true, rutaNueva, Type.Missing);
                ExApp.Quit();
                ExApp = null;
                res.Objeto = "Layouts/" + nomArch;
                res.Correcto = true;
                escribirLog("final " + DateTime.Now.ToString("ddMMyyyyhhmmss"));
            }
            catch(Exception ex)
            {
                escribirLog("error: " + ex.Message);
                res.Correcto = false;
                res.Excepcion = ex;
                res.Mensaje = ex.Message;
            }            
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GenerarReporteCombustible(int idComision)
        {
            Resultado res = new Resultado();
            Microsoft.Office.Interop.Excel.Application ExApp;
            ExApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook oWBook;
            Microsoft.Office.Interop.Excel._Worksheet oSheet;

            string nomArch = "AnexoCombustible_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
            string rutaOrigen = Server.MapPath("~/Resources/") + "AnexoCombustible.xlsx";
            string rutaNueva = Server.MapPath("~/Layouts/") + nomArch;

            try
            {
                System.IO.File.Copy(rutaOrigen, rutaNueva, true);

                oWBook = ExApp.Workbooks.Open(rutaNueva);
                oSheet = (Microsoft.Office.Interop.Excel._Worksheet)oWBook.Sheets[1];

                using (contabilidadEntities db = new contabilidadEntities())
                using (SISEPEntities dbSisep = new SISEPEntities())
                {
                    var com = db.ComisionGeneral.Where(x => x.id == idComision).FirstOrDefault();
                    var empleado = dbSisep.csp_rpt_DatosEmpleado(com.noEmpleado).FirstOrDefault();
                    var vehiculo = db.vehiculos
                                     .Where(v => v.placa == com.noPlacas)
                                     .FirstOrDefault();

                    var anexo = db.AnexoCombustible.Where(a => a.idComision == idComision).FirstOrDefault();


                    // ======================================================
                    //     ⚠️ NUEVO: FECHA DE OFICIO EN L6 / M6 / N6
                    // ======================================================
                    DateTime fechaOficio = com.fechaOficio ?? DateTime.Now;

                    oSheet.Cells[6, 12] = fechaOficio.Day;   // L6 → Día
                    oSheet.Cells[6, 13] = fechaOficio.Month; // M6 → Mes
                    oSheet.Cells[6, 14] = fechaOficio.Year;  // N6 → Año
                                                             // ======================================================


                    oSheet.Cells[8, 6] = (empleado.Nombre + " " + empleado.Apellidos).ToUpper();
                    oSheet.Cells[10, 6] = empleado.Area.ToUpper();
                    oSheet.Cells[12, 6] = com.noOficio.ToUpper();
                    oSheet.Cells[14, 6] = com.objetivoComision.ToUpper();
                    oSheet.Cells[19, 6] = com.lugarComision.ToUpper();
                    oSheet.Cells[21, 6] = com.diasComision.ToString();

                    oSheet.Cells[23, 6] = vehiculo == null
                        ? ""
                        : (vehiculo.marca + " " + vehiculo.modelo).ToUpper();

                    oSheet.Cells[25, 6] = anexo?.kmDestino;
                    oSheet.Cells[27, 6] = anexo?.precioCombustible;
                    oSheet.Cells[29, 6] = anexo?.montoCombustible;

                    oSheet.Cells[21, 13] = anexo?.numCilindros;
                    oSheet.Cells[25, 13] = anexo?.kmRutas;

                    oSheet.Cells[29, 13] = anexo?.montoPeaje;

                    ExApp.Visible = false;
                    ExApp.UserControl = true;
                    oWBook.Save();

                    ExApp.ActiveWorkbook.Close(true, rutaNueva);
                    ExApp.Quit();
                    ExApp = null;

                    var excelProcesses = Process.GetProcessesByName("excel");
                    foreach (Process p in excelProcesses)
                    {
                        if (string.IsNullOrEmpty(p.MainWindowTitle))
                            p.Kill();
                    }

                    res.Objeto = "Layouts/" + nomArch;
                    res.Correcto = true;
                }
            }
            catch (Exception ex)
            {
                res.Correcto = false;
                res.Mensaje = ex.Message;
                res.Excepcion = ex;
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult AgregarAnexos(int idComision, HttpPostedFileBase archivo)
        {
            Resultado res = new Resultado();            
            if (Request.Files.Count > 0)
            {
                HttpFileCollectionBase files = Request.Files;
                archivo = files[0];
            }
            else
            {
                res.Correcto = false;
                res.Mensaje = "El archivo no se adjuntó";
            }
            try
            {
                using(contabilidadEntities db=new contabilidadEntities())
                {
                    var comision = (from c in db.ComisionGeneral
                             where c.id == idComision
                             select c).FirstOrDefault();
                    if(comision == null)
                    {
                        res.Correcto = false;
                        res.Mensaje = "No se encontró la comisión";
                    }
                    else
                    {
                        string ruta = "Comisiones/" + comision.noOficio.Replace("/", "_") + "/";
                        ValidarRuta(Server.MapPath("~/" + ruta));
                        string nomArch = "Anexos_" + comision.noOficio.Replace("/", "_") + ".pdf";
                        archivo.SaveAs(Server.MapPath("~/" + ruta + nomArch));
                        comision.rutaOficio = ruta + "/" + nomArch;
                        comision.reintegrado = true;
                        db.SaveChanges();
                        res.Correcto = true;
                    }
                }
            }
            catch(Exception ex)
            {
                res.Correcto= false;
                res.Mensaje = ex.Message;
                res.Excepcion = ex;
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public ActionResult generarZip(int idComision)
        {
            Resultado res = new Resultado();
            try
            {
                using(contabilidadEntities db=new contabilidadEntities())
                {
                    var comision = (from c in db.ComisionGeneral
                                    where c.id == idComision
                                    select c).FirstOrDefault();
                    var factura=(from f in db.FacturaEncabezado
                                 join g in db.GastosComprobar on f.id equals g.idFactura
                                 join c in db.ComisionGeneral on g.idComision equals c.id
                                 where c.id == idComision
                                 select f).Distinct().ToList();
                    ValidarRuta(Server.MapPath("~/ZIP/"));
                    string nomArch = comision.noOficio.Replace("/", "_") + "_" + DateTime.Now.ToString("ddMMyyhhss") + ".zip";
                    string rutaFactura = Server.MapPath("~/");
                    string rutaNueva = Server.MapPath("~/ZIP/") + nomArch;
                    string rutaTmp = Server.MapPath("~/tmp/" + comision.id.ToString());                    
                    ValidarRuta(rutaTmp);
                    Directory.EnumerateFiles(rutaTmp).ToList().ForEach(f => System.IO.File.Delete(f));
                    factura.ForEach(f => System.IO.File.Copy(rutaFactura + "/" + f.rutaPDF, Path.Combine(rutaTmp, Path.GetFileName(rutaFactura + "/" + f.rutaPDF))));
                    factura.ForEach(f => System.IO.File.Copy(rutaFactura + "/" + f.rutaXML, Path.Combine(rutaTmp, Path.GetFileName(rutaFactura + "/" + f.rutaXML))));
                    factura.ForEach(f => System.IO.File.Copy(rutaFactura + "/" + f.rutaPDFFirmado, Path.Combine(rutaTmp, Path.GetFileName(rutaFactura + "/" + f.rutaPDFFirmado))));
                    //factura.ForEach(f => System.IO.File.Copy(rutaFactura + "/" + f.rutaComrpobacion, Path.Combine(rutaTmp, Path.GetFileName(rutaFactura + "/" + f.rutaComrpobacion))));

                    ZipFile.CreateFromDirectory(rutaTmp, rutaNueva);
                    res.Objeto = "ZIP/" + nomArch;
                    res.Correcto = true;
                }                
            }
            catch (Exception ex) {
                res.Excepcion = ex;
                res.Mensaje = ex.Message;
                res.Correcto = false;
            }            
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public csp_Tabulador_Result GetTabulador(string noEmpleado)
        {
            csp_Tabulador_Result res = new csp_Tabulador_Result();
            try
            {
                using(contabilidadEntities db=new contabilidadEntities())
                {
                    res = db.csp_Tabulador(noEmpleado).FirstOrDefault();                    
                }
            }
            catch(Exception e)
            {
                
            }
            return res;
        }
        public List<rptEmpleado> GetEmpleados(List<csp_rpt_DatosEmpleado_Result> lst)
        {
            List<rptEmpleado> lstSol = new List<rptEmpleado>();
            rptEmpleado solicitud = new rptEmpleado();
            foreach (var s in lst)
            {
                solicitud.NoEmpleado = s.NoEmpleado;
                solicitud.Nombre = s.Nombre;
                solicitud.Apellidos = s.Apellidos;
                solicitud.ClavePuesto = s.ClavePuesto;
                solicitud.AreaId = s.AreaId;
                solicitud.Tipo_Contrato = s.Tipo_Contrato;
                solicitud.Area = s.Area;
                lstSol.Add(solicitud);
                solicitud = new rptEmpleado();
            }
            return lstSol;
        }
        public List<rptSolicitud> GetSolicitud(List<csp_rpt_solicitud_Result> lst)
        {
            List<rptSolicitud> lstSol = new List<rptSolicitud>();
            rptSolicitud solicitud = new rptSolicitud();
            foreach (var s in lst) {
                solicitud.idComision = s.idComision;
                solicitud.noEmpleado = s.noEmpleado;
                solicitud.noOficio = s.noOficio;
                solicitud.objetivoComision = s.objetivoComision;
                solicitud.lugarComision = s.lugarComision;
                solicitud.fechaInicio = s.fechaInicio;
                solicitud.fechaFinal = s.fechaFinal;
                solicitud.principalesAcciones = s.principalesAcciones;
                solicitud.avion = s.avion;
                solicitud.autobus = s.autobus;
                solicitud.automovil = s.automovil;
                solicitud.noPlacas = s.noPlacas;
                solicitud.horaSalida = s.horaSalida;
                solicitud.diasComision = s.diasComision;
                solicitud.hospedaje = s.hospedaje;
                solicitud.alimentacion = s.alimentacion;
                solicitud.terrestre = s.terrestre;
                solicitud.combustible = s.combustible;
                solicitud.peaje = s.peaje;
                solicitud.traslados = s.traslados;
                solicitud.otrosGastos = s.otrosGastos;
                lstSol.Add(solicitud);
                solicitud = new rptSolicitud();
            }
            return lstSol;
        }
        public void GuardarArchivo(HttpPostedFileBase archivo, string ruta, string nombre)
        {
            string rutaCarpeta = Server.MapPath("~/" + ruta);
            ValidarRuta(rutaCarpeta);
            archivo.SaveAs(rutaCarpeta + nombre.Replace("/", "_"));
        }
        public void ValidarRuta(string ruta)
        {
            if (!Directory.Exists(ruta))
            {
                Directory.CreateDirectory(ruta);
            }
        }
        public string mesatexto(int mes)
        {
            string res = "";
            switch (mes)
            {
                case 1:
                    res = "enero";
                    break;
                case 2:
                    res = "febrero";
                    break;
                case 3:
                    res = "marzo";
                    break;
                case 4:
                    res = "abril";
                    break;
                case 5:
                    res = "mayo";
                    break;
                case 6:
                    res = "junio";
                    break;
                case 7:
                    res = "julio";
                    break;
                case 8:
                    res = "agosto";
                    break;
                case 9:
                    res = "septiembre";
                    break;
                case 10:
                    res = "octubre";
                    break;
                case 11:
                    res = "noviembre";
                    break;
                case 12:
                    res = "diciembre";
                    break;
            }
            return res;
        }
    }
}