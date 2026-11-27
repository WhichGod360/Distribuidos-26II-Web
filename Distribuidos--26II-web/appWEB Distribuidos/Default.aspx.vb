Public Class Login
    Inherits PaginaBase

    Protected Property ClaveRecaptcha As String = ""

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If AccesoWeb.UsuarioActual(Context) IsNot Nothing Then
            Redirigir("~/wfInicio.aspx")
            Return
        End If
        Try
            captchaContenedor.Visible = RecaptchaWeb.Habilitado
            If captchaContenedor.Visible Then ClaveRecaptcha = RecaptchaWeb.ClavePublica(Context)
        Catch ex As InvalidOperationException
            captchaContenedor.Visible = False
            btnIngresar.Enabled = False
            Context.Trace.Warn("reCAPTCHA", ex.Message)
            MensajeError(lblMensaje, New InvalidOperationException("La verificación de acceso no está disponible. Inténtalo más tarde."))
            Return
        End Try

        If Not IsPostBack AndAlso Session("Registro.Creado") IsNot Nothing Then
            Session.Remove("Registro.Creado")
            Exito(lblMensaje, "Tu cuenta fue registrada y está pendiente de aprobación. Podrás iniciar sesión cuando un administrador la apruebe.")
        End If
    End Sub

    Protected Sub btnIngresar_Click(sender As Object, e As EventArgs) Handles btnIngresar.Click
        If btnIngresar.Enabled Then RegisterAsyncTask(New PageAsyncTask(AddressOf IngresarAsync))
    End Sub

    Private Async Function IngresarAsync() As System.Threading.Tasks.Task
        Try
            If String.IsNullOrWhiteSpace(txtCorreo.Text) OrElse String.IsNullOrWhiteSpace(txtClave.Text) Then
                Throw New ArgumentException("Ingresa tu correo y contraseña.")
            End If
            Await RecaptchaWeb.ValidarAsync(Context, Request.Form("g-recaptcha-response"))
            Dim usuario = AccesoWeb.Autenticar(Context, txtCorreo.Text, txtClave.Text)
            If usuario Is Nothing Then Throw New ArgumentException("Correo o contraseña incorrectos.")
            Redirigir(If(AccesoWeb.EsLector(usuario.NombreRol), "~/Lector/wfMiBiblioteca.aspx", "~/wfInicio.aspx"))
        Catch ex As Exception
            MensajeError(lblMensaje, ex)
        Finally
            txtClave.Text = ""
        End Try
    End Function
End Class
