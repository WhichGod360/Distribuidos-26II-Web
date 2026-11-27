Imports System.Drawing.Imaging
Imports System.IO
Imports System.Web.SessionState

Public NotInheritable Class ImagenYape
    Implements IHttpHandler, IRequiresSessionState

    Private Shared Function Ruta(contexto As HttpContext) As String
        Return contexto.Server.MapPath("~/App_Data/YapeQr.png")
    End Function

    Friend Shared Function Configurada(contexto As HttpContext) As Boolean
        Return File.Exists(Ruta(contexto))
    End Function

    Public ReadOnly Property IsReusable As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

    Public Sub ProcessRequest(contexto As HttpContext) Implements IHttpHandler.ProcessRequest
        AccesoWeb.ActualizarIdentidad(contexto)
        If Not AccesoWeb.EsLector(AccesoWeb.UsuarioActual(contexto)?.NombreRol) Then
            Throw New HttpException(403, "El QR está disponible para lectores.")
        End If
        If Not Configurada(contexto) OrElse Not ServicioBiblioteca.Cliente.PagoYapeDisponible() Then
            Throw New HttpException(404, "QR de Yape no disponible.")
        End If

        contexto.Response.Cache.SetNoStore()
        contexto.Response.ContentType = "image/png"
        Using archivo = File.OpenRead(Ruta(contexto)),
              codigo = RecorteQrYape.ParaMostrar(archivo),
              png As New MemoryStream()
            codigo.Save(png, ImageFormat.Png)
            png.WriteTo(contexto.Response.OutputStream)
        End Using
    End Sub
End Class

