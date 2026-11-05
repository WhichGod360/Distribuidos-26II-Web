Imports capaDatos
Imports capaNegocio
Imports System.Data
Imports System.Linq

Public Class Libros
    Inherits PaginaMantenimiento

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then Ejecutar(lblMensaje, Sub()
            CargarOpciones()
            Listar()
            MostrarAviso(lblMensaje)
        End Sub)
    End Sub

    Private Sub Listar()
        Dim datos = New libroNegocio().listarTodos().Where(Function(a) Coincide(a.TituloLibro & " " & a.Autor & " " & a.Categoria, txtBuscar.Text)).ToList()
        grvDatos.DataSource = datos
        grvDatos.DataBind()
    End Sub

    Private Shared Function Coincide(texto As String, filtro As String) As Boolean
        Return If(texto, "").IndexOf(If(filtro, "").Trim(), StringComparison.OrdinalIgnoreCase) >= 0
    End Function

    Private Sub Limpiar()
        RegistroId = 0
        EliminarId = 0
        pnlConfirmar.Visible = False
        btnEliminar.Visible = False
        litEditor.Text = "Agregar registro"
        CargarOpciones()
        txtTitulo.Text = ""
        txtEditorial.Text = ""
        txtAnio.Text = Date.Today.Year.ToString()
        ddlPublico.SelectedIndex = 0
        chkActivo.Checked = True
        lstAutores.ClearSelection()
        lstCategorias.ClearSelection()
        txtCantidad.Text = "0"
        txtCantidad.Enabled = True
        lnkEjemplares.Visible = False
    End Sub

    Private Sub Cargar(id As Integer)
        RegistroId = 0
        EliminarId = 0
        pnlEditor.Visible = False
        pnlConfirmar.Visible = False
        CargarOpciones()
        Dim a = New libroNegocio().listarTodos().FirstOrDefault(Function(l) l.IdLibro = id)
        If a Is Nothing Then Throw New InvalidOperationException("El libro ya no existe.")
        txtTitulo.Text = a.TituloLibro
        txtEditorial.Text = a.EditorialLibro
        txtAnio.Text = If(a.AnoPublicacion.HasValue, a.AnoPublicacion.Value.ToString(), "")
        If ddlPublico.Items.FindByValue(a.PublicoLibro) Is Nothing Then ddlPublico.Items.Add(New ListItem(a.PublicoLibro, a.PublicoLibro))
        ddlPublico.SelectedValue = a.PublicoLibro
        chkActivo.Checked = a.EstadoLibro
        RelacionesWeb.Seleccionar(lstAutores, a.Autor)
        RelacionesWeb.Seleccionar(lstCategorias, a.Categoria)
        txtCantidad.Text = a.StockTotal.ToString()
        txtCantidad.Enabled = False
        lnkEjemplares.NavigateUrl = "~/Catalogo/wfEjemplares.aspx?libro=" & id
        lnkEjemplares.Visible = True
        RegistroId = id
        pnlEditor.Visible = True
        pnlConfirmar.Visible = False
        btnEliminar.Visible = True
        litEditor.Text = "Editar registro N.º " & id
    End Sub

    Protected Sub btnNuevo_Click(sender As Object, e As EventArgs) Handles btnNuevo.Click
        Ejecutar(lblMensaje, Sub()
            pnlEditor.Visible = False
            Limpiar()
            pnlEditor.Visible = True
        End Sub)
    End Sub

    Protected Sub btnCancelar_Click(sender As Object, e As EventArgs) Handles btnCancelar.Click
        RegistroId = 0
        EliminarId = 0
        pnlEditor.Visible = False
        pnlConfirmar.Visible = False
    End Sub

    Protected Sub btnBuscar_Click(sender As Object, e As EventArgs) Handles btnBuscar.Click
        Ejecutar(lblMensaje, Sub()
            grvDatos.PageIndex = 0
            pnlEditor.Visible = False
            pnlConfirmar.Visible = False
            Listar()
        End Sub)
    End Sub

    Protected Sub grvDatos_SelectedIndexChanged(sender As Object, e As EventArgs) Handles grvDatos.SelectedIndexChanged
        Ejecutar(lblMensaje, Sub() Cargar(CInt(grvDatos.SelectedDataKey.Value)))
    End Sub

    Protected Sub grvDatos_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles grvDatos.PageIndexChanging
        Ejecutar(lblMensaje, Sub()
            grvDatos.PageIndex = e.NewPageIndex
            Listar()
        End Sub)
    End Sub

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        Ejecutar(lblMensaje, Sub()
            Dim anio = CShort(Entero(txtAnio.Text, 1, Date.Today.Year))
            Dim autores = lstAutores.Items.Cast(Of ListItem)().Where(Function(i) i.Selected).Select(Function(i) Integer.Parse(i.Value)).ToList()
            Dim categorias = lstCategorias.Items.Cast(Of ListItem)().Where(Function(i) i.Selected).Select(Function(i) Integer.Parse(i.Value)).ToList()
            Dim negocio As New libroNegocio()
            If RegistroId = 0 Then
                Comprobar(negocio.agregarLibroCompleto(txtTitulo.Text, txtEditorial.Text, anio, ddlPublico.SelectedValue, chkActivo.Checked, autores, categorias, Entero(txtCantidad.Text, 0, 500)))
            Else
                Comprobar(negocio.actualizarLibroCompleto(RegistroId, txtTitulo.Text, txtEditorial.Text, anio, ddlPublico.SelectedValue, chkActivo.Checked, autores, categorias))
            End If
            Guardado()
        End Sub)
    End Sub

    Protected Sub btnEliminar_Click(sender As Object, e As EventArgs) Handles btnEliminar.Click
        Ejecutar(lblMensaje, Sub()
            If RegistroId <= 0 Then Throw New InvalidOperationException("Selecciona un registro.")
            EliminarId = RegistroId
            litConfirmacion.Text = "¿Confirmas desactivar el registro N.º " & EliminarId & "?"
            pnlConfirmar.Visible = True
        End Sub)
    End Sub

    Protected Sub btnNoEliminar_Click(sender As Object, e As EventArgs) Handles btnNoEliminar.Click
        EliminarId = 0
        pnlConfirmar.Visible = False
    End Sub

    Protected Sub btnConfirmar_Click(sender As Object, e As EventArgs) Handles btnConfirmar.Click
        Ejecutar(lblMensaje, Sub()
            If EliminarId <= 0 Then Throw New InvalidOperationException("Confirma un registro antes de continuar.")
            Comprobar(New libroNegocio().eliminarLibro(EliminarId))
            Guardado()
        End Sub)
    End Sub
    Private Sub CargarOpciones()
        lstAutores.Items.Clear()
        For Each autor In New autorNegocio().listarTodos()
            lstAutores.Items.Add(New ListItem(autor.NombreAutor, autor.IdAutor.ToString()))
        Next
        lstCategorias.Items.Clear()
        For Each fila As DataRow In New categoriaNegocio().listarCategoria().Rows
            lstCategorias.Items.Add(New ListItem(CStr(fila("nombre_categoria")), CStr(fila("id_categoria"))))
        Next
    End Sub
End Class

