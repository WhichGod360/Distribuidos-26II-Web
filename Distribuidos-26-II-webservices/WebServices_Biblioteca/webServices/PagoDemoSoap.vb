Option Strict On

Imports System.Globalization
Imports System.Security.Cryptography
Imports System.Web.Security
Imports System.Web.Services
Imports capaDatos
Imports capaNegocio

Partial Public Class swBiblioteca
    Private Const PropositoPagoDemo As String = "Lecturae.PagoDemo.v1"

    <WebMethod(EnableSession:=True)>
    Public Function PagoDemoDisponible() As Boolean
        seguridadNegocio.ExigirLector()
        Return pagosYape.DemoDisponible()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function EmitirTokenPagoDemo(idCargo As Integer) As String
        If Not pagosYape.DemoDisponible() Then Throw New InvalidOperationException("Demostración no disponible.")
        Dim cargo = pagosYape.CargoMio(idCargo)
        Dim importe = cargo.Field(Of Decimal)("Monto")
        If cargo.Field(Of String)("Estado") <> DetallePenalizacion.PagoPendiente OrElse importe <= 0D Then
            Throw New InvalidOperationException("El cargo no está pendiente de pago.")
        End If

        Dim cultura = CultureInfo.InvariantCulture
        Dim datos = String.Join("|", idCargo.ToString(cultura), SesionUsuario.IdUsuario.ToString(cultura),
            importe.ToString(cultura), Date.UtcNow.AddMinutes(5).Ticks.ToString(cultura), Guid.NewGuid().ToString("N"))
        Return HttpServerUtility.UrlTokenEncode(MachineKey.Protect(Encoding.UTF8.GetBytes(datos), PropositoPagoDemo))
    End Function

    <WebMethod()>
    Public Function ProcesarPagoDemo(token As String, confirmar As Boolean) As ResultadoPagoDemoSoap
        If Not pagosYape.DemoDisponible() Then Return Nothing

        Dim cargo, lector As Integer
        Dim monto As Decimal
        Dim transaccion As Guid
        If Not LeerTokenPagoDemo(token, cargo, lector, monto, transaccion) Then
            Throw New ArgumentException("El QR ya venció o no es válido. Genera otro desde Mis penalizaciones.")
        End If

        Return New ResultadoPagoDemoSoap With {.IdCargo = cargo,
            .PagoNuevo = confirmar AndAlso pagosYape.ConfirmarDemo(cargo, lector, monto, transaccion)}
    End Function

    Private Shared Function LeerTokenPagoDemo(token As String, ByRef cargo As Integer, ByRef lector As Integer,
                                              ByRef monto As Decimal, ByRef transaccion As Guid) As Boolean
        If String.IsNullOrEmpty(token) OrElse token.Length > 512 Then Return False
        Try
            Dim protegido = HttpServerUtility.UrlTokenDecode(token)
            If protegido Is Nothing Then Return False
            Dim datos = MachineKey.Unprotect(protegido, PropositoPagoDemo)
            If datos Is Nothing Then Return False
            Dim campos = Encoding.UTF8.GetString(datos).Split("|"c)
            Dim vence As Long
            Dim cultura = CultureInfo.InvariantCulture
            Return campos.Length = 5 AndAlso Integer.TryParse(campos(0), NumberStyles.None, cultura, cargo) AndAlso cargo > 0 AndAlso
                Integer.TryParse(campos(1), NumberStyles.None, cultura, lector) AndAlso lector > 0 AndAlso
                Decimal.TryParse(campos(2), NumberStyles.AllowDecimalPoint, cultura, monto) AndAlso monto > 0D AndAlso
                Long.TryParse(campos(3), NumberStyles.None, cultura, vence) AndAlso vence > Date.UtcNow.Ticks AndAlso
                Guid.TryParseExact(campos(4), "N", transaccion) AndAlso transaccion <> Guid.Empty
        Catch ex As CryptographicException
            Return False
        Catch ex As FormatException
            Return False
        End Try
    End Function
End Class
