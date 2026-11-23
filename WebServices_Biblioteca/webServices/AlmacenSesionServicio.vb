Imports capaDatos
Imports System.Web

' Nunca conserva una identidad en campos Shared; el único estado vive en Session.
Public NotInheritable Class AlmacenSesionWeb
    Implements IAlmacenSesionUsuario

    Private Shared Function SesionActual() As System.Web.SessionState.HttpSessionState
        Dim contexto = HttpContext.Current
        If contexto Is Nothing OrElse contexto.Session Is Nothing Then
            Throw New InvalidOperationException("Esta operación requiere una solicitud con sesión habilitada.")
        End If
        Return contexto.Session
    End Function

    Public Property Actual As Usuario Implements IAlmacenSesionUsuario.Actual
        Get
            Return TryCast(SesionActual()("Biblioteca.Usuario"), Usuario)
        End Get
        Set(value As Usuario)
            If value Is Nothing Then
                SesionActual().Remove("Biblioteca.Usuario")
            Else
                SesionActual()("Biblioteca.Usuario") = value
            End If
        End Set
    End Property
End Class
