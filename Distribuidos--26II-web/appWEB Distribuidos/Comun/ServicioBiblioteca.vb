Option Strict On

Imports System.Net
Imports System.ServiceModel
Imports System.ServiceModel.Channels

Friend Module ServicioBiblioteca
    Private Const ClaveCliente As String = "Libreria.Cliente"
    Private Const ClaveCookies As String = "Libreria.Cookies"

    Friend ReadOnly Property Cliente As Libreria.swBibliotecaSoapClient
        Get
            Dim contexto = HttpContext.Current
            Dim actual = TryCast(contexto.Items(ClaveCliente), Libreria.swBibliotecaSoapClient)
            If actual IsNot Nothing Then Return actual

            actual = New Libreria.swBibliotecaSoapClient("swBibliotecaSoap")
            Try
                Dim sesion = contexto.Session
                If sesion IsNot Nothing Then
                    Dim cookies = TryCast(sesion(ClaveCookies), CookieContainer)
                    If cookies Is Nothing Then
                        cookies = New CookieContainer()
                        sesion(ClaveCookies) = cookies
                    End If
                    actual.InnerChannel.GetProperty(Of IHttpCookieContainerManager)().CookieContainer = cookies
                End If
                contexto.Items(ClaveCliente) = actual
                Return actual
            Catch
                actual.Abort()
                Throw
            End Try
        End Get
    End Property

    Friend Sub Cerrar(contexto As HttpContext)
        Dim actual = TryCast(contexto.Items(ClaveCliente), Libreria.swBibliotecaSoapClient)
        If actual Is Nothing Then Return
        contexto.Items.Remove(ClaveCliente)
        Try
            If actual.State = CommunicationState.Faulted Then
                actual.Abort()
            Else
                actual.Close()
            End If
        Catch ex As CommunicationException
            actual.Abort()
        Catch ex As TimeoutException
            actual.Abort()
        End Try
    End Sub
End Module
