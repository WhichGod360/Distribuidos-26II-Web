Imports capaDatos
Imports capaNegocio
Imports System.Linq

Public Class SolicitudesWeb
    Inherits PaginaMantenimiento
    Private solicitudEntrega As SolicitudPrestamo

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then Ejecutar(lblMensaje, Sub()
            Listar()
            MostrarAviso(lblMensaje)
        End Sub)
    End Sub

    Private Sub Listar()
        grvDatos.DataSource = New solicitudPrestamoNegocio().listarTodas().Where(Function(s) (ddlEstado.SelectedValue = "Todos" OrElse s.EstadoActual = ddlEstado.SelectedValue) AndAlso FormulariosWeb.Coincide(s.IdSolicitud & " " & s.NombreLector & " " & s.DniLector & " " & s.Libros, txtBuscar.Text)).ToList()
        grvDatos.DataBind()
    End Sub

    Private Function Actual() As SolicitudPrestamo
        Dim s = New solicitudPrestamoNegocio().listarTodas().FirstOrDefault(Function(x) x.IdSolicitud = RegistroId)
        If s Is Nothing Then Throw New InvalidOperationException("Actualiza el listado de solicitudes.")
        Return s
    End Function

    Protected Sub Seleccionar(sender As Object, e As EventArgs) Handles grvDatos.SelectedIndexChanged
        pnlDetalle.Visible = False
        pnlConfirmar.Visible = False
        pnlEntrega.Visible = False
        Ejecutar(lblMensaje, Sub()
            RegistroId = CInt(grvDatos.SelectedDataKey.Value)
            Dim s = Actual()
            litDetalle.Text = "Solicitud N.º " & s.IdSolicitud & " · " & s.EstadoActual
            litDatos.Text = s.NombreLector & " · " & s.Libros & " · Recojo hasta " & s.LimiteLocal.ToString("dd/MM/yyyy HH:mm")
            btnAprobar.Visible = solicitudPrestamoNegocio.PuedeAprobar(s)
            btnRechazar.Visible = circulacionLectorNegocio.PuedeCancelar(s)
            btnPreparar.Visible = solicitudPrestamoNegocio.PuedeEntregar(s)
            lnkComprobante.Visible = solicitudPrestamoNegocio.PuedeEmitirComprobante(s)
            lnkComprobante.NavigateUrl = "~/Circulacion/wfComprobante.aspx?solicitud=" & s.IdSolicitud
            pnlDetalle.Visible = True
        End Sub)
    End Sub

    Protected Sub PedirConfirmacion(sender As Object, e As EventArgs) Handles btnAprobar.Click, btnRechazar.Click
        Ejecutar(lblMensaje, Sub()
            Dim accion = If(sender Is btnAprobar, "aprobar", "rechazar")
            Dim s = Actual()
            If Not circulacionLectorNegocio.PuedeCancelar(s) OrElse (accion = "aprobar" AndAlso Not solicitudPrestamoNegocio.PuedeAprobar(s)) Then Throw New InvalidOperationException("La solicitud cambió de estado. Actualiza el listado.")
            ViewState("Accion") = accion
            litConfirmar.Text = "¿Confirmas " & accion & " la solicitud N.º " & RegistroId & "?"
            pnlEntrega.Visible = False
            pnlConfirmar.Visible = True
        End Sub)
    End Sub

    Protected Sub Confirmar(sender As Object, e As EventArgs) Handles btnConfirmar.Click
        Ejecutar(lblMensaje, Sub()
            If Not pnlConfirmar.Visible Then Throw New InvalidOperationException("Selecciona una acción antes de confirmar.")
            Dim negocio As New solicitudPrestamoNegocio()
            Select Case CStr(ViewState("Accion"))
                Case "aprobar" : negocio.aprobar(RegistroId)
                Case "rechazar" : negocio.rechazar(RegistroId)
                Case Else : Throw New InvalidOperationException("Selecciona una acción válida.")
            End Select
            Guardado()
        End Sub)
    End Sub

    Protected Sub Preparar(sender As Object, e As EventArgs) Handles btnPreparar.Click
        pnlEntrega.Visible = False
        Ejecutar(lblMensaje, Sub()
            solicitudEntrega = Actual()
            If Not solicitudPrestamoNegocio.PuedeEntregar(solicitudEntrega) Then Throw New InvalidOperationException("Solo se pueden entregar solicitudes aprobadas y vigentes.")
            grvEntrega.DataSource = solicitudEntrega.Detalles
            grvEntrega.DataBind()
            pnlConfirmar.Visible = False
            pnlEntrega.Visible = True
        End Sub)
    End Sub

    Protected Sub Copias(sender As Object, e As GridViewRowEventArgs) Handles grvEntrega.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then Return
        Dim detalle = DirectCast(e.Row.DataItem, DetalleSolicitudPrestamo)
        FormulariosWeb.Lista(DirectCast(e.Row.FindControl("ddlCopia"), DropDownList), New ejemplarNegocio().listarDisponiblesPorLibro(detalle.IdLibro, solicitudEntrega.IdUsuario), "IdEjemplar", "IdEjemplar")
    End Sub

    Protected Sub Entregar(sender As Object, e As EventArgs) Handles btnEntregar.Click
        Ejecutar(lblMensaje, Sub()
            If Not pnlEntrega.Visible Then Throw New InvalidOperationException("Prepara primero la entrega.")
            Dim copias As New Dictionary(Of Integer, Integer)()
            For Each fila As GridViewRow In grvEntrega.Rows
                copias.Add(CInt(grvEntrega.DataKeys(fila.RowIndex).Value), FormulariosWeb.Id(DirectCast(fila.FindControl("ddlCopia"), DropDownList)))
            Next
            Dim s = Actual()
            If Not solicitudPrestamoNegocio.PuedeEntregar(s) OrElse Not copias.Keys.OrderBy(Function(x) x).SequenceEqual(s.Detalles.Select(Function(d) d.IdLibro).OrderBy(Function(x) x)) Then Throw New InvalidOperationException("La solicitud cambió. Prepara nuevamente la entrega.")
            Dim prestamos = New solicitudPrestamoNegocio().entregar(RegistroId, copias)
            Guardado("Entrega registrada. Préstamos: " & String.Join(", ", prestamos) & ".")
        End Sub)
    End Sub

    Protected Sub Cerrar(sender As Object, e As EventArgs) Handles btnCerrar.Click
        pnlDetalle.Visible = False
    End Sub
    Protected Sub Cancelar(sender As Object, e As EventArgs) Handles btnCancelarAccion.Click
        pnlConfirmar.Visible = False
        ViewState.Remove("Accion")
    End Sub
    Protected Sub Buscar(sender As Object, e As EventArgs) Handles btnBuscar.Click
        pnlDetalle.Visible = False
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
