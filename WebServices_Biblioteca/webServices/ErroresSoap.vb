Option Strict On

Imports System.Web.Services.Protocols

Public NotInheritable Class ErroresSoap
    Inherits SoapExtension

    Public Overrides Function GetInitializer(tipo As Type) As Object
        Return Nothing
    End Function

    Public Overrides Function GetInitializer(metodo As LogicalMethodInfo, atributo As SoapExtensionAttribute) As Object
        Return Nothing
    End Function

    Public Overrides Sub Initialize(inicializador As Object)
    End Sub

    Public Overrides Sub ProcessMessage(mensaje As SoapMessage)
        If mensaje.Stage <> SoapMessageStage.BeforeSerialize Then Return

        Dim errorOperacion = mensaje.Exception?.InnerException
        If errorOperacion Is Nothing Then Return

        If TypeOf errorOperacion Is UnauthorizedAccessException OrElse
            TypeOf errorOperacion Is ArgumentException OrElse TypeOf errorOperacion Is InvalidOperationException Then
            mensaje.Exception = New SoapException(errorOperacion.Message, SoapException.ClientFaultCode)
        Else
            HttpContext.Current.Trace.Warn("Lecturae SOAP", "Error al ejecutar la operación.", errorOperacion)
            mensaje.Exception = New SoapException("No se pudo completar la operación. Revisa la conexión e inténtalo nuevamente.",
                                                   SoapException.ServerFaultCode)
        End If
    End Sub
End Class
