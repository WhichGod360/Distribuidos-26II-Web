Imports capaDatos
Imports capaNegocio

Public Class Site1
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        litUsuario.Text = SesionUsuario.NombreCompleto
        litRol.Text = SesionUsuario.Rol
        Dim personal = SesionUsuario.EsPersonal
        Dim admin = seguridadNegocio.PuedeGestionarUsuarios(SesionUsuario.IdUsuario, SesionUsuario.Rol)
        For Each boton As Button In {btnBuscar, btnPrestamos, btnSolicitudes, btnLibros, btnEjemplares, btnAutores, btnCategorias, btnPenalizaciones, btnCargos}
            boton.Visible = personal
        Next
        btnUsuarios.Visible = admin
        btnBitacora.Visible = admin
        btnMiBiblioteca.Visible = seguridadNegocio.EsRolLector(SesionUsuario.Rol)
        btnMenuInicio.Visible = personal
        Dim ruta = Request.AppRelativeCurrentExecutionFilePath
        For Each boton As Button In {btnMenuInicio, btnMiBiblioteca, btnBuscar, btnPrestamos, btnSolicitudes, btnLibros, btnEjemplares, btnAutores, btnCategorias, btnPenalizaciones, btnCargos, btnUsuarios, btnBitacora}
            Dim destino = If(boton Is btnMenuInicio, "~/wfInicio.aspx", boton.CommandArgument)
            boton.CssClass = "menu-button" & If(String.Equals(ruta, destino, StringComparison.OrdinalIgnoreCase), " selected", "")
        Next
    End Sub
    Private Sub Ir(ruta As String)
        Response.Redirect(ResolveUrl(ruta), False)
        Context.ApplicationInstance.CompleteRequest()
    End Sub
    Protected Sub Inicio(sender As Object, e As EventArgs) Handles btnMenuInicio.Click
        Ir(If(SesionUsuario.EsPersonal, "~/wfInicio.aspx", "~/Lector/wfMiBiblioteca.aspx"))
    End Sub
    Protected Sub Menu(sender As Object, e As EventArgs) Handles btnMiBiblioteca.Click, btnBuscar.Click, btnPrestamos.Click, btnSolicitudes.Click, btnLibros.Click, btnEjemplares.Click, btnAutores.Click, btnCategorias.Click, btnPenalizaciones.Click, btnCargos.Click, btnUsuarios.Click, btnBitacora.Click
        Ir(DirectCast(sender, Button).CommandArgument)
    End Sub
    Protected Sub Salir(sender As Object, e As EventArgs) Handles btnSalir.Click
        AccesoWeb.Salir(Context)
        Ir("~/Default.aspx")
    End Sub
End Class
