Public Class RecuperarClave
    Inherits PaginaBase

    Protected Sub btnPregunta_Click(sender As Object, e As EventArgs) Handles btnPregunta.Click
        Ejecutar(lblMensaje, Sub()
            CancelarToken()
            AccesoWeb.Limitar(Context, txtCorreo.Text, "pregunta")
            Dim pregunta = WS.ObtenerPregunta(txtCorreo.Text)
            Session("Recuperacion.Correo") = txtCorreo.Text.Trim()
            litPregunta.Text = pregunta
            pnlCorreo.Visible = False
            pnlRespuesta.Visible = True
            pnlNueva.Visible = False
        End Sub)
    End Sub

    Protected Sub btnVerificar_Click(sender As Object, e As EventArgs) Handles btnVerificar.Click
        Ejecutar(lblMensaje, Sub()
            If Session("Recuperacion.Correo") Is Nothing Then Throw New InvalidOperationException("Consulta primero tu pregunta de seguridad.")
            Dim ahora = DateTime.UtcNow
            Dim vencimiento = Session("Recuperacion.IntentosVence")
            If vencimiento Is Nothing OrElse CDate(vencimiento) <= ahora Then
                Session.Remove("Recuperacion.Intentos")
                Session("Recuperacion.IntentosVence") = ahora.AddMinutes(15)
            End If
            Dim intentos = If(Session("Recuperacion.Intentos") Is Nothing, 0, CInt(Session("Recuperacion.Intentos")))
            If intentos >= 5 Then Throw New InvalidOperationException("Alcanzaste el límite de intentos. Vuelve a intentarlo en unos minutos.")
            Session("Recuperacion.Intentos") = intentos + 1
            AccesoWeb.Limitar(Context, CStr(Session("Recuperacion.Correo")), "respuesta")
            Dim token = WS.ValidarRespuesta(CStr(Session("Recuperacion.Correo")), txtRespuesta.Text)
            Session("Recuperacion.Token") = token
            Session.Remove("Recuperacion.Intentos")
            Session.Remove("Recuperacion.IntentosVence")
            pnlRespuesta.Visible = False
            pnlNueva.Visible = True
        End Sub)
        txtRespuesta.Text = ""
    End Sub

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        Ejecutar(lblMensaje, Sub()
            If Session("Recuperacion.Token") Is Nothing OrElse Session("Recuperacion.Correo") Is Nothing Then Throw New UnauthorizedAccessException("Verifica primero tu respuesta de seguridad.")
            WS.CambiarContrasena(CStr(Session("Recuperacion.Correo")), txtNueva.Text, txtConfirmar.Text, CStr(Session("Recuperacion.Token")))
            LimpiarRecuperacion()
            pnlNueva.Visible = False
            Exito(lblMensaje, "Contraseña actualizada. Ya puedes iniciar sesión con tu nueva clave.")
        End Sub)
        txtNueva.Text = ""
        txtConfirmar.Text = ""
    End Sub

    Protected Sub btnCancelar_Click(sender As Object, e As EventArgs) Handles btnCancelar.Click
        Ejecutar(lblMensaje, Sub()
            CancelarToken()
            Redirigir("~/Default.aspx")
        End Sub)
    End Sub

    Private Sub CancelarToken()
        If Session("Recuperacion.Token") IsNot Nothing Then WS.CancelarRecuperacion(CStr(Session("Recuperacion.Token")))
        LimpiarRecuperacion()
    End Sub

    Private Sub LimpiarRecuperacion()
        Session.Remove("Recuperacion.Token")
        Session.Remove("Recuperacion.Correo")
    End Sub
End Class
