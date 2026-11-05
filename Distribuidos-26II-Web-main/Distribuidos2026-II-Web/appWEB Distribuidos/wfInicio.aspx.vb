Imports System.Data

Public Class PanelInicio
    Inherits PaginaAutenticada

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then MostrarListadosVacios()
    End Sub

    ' Solo estructura visual: no se consulta ni modifica la base de datos.
    Private Sub MostrarListadosVacios()
        Dim devoluciones As New DataTable()
        For Each columna As String In {"Lector", "Libro", "FechaLimite", "Estado"}
            devoluciones.Columns.Add(columna, GetType(String))
        Next
        grvDevoluciones.DataSource = devoluciones
        grvDevoluciones.DataBind()

        Dim categorias As New DataTable()
        categorias.Columns.Add("Categoria", GetType(String))
        categorias.Columns.Add("Prestamos", GetType(Integer))
        grvCategorias.DataSource = categorias
        grvCategorias.DataBind()
    End Sub
End Class

