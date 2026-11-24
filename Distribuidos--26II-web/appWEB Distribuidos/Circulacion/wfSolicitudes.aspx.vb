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
        grvDatos.DataSource = WS.ListarSolicitudes().Where(Function(s) (ddlEstado.SelectedValue = "Todos" OrElse s.EstadoActual = ddlEstado.SelectedValue) AndAlso FormulariosWeb.Coincide(s.IdSolicitud & " " & s.NombreLector & " " & s.DniLector & " " & s.Libros, txtBuscar.Text)).ToList()
        grvDatos.DataBind()
    End Sub

    Protected Function FechaSolicitudLima(datos As Object) As String
        Dim solicitud = DirectCast(datos, SolicitudPrestamo)
        Dim fechaUtc = Date.SpecifyKind(solicitud.FechaSolicitud, DateTimeKind.Utc)
        Dim lima = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time")
        Return TimeZoneInfo.ConvertTimeFromUtc(fechaUtc, lima).ToString("dd/MM/yyyy")
    End Function

    Private Function Actual() As SolicitudPrestamo
        Dim s = WS.ListarSolicitudes().FirstOrDefault(Function(x) x.IdSolicitud = RegistroId)
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
            btnAprobar.Visible = (s.EstaVigente AndAlso s.EstadoActual = "Pendiente")
            btnRechazar.Visible = s.EstaVigente
            btnPreparar.Visible = (s.EstaVigente AndAlso s.EstadoActual = "Aprobada")
            lnkComprobante.Visible = s.EstadoActual = "Atendida"
            lnkComprobante.NavigateUrl = "~/Circulacion/wfComprobante.aspx?solicitud=" & s.IdSolicitud
            pnlDetalle.Visible = True
        End Sub)
    End Sub

    Protected Sub PedirConfirmacion(sender As Object, e As EventArgs) Handles btnAprobar.Click, btnRechazar.Click
        Ejecutar(lblMensaje, Sub()
            Dim accion = If(sender Is btnAprobar, "aprobar", "rechazar")
            Dim s = Actual()
            If Not s.EstaVigente OrElse (accion = "aprobar" AndAlso Not (s.EstaVigente AndAlso s.EstadoActual = "Pendiente")) Then Throw New InvalidOperationException("La solicitud cambió de estado. Actualiza el listado.")
            ViewState("Accion") = accion
            litConfirmar.Text = "¿Confirmas " & accion & " la solicitud N.º " & RegistroId & "?"
            pnlEntrega.Visible = False
            pnlConfirmar.Visible = True
        End Sub)
    End Sub

    Protected Sub Confirmar(sender As Object, e As EventArgs) Handles btnConfirmar.Click
        Ejecutar(lblMensaje, Sub()
            If Not pnlConfirmar.Visible Then Throw New InvalidOperationException("Selecciona una acción antes de confirmar.")
            Select Case CStr(ViewState("Accion"))
                Case "aprobar" : WS.AprobarSolicitud(RegistroId)
                Case "rechazar" : WS.RechazarSolicitud(RegistroId)
                Case Else : Throw New InvalidOperationException("Selecciona una acción válida.")
            End Select
            Guardado()
        End Sub)
    End Sub

    Protected Sub Preparar(sender As Object, e As EventArgs) Handles btnPreparar.Click
        pnlEntrega.Visible = False
        Ejecutar(lblMensaje, Sub()
            solicitudEntrega = Actual()
            If Not (solicitudEntrega.EstaVigente AndAlso solicitudEntrega.EstadoActual = "Aprobada") Then Throw New InvalidOperationException("Solo se pueden entregar solicitudes aprobadas y vigentes.")
            grvEntrega.DataSource = solicitudEntrega.Detalles
            grvEntrega.DataBind()
            pnlConfirmar.Visible = False
            pnlEntrega.Visible = True
        End Sub)
    End Sub

    Protected Sub Copias(sender As Object, e As GridViewRowEventArgs) Handles grvEntrega.RowDataBound
        If e.Row.RowType <> DataControlRowType.DataRow Then Return
        Dim detalle = DirectCast(e.Row.DataItem, DetalleSolicitudPrestamo)
        FormulariosWeb.Lista(DirectCast(e.Row.FindControl("ddlCopia"), DropDownList), WS.ListarEjemplaresDisponibles(detalle.IdLibro, solicitudEntrega.IdUsuario), "IdEjemplar", "IdEjemplar")
    End Sub

    Protected Sub Entregar(sender As Object, e As EventArgs) Handles btnEntregar.Click
        Ejecutar(lblMensaje, Sub()
            If Not pnlEntrega.Visible Then Throw New InvalidOperationException("Prepara primero la entrega.")
            Dim s = Actual()
            Dim libros = grvEntrega.DataKeys.Cast(Of DataKey)().Select(Function(clave) CInt(clave.Value))
            If Not (s.EstaVigente AndAlso s.EstadoActual = "Aprobada") OrElse Not libros.OrderBy(Function(x) x).SequenceEqual(s.Detalles.Select(Function(d) d.IdLibro).OrderBy(Function(x) x)) Then Throw New InvalidOperationException("La solicitud cambió. Prepara nuevamente la entrega.")

            Dim copias As New List(Of EjemplarEntregaSoap)()
            For Each fila As GridViewRow In grvEntrega.Rows
                Dim idLibro = CInt(grvEntrega.DataKeys(fila.RowIndex).Value)
                Dim selector = DirectCast(fila.FindControl("ddlCopia"), DropDownList)
                If String.IsNullOrEmpty(selector.SelectedValue) OrElse selector.SelectedValue = "0" Then
                    Dim titulo = s.Detalles.First(Function(d) d.IdLibro = idLibro).TituloLibro
                    Throw New ArgumentException("Selecciona un ejemplar para «" & titulo & "».")
                End If
                copias.Add(New EjemplarEntregaSoap With {.IdLibro = idLibro, .IdEjemplar = FormulariosWeb.Id(selector)})
            Next
            Dim prestamos = WS.EntregarSolicitud(RegistroId, copias.ToArray())
            Guardado("Entrega registrada como un solo préstamo N.º " & prestamos(0) & " con " & copias.Count & " ejemplar(es).")
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
