Imports System.Globalization
Imports System.Web.UI.WebControls

Public NotInheritable Class FormulariosWeb
    Private Sub New()
    End Sub

    Public Shared Sub Lista(control As ListControl, datos As Object, texto As String, valor As String, Optional opcion As String = "Selecciona una opción")
        control.DataTextField = texto
        control.DataValueField = valor
        control.DataSource = datos
        control.DataBind()
        If opcion IsNot Nothing Then control.Items.Insert(0, New ListItem(opcion, "0"))
    End Sub

    Public Shared Function Id(control As ListControl) As Integer
        Dim resultado As Integer
        If Not Integer.TryParse(control.SelectedValue, resultado) OrElse resultado <= 0 Then Throw New ArgumentException("Selecciona una opción válida.")
        Return resultado
    End Function

    Public Shared Function Coincide(contenido As String, texto As String) As Boolean
        Return If(contenido, "").IndexOf(If(texto, "").Trim(), StringComparison.OrdinalIgnoreCase) >= 0
    End Function

    Public Shared Function Dinero(valor As Decimal) As String
        Return "S/ " & valor.ToString("N2", CultureInfo.GetCultureInfo("es-PE"))
    End Function
End Class
