Option Strict On

Imports System.Web.Services
Imports capaDatos
Imports capaNegocio

Partial Public Class swBiblioteca
    Private ReadOnly negocioAutor As New autorNegocio()
    Private ReadOnly negocioCatalogoLector As New catalogoLectorNegocio()
    Private ReadOnly negocioCategoria As New categoriaNegocio()
    Private ReadOnly negocioEjemplar As New ejemplarNegocio()
    Private ReadOnly negocioLibro As New libroNegocio()

    <WebMethod(EnableSession:=True)>
    Public Function ListarCatalogoLector() As Libro()
        Return negocioCatalogoLector.listar().ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListarLibros() As Libro()
        seguridadNegocio.ExigirPersonal()
        Return negocioLibro.listarTodos().ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListarLibrosDisponibles(idLector As Integer) As Libro()
        seguridadNegocio.ExigirPersonal()
        Return negocioLibro.listarDisponibles(idLector).ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function AgregarLibro(titulo As String, editorial As String, anoPublicacion As Short, publico As String,
                                activo As Boolean, idsAutores As Integer(), idsCategorias As Integer(), cantidadEjemplares As Integer) As Boolean
        Return negocioLibro.agregarLibroCompleto(titulo, editorial, anoPublicacion, publico,
            activo, idsAutores, idsCategorias, cantidadEjemplares)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ActualizarLibro(idLibro As Integer, titulo As String, editorial As String, anoPublicacion As Short,
                                   publico As String, activo As Boolean, idsAutores As Integer(), idsCategorias As Integer()) As Boolean
        Return negocioLibro.actualizarLibroCompleto(idLibro, titulo, editorial, anoPublicacion,
            publico, activo, idsAutores, idsCategorias)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function DesactivarLibro(idLibro As Integer) As Boolean
        Return negocioLibro.eliminarLibro(idLibro)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListarAutores() As Autor()
        seguridadNegocio.ExigirPersonal()
        Return negocioAutor.listarTodos().ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ObtenerAutor(idAutor As Integer) As Autor
        seguridadNegocio.ExigirPersonal()
        Return negocioAutor.buscarPorId(idAutor)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function AgregarAutor(nombre As String, nacionalidad As String, fechaNacimiento As Date?,
                                fechaFallecimiento As Date?, biografia As String) As Boolean
        Return negocioAutor.agregar(nombre, nacionalidad, fechaNacimiento, fechaFallecimiento, biografia)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ActualizarAutor(idAutor As Integer, nombre As String, nacionalidad As String, fechaNacimiento As Date?,
                                   fechaFallecimiento As Date?, biografia As String) As Boolean
        Return negocioAutor.actualizar(idAutor, nombre, nacionalidad, fechaNacimiento, fechaFallecimiento, biografia)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function EliminarAutor(idAutor As Integer) As Boolean
        Return negocioAutor.eliminar(idAutor)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListarCategorias(estado As String) As DataTable
        seguridadNegocio.ExigirPersonal()
        Return TablaSoap(negocioCategoria.listarCategoria(If(estado, "Todos")), "Categorias")
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function BuscarCategorias(texto As String, estado As String) As DataTable
        seguridadNegocio.ExigirPersonal()
        Return TablaSoap(negocioCategoria.buscarCategoria(If(texto, ""), If(estado, "Todos")), "Categorias")
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ObtenerCategoria(idCategoria As Integer) As DataTable
        seguridadNegocio.ExigirPersonal()
        Return TablaSoap(negocioCategoria.obtenerCategoria(idCategoria), "Categoria")
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function AgregarCategoria(nombre As String, activa As Boolean) As Boolean
        Return negocioCategoria.agregarCategoria(nombre, If(activa, 1, 0))
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ActualizarCategoria(idCategoria As Integer, nombre As String, activa As Boolean) As Boolean
        Return negocioCategoria.actualizarCategoria(idCategoria, nombre, If(activa, 1, 0))
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function DesactivarCategoria(idCategoria As Integer) As Boolean
        Return negocioCategoria.desactivarCategoria(idCategoria)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListarEjemplares(idLibro As Integer) As DataTable
        seguridadNegocio.ExigirPersonal()
        Return TablaSoap(negocioEjemplar.listarPorLibro(idLibro), "Ejemplares")
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListarEjemplaresDisponibles(idLibro As Integer, idLector As Integer) As Ejemplar()
        seguridadNegocio.ExigirPersonal()
        Return negocioEjemplar.listarDisponiblesPorLibro(idLibro, idLector).ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function AgregarEjemplar(idLibro As Integer) As Boolean
        Return negocioEjemplar.agregar(idLibro)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function CambiarEstadoEjemplar(idLibro As Integer, idEjemplar As Integer, activo As Boolean) As Boolean
        Return negocioEjemplar.cambiarEstado(idLibro, idEjemplar, activo)
    End Function
End Class
