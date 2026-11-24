Public Class AplicacionWeb
    Inherits System.Web.HttpApplication

    Protected Sub Application_Start(sender As Object, e As EventArgs)
        capaDatos.SesionUsuario.ConfigurarAlmacen(New AlmacenSesionWeb())
    End Sub
End Class