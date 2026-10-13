(function () {
    function ComprobacionFactory($http, $q, $window) {
        return {
            BuscarEmpleado: function (noEmpleado) {
                return $http.get(raiz + "Comprobacion/BuscarEmpleado", {
                    params: { noEmpleado: noEmpleado }
                })
                    .then(function (result) {
                        return result.data;
                    })
                    .catch(function (error) {
                        return $q.reject(error);
                    });
            },
            ObtenerSolicitud: function (noOficio) {
                return $http.get(raiz + "Comprobacion/ObtenerSolicitud", {
                    params: { noOficio: noOficio }
                })
                    .then(function (result) {
                        return result.data;
                    })
                    .catch(function (error) {
                        return $q.reject(error);
                    });
            },
            ObtenerMarcaModelo: function (noPlacas) {
                return $http.get("/Comprobacion/ObtenerMarcaModelo", {
                    params: { noPlacas: noPlacas }
                });
            },
///
            ObtenerSolicitudId: function (idComision) {
                return $http.get(raiz + "Comprobacion/ObtenerSolicitudId", {
                    params: { idComision: idComision }
                })
                    .then(function (result) {
                        return result.data;
                    })
                    .catch(function (error) {
                        return $q.reject(error);
                    });
            },
            EliminarFactura: function (id) {
                return $http.get(raiz + "Comprobacion/EliminarFactura", {
                    params: { id: id }
                })
                    .then(function (result) {
                        return result.data;
                    })
                    .catch(function (error) {
                        return $q.reject(error);
                    });
            },
            generarZip: function (idComision) {
                return $http.get(raiz + "Comprobacion/generarZip", {
                    params: { idComision: idComision }
                })
                    .then(function (result) {
                        return result.data;
                    })
                    .catch(function (error) {
                        return $q.reject(error);
                    });
            },
            ObtenerAreas: function (id) {
                return $http.get(raiz + "Comprobacion/ObtenerAreas", {
                    params: { id: id }
                })
                    .then(function (result) {
                        return result.data;
                    })
                    .catch(function (error) {
                        return $q.reject(error);
                    });
            },
            ObtenerJefeArea: function (idArea) {
                return $http.get(raiz + "Comprobacion/ObtenerJefeArea", {
                    params: { idArea: idArea }
                })
                    .then(function (result) {
                        return result.data;
                    })
                    .catch(function (error) {
                        return $q.reject(error);
                    });
            },
            ObtenerVehiculos: function (id) {
                return $http.get(raiz + "Comprobacion/ObtenerVehiculos", {
                    params: { id: id }
                })
                    .then(function (result) {
                        return result.data;
                    })
                    .catch(function (error) {
                        return $q.reject(error);
                    });
            },
            ObtenerEmpleados: function (idArea, noEmpleado) {
                return $http.get(raiz + "Comprobacion/ObtenerEmpleados", {
                    params: { idArea: idArea, noEmpleado: noEmpleado }
                })
                    .then(function (result) {
                        return result.data;
                    })
                    .catch(function (error) {
                        return $q.reject(error);
                    });
            },
            BuscarComision: function (noOficio, fechaInicio, fechaFin, idArea, noPersonal) {
                return $http.get(raiz + "Comprobacion/BuscarComision", {
                    params: { noOficio: noOficio, fechaInicio: fechaInicio, fechaFin: fechaFin, idArea: idArea, noPersonal: noPersonal }
                })
                    .then(function (result) {
                        return result.data;
                    })
                    .catch(function (error) {
                        return $q.reject(error);
                    });
            },
            GenerarReporte: function (idSolicitud) {
                return $http.post(raiz + "Comprobacion/GenerarReporte", {
                    idSolicitud: idSolicitud
                })
                    .then(function (result) {
                        return result.data;
                    })
                    .catch(function (error) {
                        return $q.reject(error);
                    });
            },

            GenerarReporteCombustible: function (idComision) {
                return $http.post(raiz + "Comprobacion/GenerarReporteCombustible", { idComision: idComision });
            },

            InfoComplemento: function (ruta) {
                return $http.post(raiz + "Comprobacion/InfoComplemento", {
                    ruta: ruta
                })
                    .then(function (result) {
                        return result.data;
                    })
                    .catch(function (error) {
                        return $q.reject(error);
                    });
            },
            InfoCarpeta: function (ruta, tipo) {
                return $http.post(raiz + "Comprobacion/InfoCarpeta", {
                    ruta: ruta, tipo: tipo
                })
                    .then(function (result) {
                        return result.data;
                    })
                    .catch(function (error) {
                        return $q.reject(error);
                    });
            },
            ExportarExcel: function (lst) {
                return $http.post(raiz + "Comprobacion/ExportarExcel", {
                    lst: lst
                })
                    .then(function (result) {
                        return result.data;
                    })
                    .catch(function (error) {
                        return $q.reject(error);
                    });
            },
            GuardarNoPoliza: function (ltsfacturas, com) {
                return $http.post(raiz + "Comprobacion/GuardarNoPoliza", {
                    ltsfacturas: ltsfacturas, com: com
                })
                    .then(function (result) {
                        return result.data;
                    })
                    .catch(function (error) {
                        return $q.reject(error);
                    });
            },
            GuardarComprobarGasto: function (comision, monto, idUsuario, usuarioCaptura) {
                return $http.post(raiz + "Comprobacion/GuardarComprobarGasto", {
                    comision: comision, monto: monto, idUsuario: idUsuario, usuarioCaptura: usuarioCaptura
                })
                    .then(function (result) {
                        return result.data;
                    })
                    .catch(function (error) {
                        return $q.reject(error);
                    });
            },

            GuardarAnexoCombustible: function (data) {
                return $http.post("/Comprobacion/GuardarAnexoCombustible", data);
            },

            ObtenerAnexoCombustible: function (idComision) {
                return $http.get('/Comprobacion/ObtenerAnexoCombustible', {
                    params: { idComision: idComision }
                });
            },

            GuardarComprobacion: function (ltsfacturas, Comision) {
                return $http.post(raiz + "Comprobacion/GuardarComprobacion", {
                    ltsfacturas: ltsfacturas, Comision: Comision
                })
                    .then(function (result) {
                        return result.data;
                    })
                    .catch(function (error) {
                        return $q.reject(error);
                    });
            },
            ActualizarPermisoCorreccion: function (idComision, permitirCorrecion) {
                return $http.post(raiz + "Comprobacion/ActualizarPermisoCorreccion", {
                    idComision: idComision,
                    permitirCorrecion: permitirCorrecion
                }).then(function (res) {
                    return res.data;
                });
            },

            ObtenerComisionesHabilitadasPorArea: function (noEmpleado) {
                return $http.get(raiz + "Comprobacion/ObtenerComisionesHabilitadasPorArea", {
                    params: { noEmpleado: noEmpleado }
                }).then(function (response) {
                    return response.data;
                });
            },






            ////////////////////////////////////////////////////////////////////////

            AgregarAnexos: function (idComision, archivo) {
                return $http.post(raiz + "Comprobacion/AgregarAnexos", {
                    idComision: idComision, archivo: archivo
                })
                    .then(function (result) {
                        return result.data;
                    })
                    .catch(function (error) {
                        return $q.reject(error);
                    });
            }
        }
    }

    ComprobacionFactory.$inject = ["$http", "$q", "$window"];

    angular.module("comprobacionApp")
        .factory("ComprobacionFactory", ComprobacionFactory);
}());