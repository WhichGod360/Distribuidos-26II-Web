Imports capaNegocio

Public Class BitacoraWeb
    Inherits PaginaAdministrador

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then Ejecutar(lblMensaje, AddressOf Listar)
    End Sub
    Private Sub Listar()
        Dim usuario As Integer? = Nothing
        If Not String.IsNullOrWhiteSpace(txtUsuario.Text) Then usuario = Entero(txtUsuario.Text)
        Dim datos = New bitacoraNegocio().Listar(FechaOpcional(txtDesde.Text), FechaOpcional(txtHasta.Text), usuario, txtObjeto.Text, ddlAccion.SelectedValue)
        grvDatos.DataSource = datos
        grvDatos.DataBind()
        litResumen.Text = datos.Count & " movimiento(s) · Actualizado " & Date.Now.ToString("HH:mm:ss")
    End Sub
    Protected Sub Aplicar(sender As Object, e As EventArgs) Handles btnAplicar.Click, btnActualizar.Click
        Ejecutar(lblMensaje, Sub()
            If sender Is btnAplicar Then grvDatos.PageIndex = 0
            Listar()
        End Sub)
    End Sub
    Protected Sub Limpiar(sender As Object, e As EventArgs) Handles btnLimpiar.Click
        txtDesde.Text = ""
        txtHasta.Text = ""
        txtUsuario.Text = ""
        txtObjeto.Text = ""
        ddlAccion.SelectedIndex = 0
        pnlDetalle.Visible = False
        grvDatos.PageIndex = 0
        Ejecutar(lblMensaje, AddressOf Listar)
    End Sub
    Protected Sub Seleccionar(sender As Object, e As EventArgs) Handles grvDatos.SelectedIndexChanged
        pnlDetalle.Visible = False
        Ejecutar(lblMensaje, Sub()
            Dim registro = New bitacoraNegocio().ObtenerPorId(CInt(grvDatos.SelectedDataKey.Value))
            If registro Is Nothing Then Throw New InvalidOperationException("El movimiento ya no está disponible.")
            litCadena.Text = registro.CadenaMan
            pnlDetalle.Visible = True
        End Sub)
    End Sub
    Protected Sub Paginar(sender As Object, e As GridViewPageEventArgs) Handles grvDatos.PageIndexChanging
        Ejecutar(lblMensaje, Sub()
            grvDatos.PageIndex = e.NewPageIndex
            Listar()
        End Sub)
    End Sub
End Class
