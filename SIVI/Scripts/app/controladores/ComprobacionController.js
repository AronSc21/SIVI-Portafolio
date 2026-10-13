(function () {
    function ComprobacionController(ComprobacionFactory, $timeout) {
        var vm = this;
        vm.lstHospedaje = [];
        vm.lstAlimentos = [];
        vm.lstTransporte = [];
        vm.SubCombustible = 0.0;
        vm.SubPeaje = 0.0;
        vm.SubTraslado = 0.0;
        vm.SubOtros = 0.0;
        vm.lstOtros = [];
        vm.lstTraslado = [];
        vm.lstPeaje = [];
        vm.lstCombustible = [];
        vm.Monto = {};
        vm.MontoHospedaje = 0;
        vm.MontoAlimento = 0;
        vm.SubTransporte = 0.0;        
        vm.SubHospedaje = 0.0;
        vm.SubAlimentos = 0.0;
        vm.totalComprobado = 0.0;
        vm.totalReintegro = 0.0;
        vm.Datos = [];
        vm.idUsuario = idUsuario;
        vm.usuarioCaptura = usuarioCaptura;
        vm.idComision = idComision;
        vm.vehiculo = { marca: "", modelo: "" };
        vm.Anexo = {
            kmDestino: null,
            kmRutas: null,
            precioCombustible: null,
            montoCombustible: null,
            montoPeaje: null,
            numCilindros: null
        };
        vm.anexoGuardado = false;        // <--- nuevo: indica que los datos están guardados en BD
        vm.encontrado = false;
        vm.mostrarCamposAnexo = false;
        vm.puedeGenerarReporte = false;

        



        function pad(str, max) {
            str = str.toString();
            return str.length < max ? pad("0" + str, max) : str;
        }

        vm.BuscarEmpleado = function () {

            return ComprobacionFactory.BuscarEmpleado(vm.Comision.noEmpleado)
                .then(function (res) {
                    if (res.Correcto) {
                        if (res.Objeto == null) {
                            sweetAlert("ERROR", "El número de empleado ingresado no existe", "error");
                            return res;
                        }
                        vm.nombre = res.Objeto.Nombre + ' ' + res.Objeto.Apellidos;
                        vm.cargo = res.Objeto.Tipo_Contrato;
                        vm.area = res.Objeto.Area;
                        vm.MontoHospedaje = res.Objeto.hospedaje;
                        vm.Monto.hospedaje = res.Objeto.hospedaje;
                        vm.MontoAlimento = res.Objeto.alimentacion;
                        vm.Monto.alimentacion = res.Objeto.alimentacion;
                        vm.ObtenerJefeArea(res.Objeto.AreaId);
                        vm.encontrado = true;
                    } else {
                        sweetAlert("ERROR", res.Mensaje, "error");
                    }
                    return res;
                });
        };

        vm.CalculoMonto = function () {
            var Dias = 0;
            if (vm.Comision.diasComision == null || vm.Comision.diasComision == 0) {
                Dias = 1;
            }
            else {
                Dias = vm.Comision.diasComision;
            }
            vm.Monto.hospedaje = vm.MontoHospedaje * (Dias - 1);
            vm.Monto.alimentacion = vm.MontoAlimento * Dias;
        }
        vm.GuardarComprobarGasto = function () {
            if (vm.Comision.automovil==true&&(vm.Comision.noPlacas == null || vm.Comision.noPlacas == '')) {
                sweetAlert("Debe indicar el número de placas del vehículo","","error");
                return;
            }
            vm.Comision.usuarioCaptura = usuarioCaptura;

            ComprobacionFactory.GuardarComprobarGasto(vm.Comision, vm.Monto, 1, vm.usuarioCaptura)
                .then(function (res) {
                    if (res.Correcto) {
                        vm.GenerarReporte(res.Objeto);
                        //window.open(raiz + res.Objeto, "download");
                        sweetAlert("Se guardó el registro", "", "success");
                        vm.encontrado = false;
                        vm.Comision = null;
                        vm.Monto = null;
                        vm.nombre = null;
                        vm.cargo = null;
                        vm.area = null;
                    }
                    else {
                        sweetAlert("ERROR", res.Mensaje, "error");
                    }
                })
        }

        vm.Redirect = function (idComision) {
            if (idComision != null && idComision > 0)
                window.location.href = "" + raiz + "Comprobacion/solicitud?idComision=" + idComision;
        }

        vm.Iniciar = function () {
            console.log("Entró a vm.Iniciar");


            var noEmpleado = idUsuario.toString().padStart(3, '0');
            var rf = ["090", "329", "372", "340"];

            if (!rf.includes(noEmpleado)) {
                ComprobacionFactory.ObtenerComisionesHabilitadasPorArea(noEmpleado)
                    .then(function (resp) {
                        if (resp.Correcto && resp.Objeto && resp.Objeto.length > 0) {

                            var folios = resp.Objeto.map(c => c.noOficio).join(', ');
                            swal({
                                title: "Aviso",
                                text: "Las comisiones con folio(s): " + folios + " han sido habilitadas para edición porque necesitan correcciones.",
                                type: "info",
                                button: "OK"
                            });
                        }
                    })
                    .catch(function (error) {
                        vm.PuedeEditar = false;
                    });
            } else {
                vm.PuedeEditar = true;
            }

            if (idComision > 0) {
                ComprobacionFactory.ObtenerSolicitudId(idComision)
                    .then(function (res) {
                        if (!res || !res.Correcto || !res.Objeto) {
                            return;
                        }

                        vm.Comision = res.Objeto.comision;
                        vm.Monto = res.Objeto.monto;

                        try {
                            if (vm.Comision.fechaInicio) 
                                vm.Comision.fechaInicio = new Date(parseInt(vm.Comision.fechaInicio.substr(6)));
                            if (vm.Comision.fechaFinal)
                                vm.Comision.fechaFinal = new Date(parseInt(vm.Comision.fechaFinal.substr(6)));
                            if (vm.Comision.fechaOficio)
                                vm.Comision.fechaOficio = new Date(parseInt(vm.Comision.fechaOficio.substr(6)));
                            if (vm.Comision.horaSalida) {
                                var nowDate = new Date(parseInt(vm.Comision.horaSalida.substr(6)));
                                vm.Comision.horaSalida = pad(nowDate.getHours(), 2) + ":" + pad(nowDate.getMinutes(), 2);
                            }
                        } catch (e) {
                        }

                        vm.BuscarEmpleado();
                    })
                    .catch(function (error) {
                    });
            }
        };





        ////////////////////////////////////////////////////////////////////////

        

        vm.GuardarNoPoliza = function () {
            if (vm.Comision.noPoliza == null || vm.Comision.noPoliza.trim() == "")
                return;
            var lst = [];
            vm.lstAlimentos.length > 0 ? lst = lst.concat(vm.lstAlimentos) : 1;
            vm.lstCombustible.length > 0 ? lst = lst.concat(vm.lstCombustible) : 1;
            vm.lstHospedaje.length > 0 ? lst = lst.concat(vm.lstHospedaje) : 1;
            vm.lstOtros.length > 0 ? lst = lst.concat(vm.lstOtros) : 1;
            vm.lstPeaje.length > 0 ? lst = lst.concat(vm.lstPeaje) : 1;
            vm.lstTransporte.length > 0 ? lst = lst.concat(vm.lstTransporte) : 1;
            vm.lstTraslado.length > 0 ? lst = lst.concat(vm.lstTraslado) : 1;
            ComprobacionFactory.GuardarNoPoliza(lst, vm.Comision)
                .then(function (res) {
                    if (res.Correcto) {                        
                        sweetAlert("Se guardó el registro", "", "success");
                        vm.encontrado = false;
                        vm.Comision = null;
                        vm.Monto = null;
                        vm.nombre = null;
                        vm.cargo = null;
                        vm.area = null;
                    }
                    else {
                        alert(res.Mensaje);
                    }
                })
        }
        vm.GenerarReporte = function (idComision) {
            ComprobacionFactory.GenerarReporte(idComision)
                .then(function (res) {
                    if (res.Correcto) {
                        window.open(raiz + res.Objeto, "download");
                    }
                })
        }

        vm.GenerarReporteCombustible = function (idComision) {

            ComprobacionFactory.GenerarReporteCombustible(idComision)
                .then(function (res) {
                    if (res.data.Correcto) {
                        window.open(raiz + res.data.Objeto, "download");
                    } else {
                        swal("Error", res.data.Mensaje, "error");
                    }
                })
                .catch(function (err) {
                    console.error(err);
                    swal("Error", "No se pudo generar el reporte.", "error");
                });
        };

        vm.CalcularMontoMaximo = function () {

            var kmDestino = parseFloat(vm.Anexo.kmDestino) || 0;
            var kmRutas = parseFloat(vm.Anexo.kmRutas) || 0;
            var precioCombustible = parseFloat(vm.Anexo.precioCombustible) || 0;

            var rendimiento = 0;

            switch (parseInt(vm.Anexo.numCilindros)) {
                case 4:
                    rendimiento = 8; // 8 km por litro
                    break;
                case 6:
                    rendimiento = 6; // 6 km por litro
                    break;
                case 8:
                    rendimiento = 4; // 4 km por litro
                    break;
                default:
                    return 0;
            }

            var montoMaximo = ((kmDestino + kmRutas) / rendimiento) * precioCombustible;

            return montoMaximo.toFixed(2);
        };


        vm.ObtenerSolicitud = function () {

            vm.lstHospedaje = [];
            vm.lstAlimentos = [];
            vm.lstTransporte = [];
            vm.lstOtros = [];
            vm.lstTraslado = [];
            vm.lstPeaje = [];
            vm.lstCombustible = [];

            ComprobacionFactory.ObtenerSolicitud(vm.noOficio)
                .then(function (res) {
                    if (res.Correcto && res.Objeto && res.Objeto.comision) {


                        vm.Comision = res.Objeto.comision;
                        vm.PuedeEditar = res.Objeto.comision.permitirCorrecion;
                        vm.Monto = res.Objeto.monto;
                        vm.nombre = res.Objeto.empleado.Nombre;
                        vm.cargo = res.Objeto.empleado.Tipo_Contrato;
                        vm.area = res.Objeto.empleado.Area;


                        try {
                            vm.Comision.fechaInicio = new Date(parseInt(res.Objeto.comision.fechaInicio.substr(6)));
                            vm.Comision.fechaFinal = new Date(parseInt(res.Objeto.comision.fechaFinal.substr(6)));
                            vm.Comision.fechaOficio = new Date(parseInt(res.Objeto.comision.fechaOficio.substr(6)));
                            if (res.Objeto.comision.fechaComprobacion != null)
                                vm.Comision.fechaComprobacion = new Date(parseInt(res.Objeto.comision.fechaComprobacion.substr(6)));
                        } catch (e) { }


                        var jefeTemp = res.Objeto.comision.jefeComision;
                        ComprobacionFactory.ObtenerJefeArea(res.Objeto.empleado.AreaId)
                            .then(function (r) {
                                vm.Jefes = r.Objeto;
                                vm.Comision.jefeComision = jefeTemp;
                            });


                        vm.permitirCorreccion = res.Objeto.comision.permitirCorrecion === true || res.Objeto.comision.permitirCorrecion === 1;
                        vm.Datos = res.Objeto.datos;
                        vm.encontrado = true;
                        vm.Terminado = res.Objeto.comision.actividadesrealizadas != null &&
                            (idUsuario != '090' && idUsuario != '329' && idUsuario != '372' && idUsuario != '340');
                        vm.ValidarRF();
                        vm.Revisado = res.Objeto.comision.noPoliza != null;

                        $.each(res.Objeto.datos, function (index, value) {
                            vm.DetalleFactura(value, value.idTipoGasto);
                        });

                    } else {

                        swal({
                            title: "Comisión no encontrada",
                            text: "No existe ninguna comisión con el folio ingresado.",
                            type: "info",
                            button: "OK"
                        });


                        vm.Comision = {};
                        vm.Monto = {};
                        vm.nombre = "";
                        vm.cargo = "";
                        vm.area = "";
                        vm.Datos = [];
                        vm.encontrado = false;
                        vm.PuedeEditar = false;
                        vm.mostrarCamposAnexo = false;
                    }
                })
                .catch(function (error) {
                    console.error("Error al obtener la comisión:", error);
                    swal({
                        title: "Error",
                        text: "Ocurrió un problema al buscar la comisión.",
                        type: "error",
                        button: "OK"
                    });
                });
        };

        vm.ObtenerMarcaModelo = function () {

            if (!vm.Comision.noPlacas) {

                $timeout(function () {
                    vm.vehiculo.marca = "";
                    vm.vehiculo.modelo = "";
                    vm.mostrarCamposAnexo = false;
                });

                swal({
                    title: "Sin vehículo registrado",
                    text: "Este número de folio no tiene un vehículo registrado.",
                    type: "info",
                    button: "OK"
                });

                return;
            }

            ComprobacionFactory.ObtenerMarcaModelo(vm.Comision.noPlacas)
                .then(function (response) {

                    if (response.data.Exito) {

                        $timeout(function () {
                            vm.vehiculo.marca = response.data.Marca;
                            vm.vehiculo.modelo = response.data.Modelo;
                            vm.mostrarCamposAnexo = true;
                        });

                    } else {

                        $timeout(function () {
                            vm.vehiculo.marca = "";
                            vm.vehiculo.modelo = "";
                        });

                        swal({
                            title: "Sin coincidencias",
                            text: response.data.Mensaje,
                            type: "warning",
                            button: "OK"
                        });
                    }

                })
                .catch(function (error) {

                    $timeout(function () {
                        vm.vehiculo.marca = "";
                        vm.vehiculo.modelo = "";
                    });

                    swal({
                        title: "Error",
                        text: "No se pudo obtener la información del vehículo.",
                        type: "error",
                        button: "OK"
                    });
                });
        };


        vm.BuscarAnexoCombustible = function () {

            vm.ObtenerSolicitud();

            setTimeout(function () {

                if (!vm.encontrado) {
                    return;
                }

                vm.ObtenerMarcaModelo();
                vm.CargarAnexoCombustible();

            }, 400);
        };


        vm.CargarAnexoCombustible = function () {

            if (!vm.Comision.id) {
                console.warn("No hay comisión cargada para consultar anexo.");
                return;
            }

            ComprobacionFactory.ObtenerAnexoCombustible(vm.Comision.id)
                .then(function (res) {

                    if (res.data && res.data.Correcto && res.data.Objeto) {

                        var d = res.data.Objeto;

                        vm.Anexo.kmDestino = d.kmDestino;
                        vm.Anexo.kmRutas = d.kmRutas;
                        vm.Anexo.precioCombustible = d.precioCombustible;
                        vm.Anexo.montoCombustible = d.montoCombustible;
                        vm.Anexo.montoPeaje = d.montoPeaje;
                        vm.Anexo.numCilindros = d.numCilindros;

                        // 📌 AQUÍ SE ACTIVA CORRECTAMENTE
                        vm.anexoGuardado = true;
                        vm.ValidarCamposAnexo();
                    }
                    else {
                        vm.Anexo.kmDestino = "";
                        vm.Anexo.kmRutas = "";
                        vm.Anexo.precioCombustible = "";
                        vm.Anexo.montoCombustible = "";
                        vm.Anexo.montoPeaje = "";
                        vm.Anexo.numCilindros = "";

                        vm.anexoGuardado = false;
                        vm.ValidarCamposAnexo();
                    }
                })
                .catch(function (error) {
                    console.error("Error cargando anexo:", error);
                    vm.anexoGuardado = false;
                    vm.ValidarCamposAnexo();
                });
        };


        vm.ValidarCamposAnexo = function () {
            vm.puedeGenerarReporte =
                vm.Anexo.kmDestino &&
                vm.Anexo.kmRutas &&
                vm.Anexo.precioCombustible &&
                vm.Anexo.montoCombustible &&
                vm.Anexo.montoPeaje &&
                vm.Anexo.numCilindros;
        };

        vm.MarcarAnexoNoGuardado = function () {
            vm.anexoGuardado = false;
        };



        vm.infoXML = function (idTipo) {
            var fileUpload = $("#archivo"+idTipo).get(0);            
            var filesXML = fileUpload.files;
            fileUpload = $("#fact" + idTipo).get(0);
            var filesPDF = fileUpload.files;
            fileUpload = $("#firm" + idTipo).get(0);
            var filesPDFFirm = fileUpload.files;
            //fileUpload = $("#comp" + idTipo).get(0);
            //var filesPDFComp = fileUpload.files;
            if (filesXML.length == 0 || filesPDF.length == 0 || filesPDFFirm.length == 0) {
                //if (vm.ArchivoAdjunto == null) {
                alert('Debe seleccionar los 4 archivos');
                return;
            }
            var fileData = new FormData();
            fileData.append("idComision", vm.Comision.id); 
            fileData.append("idTipo", idTipo);
            fileData.append("xml", filesXML[0]);
            fileData.append("PDF", filesPDF[0]);
            fileData.append("PDFFirm", filesPDFFirm[0]);
            //fileData.append("PDFComp", filesPDFComp[0]);
            fetch(raiz + "Comprobacion/InfoXML", {
                method: 'POST',
                body: fileData
            })
                .then(function (result) {
                    if (result.ok) {
                        return result.json();
                    }
                    else {
                        sweetAlert(result.statusText, "", "error");
                    }
                })
                .then(function (res) {
                    if (res.Correcto) {       
                        
                        vm.DetalleFactura(res.Objeto, idTipo);
                                               
                        $("#archivo" + idTipo).get(0).value = "";
                        $("#fact" + idTipo).get(0).value = "";
                        $("#firm" + idTipo).get(0).value = "";
                        $("#comp" + idTipo).get(0).value = "";
                    }                    
                })
        }
        vm.DetalleFactura = function (dato, idTipo) {
            switch (idTipo) {
                case (1):
                    if (vm.lstHospedaje.find(p => p.noFactura == dato.noFactura && p.idTipoGasto == dato.idTipo) != null) {
                        sweetAlert("", "No se puede agregar la misma factura en el mismo concepto", "error");
                        break;
                    }
                    vm.Hospedaje = {};
                    vm.Hospedaje.contador = vm.lstHospedaje.length + 1;
                    vm.Hospedaje.RFC = dato.RFCEmisor;
                    vm.Hospedaje.noFactura = dato.noFactura;
                    vm.Hospedaje.fecha = dato.fechaExpedicion;
                    vm.Hospedaje.nombre = dato.razonSocialEmisor;
                    vm.Hospedaje.importe = dato.importe;
                    vm.Hospedaje.gasto = dato.gasto;
                    vm.SubHospedaje = vm.SubHospedaje + vm.Hospedaje.importe;
                    vm.Hospedaje.idTipoGasto = idTipo;
                    vm.Hospedaje.idFactura = dato.idFactura;
                    vm.Hospedaje.idComision = dato.idComision;
                    vm.Hospedaje.id = dato.id;
                    vm.Hospedaje.clasificadorGasto = dato.clasificadorGasto;
                    vm.lstHospedaje.push(vm.Hospedaje);
                    $timeout(function () { vm.Hospedaje = []; });
                    break;
                case (2):
                    if (vm.lstAlimentos.find(p => p.noFactura == dato.noFactura && p.idTipoGasto == dato.idTipo) != null) {
                        sweetAlert("", "No se puede agregar la misma factura en el mismo concepto", "error");
                        break;
                    }
                    vm.Alimento = {};
                    vm.Alimento.contador = vm.lstAlimentos.length + 1;
                    vm.Alimento.RFC = dato.RFCEmisor;
                    vm.Alimento.noFactura = dato.noFactura;
                    vm.Alimento.fecha = dato.fechaExpedicion;
                    vm.Alimento.nombre = dato.razonSocialEmisor;
                    vm.Alimento.importe = dato.importe;
                    vm.Alimento.gasto = dato.gasto;
                    vm.SubAlimentos = vm.SubAlimentos + vm.Alimento.importe;
                    vm.Alimento.idTipoGasto = idTipo;
                    vm.Alimento.idFactura = dato.idFactura;
                    vm.Alimento.idComision = dato.idComision;
                    vm.Alimento.id = dato.id;
                    vm.Alimento.clasificadorGasto = dato.clasificadorGasto;
                    vm.lstAlimentos.push(vm.Alimento);
                    $timeout(function () { vm.Alimento = []; });
                    break;
                case (3):
                    if (vm.lstTransporte.find(p => p.noFactura == dato.noFactura && p.idTipoGasto == dato.idTipo) != null) {
                        sweetAlert("", "No se puede agregar la misma factura en el mismo concepto", "error");
                        break;
                    }
                    vm.transporte = {};
                    vm.transporte.contador = vm.lstTransporte.length + 1;
                    vm.transporte.RFC = dato.RFCEmisor;
                    vm.transporte.noFactura = dato.noFactura;
                    vm.transporte.fecha = dato.fechaExpedicion;
                    vm.transporte.nombre = dato.razonSocialEmisor;
                    vm.transporte.importe = dato.importe;
                    vm.transporte.gasto = dato.importe;
                    vm.SubTransporte = vm.SubTransporte + vm.transporte.importe;
                    vm.transporte.idTipoGasto = idTipo;
                    vm.transporte.idFactura = dato.idFactura;
                    vm.transporte.idComision = dato.idComision;
                    vm.transporte.idTipo = dato.idTipo;
                    vm.transporte.id = dato.id;
                    vm.transporte.clasificadorGasto = dato.clasificadorGasto;
                    vm.lstTransporte.push(vm.transporte);
                    $timeout(function () { vm.transporte = []; });
                    break;
                case (4):
                    if (vm.lstCombustible.find(p => p.noFactura == dato.noFactura && p.idTipoGasto == dato.idTipo) != null) {
                        sweetAlert("", "No se puede agregar la misma factura en el mismo concepto", "error");
                        break;
                    }
                    vm.Combustible = {};
                    vm.Combustible.contador = vm.lstCombustible.length + 1;
                    vm.Combustible.RFC = dato.RFCEmisor;
                    vm.Combustible.noFactura = dato.noFactura;
                    vm.Combustible.fecha = dato.fechaExpedicion;
                    vm.Combustible.nombre = dato.razonSocialEmisor;
                    vm.Combustible.importe = dato.importe;
                    vm.Combustible.gasto = dato.importe;
                    vm.SubCombustible = vm.SubCombustible + vm.Combustible.importe;
                    vm.Combustible.idTipoGasto = idTipo;
                    vm.Combustible.idFactura = dato.idFactura;
                    vm.Combustible.idComision = dato.idComision;
                    vm.Combustible.idTipo = dato.idTipo;
                    vm.Combustible.id = dato.id;
                    vm.Combustible.clasificadorGasto = dato.clasificadorGasto;
                    vm.lstCombustible.push(vm.Combustible);
                    $timeout(function () { vm.Combustible = []; });
                    break;
                case (5):
                    if (vm.lstPeaje.find(p => p.noFactura == dato.noFactura && p.idTipoGasto == dato.idTipo) != null) {
                        sweetAlert("", "No se puede agregar la misma factura en el mismo concepto", "error");
                        break;
                    }
                    vm.Peaje = {};
                    vm.Peaje.contador = vm.lstPeaje.length + 1;
                    vm.Peaje.RFC = dato.RFCEmisor;
                    vm.Peaje.noFactura = dato.noFactura;
                    vm.Peaje.fecha = dato.fechaExpedicion;
                    vm.Peaje.nombre = dato.razonSocialEmisor;
                    vm.Peaje.importe = dato.importe;
                    vm.Peaje.gasto = dato.importe;
                    vm.SubPeaje = vm.SubPeaje + vm.Peaje.importe;
                    vm.Peaje.idTipoGasto = idTipo;
                    vm.Peaje.idFactura = dato.idFactura;
                    vm.Peaje.idComision = dato.idComision;
                    vm.Peaje.id = dato.id;
                    vm.Peaje.clasificadorGasto = dato.clasificadorGasto;
                    vm.lstPeaje.push(vm.Peaje);
                    $timeout(function () { vm.Peaje = []; });
                    break;
                case (6):
                    if (vm.lstTraslado.find(p => p.noFactura == dato.noFactura && p.idTipoGasto == dato.idTipo) != null) {
                        sweetAlert("", "No se puede agregar la misma factura en el mismo concepto", "error");
                        break;
                    }
                    vm.Traslado = {};
                    vm.Traslado.contador = vm.lstTraslado.length + 1;
                    vm.Traslado.RFC = dato.RFCEmisor;
                    vm.Traslado.noFactura = dato.noFactura;
                    vm.Traslado.fecha = dato.fechaExpedicion;
                    vm.Traslado.nombre = dato.razonSocialEmisor;
                    vm.Traslado.importe = dato.importe;
                    vm.Traslado.gasto = dato.importe;
                    vm.SubTraslado = vm.SubTraslado + vm.Traslado.importe;
                    vm.Traslado.idTipoGasto = idTipo;
                    vm.Traslado.idFactura = dato.idFactura;
                    vm.Traslado.idComision = dato.idComision;
                    vm.Traslado.id = dato.id;
                    vm.Traslado.clasificadorGasto = dato.clasificadorGasto;
                    vm.lstTraslado.push(vm.Traslado)
                    $timeout(function () { vm.Traslado = []; });
                    break;
                case (7):
                    if (vm.lstOtros.find(p => p.noFactura == dato.noFactura && p.idTipoGasto == dato.idTipo) != null) {
                        sweetAlert("", "No se puede agregar la misma factura en el mismo concepto", "error");
                        break;
                    }
                    vm.Otros = {};
                    vm.Otros.contador = vm.lstOtros.length + 1;
                    vm.Otros.RFC = dato.RFCEmisor;
                    vm.Otros.noFactura = dato.noFactura;
                    vm.Otros.fecha = dato.fechaExpedicion;
                    vm.Otros.nombre = dato.razonSocialEmisor;
                    vm.Otros.importe = dato.importe;
                    vm.Otros.gasto = dato.importe;
                    vm.SubOtros = vm.SubOtros + vm.Otros.importe;
                    vm.Otros.idTipoGasto = idTipo;
                    vm.Otros.idFactura = dato.idFactura;
                    vm.Otros.idComision = dato.idComision;
                    vm.Otros.id = dato.id;
                    vm.Otros.clasificadorGasto = dato.clasificadorGasto;
                    vm.lstOtros.push(vm.Otros)
                    $timeout(function () { vm.Otros = []; });
                    break;
            } 
        }
        vm.generarZip = function (idC) {
            if (idC == null)
                idC = vm.Comision.id;
            ComprobacionFactory.generarZip(idC)
                .then(function (res) {
                    if (res.Correcto) {
                        window.open(raiz + res.Objeto, "download");
                    }
                })
        }
        vm.EliminarFactura = function (id) {
            ComprobacionFactory.EliminarFactura(id)
                .then(function (res) {
                    if (res.Correcto) {
                        sweetAlert("Se eliminó el registro", "", "success");
                    }
                })
        }
        vm.Eliminar = function (valor, idTipo) {
            swal({
                title: "Confirmar",
                text: "¿Desea eliminar el registro seleccionado?",
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn-danger",
                confirmButtonText: "SI",
                cancelButtonText: "NO",
                closeOnConfirm: false,
                closeOnCancel: false
            },
                function (isConfirm) {
                    if (isConfirm) {
                        $timeout(function () {
                                    switch (idTipo) {
                                        case (1):
                                            if (valor == null)
                                                vm.lstHospedaje.splice(valor, 1);
                                            else {
                                                vm.EliminarFactura(valor);
                                            }
                                            break;
                                        case (2):
                                            if (valor == null)
                                                vm.lstAlimentos.splice(valor, 1);////////
                                            else {
                                                vm.EliminarFactura(valor);
                                            }
                                            break;
                                        case (3):
                                            if (valor == null)
                                                vm.lstTransporte.splice(valor, 1)
                                            else {
                                                vm.EliminarFactura(valor);
                                            }
                                            break;
                                        case (4):
                                            if (valor == null)
                                                vm.lstCombustible.splice(valor, 1)/////
                                            else {
                                                vm.EliminarFactura(valor);
                                            }
                                            break;
                                        case (5):
                                            if (valor == null)
                                                vm.lstPeaje.splice(valor, 1)//////
                                            else {
                                                vm.EliminarFactura(valor);
                                            }
                                            break;
                                        case (6):
                                            if (valor == null)
                                                vm.lstTraslado.splice(valor, 1)
                                            else {
                                                vm.EliminarFactura(valor);
                                            }
                                            break;
                                        case (7):
                                            if (valor = null)
                                                vm.lstOtros.splice(valor, 1)/////
                                            else {
                                                vm.EliminarFactura(valor);
                                            }
                                            break;
                            }
                            vm.ObtenerSolicitud();
                            swal.close();
                                });
                    } else {
                        swal.close();
                    }
                });            
        }

        vm.ValidarMontosCero = function () {
            var errores = [];

            var listas = [
                { nombre: "Hospedaje", lst: vm.lstHospedaje },
                { nombre: "Alimentación", lst: vm.lstAlimentos },
                { nombre: "Avión / Autobús", lst: vm.lstTransporte },
                { nombre: "Combustible", lst: vm.lstCombustible },
                { nombre: "Peaje", lst: vm.lstPeaje },
                { nombre: "Traslados", lst: vm.lstTraslado },
                { nombre: "Otros Gastos", lst: vm.lstOtros }
            ];

            listas.forEach(function (item) {
                var total = 0;
                if (item.lst && item.lst.length > 0) {
                    $.each(item.lst, function (index, value) {
                        total += parseFloat(value.gasto) || 0;
                    });
                    if (total === 0) {
                        errores.push(item.nombre);
                    }
                    vm["Sub" + item.nombre.replace(/\s|\/|-/g, "")] = total;
                }
            });

            if (errores.length > 0) {
                var msg = "No se puede guardar la comprobación.\n\n" +
                    "Los siguientes rubros tienen monto a comprobar igual a 0 pero tienen facturas cargadas:\n\n" +
                    errores.join("\n");
                swal({
                    title: "Error",
                    text: msg,
                    type: "error",
                    button: "Aceptar"
                });
                return false;
            }

            return true;
        };

        vm.GuardarAnexoCombustible = function () {

            if (!vm.Comision.id) {
                swal("Error", "No hay una comisión cargada.", "error");
                return;
            }

            if (!vm.vehiculo || !vm.vehiculo.marca || !vm.vehiculo.modelo) {
                swal("Error",
                    "Esta comisión no tiene un vehículo registrado, no se puede guardar el Anexo de combustible.",
                    "error");
                return;
            }

            if (!vm.Anexo.kmDestino || !vm.Anexo.kmRutas ||
                !vm.Anexo.precioCombustible || !vm.Anexo.montoCombustible ||
                !vm.Anexo.numCilindros) {

                swal("Error",
                    "Favor de llenar todos los campos obligatorios del Anexo de combustible.",
                    "error");
                return;
            }

            var mensajeConfirmacion =
                "¿Desea guardar el Anexo de Combustible?\n\n" +
                "Se realizarán cambios en estos campos";

            swal({
                title: "Confirmación de guardado",
                text: mensajeConfirmacion,
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn-danger",
                confirmButtonText: "Sí, guardar",
                cancelButtonText: "Cancelar",
                closeOnConfirm: false,
                closeOnCancel: true
            },
                function (isConfirm) {

                    if (!isConfirm) return;

                    var data = {
                        idComision: vm.Comision.id,
                        kmDestino: vm.Anexo.kmDestino,
                        kmRutas: vm.Anexo.kmRutas,
                        precioCombustible: vm.Anexo.precioCombustible,
                        montoCombustible: vm.Anexo.montoCombustible,
                        montoPeaje: vm.Anexo.montoPeaje,
                        numCilindros: vm.Anexo.numCilindros
                    };

                    ComprobacionFactory.GuardarAnexoCombustible(data)
                        .then(function (response) {

                            if (response.data.Correcto) {
                                vm.anexoGuardado = true;
                                swal({
                                    title: "Guardado correctamente",
                                    text: "El Anexo de Combustible se guardó con éxito.",
                                    type: "success"
                                }, function () {
                                    vm.GenerarReporteCombustible(vm.Comision.id);
                                    setTimeout(function () {
                                        location.reload();
                                    }, 2500);
                                                                     
                                });

                            } else {

                                swal("ERROR",
                                    response.data.Mensaje || "No se pudo guardar.",
                                    "error");

                            }

                        })
                        .catch(function (error) {
                            console.error(error);
                            swal("Error", "Ocurrió un error inesperado al guardar.", "error");
                        });

                }
            );

        };


        vm.ValidarFacturasIndividuales = function () {
            var rubros = [
                { nombre: "Hospedaje", lista: vm.lstHospedaje },
                { nombre: "Alimentación", lista: vm.lstAlimentos },
                { nombre: "Avión / Autobús", lista: vm.lstTransporte },
                { nombre: "Combustible", lista: vm.lstCombustible },
                { nombre: "Peaje", lista: vm.lstPeaje },
                { nombre: "Traslados", lista: vm.lstTraslado },
                { nombre: "Otros Gastos", lista: vm.lstOtros }
            ];

            var errores = [];

            rubros.forEach(function (r) {
                if (!r.lista || r.lista.length === 0) return;

                r.lista.forEach(function (factura) {
                   
                    var folio = factura.noFactura || factura.folio || factura.idFactura || factura.id || "sin folio";

                    var importe = parseFloat(factura.importe);
                    var gasto = parseFloat(factura.gasto);

                   
                    if (isNaN(importe)) importe = 0;
                    if (isNaN(gasto)) gasto = 0;

                    
                    if (gasto <= 0) {
                        errores.push("- En el rubro de " + r.nombre + " el No. de factura '" + folio + "' la cantidad ingresada es 0 o está vacío");
                        return;
                    }

                    
                    if (gasto > importe) {
                        errores.push("- En el rubro de " + r.nombre + " el No. de factura '" + folio + "' la cantidad ingresada (" + gasto.toFixed(2) + ") es mayor que el importe (" + importe.toFixed(2) + ").");
                        return;
                    }
                });
            });

            if (errores.length > 0) {
                
                var mensaje = "No se puede guardar. Se detectaron los siguientes errores:\n\n" + errores.join("\n");
                if (mensaje.length > 2000) {
                    
                    mensaje = "No se puede guardar. Se detectaron varios errores en las facturas. Revisa cada rubro.";
                }

                swal({
                    title: "Error en las facturas",
                    text: mensaje,
                    type: "error",
                    button: "OK"
                });
                return false;
            }

            return true;
        };






        vm.GuardarComprobacion = function () {

            if (!vm.Comision.actividadesrealizadas || !vm.Comision.resultadosObtenidos ||
                !vm.Comision.contribuciones || !vm.Comision.fechaComprobacion) {
                swal("Error", "Favor de llenar todos los campos", "error");
                return;
            }

            var listasTodas = [vm.lstHospedaje, vm.lstAlimentos, vm.lstTransporte,
            vm.lstCombustible, vm.lstPeaje, vm.lstTraslado, vm.lstOtros];
            var tieneFacturas = listasTodas.some(function (lst) { return lst && lst.length > 0; });
            if (!tieneFacturas) {
                swal("Error", "No hay ninguna factura cargada", "error");
                return;
            }

            if (!vm.ValidarMontosCero()) return;
            
            if (!vm.ValidarFacturasIndividuales()) return;


            function montoOtorgado(valor) {
                return parseFloat(valor) || 0;
            }
            var excedidos = [];

            var rubros = [
                { etiqueta: "HOSPEDAJE", total: vm.SubHospedaje, monto: vm.Monto.hospedaje },
                { etiqueta: "ALIMENTACIÓN", total: vm.SubAlimentos, monto: vm.Monto.alimentacion },
                { etiqueta: "AVIÓN / AUTOBÚS", total: vm.SubTransporte, monto: vm.Monto.terrestre },
                { etiqueta: "COMBUSTIBLE", total: vm.SubCombustible, monto: vm.Monto.combustible },
                { etiqueta: "PEAJE", total: vm.SubPeaje, monto: vm.Monto.peaje },
                { etiqueta: "TRASLADOS", total: vm.SubTraslado, monto: vm.Monto.traslados },
                { etiqueta: "OTROS GASTOS", total: vm.SubOtros, monto: vm.Monto.otrosGastos }
            ];

            rubros.forEach(function (r) {

                var total = parseFloat(r.total) || 0;
                var monto = parseFloat(r.monto) || 0;

                if (total > monto) {
                    excedidos.push({
                        etiqueta: r.etiqueta,
                        comprobado: total,
                        otorgado: monto
                    });
                }
            });
            //var excedidos = [];
            //var nombres = ["Hospedaje", "Alimentación", "Avión / Autobús", "Combustible", "Peaje", "Traslados", "Otros Gastos"];
            //nombres.forEach(function (nombre) {
            //    var key = "Sub" + nombre.replace(/\s|\/|-/g, "");
            //    var total = vm[key] || 0;
            //    var monto = vm.Monto && vm.Monto[nombre.replace(/\s|\/|-/g, "").toLowerCase()];
            //    if (total > montoOtorgado(monto)) {
            //        excedidos.push({ etiqueta: nombre.toUpperCase(), comprobado: total, otorgado: montoOtorgado(monto) });
            //    }
            //});

           
            var mensaje = "";
            if (excedidos.length > 0) {
                mensaje += "Se detectó que los siguientes rubros exceden el monto otorgado:\n\n";
                excedidos.forEach(function (e) {
                    mensaje += e.etiqueta + ": comprobado " + e.comprobado.toFixed(2) +
                        " / otorgado " + e.otorgado.toFixed(2) + "\n";
                });
                mensaje += "\n¿Desea guardar de todos modos? Una vez guardada la información ya no podrá ser modificada.";
            } else {
                mensaje = "¿Desea guardar la comprobación? Una vez guardada la información ya no podrá ser modificada.";
            }

            swal({
                title: "Confirmación de guardado",
                text: mensaje,
                type: "warning",
                showCancelButton: true,
                confirmButtonClass: "btn-danger",
                confirmButtonText: "Sí, guardar",
                cancelButtonText: "Cancelar",
                closeOnConfirm: false,
                closeOnCancel: true
            },
                function (isConfirm) {
                    if (isConfirm) {
                        var lst = [].concat(vm.lstAlimentos || [], vm.lstCombustible || [],
                            vm.lstHospedaje || [], vm.lstOtros || [],
                            vm.lstPeaje || [], vm.lstTransporte || [],
                            vm.lstTraslado || []);

                    ComprobacionFactory.GuardarComprobacion(lst, vm.Comision)
                        .then(function (res) {
                            if (res.Correcto) {
                                vm.Comision.permitirCorrecion = false;
                                vm.CambiarPermisoCorreccion(vm.Comision);
                                vm.GenerarReporte(res.Objeto);
                                swal("", "La comprobación se guardó con éxito", "success").then(function () {
                                    location.reload();
                                });
                            } else {
                                swal("ERROR", res.Mensaje, "error");
                            }
                        });
                }
            });

        };

        vm.CambiarPermisoCorreccion = function (comision) {
            if (!vm.RF) return;

            ComprobacionFactory.ActualizarPermisoCorreccion(comision.idComision, comision.permitirCorrecion)
                .then(function (res) {
                    if (res.Correcto) {
                        if (comision.permitirCorrecion == 1 || comision.permitirCorrecion === true) {
                            swal({
                                title: "Permisos habilitados",
                                text: "Se han habilitado los permisos de edición para esta comisión.",
                                type: "success",
                                button: "OK"
                            });
                        } else {
                            swal({
                                title: "Permisos deshabilitados",
                                text: "Se han deshabilitado los permisos de edición para esta comisión.",
                                type: "info",
                                button: "OK"
                            });
                        }
                    } else {
                        swal({
                            title: "Error",
                            text: "Ocurrió un error al actualizar el permiso.",
                            type: "error",
                            button: "OK"
                        });
                    }
                });
        };





        vm.ModalDocumentos = function (Documento) {
            vm.Documento = Documento;
            vm.Documento.Factura = vm.Datos.find(p => p.idFactura == Documento.idFactura);
            $('#mdl-Documentos').modal({ backdrop: 'static', keyboard: true });
        }
        vm.ModalAnexos = function (comision) {
            vm.Comision = comision;
            $('#mdl-Anexos').modal({ backdrop: 'static', keyboard: true });
        }
        vm.AgregarAnexos = function () {
            var err = '';
            var fileUpload = $("#archivo").get(0);
            var files = fileUpload.files;
            if (files.length == 0) {
                sweetAlert('', 'Seleccionar Archivo', 'error');
                return;
            }
            var fileData = new FormData();
            fileData.append("idComision", vm.Comision.idComision);
            fileData.append("archivo", files[0]);
            //ComprobacionFactory.AgregarAnexos(vm.Comision.idComision, files[0])
            //    .then(function (res) {
            //        if (res.Correcto) {
            //            vm.BuscarComision();
            //            $('#mdl-Anexos').modal('hide'); 
            //        }
            //        else {
            //            sweetAlert("No se pudieron obtener las Áreas", ex.Mensaje, "error");
            //        }
            //    })
            fetch(raiz + "Comprobacion/AgregarAnexos", {
                method: 'POST',
                body: fileData
            })
                .then(function (result) {
                    if (result.ok) {
                        return result.json();
                    }
                    else {
                        sweetAlert(result.statusText, "", "error");
                    }
                })
                .then(function (res) {
                    if (res != null && res.Correcto) {      
                        vm.BuscarComision();
                        $('#mdl-Anexos').modal('hide');                           
                    }
                    else {
                        sweetAlert("El archivo no pudo guardarse");
                    }
                })
        }
        vm.DiasComision = function () {
            if (!vm.Comision || !vm.Comision.fechaInicio || !vm.Comision.fechaFinal) {
                return 0; 
            }
            vm.Comision.diasComision =
                ((vm.Comision.fechaFinal.getTime() - vm.Comision.fechaInicio.getTime()) / (1000 * 60 * 60 * 24)) + 1;

            return vm.Comision.diasComision;
        };

        vm.Total = function () {
            return (vm.Monto.hospedaje + vm.Monto.alimentacion + vm.Monto.terrestre + vm.Monto.combustible + vm.Monto.peaje + vm.Monto.traslados + vm.Monto.otrosGastos) - (vm.SubHospedaje + vm.SubAlimentos + vm.SubCombustible + vm.SubPeaje + vm.SubTraslado + vm.SubOtros + vm.SubTransporte)
        }
        vm.ObtenerJefeArea = function (idA) {
            ComprobacionFactory.ObtenerJefeArea(idA)
                .then(function (res) {
                    if (res.Correcto) {
                        vm.Jefes = res.Objeto;
                    }
                    else {
                        sweetAlert("No se pudieron obtener los jefes de área", ex.Mensaje, "error");
                    }
                })
        }
        vm.ObtenerVehiculos = function () {
            ComprobacionFactory.ObtenerVehiculos()
                .then(function (res) {
                    if (res.Correcto) {
                        vm.Vehiculos = res.Objeto;
                    }
                    else {
                        sweetAlert("No se pudieron obtener los vehiculos", ex.Mensaje, "error");
                    }
                })
        }
        vm.ObtenerAreas = function () {
            var Areaid = null;
            if (idUsuario != '090' && idUsuario != '329' && idUsuario != '372' && idUsuario != '340')
                Areaid = idArea;
            ComprobacionFactory.ObtenerAreas(Areaid)
                .then(function (res) {
                    if (res.Correcto) {
                        vm.Areas = res.Objeto;
                    }
                    else {
                        sweetAlert("No se pudieron obtener las Áreas", ex.Mensaje, "error");
                    }
                })            
        }
        //vm.Iniciar = function () {
        //    openCity(event, '01');            
        //}
        vm.ObtenerEmpleados = function () {
            vm.Personas = null;
            ComprobacionFactory.ObtenerEmpleados(vm.idArea, null)
                .then(function (res) {
                    if (res.Correcto) {
                        vm.Personas = res.Objeto;
                    }
                    else {
                        sweetAlert("No se pudieron obtener las Personas", ex.Mensaje, "error");
                    }
                })
        }
        vm.BuscarComision = function () {
            if (vm.idArea == null) {
                sweetAlert("", "Selecciona el Área", "error");
                return;
            }
            ComprobacionFactory.BuscarComision(vm.noOficio, vm.fechaInicio, vm.fechaFin, vm.idArea, vm.noPersonal)
                .then(function (res) {
                    if (res.Correcto) {
                        vm.Comisiones = res.Objeto; 
                        vm.ValidarRF(); 
                    }
                    else {
                        sweetAlert("No se pudieron obtener las Personas", ex.Mensaje, "error");
                    }
                })
        }

        vm.ValidarRF = function () {
            vm.RF = (idUsuario == '090' || idUsuario == '329' || idUsuario == '372' || idUsuario == '340') ? true : false; 
        }

        vm.InfoComplemento = function () {
            ComprobacionFactory.InfoComplemento(vm.ComplementoXML)
                .then(function (res) {
                    if (res.Correcto) {
                        //window.open(raiz + res.Objeto, "download");
                        sweetAlert("", res.Objeto + " registros ingresados", "success");
                    }
                })
        }
        vm.InfoCarpeta = function () {            
            ComprobacionFactory.InfoCarpeta(vm.RutaXML,1)
                .then(function (res) {
                    if (res.Correcto) {
                        window.open(raiz + res.Objeto, "download");
                    }
                })
        }
        vm.ExportarExcel = function () {
            if (vm.Comisiones == null || vm.Comisiones.length == 0)
                return;
            ComprobacionFactory.ExportarExcel(vm.Comisiones)
                .then(function (res) {
                    if (res.Correcto) {
                        window.open(raiz + res.Objeto, "download");
                    }
                })
        }
        vm.MostrarDoc = function (RutaRemota) {
            setTimeout(function () {                
                direccion = raiz + RutaRemota;                        
                if (direccion != '') {
                    vm.MostrarDocumento(direccion);
                }
            }, 1000);
        }
        vm.MostrarDocumento = function (Ruta) {
            var a = document.createElement('a');
            $(a).attr({
                href: Ruta
                , download: 'Documento.pdf'
            }).text('');
            document.body.appendChild(a);
            a.click();
        } 
        vm.FormatoDecimal = function (valor) {
            return isNaN(valor) ? valor : parseFloat(valor).toFixed(2);
        }        
        vm.SumaHospedaje = function () {
            vm.SubHospedaje = 0;

            $.each(vm.lstHospedaje, function (index, value) {
                vm.SubHospedaje += value.gasto;
            });
            
            return vm.SubHospedaje;
        };


        vm.SumaAlimentos = function () {
            vm.SubAlimentos = 0;
            $.each(vm.lstAlimentos, function (index, value) {
                vm.SubAlimentos += value.gasto;
            });
            return vm.SubAlimentos;
        }
        vm.SumaTransporte = function () {
            vm.SubTransporte = 0;
            $.each(vm.lstTransporte, function (index, value) {
                vm.SubTransporte += value.gasto;
            });
            return vm.SubTransporte;
        }
        vm.SumaCombustible = function () {
            vm.SubCombustible = 0;
            $.each(vm.lstCombustible, function (index, value) {
                vm.SubCombustible += value.gasto;
            });
            return vm.SubCombustible;
        }
        vm.SumaPeaje = function () {
            vm.SubPeaje = 0;
            $.each(vm.lstPeaje, function (index, value) {
                vm.SubPeaje += value.gasto;
            });
            return vm.SubPeaje;
        }
        vm.SumaTraslado = function () {
            vm.SubTraslado = 0;
            $.each(vm.lstTraslado, function (index, value) {
                vm.SubTraslado += value.gasto;
            });
            return vm.SubTraslado;
        }
        vm.SumaOtros = function () {
            vm.SubOtros = 0;
            $.each(vm.lstOtros, function (index, value) {
                vm.SubOtros += value.gasto;
            });
            return vm.SubOtros;
        }
    }

    ComprobacionController.$inject = ["ComprobacionFactory", "$timeout"];
    angular.module("comprobacionApp")
        .controller("ComprobacionController", ComprobacionController)
}());