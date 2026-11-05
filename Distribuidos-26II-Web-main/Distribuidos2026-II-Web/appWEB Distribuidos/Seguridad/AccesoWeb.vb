Imports System.Web.Security
Imports System.Web.Script.Serialization
Imports capaDatos

Public NotInheritable Class AccesoWeb
    Private Sub New()
    End Sub

    Public Shared Sub Autenticar(contexto As HttpContext, usuario As Usuario)
        ' El ticket contiene solo identidad mínima protegida por Forms Authentication.
        Dim identidad As New Usuario With {.IdUsuario = usuario.IdUsuario, .NombreUsuario = usuario.NombreUsuario,
            .ApPaterno = usuario.ApPaterno, .ApMaterno = usuario.ApMaterno, .NombreRol = usuario.NombreRol}
        Dim ticket As New FormsAuthenticationTicket(2, usuario.IdUsuario.ToString(), Date.Now, Date.Now.AddMinutes(30), False,
            New JavaScriptSerializer().Serialize(identidad), FormsAuthentication.FormsCookiePath)
        Dim cookie As New HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket)) With {
            .HttpOnly = True, .Secure = contexto.Request.IsSecureConnection, .Path = FormsAuthentication.FormsCookiePath,
            .SameSite = SameSiteMode.Lax}
        contexto.Response.Cookies.Add(cookie)
        ' Se abandona la sesión anónima para evitar conservar su identidad de sesión.
        contexto.Session.Clear()
        contexto.Session.Abandon()
    End Sub

    Public Shared Sub Restaurar(contexto As HttpContext)
        If contexto.Session Is Nothing OrElse Not contexto.Request.IsAuthenticated Then Return
        If SesionUsuario.IdUsuario > 0 Then Return
        Dim identidad = TryCast(contexto.User.Identity, FormsIdentity)
        If identidad Is Nothing OrElse identidad.Ticket.Expired Then Return
        Try
            Dim usuario = New JavaScriptSerializer().Deserialize(Of Usuario)(identidad.Ticket.UserData)
            If usuario Is Nothing OrElse usuario.IdUsuario <= 0 OrElse usuario.IdUsuario.ToString() <> identidad.Name Then Throw New InvalidOperationException()
            Dim almacen As New AlmacenSesionWeb()
            almacen.Actual = usuario
        Catch
            FormsAuthentication.SignOut()
        End Try
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

    Public Shared Sub Salir(contexto As HttpContext)
        SesionUsuario.Cerrar()
        FormsAuthentication.SignOut()
        contexto.Session.Clear()
        contexto.Session.Abandon()
    End Sub
End Class


