Option Strict On

Imports capaDatos

Public Class UsuarioSoap
    Public Property IdUsuario As Integer
    Public Property NombreUsuario As String
    Public Property ApPaterno As String
    Public Property ApMaterno As String
    Public Property NombreCompleto As String
    Public Property EmailUsuario As String
    Public Property NumeroDniUsuario As String
    Public Property FechaNacimientoUsuario As Date?
    Public Property GeneroUsuario As Boolean?
    Public Property EstadoUsuario As String
    Public Property IdRolUsuario As Integer
    Public Property NombreRol As String

    Friend Shared Function Desde(usuario As Usuario) As UsuarioSoap
        If usuario Is Nothing Then Return Nothing
        Return New UsuarioSoap With {
            .IdUsuario = usuario.IdUsuario, .NombreUsuario = usuario.NombreUsuario,
            .ApPaterno = usuario.ApPaterno, .ApMaterno = usuario.ApMaterno, .NombreCompleto = usuario.NombreCompleto,
            .EmailUsuario = usuario.EmailUsuario, .NumeroDniUsuario = usuario.NumeroDniUsuario,
            .FechaNacimientoUsuario = usuario.FechaNacimientoUsuario, .GeneroUsuario = usuario.GeneroUsuario,
            .EstadoUsuario = usuario.EstadoUsuario, .IdRolUsuario = usuario.IdRolUsuario, .NombreRol = usuario.NombreRol}
    End Function
End Class

Public Class EjemplarEntregaSoap
    Public Property IdLibro As Integer
    Public Property IdEjemplar As Integer
End Class

Public Class BibliotecaLectorSoap
    Public Property Catalogo As Libro()
    Public Property Reservas As SolicitudPrestamo()
    Public Property Prestamos As Prestamo()
    Public Property Cargos As ResumenPenalizaciones
    Public Property Circulacion As EstadoCirculacionLector
    Public Property PuedeSolicitar As Boolean
    Public Property LibrosAgregables As Integer()
End Class

Public Class ResultadoPagoDemoSoap
    Public Property IdCargo As Integer
    Public Property PagoNuevo As Boolean
End Class
