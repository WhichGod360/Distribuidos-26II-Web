
Public Class PagarYapeWeb
    Inherits PaginaLector

    Private Property RegistroId As Integer
        Get
            Return If(ViewState("CargoPago") Is Nothing, 0, CInt(ViewState("CargoPago")))
        End Get
        Set(value As Integer)
            ViewState("CargoPago") = value
        End Set
    End Property

    Protected Sub Cargar(sender As Object, e As EventArgs) Handles Me.Load
        If IsPostBack Then Return
        Ejecutar(lblMensaje, Sub()
            RegistroId = Entero(Request.QueryString("cargo"), 1)
            MostrarCargo()
            MostrarAviso(lblMensaje)
        End Sub)
    End Sub

    Private Sub MostrarCargo()
        Dim cargo = WS.ConsultarMiCargoYape(RegistroId).Rows(0)
        Dim estado = cargo.Field(Of String)("Estado")
        Dim monto = cargo.Field(Of Decimal)("Monto")
        Dim pendiente = estado = DetallePenalizacion.PagoPendiente AndAlso monto > 0D
        Dim demo = WS.PagoDemoDisponible()
        Dim yape = WS.PagoYapeDisponible() AndAlso ImagenYape.Configurada(Context)
        litCargo.Text = $"Cargo N.º {RegistroId} · {cargo.Field(Of String)("Libro")} · " &
            FormulariosWeb.Dinero(monto) & " · " & If(estado = DetallePenalizacion.PagoPorRevisar, "Por revisar", estado)
        pnlCargo.Visible = True
        pnlPago.Visible = yape AndAlso pendiente
        pnlDemo.Visible = demo AndAlso pendiente
        pnlHistorial.Visible = yape
        ViewState("ImportePago") = monto

        If pnlDemo.Visible Then
            Dim destino = PagoDemoWeb.CrearEnlace(Context, WS.EmitirTokenPagoDemo(RegistroId))
            imgPagoDemo.ImageUrl = PagoDemoWeb.Imagen(destino)
            lnkPagoDemo.NavigateUrl = destino
        ElseIf yape Then
            grvHistorial.DataSource = WS.ConsultarMiHistorialYape(RegistroId)
            grvHistorial.DataBind()
        ElseIf pendiente Then
            Throw New InvalidOperationException("El pago por Yape aún no está disponible. Consulta con la biblioteca.")
        End If
    End Sub

    Protected Sub Enviar(sender As Object, e As EventArgs) Handles btnEnviar.Click
        Ejecutar(lblMensaje, Sub()
            If Not pnlPago.Visible OrElse ViewState("ImportePago") Is Nothing OrElse
                Not WS.PagoYapeDisponible() OrElse Not ImagenYape.Configurada(Context) Then
                Throw New InvalidOperationException("El reporte por Yape no está disponible para este cargo.")
            End If
            WS.RegistrarReporteYape(RegistroId, CDec(ViewState("ImportePago")), txtNombre.Text, txtCelular.Text, txtCodigo.Text)
            Guardado("Reporte enviado. La biblioteca verificará el pago.", Request.AppRelativeCurrentExecutionFilePath & "?cargo=" & RegistroId)
        End Sub)
    End Sub
End Class
