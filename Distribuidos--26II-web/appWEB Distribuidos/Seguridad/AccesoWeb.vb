Option Strict On

Imports System.Net
Imports System.ServiceModel
Imports System.Web.Security
Imports appWEB_Distribuidos.Librerias

Friend NotInheritable Class AccesoWeb
    Private Const ClaveSesion As String = "Lecturae.SesionSoap"

    Private Sub New()
    End Sub

    Private Class SesionAutenticada
        Public Property Marca As String
        Public Property Usuario As UsuarioSoap
        Public Property Cookies As CookieContainer
    End Class

    Private Shared Function SesionActual(contexto As HttpContext) As SesionAutenticada
        If contexto Is Nothing OrElse contexto.Session Is Nothing OrElse Not contexto.Request.IsAuthenticated Then Return Nothing
        Dim sesion = TryCast(contexto.Session(ClaveSesion), SesionAutenticada)
        If sesion Is Nothing OrElse sesion.Marca <> contexto.User.Identity.Name Then Return Nothing
        Return sesion
    End Function

    Friend Shared Function UsuarioActual(contexto As HttpContext) As UsuarioSoap
        Return SesionActual(contexto)?.Usuario
    End Function

    Friend Shared Function CookiesActuales(contexto As HttpContext) As CookieContainer
        Return SesionActual(contexto)?.Cookies
    End Function

    Friend Shared Function Autenticar(contexto As HttpContext, correo As String, clave As String) As UsuarioSoap
        Dim cookies As New CookieContainer()
        Dim cliente = ClienteSoap.Crear(cookies)
        Try
            Dim usuario = cliente.AutenticarWeb(correo, clave)
            If usuario Is Nothing Then Return Nothing
            Dim marca = Guid.NewGuid().ToString("N")
            Dim cookie = FormsAuthentication.GetAuthCookie(marca, False)
            cookie.HttpOnly = True
            cookie.Secure = contexto.Request.IsSecureConnection
            cookie.SameSite = SameSiteMode.Lax
            contexto.Session.Clear()
            contexto.Session(ClaveSesion) = New SesionAutenticada With {.Marca = marca, .Usuario = usuario, .Cookies = cookies}
            contexto.Response.Cookies.Set(cookie)
            Return usuario
        Finally
            ClienteSoap.Cerrar(cliente)
        End Try
    End Function

    Friend Shared Sub ActualizarIdentidad(contexto As HttpContext)
        Dim sesion = SesionActual(contexto)
        If sesion Is Nothing Then Throw New UnauthorizedAccessException("Inicia sesión para continuar.")
        Dim usuario = ClienteSoap.Actual().ObtenerSesion()
        If usuario Is Nothing OrElse usuario.IdUsuario <> sesion.Usuario.IdUsuario Then
            Throw New UnauthorizedAccessException("Inicia sesión para continuar.")
        End If
        sesion.Usuario = usuario
    End Sub

    Friend Shared Sub Salir(contexto As HttpContext)
        Try
            If SesionActual(contexto) IsNot Nothing Then ClienteSoap.Actual().CerrarSesion()
        Catch ex As Exception When TypeOf ex Is CommunicationException OrElse TypeOf ex Is TimeoutException
            contexto.Trace.Warn("Lecturae SOAP", "No se pudo terminar la sesión en el servicio.", ex)
        Finally
            Descartar(contexto)
        End Try
    End Sub

    Friend Shared Sub Descartar(contexto As HttpContext)
        FormsAuthentication.SignOut()
        contexto.Session.Clear()
        contexto.Session.Abandon()
    End Sub

    Friend Shared Function EsLector(rol As String) As Boolean
        Return {"ESTUDIANTE", "LECTOR"}.Contains(If(rol, "").Trim().ToUpperInvariant())
    End Function

    Friend Shared ReadOnly Property EsAdministrador As Boolean
        Get
            Return String.Equals(UsuarioActual(HttpContext.Current)?.NombreRol, "Administrador", StringComparison.OrdinalIgnoreCase)
        End Get
    End Property

    Friend Shared ReadOnly Property EsPersonal As Boolean
        Get
            Return EsAdministrador OrElse String.Equals(UsuarioActual(HttpContext.Current)?.NombreRol, "Bibliotecario", StringComparison.OrdinalIgnoreCase)
        End Get
    End Property
End Class
