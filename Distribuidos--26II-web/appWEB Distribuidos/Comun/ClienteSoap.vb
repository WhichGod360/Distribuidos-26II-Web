Option Strict On

Imports System.Net
Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports appWEB_Distribuidos.Librerias

Friend NotInheritable Class ClienteSoap
    Private Const ClaveSolicitud As String = "Lecturae.ClienteSoap"

    Private Sub New()
    End Sub

    Friend Shared Function Crear(cookies As CookieContainer) As swBibliotecaSoapClient
        Dim cliente As New swBibliotecaSoapClient()
        Try
            Dim enlace = DirectCast(cliente.Endpoint.Binding, BasicHttpBinding)
            enlace.AllowCookies = True
            Dim configurada = Environment.GetEnvironmentVariable("BIBLIOTECA_SOAP_URL")
            If Not String.IsNullOrWhiteSpace(configurada) Then
                Dim destino As Uri = Nothing
                If Not Uri.TryCreate(configurada, UriKind.Absolute, destino) OrElse
                    (destino.Scheme <> Uri.UriSchemeHttp AndAlso destino.Scheme <> Uri.UriSchemeHttps) Then
                    Throw New InvalidOperationException("La dirección del servicio de biblioteca no es válida.")
                End If
                cliente.Endpoint.Address = New EndpointAddress(destino)
                enlace.Security.Mode =
                    If(destino.Scheme = Uri.UriSchemeHttps, BasicHttpSecurityMode.Transport, BasicHttpSecurityMode.None)
            End If
            Dim canal = cliente.InnerChannel.GetProperty(Of IHttpCookieContainerManager)()
            If canal Is Nothing Then Throw New InvalidOperationException("El cliente SOAP requiere cookies habilitadas.")
            canal.CookieContainer = cookies
            Return cliente
        Catch
            cliente.Abort()
            Throw
        End Try
    End Function

    Friend Shared Function Actual() As swBibliotecaSoapClient
        Dim contexto = HttpContext.Current
        Dim cliente = TryCast(contexto.Items(ClaveSolicitud), swBibliotecaSoapClient)
        If cliente IsNot Nothing AndAlso cliente.State = CommunicationState.Faulted Then
            cliente.Abort()
            cliente = Nothing
        End If
        If cliente Is Nothing Then
            cliente = Crear(If(AccesoWeb.CookiesActuales(contexto), New CookieContainer()))
            contexto.Items(ClaveSolicitud) = cliente
        End If
        Return cliente
    End Function

    Friend Shared Sub CerrarSolicitud(contexto As HttpContext)
        Cerrar(TryCast(contexto.Items(ClaveSolicitud), swBibliotecaSoapClient))
        contexto.Items.Remove(ClaveSolicitud)
    End Sub

    Friend Shared Sub Cerrar(cliente As swBibliotecaSoapClient)
        If cliente Is Nothing Then Return
        Try
            If cliente.State = CommunicationState.Faulted Then
                cliente.Abort()
            Else
                cliente.Close()
            End If
        Catch ex As Exception When TypeOf ex Is CommunicationException OrElse TypeOf ex Is TimeoutException
            cliente.Abort()
        End Try
    End Sub
End Class
