Imports capaDatos
Imports capaNegocio
Imports System.Linq

Public Class DetallePenalizacionesWeb
    Inherits PaginaMantenimiento

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then Ejecutar(lblMensaje, Sub()
            FormulariosWeb.Lista(ddlTipoFiltro, New penalizacionNegocio().listarTodos(), "DescripcionCombo", "IdPenalizacion", "Todas las penalizaciones")
            Dim tipo As Integer
            If Integer.TryParse(Request.QueryString("tipo"), tipo) AndAlso ddlTipoFiltro.Items.FindByValue(tipo.ToString()) IsNot Nothing Then ddlTipoFiltro.SelectedValue = tipo.ToString()
            Listar()
            MostrarAviso(lblMensaje)
        End Sub)
    End Sub
    Private Sub Listar()
        grvDatos.DataSource = New detallePenalizacionNegocio().listar(Entero(ddlTipoFiltro.SelectedValue)).Where(Function(d) (ddlPagoFiltro.SelectedValue = "" OrElse d.EstadoPago = ddlPagoFiltro.SelectedValue) AndAlso FormulariosWeb.Coincide(d.IdPrestamo & " " & d.NombreUsuario & " " & d.TituloLibro & " " & d.Motivo, txtBuscar.Text)).ToList()
        grvDatos.DataBind()
    End Sub
    Private Sub Opciones(nuevo As Boolean)
        Dim negocio As New penalizacionNegocio()
        FormulariosWeb.Lista(ddlTipo, If(nuevo, negocio.listarActivas(), negocio.listarTodos()), "DescripcionCombo", "IdPenalizacion")
        FormulariosWeb.Lista(ddlPrestamo, New detallePenalizacionNegocio().listarPrestamos(), "Texto", "IdPrestamo")
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
            Dim d = New detallePenalizacionNegocio().listar(0).FirstOrDefault(Function(x) x.IdDetallePenalizacion = id)
            If d Is Nothing Then Throw New InvalidOperationException("El cargo ya no está disponible.")
            Opciones(False)
            RegistroId = id
            ViewState("TipoOriginal") = d.IdPenalizacion
            ViewState("PrestamoOriginal") = d.IdPrestamo
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
            Dim negocio As New detallePenalizacionNegocio()
            If RegistroId = 0 Then
                Comprobar(negocio.agregarDetalle(txtMotivo.Text.Trim(), FormulariosWeb.Id(ddlTipo), FormulariosWeb.Id(ddlPrestamo), ddlPago.SelectedValue))
            Else
                Comprobar(negocio.actualizarDetalle(RegistroId, txtMotivo.Text.Trim(), CInt(ViewState("TipoOriginal")), CInt(ViewState("PrestamoOriginal")), ddlPago.SelectedValue))
            End If
            Guardado()
        End Sub)
    End Sub
    Protected Sub Cancelar(sender As Object, e As EventArgs) Handles btnCancelar.Click
        pnlEditor.Visible = False
    End Sub
    Protected Sub Buscar(sender As Object, e As EventArgs) Handles btnBuscar.Click
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
