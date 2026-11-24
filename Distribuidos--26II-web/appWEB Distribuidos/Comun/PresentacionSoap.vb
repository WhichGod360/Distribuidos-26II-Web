Option Strict On

Namespace Libreria
    Partial Public Class DetallePenalizacion
        Public Const PagoPendiente As String = "Pendiente"
        Public Const PagoPorRevisar As String = "PorRevisar"

        Public ReadOnly Property EstadoPagoTexto As String
            Get
                Return If(EstadoPago = PagoPorRevisar, "Por revisar", EstadoPago)
            End Get
        End Property

        Public ReadOnly Property Motivo As String
            Get
                If Not String.IsNullOrWhiteSpace(DescripcionDetallePenalizacion) Then Return DescripcionDetallePenalizacion
                If Not String.IsNullOrWhiteSpace(DescripcionTipoPenalizacion) Then Return DescripcionTipoPenalizacion
                Return TipoPenalizacion
            End Get
        End Property
    End Class

    Partial Public Class PrestamoResumen
        Public ReadOnly Property Texto As String
            Get
                Return $"{IdPrestamo} - {NombreUsuario} - {TituloLibro}"
            End Get
        End Property
    End Class

    Partial Public Class Penalizacion
        Public ReadOnly Property TramoTexto As String
            Get
                Return If(DiasDesde.HasValue, "Desde " & DiasDesde.GetValueOrDefault() & " día(s)", "Otro motivo")
            End Get
        End Property

        Public ReadOnly Property EstadoTexto As String
            Get
                Return If(EstadoPenalizacion, "Activo", "Inactivo")
            End Get
        End Property

        Public ReadOnly Property DescripcionCombo As String
            Get
                Return $"{IdPenalizacion} - {TipoPenalizacion} - S/ {MontoPenalizacion:N2} · {TramoTexto}"
            End Get
        End Property
    End Class

    Partial Public Class SolicitudPrestamo
        Public ReadOnly Property Libros As String
            Get
                Return String.Join(", ", Detalles.Select(Function(d) d.TituloLibro))
            End Get
        End Property

        Public ReadOnly Property LimiteLocal As Date
            Get
                Return TimeZoneInfo.ConvertTimeFromUtc(Date.SpecifyKind(FechaLimiteRetiro, DateTimeKind.Utc), TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"))
            End Get
        End Property
    End Class

    Partial Public Class DevolucionCalculada
        Public ReadOnly Property Resumen As String
            Get
                Dim detalle = If(CantidadEjemplares > 1, $"{CantidadEjemplares} ejemplares en una devolución. ", "")
                If DiasRetraso = 0 Then Return detalle & "Devolución a tiempo. Sin mora."
                Return detalle & $"{DiasRetraso} día(s) de retraso · S/ {Monto:0.00} · {Motivo}" &
                    If(YaRegistrada, " (ya registrada; no se cobrará otra vez)", " (monto fijo, sin acumulación)")
            End Get
        End Property
    End Class

    Partial Public Class Prestamo
        Public ReadOnly Property EstadoTexto As String
            Get
                Return If(EstaActivo AndAlso FechaFin.Date < Date.Today, "Vencido", EstadoPrestamo)
            End Get
        End Property
    End Class
End Namespace
