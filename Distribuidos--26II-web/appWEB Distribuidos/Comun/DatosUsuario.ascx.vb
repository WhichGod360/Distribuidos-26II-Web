Imports System.Globalization

Public Class DatosUsuario
    Inherits System.Web.UI.UserControl

    Public Property AccesoVisible As Boolean
        Get
            Return pnlAcceso.Visible
        End Get
        Set(value As Boolean)
            pnlAcceso.Visible = value
        End Set
    End Property

    Public ReadOnly Property Confirmacion As String
        Get
            Return txtConfirmar.Text
        End Get
    End Property

    Public Function Leer(id As Integer) As Usuario
        Dim nacimiento As Date? = Nothing
        If txtNacimiento.Text.Trim() <> "" Then
            Dim fecha As Date
            If Not Date.TryParseExact(txtNacimiento.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, fecha) Then Throw New ArgumentException("Ingresa una fecha de nacimiento válida.")
            Dim zonaLima = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time")
            Dim hoy = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, zonaLima).Date
            If fecha > hoy Then Throw New ArgumentException("La fecha de nacimiento no puede ser futura.")
            If fecha > hoy.AddYears(-18) Then Throw New ArgumentException("Debes tener al menos 18 años.")
            nacimiento = fecha
        End If
        Return New Usuario With {.IdUsuario = id, .NombreUsuario = txtNombre.Text.Trim(), .ApPaterno = txtPaterno.Text.Trim(),
            .ApMaterno = txtMaterno.Text.Trim(), .NumeroDniUsuario = txtDni.Text.Trim(), .EmailUsuario = txtCorreo.Text.Trim(),
            .FechaNacimientoUsuario = nacimiento, .GeneroUsuario = If(ddlGenero.SelectedValue = "", CType(Nothing, Boolean?), CType(ddlGenero.SelectedValue = "1", Boolean?)),
            .Contrasena = txtClave.Text, .PreguntaSegura = txtPregunta.Text.Trim(), .RptaSegura = txtRespuesta.Text}
    End Function

    Public Sub Cargar(usuario As UsuarioSoap)
        txtNombre.Text = usuario.NombreUsuario
        txtPaterno.Text = usuario.ApPaterno
        txtMaterno.Text = usuario.ApMaterno
        txtCorreo.Text = usuario.EmailUsuario
        txtDni.Text = usuario.NumeroDniUsuario
        txtNacimiento.Text = If(usuario.FechaNacimientoUsuario.HasValue, usuario.FechaNacimientoUsuario.GetValueOrDefault().ToString("yyyy-MM-dd"), "")
        ddlGenero.SelectedValue = If(usuario.GeneroUsuario.HasValue, If(usuario.GeneroUsuario.GetValueOrDefault(), "1", "0"), "")
        txtClave.Text = ""
        txtConfirmar.Text = ""
        txtPregunta.Text = ""
        txtRespuesta.Text = ""
        AccesoVisible = usuario.IdUsuario = 0
    End Sub
End Class
