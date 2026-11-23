Imports appWEB_Distribuidos.Librerias
Imports System.Linq

Public Class PrestamosWeb
    Inherits PaginaMantenimiento

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then Ejecutar(lblMensaje, Sub()
            Dim filtro = Request.QueryString("estado")
            If ddlEstado.Items.FindByValue(filtro) IsNot Nothing Then ddlEstado.SelectedValue = filtro
            txtBuscar.Text = If(Request.QueryString("buscar"), "")
            Listar()
            If Request.QueryString("nuevo") = "1" Then Nuevo()
            MostrarAviso(lblMensaje)
        End Sub)
    End Sub

    Private Function Datos() As Prestamo()
        Dim lector As Integer
        If Integer.TryParse(Request.QueryString("lector"), lector) AndAlso lector > 0 Then
            Return Servicio.BuscarPrestamosPorLector(lector, ddlEstado.SelectedValue, txtBuscar.Text)
        End If
        Return Servicio.ListarPrestamos(ddlEstado.SelectedValue, txtBuscar.Text)
    End Function

    Private Sub Listar()
        grvDatos.DataSource = Datos().Select(Function(p) New With {p.IdPrestamo, p.NombreLector, p.TituloLibro, p.Ejemplares, p.FechaFin, p.FechaRetorno, .Estado = p.EstadoEnVista}).ToList()
        grvDatos.DataBind()
    End Sub

    Private Sub Nuevo()
        pnlDetalle.Visible = False
        pnlNuevo.Visible = True
        ddlLector.Items.Clear()
        ddlLibro.Items.Clear()
        ddlEjemplar.Items.Clear()
        lstSeleccion.Items.Clear()
        litPlazo.Text = "Inicio: " & Date.Today.ToString("dd/MM/yyyy") & " · Límite: " & Servicio.FechaLimitePrestamo(Date.Today).ToString("dd/MM/yyyy")
        Dim id As Integer
        If Integer.TryParse(Request.QueryString("lector"), id) AndAlso id > 0 Then
            FormulariosWeb.Lista(ddlLector, Servicio.BuscarLectores("").Where(Function(u) u.IdUsuario = id).ToList(), "NombreCompleto", "IdUsuario")
            If ddlLector.Items.FindByValue(id.ToString()) IsNot Nothing Then
                ddlLector.SelectedValue = id.ToString()
                CargarLibros()
            End If
        End If
    End Sub

    Private Sub CargarLibros()
        ddlLibro.Items.Clear()
        ddlEjemplar.Items.Clear()
        FormulariosWeb.Lista(ddlLibro, Servicio.ListarLibrosDisponibles(FormulariosWeb.Id(ddlLector)), "TituloLibro", "IdLibro")
    End Sub

    Protected Sub btnBuscarLector_Click(sender As Object, e As EventArgs) Handles btnBuscarLector.Click
        Ejecutar(lblMensaje, Sub()
            If String.IsNullOrWhiteSpace(txtLector.Text) Then Throw New ArgumentException("Escribe el nombre, DNI o correo del lector.")
            FormulariosWeb.Lista(ddlLector, Servicio.BuscarLectores(txtLector.Text).Select(Function(u) New With {u.IdUsuario, .Texto = u.NombreCompleto & " · " & u.NumeroDniUsuario}).ToList(), "Texto", "IdUsuario")
            ddlLibro.Items.Clear()
            ddlEjemplar.Items.Clear()
            lstSeleccion.Items.Clear()
        End Sub)
    End Sub

    Protected Sub LectorCambiado(sender As Object, e As EventArgs) Handles ddlLector.SelectedIndexChanged
        lstSeleccion.Items.Clear()
        Ejecutar(lblMensaje, AddressOf CargarLibros)
    End Sub

    Protected Sub LibroCambiado(sender As Object, e As EventArgs) Handles ddlLibro.SelectedIndexChanged
        ddlEjemplar.Items.Clear()
        Ejecutar(lblMensaje, Sub() FormulariosWeb.Lista(ddlEjemplar, Servicio.ListarEjemplaresDisponibles(FormulariosWeb.Id(ddlLibro), FormulariosWeb.Id(ddlLector)), "IdEjemplar", "IdEjemplar"))
    End Sub

    Protected Sub Agregar(sender As Object, e As EventArgs) Handles btnAgregar.Click
        Ejecutar(lblMensaje, Sub()
            If FormulariosWeb.Id(ddlLibro) <= 0 OrElse FormulariosWeb.Id(ddlEjemplar) <= 0 Then Throw New ArgumentException("Selecciona un libro y un ejemplar.")
            If lstSeleccion.Items.Count >= 5 Then Throw New ArgumentException("El préstamo admite como máximo cinco ejemplares.")
            Dim titulo = ddlLibro.SelectedItem.Text
            If lstSeleccion.Items.Cast(Of ListItem)().Any(Function(item) item.Text.StartsWith(titulo & " · ", StringComparison.Ordinal)) Then Throw New ArgumentException("Ese libro ya está agregado.")
            lstSeleccion.Items.Add(New ListItem(titulo & " · ejemplar " & ddlEjemplar.SelectedValue, ddlEjemplar.SelectedValue))
        End Sub)
    End Sub

    Protected Sub Quitar(sender As Object, e As EventArgs) Handles btnQuitar.Click
        If lstSeleccion.SelectedIndex >= 0 Then lstSeleccion.Items.RemoveAt(lstSeleccion.SelectedIndex)
    End Sub

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        Ejecutar(lblMensaje, Sub()
            If String.IsNullOrEmpty(ddlLector.SelectedValue) OrElse ddlLector.SelectedValue = "0" Then Throw New ArgumentException("Selecciona un lector.")
            Dim ids = lstSeleccion.Items.Cast(Of ListItem)().Select(Function(item) Integer.Parse(item.Value)).ToList()
            If ids.Count = 0 Then
                If FormulariosWeb.Id(ddlEjemplar) <= 0 Then Throw New ArgumentException("Selecciona un ejemplar.")
                ids.Add(FormulariosWeb.Id(ddlEjemplar))
            End If
            Dim id = Servicio.RegistrarPrestamo(FormulariosWeb.Id(ddlLector), ids.ToArray(), Date.Today)
            Guardado("Préstamo N.º " & id & " registrado con " & ids.Count & " ejemplar(es).")
        End Sub)
    End Sub

    Protected Sub Seleccionar(sender As Object, e As EventArgs) Handles grvDatos.SelectedIndexChanged
        pnlDetalle.Visible = False
        pnlConfirmar.Visible = False
        Ejecutar(lblMensaje, Sub()
            RegistroId = CInt(grvDatos.SelectedDataKey.Value)
            Dim p = Datos().FirstOrDefault(Function(x) x.IdPrestamo = RegistroId)
            If p Is Nothing Then Throw New InvalidOperationException("Actualiza el listado de préstamos.")
            litDetalle.Text = "Préstamo N.º " & p.IdPrestamo
            litDatos.Text = p.NombreLector & " · " & p.TituloLibro & " · Ejemplares " & p.Ejemplares & " · " & p.EstadoEnVista
            lnkComprobante.NavigateUrl = "~/Circulacion/wfComprobante.aspx?prestamo=" & p.IdPrestamo
            lnkDevolucion.Visible = p.FechaRetorno.HasValue
            lnkDevolucion.NavigateUrl = lnkComprobante.NavigateUrl & "&devolucion=1"
            txtRetorno.Text = FechaTexto(Date.Today)
            pnlDevolver.Visible = p.EstaActivo
            pnlNuevo.Visible = False
            pnlDetalle.Visible = True
        End Sub)
    End Sub

    Protected Sub btnCalcular_Click(sender As Object, e As EventArgs) Handles btnCalcular.Click
        pnlConfirmar.Visible = False
        Ejecutar(lblMensaje, Sub()
            Dim fecha = FechaOpcional(txtRetorno.Text)
            If Not fecha.HasValue Then Throw New ArgumentException("Selecciona la fecha de devolución.")
            Dim calculo = Servicio.PrevisualizarDevolucion(RegistroId, fecha.Value)
            Dim clave = Guid.NewGuid().ToString("N")
            If ViewState("Calculo") IsNot Nothing Then Session.Remove("Devolucion." & CStr(ViewState("Calculo")))
            Session("Devolucion." & clave) = calculo
            ViewState("Calculo") = clave
            litCalculo.Text = calculo.Resumen
            pnlConfirmar.Visible = True
        End Sub)
    End Sub

    Protected Sub btnDevolver_Click(sender As Object, e As EventArgs) Handles btnDevolver.Click
        Ejecutar(lblMensaje, Sub()
            Dim clave = "Devolucion." & CStr(ViewState("Calculo"))
            Dim calculo = TryCast(Session(clave), DevolucionCalculada)
            Dim fecha = FechaOpcional(txtRetorno.Text)
            If calculo Is Nothing OrElse calculo.IdPrestamo <> RegistroId OrElse Not fecha.HasValue OrElse fecha.Value.Date <> calculo.FechaRetorno.Date Then Throw New InvalidOperationException("Calcula nuevamente la devolución antes de confirmarla.")
            Dim resultado = Servicio.ConfirmarDevolucion(calculo)
            Session.Remove(clave)
            Guardado("Devolución registrada. " & resultado.Resumen)
        End Sub)
    End Sub

    Protected Sub btnNuevo_Click(sender As Object, e As EventArgs) Handles btnNuevo.Click
        Ejecutar(lblMensaje, AddressOf Nuevo)
    End Sub
    Protected Sub Cancelar(sender As Object, e As EventArgs) Handles btnCancelarNuevo.Click, btnCerrar.Click
        pnlNuevo.Visible = False
        pnlDetalle.Visible = False
        pnlConfirmar.Visible = False
    End Sub
    Protected Sub Buscar(sender As Object, e As EventArgs) Handles btnBuscar.Click
        Dim filtros = HttpUtility.ParseQueryString(String.Empty)
        filtros("estado") = ddlEstado.SelectedValue
        filtros("buscar") = txtBuscar.Text
        Dim lector As Integer
        If Integer.TryParse(Request.QueryString("lector"), lector) AndAlso lector > 0 Then filtros("lector") = lector.ToString()
        Redirigir(Request.AppRelativeCurrentExecutionFilePath & "?" & filtros.ToString())
    End Sub
    Protected Sub Paginar(sender As Object, e As GridViewPageEventArgs) Handles grvDatos.PageIndexChanging
        Ejecutar(lblMensaje, Sub()
            grvDatos.PageIndex = e.NewPageIndex
            Listar()
        End Sub)
    End Sub
End Class
