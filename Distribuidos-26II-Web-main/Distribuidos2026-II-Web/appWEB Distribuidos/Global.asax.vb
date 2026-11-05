Imports System.IO

Public Class AplicacionWeb
    Inherits System.Web.HttpApplication
    Protected Sub Application_Start(sender As Object, e As EventArgs)
        ' En IIS Express SpecialFolder.LocalApplicationData puede no resolver el perfil.
        ' LOCALAPPDATA y la variable explícita del iniciador evitan depender de ese perfil.
        If String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("BIBLIOTECA_CONNECTION_FILE")) AndAlso
           String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("BIBLIOTECA_CONNECTION_STRING")) Then
            Dim perfil = Environment.GetEnvironmentVariable("LOCALAPPDATA")
            If Not String.IsNullOrWhiteSpace(perfil) Then
                Dim archivo = Path.Combine(perfil, "Biblioteca", "connection-string.txt")
                If File.Exists(archivo) Then Environment.SetEnvironmentVariable("BIBLIOTECA_CONNECTION_FILE", archivo, EnvironmentVariableTarget.Process)
            End If
        End If
        capaDatos.SesionUsuario.ConfigurarAlmacen(New AlmacenSesionWeb())
    End Sub
    Protected Sub Application_AcquireRequestState(sender As Object, e As EventArgs)
        AccesoWeb.Restaurar(Context)
    End Sub
End Class
