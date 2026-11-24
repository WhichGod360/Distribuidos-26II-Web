
Public Class UsuariosWeb
    Inherits PaginaAdministrador

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then Ejecutar(lblMensaje, Sub()
            Dim roles = WS.ListarRoles()
            FormulariosWeb.Lista(ddlRolFiltro, roles, "NombreRolUsuario", "IdRolUsuario", "Todos los roles")
            FormulariosWeb.Lista(ddlRol, roles, "NombreRolUsuario", "IdRolUsuario")
            Listar()
            MostrarAviso(lblMensaje)
        End Sub)
    End Sub
    Private Sub Listar()
        grvDatos.DataSource = WS.ListarUsuarios(txtBuscar.Text, Entero(ddlRolFiltro.SelectedValue), ddlEstadoFiltro.SelectedValue)
        grvDatos.DataBind()
    End Sub
    Protected Sub Nuevo(sender As Object, e As EventArgs) Handles btnNuevo.Click
        RegistroId = 0
        campos.Cargar(New UsuarioSoap())
        ddlRol.SelectedIndex = 0
        ddlEstado.SelectedValue = "Activo"
        litEditor.Text = "Agregar usuario"
        ddlRol.Enabled = True
        ddlEstado.Enabled = True
        btnEstado.Visible = False
        pnlConfirmar.Visible = False
        pnlEditor.Visible = True
    End Sub
    Protected Sub Seleccionar(sender As Object, e As EventArgs) Handles grvDatos.SelectedIndexChanged
        pnlEditor.Visible = False
        pnlConfirmar.Visible = False
        Ejecutar(lblMensaje, Sub()
            Dim u = WS.ObtenerUsuario(CInt(grvDatos.SelectedDataKey.Value))
            campos.Cargar(u)
            RegistroId = u.IdUsuario
            ddlRol.SelectedValue = u.IdRolUsuario.ToString()
            ddlEstado.SelectedValue = If(String.Equals(u.EstadoUsuario.Trim(), "Activo", StringComparison.OrdinalIgnoreCase), "Activo", "Inactivo")
            litEditor.Text = "Editar usuario N.º " & RegistroId
            btnEstado.Text = If(ddlEstado.SelectedValue = "Activo", "Desactivar cuenta", "Activar cuenta")
            Dim esCuentaActual = RegistroId = AccesoWeb.UsuarioActual(Context).IdUsuario
            ddlRol.Enabled = Not esCuentaActual
            ddlEstado.Enabled = Not esCuentaActual
            btnEstado.Visible = Not esCuentaActual
            pnlEditor.Visible = True
        End Sub)
    End Sub
    Protected Sub Guardar(sender As Object, e As EventArgs) Handles btnGuardar.Click
        Ejecutar(lblMensaje, Sub()
            Dim u = campos.Leer(RegistroId)
            If RegistroId = AccesoWeb.UsuarioActual(Context).IdUsuario Then
                Dim actual = WS.ObtenerUsuario(RegistroId)
                u.IdRolUsuario = actual.IdRolUsuario
                u.EstadoUsuario = actual.EstadoUsuario
            Else
                If ddlRol.SelectedValue = "0" OrElse String.IsNullOrEmpty(ddlRol.SelectedValue) Then Throw New ArgumentException("Selecciona el rol del usuario.")
                u.IdRolUsuario = FormulariosWeb.Id(ddlRol)
                u.EstadoUsuario = ddlEstado.SelectedValue
            End If
            If RegistroId = 0 Then
                WS.CrearUsuario(u, campos.Confirmacion)
            Else
                WS.ActualizarUsuario(u)
            End If
            Guardado()
        End Sub)
    End Sub
    Protected Sub CambiarEstado(sender As Object, e As EventArgs) Handles btnEstado.Click
        Ejecutar(lblMensaje, Sub()
            If RegistroId = AccesoWeb.UsuarioActual(Context).IdUsuario Then Throw New InvalidOperationException("No puedes cambiar el estado de tu propia cuenta.")
            Dim u = WS.ObtenerUsuario(RegistroId)
            ViewState("EstadoAnterior") = u.EstadoUsuario
            ViewState("EstadoNuevo") = If(String.Equals(u.EstadoUsuario.Trim(), "Activo", StringComparison.OrdinalIgnoreCase), "Inactivo", "Activo")
            litConfirmar.Text = "¿Cambiar la cuenta de " & u.NombreCompleto & " a " & CStr(ViewState("EstadoNuevo")) & "?"
            pnlConfirmar.Visible = True
        End Sub)
    End Sub
    Protected Sub Confirmar(sender As Object, e As EventArgs) Handles btnConfirmar.Click
        Ejecutar(lblMensaje, Sub()
            If RegistroId = AccesoWeb.UsuarioActual(Context).IdUsuario Then Throw New InvalidOperationException("No puedes cambiar el estado de tu propia cuenta.")
            If Not pnlConfirmar.Visible OrElse WS.ObtenerUsuario(RegistroId).EstadoUsuario <> CStr(ViewState("EstadoAnterior")) Then Throw New InvalidOperationException("El usuario cambió. Selecciónalo nuevamente.")
            WS.CambiarEstadoUsuario(RegistroId, CStr(ViewState("EstadoNuevo")))
            Guardado()
        End Sub)
    End Sub
    Protected Sub Cancelar(sender As Object, e As EventArgs) Handles btnCancelar.Click, btnNoCambiar.Click
        pnlEditor.Visible = False
        pnlConfirmar.Visible = False
    End Sub
    Protected Sub Buscar(sender As Object, e As EventArgs) Handles btnBuscar.Click
        pnlEditor.Visible = False
        pnlConfirmar.Visible = False
        Ejecutar(lblMensaje, Sub()
            grvDatos.PageIndex = 0
            Listar()
        End Sub)
    End Sub
    Protected Sub Paginar(sender As Object, e As GridViewPageEventArgs) Handles grvDatos.PageIndexChanging
        Ejecutar(lblMensaje, Sub()
            grvDatos.PageIndex = e.NewPageIndex
            Listar()
        End Sub)
    End Sub
End Class
