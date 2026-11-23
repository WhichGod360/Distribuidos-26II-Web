Imports System.Collections.Generic
Imports System.Web.UI.WebControls

' El backend devuelve nombres agregados separados por coma. No se deben perder relaciones
' cuando un nombre contiene una coma: se exige una interpretación única del catálogo.
Public NotInheritable Class RelacionesWeb
    Private Sub New()
    End Sub
    Public Shared Sub Seleccionar(lista As CheckBoxList, texto As String)
        lista.ClearSelection()
        If String.IsNullOrWhiteSpace(texto) Then Return
        Dim cache As New Dictionary(Of Integer, List(Of List(Of ListItem)))()
        Dim opciones = Resolver(lista, texto, 0, cache)
        If opciones.Count <> 1 Then Throw New InvalidOperationException("No se pudieron identificar de forma única los autores o categorías de este libro. Revisa sus nombres antes de editar.")
        For Each item In opciones(0)
            item.Selected = True
        Next
    End Sub
    Private Shared Function Resolver(lista As CheckBoxList, texto As String, inicio As Integer, cache As Dictionary(Of Integer, List(Of List(Of ListItem)))) As List(Of List(Of ListItem))
        If cache.ContainsKey(inicio) Then Return cache(inicio)
        Dim soluciones As New List(Of List(Of ListItem))()
        For Each item As ListItem In lista.Items
            If item.Text.Length = 0 OrElse inicio + item.Text.Length > texto.Length Then Continue For
            If String.Compare(texto, inicio, item.Text, 0, item.Text.Length, StringComparison.OrdinalIgnoreCase) <> 0 Then Continue For
            Dim fin = inicio + item.Text.Length
            If fin = texto.Length Then
                soluciones.Add(New List(Of ListItem) From {item})
            ElseIf fin + 2 <= texto.Length AndAlso texto.Substring(fin, 2) = ", " Then
                For Each resto In Resolver(lista, texto, fin + 2, cache)
                    Dim ruta As New List(Of ListItem) From {item}
                    ruta.AddRange(resto)
                    soluciones.Add(ruta)
                    If soluciones.Count > 1 Then Exit For
                Next
            End If
            If soluciones.Count > 1 Then Exit For
        Next
        cache(inicio) = soluciones
        Return soluciones
    End Function
End Class
