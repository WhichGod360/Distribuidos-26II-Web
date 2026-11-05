<%@ Page Language="VB" %>
<script runat="server">
Protected Sub Page_Load(sender As Object, e As EventArgs)
    If Not Request.IsLocal Then
        Response.StatusCode = 404
        Return
    End If
    Response.ContentType = "text/plain"
    Try
        Dim ruta = Environment.GetEnvironmentVariable("BIBLIOTECA_CONNECTION_FILE")
        Response.Write("ARCHIVO_PRIVADO=" & System.IO.File.Exists(ruta).ToString() & vbLf)
        Using cn As System.Data.SqlClient.SqlConnection = capaConexion.Conexion.ObtenerInstancia().CrearConexionIndependiente()
            Response.Write("CONEXION_IIS=" & cn.State.ToString() & vbLf)
        End Using
    Catch ex As Exception
        Response.StatusCode = 500
        Response.Write("ERROR=" & ex.GetType().Name)
    End Try
End Sub
</script>
