Public Class AplicacionWeb
    Inherits System.Web.HttpApplication

    Protected Sub Application_Start(sender As Object, e As EventArgs)
        ' Cada cliente WCF conserva únicamente las cookies de su sesión web.
        System.ServiceModel.ClientBase(Of Libreria.swBibliotecaSoap).CacheSetting = System.ServiceModel.CacheSetting.AlwaysOff
    End Sub

    Protected Sub Application_EndRequest(sender As Object, e As EventArgs)
        ServicioBiblioteca.Cerrar(Context)
    End Sub
End Class
