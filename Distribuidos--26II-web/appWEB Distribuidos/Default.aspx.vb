Public Class Login
    Inherits PaginaBase
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If AccesoWeb.UsuarioActual(Context) IsNot Nothing Then Redirigir("~/wfInicio.aspx")
        If Not IsPostBack AndAlso Session("Registro.Creado") IsNot Nothing Then
            Session.Remove("Registro.Creado")
            Exito(lblMensaje, "Tu cuenta fue registrada y está pendiente de aprobación. Podrás iniciar sesión cuando un administrador la apruebe.")
        End If
    End Sub
    Protected Sub btnIngresar_Click(sender As Object, e As EventArgs) Handles btnIngresar.Click
        Ejecutar(lblMensaje, Sub()
            If String.IsNullOrWhiteSpace(txtCorreo.Text) OrElse String.IsNullOrWhiteSpace(txtClave.Text) Then Throw New ArgumentException("Ingresa tu correo y contraseña.")
            Dim usuario = AccesoWeb.Autenticar(Context, txtCorreo.Text, txtClave.Text)
            If usuario Is Nothing Then Throw New ArgumentException("Correo o contraseña incorrectos.")
            Redirigir(If(AccesoWeb.EsLector(usuario.NombreRol), "~/Lector/wfMiBiblioteca.aspx", "~/wfInicio.aspx"))
        End Sub)
        txtClave.Text = ""
    End Sub
End Class


