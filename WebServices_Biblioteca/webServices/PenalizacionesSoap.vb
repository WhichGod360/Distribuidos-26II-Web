Option Strict On

Imports System.Web.Services
Imports capaDatos
Imports capaNegocio

Partial Public Class swBiblioteca

    Private ReadOnly pagosYape As New pagoYapeNegocio()
    <WebMethod(EnableSession:=True)>
    Public Function ListarTiposPenalizacion() As Penalizacion()
        seguridadNegocio.ExigirPersonal()
        Return New penalizacionNegocio().listarTodos().ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListarTiposPenalizacionActivos() As Penalizacion()
        seguridadNegocio.ExigirPersonal()
        Return New penalizacionNegocio().listarActivas().ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function BuscarTiposPenalizacion(texto As String, activo As Boolean?) As Penalizacion()
        seguridadNegocio.ExigirPersonal()
        Return New penalizacionNegocio().buscar(texto, activo).ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ObtenerTipoPenalizacion(idPenalizacion As Integer) As Penalizacion
        seguridadNegocio.ExigirPersonal()
        Return New penalizacionNegocio().buscarPorId(idPenalizacion)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function AgregarTipoPenalizacion(tipo As String, descripcion As String, monto As Decimal, activo As Boolean, diasDesde As Integer?) As Boolean
        Return New penalizacionNegocio().agregarPenalizacion(tipo, descripcion, monto, activo, diasDesde)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ActualizarTipoPenalizacion(idPenalizacion As Integer, tipo As String, descripcion As String,
                                              monto As Decimal, activo As Boolean, diasDesde As Integer?) As Boolean
        Return New penalizacionNegocio().actualizarPenalizacion(idPenalizacion, tipo, descripcion, monto, activo, diasDesde)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function CambiarEstadoTipoPenalizacion(idPenalizacion As Integer, activo As Boolean) As Boolean
        Return New penalizacionNegocio().cambiarEstado(idPenalizacion, activo)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListarCargos(idPenalizacion As Integer) As DetallePenalizacion()
        Return New detallePenalizacionNegocio().listar(idPenalizacion).ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListarPrestamosParaCargo() As PrestamoResumen()
        Return New detallePenalizacionNegocio().listarPrestamos().ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function AgregarCargo(descripcion As String, idPenalizacion As Integer, idPrestamo As Integer, estadoPago As String) As Boolean
        Return New detallePenalizacionNegocio().agregarDetalle(descripcion, idPenalizacion, idPrestamo,
            If(estadoPago, DetallePenalizacion.PagoPendiente))
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ActualizarCargo(idDetalle As Integer, descripcion As String, idPenalizacion As Integer, idPrestamo As Integer,
                                   estadoPago As String) As Boolean
        Return New detallePenalizacionNegocio().actualizarDetalle(idDetalle, descripcion, idPenalizacion, idPrestamo, estadoPago)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ConsultarMisCargos() As ResumenPenalizaciones
        Return New detallePenalizacionNegocio().ConsultarMias()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function PagoYapeDisponible() As Boolean
        If SesionUsuario.EsPersonal Then
            seguridadNegocio.ExigirPersonal()
        Else
            seguridadNegocio.ExigirLector()
        End If
        Return (SesionUsuario.EsPersonal OrElse Not pagoYapeNegocio.ModoDemoSolicitado()) AndAlso pagosYape.Disponible()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ConsultarMiCargoYape(idCargo As Integer) As DataTable
        Return FilaSoap(pagosYape.CargoMio(idCargo), "Cargo")
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ConsultarMiHistorialYape(idCargo As Integer) As DataTable
        Return TablaSoap(pagosYape.HistorialMio(idCargo), "HistorialYape")
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListarMisAvisosYape() As AvisoLector()
        Return pagosYape.AvisosMios().ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Sub RegistrarReporteYape(idCargo As Integer, importe As Decimal, nombre As String, celular As String, codigo As String)
        pagosYape.Registrar(idCargo, importe, nombre, celular, codigo)
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Function ListarReportesYape(estado As String) As DataTable
        Return TablaSoap(pagosYape.Bandeja(If(estado, "")), "ReportesYape")
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ObtenerReporteYape(idReporte As Integer) As DataTable
        Return FilaSoap(pagosYape.Reporte(idReporte), "ReporteYape")
    End Function

    <WebMethod(EnableSession:=True)>
    Public Sub RevisarReporteYape(idReporte As Integer, confirmar As Boolean, ingresoVerificado As Boolean, motivo As String)
        If confirmar AndAlso Not ingresoVerificado Then
            Throw New ArgumentException("Marca la verificación del ingreso en la cuenta receptora.")
        End If
        pagosYape.Revisar(idReporte, confirmar, motivo)
    End Sub
End Class
