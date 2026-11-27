Option Strict On

Imports System.Net.Http
Imports System.Threading.Tasks
Imports System.Web.Script.Serialization

Friend Module RecaptchaWeb
    Private Const SiteVerify As String = "https://www.google.com/recaptcha/api/siteverify"
    Private ReadOnly Cliente As New HttpClient(New HttpClientHandler With {.AllowAutoRedirect = False, .UseCookies = False}) With {
        .Timeout = TimeSpan.FromSeconds(8), .MaxResponseContentBufferSize = 8192
    }
    Private ReadOnly Publica As String = Configuracion("SITE_KEY", "SiteKey")
    Private ReadOnly Secreta As String = Configuracion("SECRET_KEY", "SecretKey")

    Private Function Configuracion(variable As String, clave As String) As String
        Return If(Environment.GetEnvironmentVariable("BIBLIOTECA_RECAPTCHA_" & variable),
                  If(ConfigurationManager.AppSettings("Recaptcha." & clave), "")).Trim()
    End Function

    Friend ReadOnly Property Habilitado As Boolean
        Get
            Select Case Configuracion("ENABLED", "Enabled").ToLowerInvariant()
                Case "", "true", "1"
                    Return True
                Case "false", "0"
                    Return False
                Case Else
                    Throw New InvalidOperationException("Recaptcha.Enabled debe ser true o false.")
            End Select
        End Get
    End Property

    Friend Function ClavePublica(contexto As HttpContext) As String
        If Publica.Length = 0 OrElse Secreta.Length = 0 Then
            Throw New InvalidOperationException("Faltan las claves de reCAPTCHA.")
        End If
        If Not contexto.Request.IsLocal AndAlso (Publica = ClavePublicaPrueba OrElse Secreta = ClaveSecretaPrueba) Then
            Throw New InvalidOperationException("Las claves de prueba de reCAPTCHA son solo para uso local.")
        End If
        Return Publica
    End Function

    ' Claves públicas de prueba de Google. Sus respuestas usan un dominio de prueba.
    Private Const ClavePublicaPrueba As String = "6LeIxAcTAAAAAJcZVRqyHh71UMIEGNQ_MXjiZKhI"
    Private Const ClaveSecretaPrueba As String = "6LeIxAcTAAAAAGG-vFI1TnRWxMZNFuojJ4WifJWe"

    Friend Async Function ValidarAsync(contexto As HttpContext, token As String) As Task
        If Not Habilitado Then Return
        If String.IsNullOrWhiteSpace(token) OrElse token.Length > 8192 Then
            Throw New ArgumentException("Completa la verificación «No soy un robot» antes de ingresar.")
        End If

        Dim campos As New Dictionary(Of String, String) From {{"secret", Secreta}, {"response", token}}
        Dim respuesta As Dictionary(Of String, Object)
        Try
            Using contenido As New FormUrlEncodedContent(campos),
                  http = Await Cliente.PostAsync(SiteVerify, contenido)
                http.EnsureSuccessStatusCode()
                Dim json = Await http.Content.ReadAsStringAsync()
                respuesta = TryCast(New JavaScriptSerializer().DeserializeObject(json), Dictionary(Of String, Object))
            End Using
        Catch ex As Exception When TypeOf ex Is HttpRequestException OrElse TypeOf ex Is OperationCanceledException OrElse
                                  TypeOf ex Is ArgumentException OrElse TypeOf ex Is InvalidOperationException
            Throw New InvalidOperationException("No se pudo verificar el CAPTCHA. Inténtalo nuevamente en unos momentos.", ex)
        End Try

        Dim valor As Object = Nothing
        If respuesta Is Nothing OrElse Not respuesta.TryGetValue("success", valor) OrElse Not Object.Equals(valor, True) Then
            Throw New ArgumentException("La verificación del CAPTCHA no es válida o venció. Complétala nuevamente.")
        End If
        If respuesta.TryGetValue("error-codes", valor) AndAlso
            (Not TypeOf valor Is Object() OrElse DirectCast(valor, Object()).Length > 0) Then
            Throw New InvalidOperationException("Google no pudo verificar el CAPTCHA. Inténtalo nuevamente.")
        End If
        Dim pruebaLocal = contexto.Request.IsLocal AndAlso Publica = ClavePublicaPrueba AndAlso Secreta = ClaveSecretaPrueba
        If Not pruebaLocal AndAlso (Not respuesta.TryGetValue("hostname", valor) OrElse
            Not String.Equals(TryCast(valor, String), contexto.Request.Url.DnsSafeHost, StringComparison.OrdinalIgnoreCase)) Then
            Throw New ArgumentException("La verificación del CAPTCHA no corresponde a este sitio. Recarga la página.")
        End If
    End Function
End Module
