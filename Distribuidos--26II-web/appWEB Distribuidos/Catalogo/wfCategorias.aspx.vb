Imports appWEB_Distribuidos.Librerias
Imports System.Data
Imports System.Linq

Public Class Categorias
    Inherits PaginaMantenimiento

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then Ejecutar(lblMensaje, Sub()
            
            Listar()
            MostrarAviso(lblMensaje)
        End Sub)
    End Sub

    Private Sub Listar()
        Dim tabla = Servicio.BuscarCategorias(txtBuscar.Text, "Todos")
        Dim datos = tabla
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
        txtNombre.Text = ""
        chkActivo.Checked = True
    End Sub

    Private Sub Cargar(id As Integer)
        RegistroId = 0
        EliminarId = 0
        pnlEditor.Visible = False
        pnlConfirmar.Visible = False
        Dim tabla = Servicio.ObtenerCategoria(id)
        If tabla.Rows.Count = 0 Then Throw New InvalidOperationException("La categoría ya no existe.")
        txtNombre.Text = CStr(tabla.Rows(0)("nombre_categoria"))
        chkActivo.Checked = Convert.ToBoolean(tabla.Rows(0)("estado"))
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
        Limpiar()
        pnlEditor.Visible = False
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
            Dim nombre = txtNombre.Text.Trim()
            Dim estado = chkActivo.Checked
            Dim resultado = If(RegistroId = 0,
                              Servicio.AgregarCategoria(nombre, estado),
                              Servicio.ActualizarCategoria(RegistroId, nombre, estado))
            If Not resultado AndAlso Servicio.ListarCategorias("Todos").AsEnumerable().Any(
                Function(fila) CInt(fila("id_categoria")) <> RegistroId AndAlso
                    String.Equals(fila("nombre_categoria").ToString().Trim(), nombre, StringComparison.OrdinalIgnoreCase)) Then
                Throw New InvalidOperationException("Ya existe una categoría con ese nombre.")
            End If
            Comprobar(resultado)
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
            Comprobar(Servicio.DesactivarCategoria(EliminarId))
            Guardado()
        End Sub)
    End Sub
End Class
