Imports appWEB_Distribuidos.Librerias

Public Class ComprobanteWeb
    Inherits PaginaMantenimiento
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then Ejecutar(lblMensaje, Sub()
            Dim comprobante As ComprobanteBiblioteca
            If Request.QueryString("prestamo") IsNot Nothing AndAlso Request.QueryString("solicitud") Is Nothing Then
                comprobante = Servicio.ObtenerComprobantePrestamo(Entero(Request.QueryString("prestamo"), 1), Request.QueryString("devolucion") = "1")
            ElseIf Request.QueryString("solicitud") IsNot Nothing AndAlso Request.QueryString("prestamo") Is Nothing Then
                comprobante = Servicio.ObtenerComprobanteSolicitud(Entero(Request.QueryString("solicitud"), 1))
            Else
                Throw New ArgumentException("Selecciona un préstamo o una solicitud entregada.")
            End If
            litTipo.Text = comprobante.Tipo
            litReferencia.Text = comprobante.Referencia
            litEmision.Text = "Emitido por " & comprobante.EmitidoPor & " · " & comprobante.FechaEmision.ToString("dd/MM/yyyy HH:mm")
            litTotal.Text = "Total de cargos: " & FormulariosWeb.Dinero(comprobante.TotalCargos)
            grvPrestamos.DataSource = comprobante.Prestamos
            grvPrestamos.DataBind()
            grvCargos.DataSource = comprobante.Penalizaciones
            grvCargos.DataBind()
            pnlCargos.Visible = comprobante.Penalizaciones.Count > 0
            pnlComprobante.Visible = True
        End Sub)
    End Sub
End Class
