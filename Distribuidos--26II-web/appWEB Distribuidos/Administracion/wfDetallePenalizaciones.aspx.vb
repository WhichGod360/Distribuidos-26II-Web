Imports System.Linq

Public Class DetallePenalizacionesWeb
    Inherits PaginaMantenimiento

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then Ejecutar(lblMensaje, Sub()
            FormulariosWeb.Lista(ddlTipoFiltro, WS.ListarTiposPenalizacion(), "DescripcionCombo", "IdPenalizacion", "Todas las penalizaciones")
            Dim tipo As Integer
            If Integer.TryParse(Request.QueryString("tipo"), tipo) AndAlso ddlTipoFiltro.Items.FindByValue(tipo.ToString()) IsNot Nothing Then ddlTipoFiltro.SelectedValue = tipo.ToString()
            Listar()
            MostrarAviso(lblMensaje)
        End Sub)
    End Sub
    Private Sub Listar()
        grvDatos.DataSource = WS.ListarCargos(Entero(ddlTipoFiltro.SelectedValue)).Where(Function(d) (ddlPagoFiltro.SelectedValue = "" OrElse d.EstadoPago = ddlPagoFiltro.SelectedValue) AndAlso FormulariosWeb.Coincide(d.IdPrestamo & " " & d.NombreUsuario & " " & d.TituloLibro & " " & d.Motivo, txtBuscar.Text)).ToList()
        grvDatos.DataBind()
    End Sub
    Private Sub Opciones(nuevo As Boolean)
        FormulariosWeb.Lista(ddlTipo, If(nuevo, WS.ListarTiposPenalizacionActivos(), WS.ListarTiposPenalizacion()), "DescripcionCombo", "IdPenalizacion")
        FormulariosWeb.Lista(ddlPrestamo, WS.ListarPrestamosParaCargo(), "Texto", "IdPrestamo")
        ddlTipo.Enabled = nuevo
        ddlPrestamo.Enabled = nuevo
    End Sub
    Protected Sub Nuevo(sender As Object, e As EventArgs) Handles btnNuevo.Click
        pnlEditor.Visible = False
        Ejecutar(lblMensaje, Sub()
            RegistroId = 0
            Opciones(True)
            If ddlTipo.Items.FindByValue(ddlTipoFiltro.SelectedValue) IsNot Nothing Then ddlTipo.SelectedValue = ddlTipoFiltro.SelectedValue
            txtMotivo.Text = ""
            ddlPago.SelectedValue = "Pendiente"
            litEditor.Text = "Agregar cargo"
            litImporte.Text = "El importe se obtiene del tipo de penalización seleccionado."
            pnlEditor.Visible = True
        End Sub)
    End Sub
    Protected Sub Seleccionar(sender As Object, e As EventArgs) Handles grvDatos.SelectedIndexChanged
        pnlEditor.Visible = False
        Ejecutar(lblMensaje, Sub()
            Dim id = CInt(grvDatos.SelectedDataKey.Value)
            Dim d = WS.ListarCargos(0).FirstOrDefault(Function(x) x.IdDetallePenalizacion = id)
            If d Is Nothing Then Throw New InvalidOperationException("El cargo ya no está disponible.")
            Opciones(False)
            RegistroId = id
            ViewState("TipoOriginal") = d.IdPenalizacion
            ViewState("PrestamoOriginal") = d.IdPrestamo
            ViewState("PagoOriginal") = d.EstadoPago
            ddlTipo.SelectedValue = d.IdPenalizacion.ToString()
            ddlPrestamo.SelectedValue = d.IdPrestamo.ToString()
            ddlPago.SelectedValue = d.EstadoPago
            txtMotivo.Text = d.DescripcionDetallePenalizacion
            litEditor.Text = "Editar cargo N.º " & id
            litImporte.Text = "Importe conservado: " & FormulariosWeb.Dinero(d.MontoPenalizacion) & ". Puedes editar el motivo y el estado de pago."
            pnlEditor.Visible = True
        End Sub)
    End Sub
    Protected Sub Guardar(sender As Object, e As EventArgs) Handles btnGuardar.Click
        Ejecutar(lblMensaje, Sub()
            If RegistroId = 0 Then
                If String.IsNullOrEmpty(ddlTipo.SelectedValue) OrElse ddlTipo.SelectedValue = "0" Then Throw New ArgumentException("Selecciona un tipo de penalización.")
                If String.IsNullOrEmpty(ddlPrestamo.SelectedValue) OrElse ddlPrestamo.SelectedValue = "0" Then Throw New ArgumentException("Selecciona un préstamo.")
                Comprobar(WS.AgregarCargo(txtMotivo.Text.Trim(), FormulariosWeb.Id(ddlTipo), FormulariosWeb.Id(ddlPrestamo), ddlPago.SelectedValue))
            Else
                Dim pago = If(ddlPago.SelectedValue = CStr(ViewState("PagoOriginal")), Nothing, ddlPago.SelectedValue)
                Comprobar(WS.ActualizarCargo(RegistroId, txtMotivo.Text.Trim(), CInt(ViewState("TipoOriginal")), CInt(ViewState("PrestamoOriginal")), pago))
            End If
            Guardado()
        End Sub)
    End Sub
    Protected Sub Cancelar(sender As Object, e As EventArgs) Handles btnCancelar.Click
        pnlEditor.Visible = False
    End Sub
    Protected Sub Buscar(sender As Object, e As EventArgs) Handles btnBuscar.Click, ddlPagoFiltro.SelectedIndexChanged
        pnlEditor.Visible = False
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
