Imports System.Globalization
Imports System.ServiceModel
Imports System.Web.Security
Imports appWEB_Distribuidos.Librerias

' Estado de formulario firmado y ligado a la sesión; nunca identifica al operador desde el navegador.
Public Class PaginaBase
    Inherits System.Web.UI.Page

    Protected ReadOnly Property Servicio As swBibliotecaSoapClient
        Get
            Return ClienteSoap.Actual()
        End Get
    End Property

    Protected Overrides Sub OnInit(e As EventArgs)
        If Session("Formulario.Clave") Is Nothing Then Session("Formulario.Clave") = Guid.NewGuid().ToString("N")
        ViewStateUserKey = CStr(Session("Formulario.Clave"))
        MyBase.OnInit(e)
        Response.Cache.SetCacheability(HttpCacheability.NoCache)
        Response.Cache.SetNoStore()
    End Sub

    Protected Sub MensajeError(control As Label, ex As Exception)
        Dim fallo = TryCast(ex, FaultException)
        If fallo IsNot Nothing AndAlso (fallo.Code.IsSenderFault OrElse fallo.Code.Name = "Client") Then
            Dim mensaje = fallo.Reason.ToString().Split({ControlChars.Cr, ControlChars.Lf}, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()
            Const prefijo As String = "System.Web.Services.Protocols.SoapException: "
            If mensaje IsNot Nothing AndAlso mensaje.StartsWith(prefijo, StringComparison.Ordinal) Then mensaje = mensaje.Substring(prefijo.Length)
            control.Text = Server.HtmlEncode(If(mensaje, "No se pudo completar la operación."))
        ElseIf TypeOf ex Is EndpointNotFoundException OrElse TypeOf ex Is TimeoutException Then
            control.Text = "No se pudo conectar al servicio de biblioteca. Comprueba que WebServices_Biblioteca esté en ejecución."
        ElseIf TypeOf ex Is Security.SecurityNegotiationException OrElse TypeOf ex Is Security.MessageSecurityException Then
            control.Text = "No se pudo validar el certificado del servicio de biblioteca."
        ElseIf TypeOf ex Is UnauthorizedAccessException OrElse TypeOf ex Is ArgumentException OrElse TypeOf ex Is InvalidOperationException Then
            control.Text = Server.HtmlEncode(ex.Message)
        Else
            control.Text = "No se pudo completar la operación. Revisa la conexión e inténtalo nuevamente."
        End If
        control.CssClass = "message error"
        control.Visible = True
    End Sub

    Protected Sub Ejecutar(control As Label, accion As Action)
        control.Visible = False
        Try
            accion()
        Catch ex As Exception
            MensajeError(control, ex)
        End Try
    End Sub

    Protected Sub Exito(control As Label, texto As String)
        control.Text = Server.HtmlEncode(texto)
        control.CssClass = "message success"
        control.Visible = True
    End Sub

    Protected Function Entero(texto As String, Optional minimo As Integer = 0, Optional maximo As Integer = Integer.MaxValue) As Integer
        Dim valor As Integer
        If Not Integer.TryParse(texto, valor) OrElse valor < minimo OrElse valor > maximo Then Throw New ArgumentException("Ingresa un número entero entre " & minimo & " y " & maximo & ".")
        Return valor
    End Function

    Protected Function FechaOpcional(texto As String) As Date?
        If String.IsNullOrWhiteSpace(texto) Then Return Nothing
        Dim fecha As Date
        If Not Date.TryParseExact(texto, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, fecha) Then Throw New ArgumentException("Ingresa una fecha válida.")
        Return fecha
    End Function

    Protected Function FechaTexto(fecha As Date?) As String
        Return If(fecha.HasValue, fecha.GetValueOrDefault().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), "")
    End Function

    Protected Sub Comprobar(resultado As Boolean)
        If Not resultado Then Throw New InvalidOperationException("No se realizó el cambio. El registro puede haber cambiado, estar duplicado o estar protegido por su historial.")
    End Sub

    Protected Sub Redirigir(ruta As String)
        Response.Redirect(ResolveUrl(ruta), False)
        Context.ApplicationInstance.CompleteRequest()
    End Sub
End Class

Public Class PaginaAutenticada
    Inherits PaginaBase
    Protected Overrides Sub OnInit(e As EventArgs)
        MyBase.OnInit(e)
        If AccesoWeb.UsuarioActual(Context) Is Nothing Then
            If Request.IsAuthenticated Then AccesoWeb.Descartar(Context)
            Response.Redirect(ResolveUrl("~/Default.aspx"), True)
        End If
        Try
            AccesoWeb.ActualizarIdentidad(Context)
        Catch ex As FaultException When ex.Code.IsSenderFault OrElse ex.Code.Name = "Client"
            AccesoWeb.Descartar(Context)
            Response.Redirect(ResolveUrl("~/Default.aspx"), True)
        Catch ex As Exception When TypeOf ex Is CommunicationException OrElse TypeOf ex Is TimeoutException
            Throw New HttpException(503, "No se pudo conectar al servicio de biblioteca.")
        End Try
    End Sub

    Protected Sub Guardado(Optional texto As String = "Los cambios se guardaron correctamente.", Optional destino As String = Nothing)
        Session("Aviso." & Request.AppRelativeCurrentExecutionFilePath) = texto
        Redirigir(If(destino, Request.AppRelativeCurrentExecutionFilePath))
    End Sub

    Protected Sub MostrarAviso(control As Label)
        Dim clave = "Aviso." & Request.AppRelativeCurrentExecutionFilePath
        If Session(clave) IsNot Nothing Then
            Exito(control, CStr(Session(clave)))
            Session.Remove(clave)
        End If
    End Sub
End Class

Public Class PaginaMantenimiento
    Inherits PaginaAutenticada
    Protected Overrides Sub OnInit(e As EventArgs)
        MyBase.OnInit(e)
        If Not AccesoWeb.EsPersonal Then Throw New HttpException(403, "No tienes permiso para este mantenimiento.")
    End Sub

    Protected Property RegistroId As Integer
        Get
            Return If(ViewState("RegistroId") Is Nothing, 0, CInt(ViewState("RegistroId")))
        End Get
        Set(value As Integer)
            ViewState("RegistroId") = value
        End Set
    End Property

    Protected Property EliminarId As Integer
        Get
            Return If(ViewState("EliminarId") Is Nothing, 0, CInt(ViewState("EliminarId")))
        End Get
        Set(value As Integer)
            ViewState("EliminarId") = value
        End Set
    End Property

End Class

Public Class PaginaAdministrador
    Inherits PaginaMantenimiento
    Protected Overrides Sub OnInit(e As EventArgs)
        MyBase.OnInit(e)
        If Not AccesoWeb.EsAdministrador Then Throw New HttpException(403, "Esta página requiere una cuenta de administrador.")
    End Sub
End Class

Public Class PaginaLector
    Inherits PaginaAutenticada
    Protected Overrides Sub OnInit(e As EventArgs)
        MyBase.OnInit(e)
        If Not AccesoWeb.EsLector(AccesoWeb.UsuarioActual(Context)?.NombreRol) Then
            Throw New HttpException(403, "Esta página requiere una cuenta de lector.")
        End If
    End Sub
End Class
