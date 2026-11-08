Imports capaDatos
Imports capaNegocio
Imports System.Linq

Public Class BuscarLectores
    Inherits PaginaMantenimiento

    Private Sub Listar()
        If String.IsNullOrWhiteSpace(txtBuscar.Text) Then Throw New ArgumentException("Escribe un nombre, apellido, DNI o correo.")
        grvLectores.DataSource = New prestamoNegocio().buscarLectores(txtBuscar.Text)
        grvLectores.DataBind()
    End Sub

    Protected Sub btnBuscar_Click(sender As Object, e As EventArgs) Handles btnBuscar.Click
        pnlHistorial.Visible = False
        Ejecutar(lblMensaje, AddressOf Listar)
    End Sub

    Protected Sub btnLimpiar_Click(sender As Object, e As EventArgs) Handles btnLimpiar.Click
        txtBuscar.Text = ""
        grvLectores.DataSource = Nothing
        grvLectores.DataBind()
        pnlHistorial.Visible = False
        lblMensaje.Visible = False
    End Sub

    Protected Sub Seleccionar(sender As Object, e As EventArgs) Handles grvLectores.SelectedIndexChanged
        pnlHistorial.Visible = False
        Ejecutar(lblMensaje, Sub()
            Dim id = CInt(grvLectores.SelectedDataKey.Value)
            Dim negocio As New prestamoNegocio()
            Dim lector = negocio.buscarLectores(txtBuscar.Text).FirstOrDefault(Function(u) u.IdUsuario = id)
            If lector Is Nothing Then Throw New InvalidOperationException("Busca nuevamente al lector.")
            litLector.Text = lector.NombreCompleto & " · DNI " & lector.NumeroDniUsuario
            grvPrestamos.DataSource = negocio.listarPorLector(id).Select(Function(p) New With {p.IdPrestamo, p.TituloLibro, p.FechaInicio, p.FechaFin, p.FechaRetorno, .Estado = prestamoNegocio.EstadoParaMostrar(p, Date.Today)}).ToList()
            grvPrestamos.DataBind()
            lnkNuevo.NavigateUrl = "~/Circulacion/wfPrestamos.aspx?nuevo=1&lector=" & id
            lnkPrestamos.NavigateUrl = "~/Circulacion/wfPrestamos.aspx?lector=" & id
            pnlHistorial.Visible = True
        End Sub)
    End Sub
End Class
