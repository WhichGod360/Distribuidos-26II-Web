Imports appWEB_Distribuidos.Librerias
Imports System.Linq

Public Class BuscarLectores
    Inherits PaginaMantenimiento

    Private ReadOnly Property Busquedas As Dictionary(Of String, String)
        Get
            Dim datos = TryCast(Session("Lectores.Busquedas"), Dictionary(Of String, String))
            If datos Is Nothing Then
                datos = New Dictionary(Of String, String)()
                Session("Lectores.Busquedas") = datos
            End If
            Return datos
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim clave = Request.QueryString("consulta")
        If IsPostBack OrElse clave Is Nothing Then Return
        Ejecutar(lblMensaje, Sub()
            Dim criterio As String = Nothing
            If Not Busquedas.TryGetValue(clave, criterio) Then
                Throw New InvalidOperationException("La búsqueda venció. Ingresa el criterio nuevamente.")
            End If
            txtBuscar.Text = criterio
            Listar()
        End Sub)
    End Sub

    Private Sub Listar()
        If String.IsNullOrWhiteSpace(txtBuscar.Text) Then Throw New ArgumentException("Escribe un nombre, apellido, DNI o correo.")
        grvLectores.EmptyDataText = "No se encontraron lectores que coincidan con la búsqueda."
        grvLectores.DataSource = Servicio.BuscarLectores(txtBuscar.Text)
        grvLectores.DataBind()
    End Sub

    Protected Sub btnBuscar_Click(sender As Object, e As EventArgs) Handles btnBuscar.Click
        pnlHistorial.Visible = False
        Dim clave = Guid.NewGuid().ToString("N")
        Dim consultas = Busquedas
        If consultas.Count >= 20 Then consultas.Remove(consultas.Keys.First())
        consultas.Add(clave, txtBuscar.Text.Trim())
        Redirigir(Request.AppRelativeCurrentExecutionFilePath & "?consulta=" & clave)
    End Sub

    Protected Sub btnLimpiar_Click(sender As Object, e As EventArgs) Handles btnLimpiar.Click
        Redirigir(Request.AppRelativeCurrentExecutionFilePath)
    End Sub

    Protected Sub Seleccionar(sender As Object, e As EventArgs) Handles grvLectores.SelectedIndexChanged
        pnlHistorial.Visible = False
        Ejecutar(lblMensaje, Sub()
            Dim id = CInt(grvLectores.SelectedDataKey.Value)
            Dim lector = Servicio.BuscarLectores(txtBuscar.Text).FirstOrDefault(Function(u) u.IdUsuario = id)
            If lector Is Nothing Then Throw New InvalidOperationException("Busca nuevamente al lector.")
            litLector.Text = lector.NombreCompleto & " · DNI " & lector.NumeroDniUsuario
            grvPrestamos.DataSource = Servicio.ListarPrestamosPorLector(id).Select(Function(p) New With {p.IdPrestamo, p.TituloLibro, p.FechaInicio, p.FechaFin, p.FechaRetorno, .Estado = p.EstadoEnVista}).ToList()
            grvPrestamos.DataBind()
            lnkNuevo.NavigateUrl = "~/Circulacion/wfPrestamos.aspx?nuevo=1&lector=" & id
            lnkPrestamos.NavigateUrl = "~/Circulacion/wfPrestamos.aspx?lector=" & id
            pnlHistorial.Visible = True
        End Sub)
    End Sub
End Class
