Option Strict On

Imports System.Web.Services
Imports capaDatos
Imports capaNegocio

Partial Public Class swBiblioteca

    Private ReadOnly solicitudes As New solicitudPrestamoNegocio()

    <WebMethod(EnableSession:=True)>
    Public Function ConsultarBibliotecaLector(idsCarrito As Integer()) As BibliotecaLectorSoap
        seguridadNegocio.ExigirLector()
        Dim catalogo = New catalogoLectorNegocio().listar()
        Dim reservas = solicitudes.listarMias()
        Dim prestamos = New prestamoNegocio().listarPorLector(SesionUsuario.IdUsuario)
        Dim cargos = New detallePenalizacionNegocio().ConsultarMias()
        Dim estado = circulacionLectorNegocio.Evaluar(prestamos, reservas, cargos)
        Dim porId = catalogo.ToDictionary(Function(libro) libro.IdLibro)
        Dim carrito = If(idsCarrito, Array.Empty(Of Integer)()).Select(
            Function(id) If(porId.ContainsKey(id), porId(id), New Libro With {.IdLibro = id})).ToList()
        Return New BibliotecaLectorSoap With {
            .Catalogo = catalogo.ToArray(), .Reservas = reservas.ToArray(),
            .Prestamos = prestamos.ToArray(), .Cargos = cargos, .Circulacion = estado,
            .PuedeSolicitar = circulacionLectorNegocio.PuedeSolicitar(carrito, estado) AndAlso carrito.All(Function(libro) libro.Disponibles > 0),
            .LibrosAgregables = catalogo.Where(Function(libro) circulacionLectorNegocio.PuedeAgregar(libro, carrito, estado)).
                Select(Function(libro) libro.IdLibro).ToArray()}
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function BuscarPrestamosPorLector(idLector As Integer, estado As String, texto As String) As Prestamo()
        seguridadNegocio.ExigirPersonal()
        Dim prestamos = New prestamoNegocio().listarPorLector(idLector)
        Return prestamoNegocio.FiltrarConsulta(prestamos, If(estado, "Todos"), If(texto, "").Trim(), idLector, 0, Date.Today).ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function FechaLimitePrestamo(fechaInicio As Date) As Date
        seguridadNegocio.ExigirPersonal()
        Return prestamoNegocio.FechaLimite(fechaInicio)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function BuscarLectores(texto As String) As UsuarioSoap()
        Return New prestamoNegocio().buscarLectores(texto).Select(AddressOf UsuarioSoap.Desde).ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListarPrestamos(estado As String, texto As String) As Prestamo()
        Return New prestamoNegocio().listar(If(estado, "Todos"), texto).ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListarPrestamosPorLector(idLector As Integer) As Prestamo()
        Return New prestamoNegocio().listarPorLector(idLector).ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListarMisPrestamos() As Prestamo()
        seguridadNegocio.ExigirLector()
        Return New prestamoNegocio().listarPorLector(SesionUsuario.IdUsuario).ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function RegistrarPrestamo(idLector As Integer, idsEjemplares As Integer(), fechaInicio As Date) As Integer
        Return New prestamoNegocio().registrar(idLector, SesionUsuario.IdUsuario, idsEjemplares, fechaInicio)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function PrevisualizarDevolucion(idPrestamo As Integer, fechaRetorno As Date) As DevolucionCalculada
        Return New prestamoNegocio().PrevisualizarDevolucion(idPrestamo, fechaRetorno)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ConfirmarDevolucion(calculo As DevolucionCalculada) As DevolucionCalculada
        Return New prestamoNegocio().DevolverConfirmado(calculo)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListarSolicitudes() As SolicitudPrestamo()
        Return solicitudes.listarTodas().ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListarMisSolicitudes() As SolicitudPrestamo()
        Return solicitudes.listarMias().ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function SolicitarLibros(idsLibros As Integer()) As Integer
        Return solicitudes.solicitar(idsLibros)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Sub CancelarSolicitud(idSolicitud As Integer)
        solicitudes.cancelar(idSolicitud)
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Sub AprobarSolicitud(idSolicitud As Integer)
        solicitudes.aprobar(idSolicitud)
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Sub RechazarSolicitud(idSolicitud As Integer)
        solicitudes.rechazar(idSolicitud)
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Function EntregarSolicitud(idSolicitud As Integer, ejemplares As EjemplarEntregaSoap()) As Integer()
        Const mensaje As String = "Selecciona un ejemplar por cada libro de la solicitud."
        If ejemplares Is Nothing Then Throw New ArgumentException(mensaje)

        Dim seleccion As New Dictionary(Of Integer, Integer)()
        For Each ejemplar In ejemplares
            If ejemplar Is Nothing OrElse seleccion.ContainsKey(ejemplar.IdLibro) Then Throw New ArgumentException(mensaje)
            seleccion.Add(ejemplar.IdLibro, ejemplar.IdEjemplar)
        Next
        Return solicitudes.entregar(idSolicitud, seleccion).ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ObtenerComprobantePrestamo(idPrestamo As Integer, devolucion As Boolean) As ComprobanteBiblioteca
        Return New comprobanteNegocio().PorPrestamo(idPrestamo, devolucion)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ObtenerComprobanteSolicitud(idSolicitud As Integer) As ComprobanteBiblioteca
        Return New comprobanteNegocio().PorSolicitud(idSolicitud)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListarMisAvisos() As AvisoLector()
        Return New avisoLectorNegocio().ConsultarMios().ToArray()
    End Function
End Class
