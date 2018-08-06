Imports System.IO
Imports System.Windows.Forms

Public Class runDlg

    Private Sub saveToFile()
        If openScriptDlg.FileName IsNot "" Then
            Dim stream As StreamWriter = New StreamWriter(openScriptDlg.FileName)
            stream.Write(script.Text)
            stream.Close()
        End If
    End Sub

    Private Sub loadFile()
        If openScriptDlg.FileName IsNot "" Then
            Dim file As IO.Stream = openScriptDlg.OpenFile()
            Dim data(file.Length) As Byte
            file.Read(data, 0, file.Length)
            file.Close()
            script.Text = System.Text.Encoding.Default.GetString(data)
        End If
    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        saveToFile()
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        saveToFile()
        Me.Close()
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If openScriptDlg.ShowDialog() = DialogResult.OK Then
            loadFile()
        End If
    End Sub

End Class
