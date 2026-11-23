Option Strict On

Imports System.Web.Services
Imports capaDatos
Imports capaNegocio

Partial Public Class swBiblioteca
    <WebMethod(EnableSession:=True)>
    Public Function ListarCatalogoLector() As Libro()
        Return New catalogoLectorNegocio().listar().ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListarLibros() As Libro()
        seguridadNegocio.ExigirPersonal()
        Return New libroNegocio().listarTodos().ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListarLibrosDisponibles(idLector As Integer) As Libro()
        seguridadNegocio.ExigirPersonal()
        Return New libroNegocio().listarDisponibles(idLector).ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function AgregarLibro(titulo As String, editorial As String, anoPublicacion As Short, publico As String,
                                activo As Boolean, idsAutores As Integer(), idsCategorias As Integer(), cantidadEjemplares As Integer) As Boolean
        Return New libroNegocio().agregarLibroCompleto(titulo, editorial, anoPublicacion, publico,
            activo, idsAutores, idsCategorias, cantidadEjemplares)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ActualizarLibro(idLibro As Integer, titulo As String, editorial As String, anoPublicacion As Short,
                                   publico As String, activo As Boolean, idsAutores As Integer(), idsCategorias As Integer()) As Boolean
        Return New libroNegocio().actualizarLibroCompleto(idLibro, titulo, editorial, anoPublicacion,
            publico, activo, idsAutores, idsCategorias)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function DesactivarLibro(idLibro As Integer) As Boolean
        Return New libroNegocio().eliminarLibro(idLibro)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListarAutores() As Autor()
        seguridadNegocio.ExigirPersonal()
        Return New autorNegocio().listarTodos().ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ObtenerAutor(idAutor As Integer) As Autor
        seguridadNegocio.ExigirPersonal()
        Return New autorNegocio().buscarPorId(idAutor)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function AgregarAutor(nombre As String, nacionalidad As String, fechaNacimiento As Date?,
                                fechaFallecimiento As Date?, biografia As String) As Boolean
        Return New autorNegocio().agregar(nombre, nacionalidad, fechaNacimiento, fechaFallecimiento, biografia)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ActualizarAutor(idAutor As Integer, nombre As String, nacionalidad As String, fechaNacimiento As Date?,
                                   fechaFallecimiento As Date?, biografia As String) As Boolean
        Return New autorNegocio().actualizar(idAutor, nombre, nacionalidad, fechaNacimiento, fechaFallecimiento, biografia)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function EliminarAutor(idAutor As Integer) As Boolean
        Return New autorNegocio().eliminar(idAutor)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListarCategorias(estado As String) As DataTable
        seguridadNegocio.ExigirPersonal()
        Return TablaSoap(New categoriaNegocio().listarCategoria(If(estado, "Todos")), "Categorias")
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function BuscarCategorias(texto As String, estado As String) As DataTable
        seguridadNegocio.ExigirPersonal()
        Return TablaSoap(New categoriaNegocio().buscarCategoria(If(texto, ""), If(estado, "Todos")), "Categorias")
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ObtenerCategoria(idCategoria As Integer) As DataTable
        seguridadNegocio.ExigirPersonal()
        Return TablaSoap(New categoriaNegocio().obtenerCategoria(idCategoria), "Categoria")
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function AgregarCategoria(nombre As String, activa As Boolean) As Boolean
        Return New categoriaNegocio().agregarCategoria(nombre, If(activa, 1, 0))
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ActualizarCategoria(idCategoria As Integer, nombre As String, activa As Boolean) As Boolean
        Return New categoriaNegocio().actualizarCategoria(idCategoria, nombre, If(activa, 1, 0))
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function DesactivarCategoria(idCategoria As Integer) As Boolean
        Return New categoriaNegocio().desactivarCategoria(idCategoria)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListarEjemplares(idLibro As Integer) As DataTable
        seguridadNegocio.ExigirPersonal()
        Return TablaSoap(New ejemplarNegocio().listarPorLibro(idLibro), "Ejemplares")
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function ListarEjemplaresDisponibles(idLibro As Integer, idLector As Integer) As Ejemplar()
        seguridadNegocio.ExigirPersonal()
        Return New ejemplarNegocio().listarDisponiblesPorLibro(idLibro, idLector).ToArray()
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function AgregarEjemplar(idLibro As Integer) As Boolean
        Return New ejemplarNegocio().agregar(idLibro)
    End Function

    <WebMethod(EnableSession:=True)>
    Public Function CambiarEstadoEjemplar(idLibro As Integer, idEjemplar As Integer, activo As Boolean) As Boolean
        Return New ejemplarNegocio().cambiarEstado(idLibro, idEjemplar, activo)
    End Function
End Class
