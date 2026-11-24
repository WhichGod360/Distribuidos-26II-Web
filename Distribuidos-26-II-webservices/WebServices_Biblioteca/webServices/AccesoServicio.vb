Imports System.Web.Security

Imports capaDatos

Public NotInheritable Class AccesoWeb
    Private Sub New()
    End Sub

    Public Shared Sub Autenticar(contexto As HttpContext)
        Dim marca = Guid.NewGuid().ToString("N")
        contexto.Session("Acceso.Marca") = marca
        Dim cookie = FormsAuthentication.GetAuthCookie(marca, False)
        cookie.HttpOnly = True
        cookie.Secure = contexto.Request.IsSecureConnection
        cookie.SameSite = SameSiteMode.Lax
        contexto.Response.Cookies.Set(cookie)
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


