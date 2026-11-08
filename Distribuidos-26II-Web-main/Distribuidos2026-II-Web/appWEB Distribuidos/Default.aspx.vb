Imports capaNegocio
Public Class Login
    Inherits PaginaBase
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Request.IsAuthenticated Then Redirigir("~/wfInicio.aspx")
        If Not IsPostBack AndAlso Session("Registro.Creado") IsNot Nothing Then
            Session.Remove("Registro.Creado")
            Exito(lblMensaje, "Tu cuenta fue creada correctamente. Ya puedes iniciar sesión.")
        End If
    End Sub
    Protected Sub btnIngresar_Click(sender As Object, e As EventArgs) Handles btnIngresar.Click
        Ejecutar(lblMensaje, Sub()
            If String.IsNullOrWhiteSpace(txtCorreo.Text) OrElse String.IsNullOrWhiteSpace(txtClave.Text) Then Throw New ArgumentException("Ingresa tu correo y contraseña.")
            AccesoWeb.Limitar(Context, txtCorreo.Text, "login")
            If txtCorreo.Text.Length > 100 OrElse txtClave.Text.Length > 128 Then Throw New ArgumentException("Revisa la longitud del correo y la contraseña.")
            Dim usuario = New usuarioNegocio().IniciarSesion(txtCorreo.Text, txtClave.Text)
            If usuario Is Nothing Then Throw New ArgumentException("Correo o contraseña incorrectos, o cuenta inactiva.")
            AccesoWeb.Autenticar(Context, usuario)
            Redirigir(If(seguridadNegocio.EsRolLector(usuario.NombreRol), "~/Lector/wfMiBiblioteca.aspx", "~/wfInicio.aspx"))
        End Sub)
        txtClave.Text = ""
    End Sub
End Class


