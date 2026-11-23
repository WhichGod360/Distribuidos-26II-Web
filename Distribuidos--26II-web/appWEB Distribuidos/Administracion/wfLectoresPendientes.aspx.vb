
Public Class LectoresPendientesWeb
    Inherits PaginaAdministrador

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then Ejecutar(lblMensaje, Sub()
                                                        Listar()
                                                        MostrarAviso(lblMensaje)
                                                    End Sub)
    End Sub

    Private Sub Listar()
        grvDatos.DataSource = Servicio.ListarLectoresPendientes(txtBuscar.Text)
        grvDatos.DataBind()
    End Sub

    Private Sub CerrarConfirmacion()
        pnlConfirmar.Visible = False
        EliminarId = 0
    End Sub

    Protected Sub Accion(sender As Object, e As GridViewCommandEventArgs) Handles grvDatos.RowCommand
        If e.CommandName <> "Aprobar" AndAlso e.CommandName <> "Rechazar" Then Return
        CerrarConfirmacion()
        Ejecutar(lblMensaje, Sub()
                                 Dim indice = Entero(CStr(e.CommandArgument), 0, grvDatos.Rows.Count - 1)
                                 Dim idLector = CInt(grvDatos.DataKeys(indice).Value)
                                 If e.CommandName = "Aprobar" Then
                                     Servicio.AprobarLector(idLector)
                                     Guardado("El lector fue aprobado y ya puede iniciar sesión.")
                                 Else
                                     EliminarId = idLector
                                     litConfirmar.Text = "¿Rechazar la solicitud de " & HttpUtility.HtmlDecode(grvDatos.Rows(indice).Cells(3).Text) & "? Su cuenta pendiente se eliminará y podrá registrarse nuevamente."
                                     pnlConfirmar.Visible = True
                                 End If
                             End Sub)
    End Sub

    Protected Sub ConfirmarRechazo(sender As Object, e As EventArgs) Handles btnConfirmar.Click
        Ejecutar(lblMensaje, Sub()
                                 If Not pnlConfirmar.Visible OrElse EliminarId <= 0 Then Throw New InvalidOperationException("La solicitud cambió. Selecciónala nuevamente.")
                                 Servicio.RechazarLector(EliminarId)
                                 EliminarId = 0
                                 Guardado("La solicitud fue rechazada y la cuenta pendiente se eliminó.")
                             End Sub)
    End Sub

    Protected Sub NoRechazar(sender As Object, e As EventArgs) Handles btnNoRechazar.Click
        CerrarConfirmacion()
    End Sub

    Protected Sub Buscar(sender As Object, e As EventArgs) Handles btnBuscar.Click
        CerrarConfirmacion()
        Ejecutar(lblMensaje, Sub()
                                 grvDatos.PageIndex = 0
                                 Listar()
                             End Sub)
    End Sub

    Protected Sub Paginar(sender As Object, e As GridViewPageEventArgs) Handles grvDatos.PageIndexChanging
        CerrarConfirmacion()
        Ejecutar(lblMensaje, Sub()
                                 grvDatos.PageIndex = e.NewPageIndex
                                 Listar()
                             End Sub)
    End Sub
End Class
