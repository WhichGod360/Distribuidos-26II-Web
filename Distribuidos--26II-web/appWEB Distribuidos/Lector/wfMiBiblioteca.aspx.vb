Imports System.Linq

Public Class MiBiblioteca
    Inherits PaginaLector

    Private biblioteca As BibliotecaLectorSoap
    Private seleccion As List(Of Libro)
    Private pagoYape As Boolean

    Private ReadOnly Property Vista As String
        Get
            Dim valor = If(Request.QueryString("vista"), "catalogo")
            Return If({"catalogo", "carrito", "reservas", "prestamos", "cargos", "avisos"}.Contains(valor), valor, "catalogo")
        End Get
    End Property
    Private Property CarritoIds As List(Of Integer)
        Get
            If Session("Lector.Carrito") Is Nothing Then Session("Lector.Carrito") = New List(Of Integer)()
            Return DirectCast(Session("Lector.Carrito"), List(Of Integer))
        End Get
        Set(value As List(Of Integer))
            Session("Lector.Carrito") = value
        End Set
    End Property
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then Ejecutar(lblMensaje, Sub()
            CargarEstado()
            ConfigurarVistas()
            FormulariosWeb.Lista(ddlCategoria, biblioteca.Catalogo.SelectMany(Function(l) If(l.Categoria, "").Split(","c)).Select(Function(c) c.Trim()).Where(Function(c) c <> "").Distinct().OrderBy(Function(c) c).Select(Function(c) New With {.Nombre = c}).ToList(), "Nombre", "Nombre", "Todas las categorías")
            Mostrar()
            MostrarAviso(lblMensaje)
        End Sub)
    End Sub
    Private Sub CargarEstado()
        If biblioteca IsNot Nothing Then Return
        biblioteca = WS.ConsultarBibliotecaLector(CarritoIds.ToArray())
        Dim estado = biblioteca.Circulacion
        seleccion = CarritoIds.Select(Function(id) If(biblioteca.Catalogo.FirstOrDefault(Function(l) l.IdLibro = id),
            New Libro With {.IdLibro = id, .TituloLibro = "Título ya no disponible. Quítalo del carrito."})).ToList()
        litEstado.Text = estado.LibrosPendientes & " libro(s) pendientes · Cupo de préstamos disponible: " & estado.CupoDisponible & " · Carrito: " & CarritoIds.Count &
            If(estado.TieneSolicitudVigente, " · Ya tienes una reserva vigente. Podrás solicitar otra cuando se cierre.", "") & If(estado.TienePagosPorRegularizar, " · Regulariza tus cargos antes de solicitar nuevos libros.", "")
    End Sub
    Private Sub ConfigurarVistas()
        Dim paneles = New Panel() {pnlCatalogo, pnlCarrito, pnlReservas, pnlPrestamos, pnlCargos, pnlAvisos}
        Dim enlaces = New HyperLink() {lnkCatalogo, lnkCarrito, lnkReservas, lnkPrestamos, lnkCargos, lnkAvisos}
        Dim nombres = {"catalogo", "carrito", "reservas", "prestamos", "cargos", "avisos"}
        For i = 0 To nombres.Length - 1
            paneles(i).Visible = nombres(i) = Vista
            If paneles(i).Visible Then
                enlaces(i).CssClass = "active"
                enlaces(i).Attributes("aria-current") = "page"
            End If
        Next
    End Sub
    Private Sub Mostrar()
        CargarEstado()
        Select Case Vista
            Case "catalogo"
                Dim librosFiltrados = biblioteca.Catalogo.Where(Function(l) FormulariosWeb.Coincide(l.TituloLibro & " " & l.Autor & " " & l.EditorialLibro, txtBuscar.Text) AndAlso
                    (ddlCategoria.SelectedValue = "0" OrElse ddlCategoria.SelectedValue = "" OrElse If(l.Categoria, "").Split(","c).Any(Function(c) c.Trim() = ddlCategoria.SelectedValue))).ToList()
                lvCatalogo.DataSource = librosFiltrados
                lvCatalogo.DataBind()
                pgrCatalogo.Visible = librosFiltrados.Count > pgrCatalogo.PageSize
            Case "carrito"
                grvCarrito.DataSource = seleccion
                grvCarrito.DataBind()
                btnSolicitar.Enabled = biblioteca.PuedeSolicitar
                btnVaciar.Enabled = seleccion.Count > 0
            Case "reservas"
                grvReservas.DataSource = biblioteca.Reservas
                grvReservas.DataBind()
            Case "prestamos"
                grvPrestamos.DataSource = biblioteca.Prestamos.Select(Function(p) New With {p.IdPrestamo, p.TituloLibro, p.Ejemplares, p.FechaInicio, p.FechaFin, p.FechaRetorno, .Estado = p.EstadoTexto}).ToList()
                grvPrestamos.DataBind()
            Case "cargos"
                pagoYape = WS.PagoYapeDisponible() AndAlso ImagenYape.Configurada(Context)
                litCargos.Text = "Total: " & FormulariosWeb.Dinero(biblioteca.Cargos.Total) & " · Pendiente: " & FormulariosWeb.Dinero(biblioteca.Cargos.Pendiente) & " · Por revisar: " & FormulariosWeb.Dinero(biblioteca.Cargos.PorRevisar) & " · Pagado: " & FormulariosWeb.Dinero(biblioteca.Cargos.Pagado)
                grvCargos.DataSource = biblioteca.Cargos.Detalles.Where(Function(d) ddlPago.SelectedValue = "" OrElse d.EstadoPago = ddlPago.SelectedValue).ToList()
                grvCargos.DataBind()
            Case "avisos"
                Dim avisos = WS.ListarMisAvisos()
                rptAvisos.DataSource = avisos
                rptAvisos.DataBind()
                litSinAvisos.Text = If(avisos.Count = 0, "No tienes avisos nuevos.", "")
        End Select
    End Sub
    Protected Function RutaPago(id As Object) As String
        Return "~/Lector/wfPagarYape.aspx?cargo=" & CInt(id)
    End Function

    Protected Function PuedeConsultarPago(datos As Object) As Boolean
        Dim cargo = DirectCast(datos, DetallePenalizacion)
        Return pagoYape
    End Function

    Private Sub Volver(vistaDestino As String, mensaje As String)
        Guardado(mensaje, "~/Lector/wfMiBiblioteca.aspx?vista=" & vistaDestino)
    End Sub
    Protected Function PuedeAgregarLibro(datos As Object) As Boolean
        Dim libro = DirectCast(datos, Libro)
        Return biblioteca.LibrosAgregables.Contains(libro.IdLibro)
    End Function
    Protected Function TextoAgregar(datos As Object) As String
        Dim libro = DirectCast(datos, Libro)
        Return If(CarritoIds.Contains(libro.IdLibro), "En tu carrito", "Agregar al carrito")
    End Function
    Protected Sub Agregar(sender As Object, e As ListViewSelectEventArgs) Handles lvCatalogo.SelectedIndexChanging
        e.Cancel = True
        Ejecutar(lblMensaje, Sub()
            CargarEstado()
            If e.NewSelectedIndex < 0 OrElse e.NewSelectedIndex >= lvCatalogo.DataKeys.Count Then Throw New ArgumentException("Selecciona un libro del catálogo.")
            Dim idLibro = CInt(lvCatalogo.DataKeys(e.NewSelectedIndex).Value)
            If Not biblioteca.LibrosAgregables.Contains(idLibro) Then Throw New InvalidOperationException("No puedes agregar este libro. Revisa disponibilidad, cupo, reservas y pagos.")
            CarritoIds.Add(idLibro)
            Volver("catalogo", "Libro agregado al carrito.")
        End Sub)
    End Sub
    Protected Sub Quitar(sender As Object, e As EventArgs) Handles grvCarrito.SelectedIndexChanged
        CarritoIds = CarritoIds.Where(Function(id) id <> CInt(grvCarrito.SelectedDataKey.Value)).ToList()
        Volver("carrito", "Libro retirado del carrito.")
    End Sub
    Protected Sub Vaciar(sender As Object, e As EventArgs) Handles btnVaciar.Click
        CarritoIds = New List(Of Integer)()
        Volver("carrito", "Carrito vacío.")
    End Sub
    Protected Sub Solicitar(sender As Object, e As EventArgs) Handles btnSolicitar.Click
        Ejecutar(lblMensaje, Sub()
            CargarEstado()
            If Not biblioteca.PuedeSolicitar Then Throw New InvalidOperationException("Revisa disponibilidad, cupo, reservas vigentes y pagos antes de solicitar.")
            Dim id = WS.SolicitarLibros(CarritoIds.ToArray())
            CarritoIds = New List(Of Integer)()
            Volver("reservas", "Solicitud N.º " & id & " registrada. Espera su aprobación antes del recojo.")
        End Sub)
    End Sub
    Protected Function PuedeCancelarReserva(datos As Object) As Boolean
        Return If(TryCast(datos, SolicitudPrestamo)?.EstaVigente, False)
    End Function

    Protected Sub Cancelar(sender As Object, e As EventArgs) Handles grvReservas.SelectedIndexChanged
        pnlCancelar.Visible = False
        Ejecutar(lblMensaje, Sub()
            CargarEstado()
            Dim id = CInt(grvReservas.SelectedDataKey.Value)
            Dim s = biblioteca.Reservas.FirstOrDefault(Function(r) r.IdSolicitud = id)
            If Not PuedeCancelarReserva(s) Then Throw New InvalidOperationException("Esta reserva ya no se puede cancelar.")
            ViewState("Cancelar") = id
            litCancelar.Text = "¿Cancelar la reserva N.º " & id & "?"
            pnlCancelar.Visible = True
        End Sub)
    End Sub
    Protected Sub ConfirmarCancelar(sender As Object, e As EventArgs) Handles btnConfirmarCancelar.Click
        Ejecutar(lblMensaje, Sub()
            CargarEstado()
            Dim id = Entero(CStr(ViewState("Cancelar")), 1)
            If Not pnlCancelar.Visible OrElse Not PuedeCancelarReserva(biblioteca.Reservas.FirstOrDefault(Function(s) s.IdSolicitud = id)) Then Throw New InvalidOperationException("La reserva cambió. Actualiza tus reservas.")
            WS.CancelarSolicitud(id)
            Volver("reservas", "Reserva cancelada.")
        End Sub)
    End Sub
    Protected Sub NoCancelar(sender As Object, e As EventArgs) Handles btnNoCancelar.Click
        pnlCancelar.Visible = False
        ViewState.Remove("Cancelar")
    End Sub
    Protected Sub Buscar(sender As Object, e As EventArgs) Handles btnBuscar.Click, btnActualizar.Click, btnFiltrarCargos.Click
        Ejecutar(lblMensaje, Sub()
            pgrCatalogo.SetPageProperties(0, pgrCatalogo.PageSize, False)
            Mostrar()
        End Sub)
    End Sub
    Protected Sub Paginar(sender As Object, e As PagePropertiesChangingEventArgs) Handles lvCatalogo.PagePropertiesChanging
        Ejecutar(lblMensaje, Sub()
            pgrCatalogo.SetPageProperties(e.StartRowIndex, e.MaximumRows, False)
            Mostrar()
        End Sub)
    End Sub
End Class
