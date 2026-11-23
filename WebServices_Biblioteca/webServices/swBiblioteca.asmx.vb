Option Strict On

Imports System.ComponentModel
Imports System.Web.Services
Imports capaDatos
Imports capaNegocio

<WebService(Namespace:="http://tempuri.org/", Description:="Operaciones de Lecturae")>
<WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)>
<ToolboxItem(False)>
Partial Public Class swBiblioteca
    Inherits WebService

    Private ReadOnly usuarios As New usuarioNegocio()

    <WebMethod(Description:="Comprueba el arranque del servicio.")>
    Public Function HelloWorld() As String
        Return "Hola a todos"
    End Function

    <WebMethod(EnableSession:=True, Description:="Valida el correo del usuario y su contraseña.")>
    Public Function iniciarSesion(usu As String, con As String) As Boolean
        Return AutenticarWeb(usu, con) IsNot Nothing
    End Function

    <WebMethod(EnableSession:=True, Description:="Inicia sesión y devuelve el nombre y el rol para la web.")>
    Public Function AutenticarWeb(usu As String, con As String) As UsuarioSoap
        If String.IsNullOrWhiteSpace(usu) OrElse String.IsNullOrWhiteSpace(con) OrElse
            usu.Length > 100 OrElse con.Length > 128 Then
            AccesoWeb.Salir(Context)
            Return Nothing
        End If

        AccesoWeb.Limitar(Context, usu, "login")
        Dim usuario = usuarios.IniciarSesion(usu, con)
        If usuario Is Nothing Then
            AccesoWeb.Salir(Context)
            Return Nothing
        End If

        AccesoWeb.Autenticar(Context, usuario)
        Return New UsuarioSoap With {
            .IdUsuario = usuario.IdUsuario, .NombreRol = usuario.NombreRol, .NombreCompleto = usuario.NombreCompleto}
    End Function

    <WebMethod(EnableSession:=True)>
    Public Sub CerrarSesion()
        AccesoWeb.Salir(Context)
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Function ObtenerSesion() As UsuarioSoap
        Dim id = SesionUsuario.IdUsuario
        Dim rol = SesionUsuario.Rol
        If seguridadNegocio.PuedeGestionarUsuarios(id, rol) Then
            seguridadNegocio.ExigirAdministrador()
        ElseIf SesionUsuario.EsPersonal Then
            seguridadNegocio.ExigirPersonal()
        Else
            seguridadNegocio.ExigirLector()
        End If
        Return New UsuarioSoap With {.IdUsuario = id, .NombreRol = rol, .NombreCompleto = SesionUsuario.NombreCompleto}
    End Function

    <WebMethod(EnableSession:=True)>
    Public Sub RegistrarNuevoLector(usuario As Usuario, confirmacionClave As String)
        ValidarFechaNacimiento(usuario)
        AccesoWeb.Limitar(Context, usuario.EmailUsuario, "registro")
        usuarios.RegistrarNuevoLector(usuario, confirmacionClave)
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Function ObtenerPregunta(email As String) As String
        AccesoWeb.Limitar(Context, email, "pregunta")
        Return usuarios.ObtenerPregunta(email)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ValidarRespuesta(email As String, respuesta As String) As String
        AccesoWeb.Limitar(Context, email, "respuesta")
        Return usuarios.ValidarRespuesta(email, respuesta)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Sub CambiarContrasena(email As String, nuevaClave As String, confirmarClave As String, autorizacion As String)
        usuarios.CambiarContrasena(email, nuevaClave, confirmarClave, autorizacion)
    End Sub

    <WebMethod(EnableSession:=True)>
    Public Sub CancelarRecuperacion(autorizacion As String)
        usuarios.CancelarRecuperacion(autorizacion)
    End Sub

    Private Shared Function TablaSoap(tabla As DataTable, nombre As String) As DataTable
        tabla.TableName = nombre
        Return tabla
    End Function

    Private Shared Function FilaSoap(fila As DataRow, nombre As String) As DataTable
        If fila Is Nothing Then Throw New InvalidOperationException("El registro ya no está disponible.")
        Dim tabla = fila.Table.Clone()
        tabla.ImportRow(fila)
        Return TablaSoap(tabla, nombre)
    End Function

    Private Shared Sub ValidarFechaNacimiento(usuario As Usuario)
        If usuario Is Nothing Then Throw New ArgumentException("Completa los datos del usuario.")
        If Not usuario.FechaNacimientoUsuario.HasValue Then Return

        Dim zonaLima = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time")
        Dim hoy = TimeZoneInfo.ConvertTimeFromUtc(Date.UtcNow, zonaLima).Date
        Dim nacimiento = usuario.FechaNacimientoUsuario.Value.Date
        If nacimiento > hoy Then Throw New ArgumentException("La fecha de nacimiento no puede ser futura.")
        If nacimiento > hoy.AddYears(-18) Then Throw New ArgumentException("Debes tener al menos 18 años.")
    End Sub
End Class
