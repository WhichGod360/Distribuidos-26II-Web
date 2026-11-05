Imports capaDatos
Imports capaNegocio
Imports System.Data
Imports System.Linq

Public Class Penalizaciones
    Inherits PaginaMantenimiento

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then Ejecutar(lblMensaje, Sub()
            
            Listar()
            MostrarAviso(lblMensaje)
        End Sub)
    End Sub

    Private Sub Listar()
        Dim datos = New penalizacionNegocio().buscar(txtBuscar.Text, Nothing)
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
        txtTipo.Text = ""
        txtDescripcion.Text = ""
        txtMonto.Text = ""
        txtDias.Text = ""
        chkActivo.Checked = True
    End Sub

    Private Sub Cargar(id As Integer)
        RegistroId = 0
        EliminarId = 0
        pnlEditor.Visible = False
        pnlConfirmar.Visible = False
        Dim a = New penalizacionNegocio().buscarPorId(id)
        If a Is Nothing Then Throw New InvalidOperationException("La penalización ya no existe.")
        txtTipo.Text = a.TipoPenalizacion
        txtDescripcion.Text = a.DescripcionPenalizacion
        txtMonto.Text = a.MontoPenalizacion.ToString("0.00", Globalization.CultureInfo.InvariantCulture)
        txtDias.Text = If(a.DiasDesde.HasValue, a.DiasDesde.Value.ToString(), "")
        chkActivo.Checked = a.EstadoPenalizacion
        RegistroId = id
        pnlEditor.Visible = True
        pnlConfirmar.Visible = False
        btnEliminar.Visible = True
        litEditor.Text = "Editar registro N.º " & id
    End Sub

    Protected Sub btnNuevo_Click(sender As Object, e As EventArgs) Handles btnNuevo.Click
        Ejecutar(lblMensaje, Sub()
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
            Dim monto As Decimal
            If Not Decimal.TryParse(txtMonto.Text.Trim().Replace(",", "."), Globalization.NumberStyles.AllowDecimalPoint, Globalization.CultureInfo.InvariantCulture, monto) Then Throw New ArgumentException("Ingresa un monto válido, con hasta dos decimales.")
            Dim dias As Integer? = Nothing
            If Not String.IsNullOrWhiteSpace(txtDias.Text) Then dias = Entero(txtDias.Text, 1)
            Dim negocio As New penalizacionNegocio()
            If RegistroId = 0 Then
                Comprobar(negocio.agregarPenalizacion(txtTipo.Text, txtDescripcion.Text, monto, chkActivo.Checked, dias))
            Else
                Comprobar(negocio.actualizarPenalizacion(RegistroId, txtTipo.Text, txtDescripcion.Text, monto, chkActivo.Checked, dias))
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
            Comprobar(New penalizacionNegocio().cambiarEstado(EliminarId, False))
            Guardado()
        End Sub)
    End Sub
End Class
