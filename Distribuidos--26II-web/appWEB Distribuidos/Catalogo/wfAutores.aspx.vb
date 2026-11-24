Imports System.Data
Imports System.Linq

Public Class Autores
    Inherits PaginaMantenimiento

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then Ejecutar(lblMensaje, Sub()
            
            Listar()
            MostrarAviso(lblMensaje)
        End Sub)
    End Sub

    Private Sub Listar()
        Dim datos = WS.ListarAutores().Where(Function(a) Coincide(a.NombreAutor & " " & a.NacionalidadAutor, txtBuscar.Text)).ToList()
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
        txtNacionalidad.Text = ""
        txtNacimiento.Text = ""
        txtFallecimiento.Text = ""
        txtBiografia.Text = ""
    End Sub

    Private Sub Cargar(id As Integer)
        RegistroId = 0
        EliminarId = 0
        pnlEditor.Visible = False
        pnlConfirmar.Visible = False
        Dim a = WS.ObtenerAutor(id)
        If a Is Nothing Then Throw New InvalidOperationException("El autor ya no existe.")
        txtNombre.Text = a.NombreAutor
        txtNacionalidad.Text = a.NacionalidadAutor
        txtNacimiento.Text = FechaTexto(a.FechaNacimientoAutor)
        txtFallecimiento.Text = FechaTexto(a.FechaFallecimientoAutor)
        txtBiografia.Text = a.BiografiaAutor
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
            If RegistroId = 0 Then
                Comprobar(WS.AgregarAutor(txtNombre.Text, txtNacionalidad.Text, FechaOpcional(txtNacimiento.Text), FechaOpcional(txtFallecimiento.Text), txtBiografia.Text))
            Else
                Comprobar(WS.ActualizarAutor(RegistroId, txtNombre.Text, txtNacionalidad.Text, FechaOpcional(txtNacimiento.Text), FechaOpcional(txtFallecimiento.Text), txtBiografia.Text))
            End If
            Guardado()
        End Sub)
    End Sub

    Protected Sub btnEliminar_Click(sender As Object, e As EventArgs) Handles btnEliminar.Click
        Ejecutar(lblMensaje, Sub()
            If RegistroId <= 0 Then Throw New InvalidOperationException("Selecciona un registro.")
            EliminarId = RegistroId
            litConfirmacion.Text = "¿Confirmas eliminar el registro N.º " & EliminarId & "?"
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
            Comprobar(WS.EliminarAutor(EliminarId))
            Guardado()
        End Sub)
    End Sub
End Class
