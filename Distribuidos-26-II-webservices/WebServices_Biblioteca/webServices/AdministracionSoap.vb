Option Strict On

Imports System.Web.Services
Imports capaDatos
Imports capaNegocio

Partial Public Class swBiblioteca
    Private ReadOnly negocioBitacora As New bitacoraNegocio()
    Private ReadOnly negocioDashboard As New dashboardNegocio()

    <WebMethod(EnableSession:=True)>
    Public Function ListarUsuarios(texto As String, idRol As Integer, estado As String) As UsuarioSoap()
        Return usuarios.ListarUsuarios(texto, idRol, If(estado, "Todos")).Select(AddressOf UsuarioSoap.Desde).ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ObtenerUsuario(idUsuario As Integer) As UsuarioSoap
        Return UsuarioSoap.Desde(usuarios.ObtenerUsuario(idUsuario))
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListarRoles() As RolUsuario()
        Return usuarios.ListarRoles().ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Sub CrearUsuario(usuario As Usuario, confirmacionClave As String)
        ValidarFechaNacimiento(usuario)
        usuarios.CrearUsuario(usuario, confirmacionClave)
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Sub ActualizarUsuario(usuario As Usuario)
        ValidarFechaNacimiento(usuario)
        usuarios.ActualizarUsuario(usuario)
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Sub CambiarEstadoUsuario(idUsuario As Integer, estado As String)
        usuarios.CambiarEstadoUsuario(idUsuario, estado)
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Function ListarLectoresPendientes(texto As String) As UsuarioSoap()
        Return usuarios.ListarLectoresPendientes(texto).Select(AddressOf UsuarioSoap.Desde).ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Sub AprobarLector(idUsuario As Integer)
        usuarios.AprobarLector(idUsuario)
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Sub RechazarLector(idUsuario As Integer)
        usuarios.RechazarLector(idUsuario)
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Function ListarBitacora(fechaDesde As Date?, fechaHasta As Date?, idUsuario As Integer?,
                                   objeto As String, accion As String, despuesDeId As Integer?) As Bitacora()
        Return negocioBitacora.Listar(fechaDesde, fechaHasta, idUsuario, objeto, accion, despuesDeId).ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ObtenerMovimientoBitacora(idMovimiento As Integer) As Bitacora
        Return negocioBitacora.ObtenerPorId(idMovimiento)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ConsultarIndicadores(desde As Date, hasta As Date, idCategoria As Integer, estado As String) As DashboardResumen
        Return negocioDashboard.consultar(desde, hasta, idCategoria, If(estado, "Todos"))
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListarCategoriasIndicadores() As Categoria()
        Return negocioDashboard.categorias().ToArray()
    End Function
End Class
