Imports capaNegocio

Public Class RegistroLectorWeb
    Inherits PaginaBase
    Protected Sub Registrar(sender As Object, e As EventArgs) Handles btnRegistrar.Click
        Ejecutar(lblMensaje, Sub()
            Dim usuario = campos.Leer(0)
            AccesoWeb.Limitar(Context, usuario.EmailUsuario, "registro")
            Dim negocio As New usuarioNegocio()
            negocio.RegistrarNuevoLector(usuario, campos.Confirmacion)
            Session("Registro.Creado") = True
            Redirigir("~/Default.aspx")
        End Sub)
    End Sub
End Class
