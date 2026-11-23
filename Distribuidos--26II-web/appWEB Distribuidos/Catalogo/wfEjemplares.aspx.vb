Imports System.Linq
Public Class Ejemplares
    Inherits PaginaMantenimiento
    Private Property LibroConsultado As Integer
        Get
            Return If(ViewState("Libro") Is Nothing, 0, CInt(ViewState("Libro")))
        End Get
        Set(value As Integer)
            ViewState("Libro") = value
        End Set
    End Property
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then Ejecutar(lblMensaje, Sub()
            ddlLibro.DataSource = Servicio.ListarLibros()
            ddlLibro.DataTextField = "TituloLibro"
            ddlLibro.DataValueField = "IdLibro"
            ddlLibro.DataBind()
            ddlLibro.Items.Insert(0, New ListItem("Selecciona un libro", "0"))
            Dim solicitado As Integer
            If Integer.TryParse(Request.QueryString("libro"), solicitado) AndAlso ddlLibro.Items.FindByValue(solicitado.ToString()) IsNot Nothing Then ddlLibro.SelectedValue = solicitado.ToString()
            LibroConsultado = Entero(ddlLibro.SelectedValue)
            If LibroConsultado > 0 Then Listar()
            MostrarAviso(lblMensaje)
        End Sub)
    End Sub
    Private Sub Listar()
        grvDatos.DataSource = Servicio.ListarEjemplares(LibroConsultado)
        grvDatos.DataBind()
    End Sub
    Protected Sub btnConsultar_Click(sender As Object, e As EventArgs) Handles btnConsultar.Click, ddlLibro.SelectedIndexChanged
        Ejecutar(lblMensaje, Sub()
            pnlConfirmar.Visible = False
            ViewState.Remove("Operacion")
            LibroConsultado = 0
            grvDatos.PageIndex = 0
            grvDatos.DataSource = Nothing
            grvDatos.DataBind()
            If ddlLibro.SelectedValue = "0" Then
                If sender Is btnConsultar Then Throw New ArgumentException("Selecciona un libro.")
                Return
            End If
            LibroConsultado = Entero(ddlLibro.SelectedValue, 1)
            Listar()
        End Sub)
    End Sub
    Protected Sub btnAgregar_Click(sender As Object, e As EventArgs) Handles btnAgregar.Click
        Ejecutar(lblMensaje, Sub()
            If ddlLibro.SelectedValue = "0" Then Throw New ArgumentException("Selecciona un libro.")
            ViewState("Operacion") = "agregar"
            ViewState("LibroPendiente") = Entero(ddlLibro.SelectedValue, 1)
            litConfirmacion.Text = "¿Agregar una copia física activa al libro «" & ddlLibro.SelectedItem.Text & "»?"
            pnlConfirmar.Visible = True
        End Sub)
    End Sub
    Protected Sub grvDatos_SelectedIndexChanged(sender As Object, e As EventArgs) Handles grvDatos.SelectedIndexChanged
        Ejecutar(lblMensaje, Sub()
            ViewState("Operacion") = "estado"
            ViewState("LibroPendiente") = LibroConsultado
            EliminarId = CInt(grvDatos.SelectedDataKey.Values("id_ejemplar"))
            Dim activo = Not CBool(grvDatos.SelectedDataKey.Values("activo"))
            ViewState("NuevoEstado") = activo
            litConfirmacion.Text = "¿Confirmas " & If(activo, "activar", "desactivar") & " el ejemplar N.º " & EliminarId & " del libro «" & ddlLibro.SelectedItem.Text & "»?"
            pnlConfirmar.Visible = True
        End Sub)
    End Sub
    Protected Sub grvDatos_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles grvDatos.PageIndexChanging
        Ejecutar(lblMensaje, Sub()
            grvDatos.PageIndex = e.NewPageIndex
            Listar()
        End Sub)
    End Sub
    Protected Sub btnCancelar_Click(sender As Object, e As EventArgs) Handles btnCancelar.Click
        pnlConfirmar.Visible = False
        ViewState.Remove("Operacion")
    End Sub
    Protected Sub btnConfirmar_Click(sender As Object, e As EventArgs) Handles btnConfirmar.Click
        Ejecutar(lblMensaje, Sub()
            If Not pnlConfirmar.Visible OrElse CStr(ViewState("LibroPendiente")) <> ddlLibro.SelectedValue Then
                Throw New InvalidOperationException("Selecciona el libro y confirma nuevamente la operación.")
            End If
            Dim libro = CInt(ViewState("LibroPendiente"))
            If CStr(ViewState("Operacion")) = "agregar" Then
                Comprobar(Servicio.AgregarEjemplar(libro))
            ElseIf CStr(ViewState("Operacion")) = "estado" AndAlso EliminarId > 0 Then
                Comprobar(Servicio.CambiarEstadoEjemplar(libro, EliminarId, CBool(ViewState("NuevoEstado"))))
            Else
                Throw New InvalidOperationException("Confirma primero una operación.")
            End If
            Guardado(destino:="~/Catalogo/wfEjemplares.aspx?libro=" & libro)
        End Sub)
    End Sub
End Class
