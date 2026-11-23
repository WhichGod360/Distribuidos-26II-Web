
Public Class RevisionYapeWeb
    Inherits PaginaMantenimiento


    Protected Sub Cargar(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack Then Return
        Ejecutar(lblMensaje, Sub()
            If Not Servicio.PagoYapeDisponible() Then Throw New InvalidOperationException("Aplica la migración de Yape en la base de Lecturae para habilitar esta bandeja.")
            Listar()
            pnlBandeja.Visible = True
            If Request.QueryString("id") IsNot Nothing Then MostrarReporte(Entero(Request.QueryString("id"), 1))
            MostrarAviso(lblMensaje)
        End Sub)
    End Sub

    Private Sub Listar()
        grvReportes.DataSource = Servicio.ListarReportesYape(ddlEstado.SelectedValue)
        grvReportes.DataBind()
    End Sub

    Private Sub MostrarReporte(id As Integer)
        Dim reporte = Servicio.ObtenerReporteYape(id).AsEnumerable().FirstOrDefault()
        If reporte Is Nothing Then Throw New InvalidOperationException("El reporte no está disponible.")
        RegistroId = id
        litTitulo.Text = "Reporte N.º " & id & " · Cargo N.º " & reporte.Field(Of Integer)("IdDetalle") &
            " · Préstamo N.º " & reporte.Field(Of Integer)("IdPrestamo")
        litLector.Text = reporte.Field(Of String)("Lector")
        litMonto.Text = FormulariosWeb.Dinero(reporte.Field(Of Decimal)("Monto"))
        litNombre.Text = reporte.Field(Of String)("NombreYape")
        litCelular.Text = reporte.Field(Of String)("Celular")
        litCodigo.Text = reporte.Field(Of String)("Codigo")
        Dim estado = reporte.Field(Of String)("Estado")
        Dim enRevision = estado = "EnRevision"
        pnlDecision.Visible = enRevision
        litEstado.Text = If(enRevision, "En revisión", estado)
        If Not enRevision Then
            litEstado.Text &= " por " & reporte.Field(Of String)("Revisor") & " · " &
                reporte.Field(Of Date)("FechaRevision").ToString("dd/MM/yyyy HH:mm")
            If estado = "Denegado" Then litEstado.Text &= " · " & reporte.Field(Of String)("Motivo")
        End If
        pnlDetalle.Visible = True
    End Sub

    Protected Sub Filtrar(sender As Object, e As EventArgs) Handles btnFiltrar.Click
        Ejecutar(lblMensaje, Sub()
            pnlDetalle.Visible = False
            RegistroId = 0
            grvReportes.PageIndex = 0
            Listar()
        End Sub)
    End Sub

    Protected Sub Paginar(sender As Object, e As GridViewPageEventArgs) Handles grvReportes.PageIndexChanging
        Ejecutar(lblMensaje, Sub()
            grvReportes.PageIndex = e.NewPageIndex
            Listar()
        End Sub)
    End Sub

    Protected Sub Decidir(sender As Object, e As EventArgs) Handles btnConfirmar.Click, btnDenegar.Click
        Ejecutar(lblMensaje, Sub()
            If Not pnlDecision.Visible OrElse RegistroId <= 0 Then Throw New InvalidOperationException("Abre un reporte en revisión.")
            Dim confirmar = sender Is btnConfirmar
            If confirmar AndAlso Not chkVerificado.Checked Then Throw New ArgumentException("Marca la verificación del ingreso en la cuenta receptora.")
            Servicio.RevisarReporteYape(RegistroId, confirmar, chkVerificado.Checked, txtMotivo.Text)
            Guardado(If(confirmar, "Pago confirmado. El cargo quedó pagado.", "Reporte denegado. El cargo volvió a pendiente."))
        End Sub)
    End Sub
End Class
