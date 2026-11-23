Public Class AplicacionWeb
    Inherits System.Web.HttpApplication

    Protected Sub Application_EndRequest(sender As Object, e As EventArgs)
        ClienteSoap.CerrarSolicitud(Context)
    End Sub
End Class
