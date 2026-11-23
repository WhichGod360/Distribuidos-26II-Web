Imports appWEB_Distribuidos.Librerias

Public Class Site1
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim usuario = AccesoWeb.UsuarioActual(Context)
        litUsuario.Text = usuario?.NombreCompleto
        litRol.Text = usuario?.NombreRol
        Dim personal = AccesoWeb.EsPersonal
        Dim admin = AccesoWeb.EsAdministrador
        For Each boton As Button In {btnBuscar, btnPrestamos, btnSolicitudes, btnLibros, btnEjemplares, btnAutores, btnCategorias, btnPenalizaciones, btnCargos}
            boton.Visible = personal
        Next
        btnUsuarios.Visible = admin
        btnLectoresPendientes.Visible = admin
        btnBitacora.Visible = admin
        btnMiBiblioteca.Visible = AccesoWeb.EsLector(usuario?.NombreRol)
        btnMenuInicio.Visible = personal
        Dim ruta = Request.AppRelativeCurrentExecutionFilePath
        For Each boton As Button In {btnMenuInicio, btnMiBiblioteca, btnBuscar, btnPrestamos, btnSolicitudes, btnLibros, btnEjemplares, btnAutores, btnCategorias, btnPenalizaciones, btnCargos, btnUsuarios, btnLectoresPendientes, btnBitacora}
            Dim destino = If(boton Is btnMenuInicio, "~/wfInicio.aspx", boton.CommandArgument)
            boton.CssClass = "menu-button" & If(String.Equals(ruta, destino, StringComparison.OrdinalIgnoreCase), " selected", "")
        Next
    End Sub
    Private Sub Ir(ruta As String)
        Response.Redirect(ResolveUrl(ruta), False)
        Context.ApplicationInstance.CompleteRequest()
    End Sub
    Protected Sub Inicio(sender As Object, e As EventArgs) Handles btnMenuInicio.Click
        Ir(If(AccesoWeb.EsPersonal, "~/wfInicio.aspx", "~/Lector/wfMiBiblioteca.aspx"))
    End Sub
    Protected Sub Menu(sender As Object, e As EventArgs) Handles btnMiBiblioteca.Click, btnBuscar.Click, btnPrestamos.Click, btnSolicitudes.Click, btnLibros.Click, btnEjemplares.Click, btnAutores.Click, btnCategorias.Click, btnPenalizaciones.Click, btnCargos.Click, btnUsuarios.Click, btnLectoresPendientes.Click, btnBitacora.Click
        Ir(DirectCast(sender, Button).CommandArgument)
    End Sub
    Protected Sub Salir(sender As Object, e As EventArgs) Handles btnSalir.Click
        AccesoWeb.Salir(Context)
        Ir("~/Default.aspx")
    End Sub
End Class
