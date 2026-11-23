Imports System.Drawing.Imaging
Imports System.IO
Imports System.ServiceModel
Imports System.Web.SessionState
Imports QRCoder

Public NotInheritable Class ImagenYape
    Implements IHttpHandler, IRequiresSessionState

    Private Shared Function Ruta(contexto As HttpContext) As String
        Return contexto.Server.MapPath("~/App_Data/YapeQr.png")
    End Function

    Friend Shared Function Configurada(contexto As HttpContext) As Boolean
        Return File.Exists(Ruta(contexto))
    End Function

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

    Public Sub ProcessRequest(contexto As HttpContext) Implements IHttpHandler.ProcessRequest
        If Not AccesoWeb.EsLector(AccesoWeb.UsuarioActual(contexto)?.NombreRol) Then
            Throw New HttpException(403, "El QR está disponible para lectores.")
        End If
        If Not Configurada(contexto) OrElse Not ClienteSoap.Actual().PagoYapeDisponible() Then
            Throw New HttpException(404, "QR de Yape no disponible.")
        End If

        contexto.Response.Cache.SetNoStore()
        contexto.Response.ContentType = "image/png"
        Using archivo = File.OpenRead(Ruta(contexto)),
              codigo = RecorteQrYape.ParaMostrar(archivo),
              png As New MemoryStream()
            codigo.Save(png, ImageFormat.Png)
            png.WriteTo(contexto.Response.OutputStream)
        End Using
    End Sub
End Class

Public NotInheritable Class PagoDemoWeb
    Implements IHttpHandler

    Friend Shared Function CrearEnlace(contexto As HttpContext, token As String) As String
        Dim configurada = Environment.GetEnvironmentVariable("BIBLIOTECA_DEMO_URL")
        Dim origen As Uri = Nothing
        If String.IsNullOrWhiteSpace(configurada) Then
            origen = New UriBuilder(contexto.Request.Url.Scheme, "localhost", contexto.Request.Url.Port).Uri
        ElseIf Not Uri.TryCreate(configurada, UriKind.Absolute, origen) OrElse
            Not {"http", "https"}.Contains(origen.Scheme) OrElse origen.UserInfo.Length > 0 Then
            Throw New InvalidOperationException("La dirección del QR de demostración no es válida.")
        End If
        Return New Uri(origen, VirtualPathUtility.ToAbsolute("~/Lector/PagoDemo.ashx") & "?t=" & token).AbsoluteUri
    End Function

    Friend Shared Function Imagen(destino As String) As String
        Using generador As New QRCodeGenerator()
            Using datos = generador.CreateQrCode(destino, QRCodeGenerator.ECCLevel.M)
                Using qr As New PngByteQRCode(datos)
                    Return "data:image/png;base64," & Convert.ToBase64String(qr.GetGraphic(5))
                End Using
            End Using
        End Using
    End Function

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

    Public Sub ProcessRequest(contexto As HttpContext) Implements IHttpHandler.ProcessRequest
        Dim respuesta = contexto.Response
        respuesta.Cache.SetCacheability(HttpCacheability.NoCache)
        respuesta.Cache.SetNoStore()
        respuesta.Headers("Referrer-Policy") = "no-referrer"
        respuesta.Headers("X-Frame-Options") = "DENY"
        respuesta.Headers("X-Robots-Tag") = "noindex, nofollow"
        respuesta.Headers("Content-Security-Policy") = "default-src 'none'; style-src 'self'; script-src 'self'; form-action 'self'; base-uri 'none'; frame-ancestors 'none'"
        respuesta.TrySkipIisCustomErrors = True
        respuesta.ContentType = "text/html; charset=utf-8"

        Try
            Dim metodo = contexto.Request.HttpMethod
            If metodo <> "GET" AndAlso metodo <> "POST" Then
                respuesta.StatusCode = 405
                respuesta.Headers("Allow") = "GET, POST"
                Mostrar(contexto, "Abre el QR en el navegador.")
                Return
            End If
            Dim token = If(metodo = "POST", contexto.Request.Form("t"), contexto.Request.QueryString("t"))
            Dim resultado = ClienteSoap.Actual().ProcesarPagoDemo(token, metodo = "POST")
            If resultado Is Nothing Then
                respuesta.StatusCode = 404
                Mostrar(contexto, "Demostración no disponible.")
                Return
            End If

            If metodo = "GET" Then
                Dim ruta = VirtualPathUtility.ToAbsolute("~/Lector/PagoDemo.ashx")
                Dim script = VirtualPathUtility.ToAbsolute("~/Scripts/pago-demo.js")
                Dim formulario = "<form method=""post"" id=""simularPago"" action=""" &
                    HttpUtility.HtmlAttributeEncode(ruta) & """><input type=""hidden"" name=""t"" value=""" &
                    HttpUtility.HtmlAttributeEncode(token) & """ /><button type=""submit"">Confirmar demostración</button></form>" &
                    "<script src=""" & HttpUtility.HtmlAttributeEncode(script) & """ defer></script>"
                Mostrar(contexto, "Confirmando el pago simulado. No se transferirá dinero.", formulario)
                Return
            End If

            Mostrar(contexto, If(resultado.PagoNuevo, $"Pago simulado confirmado. El cargo N.º {resultado.IdCargo} figura como pagado.",
                $"El pago simulado del cargo N.º {resultado.IdCargo} ya estaba confirmado."))
        Catch ex As FaultException When ex.Code.IsSenderFault OrElse ex.Code.Name = "Client"
            respuesta.StatusCode = 400
            Mostrar(contexto, "El QR ya venció o no se puede confirmar. Genera otro desde Mis penalizaciones.")
        Catch ex As Exception
            respuesta.StatusCode = 503
            Mostrar(contexto, "No se pudo completar la demostración. Revisa la conexión de prueba.")
        End Try
    End Sub

    Private Shared Sub Mostrar(contexto As HttpContext, mensaje As String, Optional formulario As String = "")
        Dim estilo = VirtualPathUtility.ToAbsolute("~/Estilos/biblioteca.css")
        contexto.Response.Write("<!doctype html><html lang=""es""><head><meta charset=""utf-8"">" &
            "<meta name=""viewport"" content=""width=device-width,initial-scale=1""><title>Pago de demostración</title>" &
            "<link rel=""stylesheet"" href=""" & HttpUtility.HtmlAttributeEncode(estilo) & """></head>" &
            "<body><main class=""editor-panel""><h1>Pago de demostración</h1><p>" &
            HttpUtility.HtmlEncode(mensaje) & "</p>" & formulario & "</main></body></html>")
    End Sub
End Class
