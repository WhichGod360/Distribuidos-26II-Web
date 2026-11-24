Imports System.Linq

Public Class PanelInicio
    Inherits PaginaMantenimiento

    Protected Overrides Sub OnInit(e As EventArgs)
        If Request.IsAuthenticated AndAlso AccesoWeb.EsLector(AccesoWeb.UsuarioActual(Context)?.NombreRol) Then
            Response.Redirect(ResolveUrl("~/Lector/wfMiBiblioteca.aspx"), True)
        End If
        MyBase.OnInit(e)
    End Sub
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then Ejecutar(lblMensaje, Sub()
            FormulariosWeb.Lista(ddlCategoria, WS.ListarCategoriasIndicadores(), "NombreCategoria", "IdCategoria", Nothing)
            Restablecer()
            Dim admin = AccesoWeb.EsAdministrador
            litTitulo.Text = If(admin, "Resumen de administración", "Tu jornada en la biblioteca")
            lnkNuevo.Text = If(admin, "Gestionar usuarios", "Nuevo préstamo")
            lnkNuevo.NavigateUrl = If(admin, "~/Administracion/wfUsuarios.aspx", "~/Circulacion/wfPrestamos.aspx?nuevo=1")
            lnkDevolver.Text = If(admin, "Gestionar categorías", "Registrar devolución")
            lnkDevolver.NavigateUrl = If(admin, "~/Catalogo/wfCategorias.aspx", "~/Circulacion/wfPrestamos.aspx?estado=Activos")
            Listar()
        End Sub)
    End Sub
    Private Sub Restablecer()
        txtDesde.Text = FechaTexto(New Date(Date.Today.Year, Date.Today.Month, 1))
        txtHasta.Text = FechaTexto(Date.Today)
        ddlCategoria.SelectedValue = "0"
        ddlEstado.SelectedValue = "Todos"
    End Sub
    Private Sub Listar()
        Dim desde = FechaOpcional(txtDesde.Text)
        Dim hasta = FechaOpcional(txtHasta.Text)
        If Not desde.HasValue OrElse Not hasta.HasValue Then Throw New ArgumentException("Selecciona ambas fechas del periodo.")
        Dim d = WS.ConsultarIndicadores(desde.Value, hasta.Value, Entero(ddlCategoria.SelectedValue), ddlEstado.SelectedValue)
        Dim admin = AccesoWeb.EsAdministrador
        litCap0.Text = If(admin, "Préstamos en selección", "Activos en selección")
        litCap1.Text = If(admin, "Lectores atendidos", "Vencidos en selección")
        litCap2.Text = If(admin, "Libros prestados", "Devueltos en selección")
        litNum0.Text = If(admin, d.TotalPrestamos, d.Activos).ToString("N0")
        litNum1.Text = If(admin, d.LectoresAtendidos, d.Vencidos).ToString("N0")
        litNum2.Text = If(admin, d.LibrosPrestados, d.Devueltos).ToString("N0")
        litDisponibles.Text = d.EjemplaresDisponibles.ToString("N0")
        litResumen.Text = "Devoluciones hoy: " & d.DevolucionesHoy & " · Lectores activos: " & d.LectoresActivos & " · Libros habilitados: " & d.LibrosHabilitados & " · Actualizado " & Date.Now.ToString("HH:mm")
        Dim maximo = If(d.PorMes.Count = 0, 1, Math.Max(1, d.PorMes.Max(Function(m) m.Cantidad)))
        rptMeses.DataSource = d.PorMes.Select(Function(m) New With {m.Etiqueta, m.Cantidad, .Maximo = maximo}).ToList()
        rptMeses.DataBind()
        litSinMeses.Text = If(d.PorMes.Count = 0, "No hay préstamos en este periodo.", "")
        grvEstados.DataSource = New Object() {New With {.Estado = "Activos", .Cantidad = d.Activos}, New With {.Estado = "Vencidos", .Cantidad = d.Vencidos}, New With {.Estado = "Devueltos", .Cantidad = d.Devueltos}}
        grvEstados.DataBind()
        grvDevoluciones.DataSource = d.Pendientes
        grvDevoluciones.DataBind()
        grvCategorias.DataSource = d.PorCategoria
        grvCategorias.DataBind()
    End Sub
    Protected Sub Actualizar(sender As Object, e As EventArgs) Handles btnActualizar.Click
        Ejecutar(lblMensaje, AddressOf Listar)
    End Sub
    Protected Sub Limpiar(sender As Object, e As EventArgs) Handles btnLimpiar.Click
        Restablecer()
        Ejecutar(lblMensaje, AddressOf Listar)
    End Sub
End Class
