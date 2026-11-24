Option Strict On

Friend NotInheritable Class AccesoWeb
    Private Sub New()
    End Sub

    Friend Shared Function UsuarioActual(contexto As HttpContext) As UsuarioSoap
        If contexto.Session Is Nothing OrElse Not contexto.Request.IsAuthenticated Then Return Nothing
        If TryCast(contexto.Session("Acceso.Marca"), String) <> contexto.User.Identity.Name Then Return Nothing
        Return TryCast(contexto.Session("Biblioteca.Usuario"), UsuarioSoap)
    End Function

    Friend Shared Function Autenticar(contexto As HttpContext, correo As String, clave As String) As UsuarioSoap
        If correo.Length > 100 OrElse clave.Length > 128 Then Return Nothing
        Limitar(contexto, correo, "login")
        contexto.Session.Clear()
        Dim usuario = ServicioBiblioteca.Cliente.AutenticarWeb(correo, clave)
        If usuario Is Nothing Then Return Nothing
        contexto.Session("Biblioteca.Usuario") = usuario
        Dim marca = Guid.NewGuid().ToString("N")
        contexto.Session("Acceso.Marca") = marca
        Dim cookie = FormsAuthentication.GetAuthCookie(marca, False)
        cookie.HttpOnly = True
        cookie.Secure = contexto.Request.IsSecureConnection
        cookie.SameSite = SameSiteMode.Lax
        contexto.Response.Cookies.Set(cookie)
        Return usuario
    End Function

    Friend Shared Sub ActualizarIdentidad(contexto As HttpContext)
        Dim anterior = UsuarioActual(contexto)
        If anterior Is Nothing Then Throw New UnauthorizedAccessException("Inicia sesión para continuar.")
        Dim actual As UsuarioSoap
        Try
            actual = ServicioBiblioteca.Cliente.ObtenerSesion()
        Catch ex As System.ServiceModel.FaultException When ex.Code.IsSenderFault OrElse ex.Code.Name = "Client"
            Throw New UnauthorizedAccessException("Tu sesión venció. Vuelve a iniciar sesión.", ex)
        End Try
        If actual Is Nothing OrElse actual.IdUsuario <> anterior.IdUsuario Then Throw New UnauthorizedAccessException("Vuelve a iniciar sesión.")
        contexto.Session("Biblioteca.Usuario") = actual
    End Sub

    Private Class Intento
        Public Cantidad As Integer
    End Class
    Public Shared Sub Limitar(contexto As HttpContext, correo As String, accion As String)
        Dim clave As String
        Using sha = System.Security.Cryptography.SHA256.Create()
            clave = "Intentos." & accion & Convert.ToBase64String(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(contexto.Request.UserHostAddress & "|" & If(correo, "").Trim().ToUpperInvariant())))
        End Using
        SyncLock GetType(AccesoWeb)
            Dim contador = TryCast(HttpRuntime.Cache(clave), Intento)
            If contador Is Nothing Then
                contador = New Intento()
                HttpRuntime.Cache.Insert(clave, contador, Nothing, DateTime.Now.AddMinutes(15), System.Web.Caching.Cache.NoSlidingExpiration)
            End If
            contador.Cantidad += 1
            If contador.Cantidad > 10 Then Throw New InvalidOperationException("Demasiados intentos. Espera quince minutos antes de volver a intentarlo.")
        End SyncLock
    End Sub

    Friend Shared Sub Salir(contexto As HttpContext)
        Try
            ServicioBiblioteca.Cliente.CerrarSesion()
        Catch ex As Exception When TypeOf ex Is System.ServiceModel.CommunicationException OrElse TypeOf ex Is TimeoutException
            contexto.Trace.Warn("Libreria", "No se pudo cerrar la sesión remota.", ex)
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
