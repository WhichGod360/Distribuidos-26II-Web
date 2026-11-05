Imports capaDatos
Public Class Site1
    Inherits System.Web.UI.MasterPage
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        litUsuario.Text = If(SesionUsuario.IdUsuario > 0, SesionUsuario.NombreCompleto, "Invitado")
        litRol.Text = If(SesionUsuario.IdUsuario > 0, SesionUsuario.Rol, "Sin sesión iniciada")
        Dim personal = SesionUsuario.EsPersonal
        For Each boton As Button In {btnLibros, btnAutores, btnCategorias, btnPenalizaciones, btnEjemplares}
            boton.Visible = personal
        Next
        btnSalir.Visible = SesionUsuario.IdUsuario > 0
        btnClave.Visible = SesionUsuario.IdUsuario > 0
        Dim ruta = Request.AppRelativeCurrentExecutionFilePath.ToLowerInvariant()
        For Each boton As Button In {btnMenuInicio, btnLibros, btnAutores, btnCategorias, btnPenalizaciones, btnEjemplares}
            boton.CssClass = "menu-button"
        Next
        If ruta.EndsWith("wflibros.aspx") Then
            btnLibros.CssClass &= " selected"
        ElseIf ruta.EndsWith("wfautores.aspx") Then
            btnAutores.CssClass &= " selected"
        ElseIf ruta.EndsWith("wfcategorias.aspx") Then
            btnCategorias.CssClass &= " selected"
        ElseIf ruta.EndsWith("wfpenalizaciones.aspx") Then
            btnPenalizaciones.CssClass &= " selected"
        ElseIf ruta.EndsWith("wfejemplares.aspx") Then
            btnEjemplares.CssClass &= " selected"
        Else
            btnMenuInicio.CssClass &= " selected"
        End If
    End Sub
    Private Sub Ir(ruta As String)
        Response.Redirect(ResolveUrl(ruta), False)
        Context.ApplicationInstance.CompleteRequest()
    End Sub
    Protected Sub Inicio_Click(sender As Object, e As EventArgs) Handles btnMenuInicio.Click
        Ir("~/wfInicio.aspx")
    End Sub
    Protected Sub Libros_Click(sender As Object, e As EventArgs) Handles btnLibros.Click
        Ir("~/Catalogo/wfLibros.aspx")
    End Sub
    Protected Sub Autores_Click(sender As Object, e As EventArgs) Handles btnAutores.Click
        Ir("~/Catalogo/wfAutores.aspx")
    End Sub
    Protected Sub Categorias_Click(sender As Object, e As EventArgs) Handles btnCategorias.Click
        Ir("~/Catalogo/wfCategorias.aspx")
    End Sub
    Protected Sub Penalizaciones_Click(sender As Object, e As EventArgs) Handles btnPenalizaciones.Click
        Ir("~/Administracion/wfPenalizaciones.aspx")
    End Sub
    Protected Sub Ejemplares_Click(sender As Object, e As EventArgs) Handles btnEjemplares.Click
        Ir("~/Catalogo/wfEjemplares.aspx")
    End Sub
    Protected Sub Clave_Click(sender As Object, e As EventArgs) Handles btnClave.Click
        AccesoWeb.Salir(Context)
        Ir("~/Seguridad/wfRecuperar.aspx")
    End Sub
    Protected Sub Salir_Click(sender As Object, e As EventArgs) Handles btnSalir.Click
        AccesoWeb.Salir(Context)
        Ir("~/Default.aspx")
    End Sub
End Class
