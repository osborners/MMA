Imports System.IO
Imports System.IO.Ports
Imports System.Threading

'Width = 145
'Depth = 128

Public Class Form1
    Private Delegate Sub UpdateTextboxDelegate(ByVal myText As String, textBox As TextBox)

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        If Ports.Items.Count < 1 Then Return
        If serPort.IsOpen() Then serPort.Close()
        Try
            serPort.PortName = Ports.SelectedItem.ToString
            serPort.Open()
            serPort.DiscardInBuffer()
            TextBox1.Text = "Port Opened"
        Catch ex As Exception
            MessageBox.Show("Error opening port " & serPort.PortName, "Error")
        End Try
    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        serPort.Close()
        TextBox2.Clear()
        TextBox3.Clear()
        TextBox4.Clear()
        TextBox1.Text = "Port Closed"
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        For Each port In SerialPort.GetPortNames()
            Ports.Items.Add(port)
        Next
    End Sub

    Dim i As Byte = 0
    Dim left_state As Byte = 1
    Dim right_state As Byte = 1
    Dim up_state As Byte = 1
    Dim down_state As Byte = 1
    Dim Command(4) As String
    Dim state As Byte = 1
    Dim state2 As Byte = 1

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        serPort.WriteLine("U")
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        serPort.WriteLine("D")
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        serPort.WriteLine("N")
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        serPort.WriteLine("A")
    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        serPort.WriteLine("P")
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        serPort.WriteLine("C")
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        serPort.WriteLine("X")
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        serPort.WriteLine("R")
    End Sub

    Private Sub Button2_Click_1(sender As Object, e As EventArgs) Handles Button14.Click
        serPort.WriteLine("S")
        cmdLine = -1
    End Sub

    Private Sub SendCommand_Click(sender As Object, e As EventArgs) Handles SendCommand.Click
        Dim FinalCommand As String = String.Join("", Command)
        serPort.WriteLine(FinalCommand)
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        Command(0) = ComboBox2.Items(ComboBox2.SelectedIndex)
    End Sub

    Dim commands As String()
    Dim cmdLine As Integer = -1
    Dim data As String

    Private Sub runNextCmd()
        If commands Is Nothing Then Return
        If cmdLine >= 0 And cmdLine < commands.Length Then
            Thread.Sleep(200)
            Dim s = commands(cmdLine).Replace(vbCr, "").Split(" ")
            If s.Length < 1 Then
                cmdLine = -1
                Return
            End If
            serPort.Write(s(0))
            For i As Integer = 1 To s.Length() - 1
                serPort.Write(s(i).PadLeft(4, "0"))
            Next
            serPort.Write(vbLf)
            cmdLine += 1
        Else
            cmdLine = -1
        End If
    End Sub

    Private Sub dataReceived(sender As Object, e As IO.Ports.SerialDataReceivedEventArgs) Handles serPort.DataReceived
        While serPort.BytesToRead
            data += Chr(serPort.ReadChar)
            If data(data.Length - 1) = vbLf Then
                If data(0) = "x" Then
                    UpdateTextbox(data.Substring(1), TextBox5)
                ElseIf data(0) = "y" Then
                    UpdateTextbox(data.Substring(1), TextBox6)
                ElseIf data(0) = "z" Then
                    UpdateTextbox(data.Substring(1), TextBox7)
                ElseIf data(0) = "a" Then
                    runNextCmd()
                Else
                    UpdateTextbox(data, TextBox1)
                End If
                data = ""
            End If
        End While
    End Sub

    Private Sub UpdateTextbox(ByVal myText As String, textBox As TextBox)
        If textBox.InvokeRequired Then
            Dim d As New UpdateTextboxDelegate(AddressOf UpdateTextbox)
            textBox.Invoke(d, New Object() {myText, textBox})
        Else
            textBox.Text = myText
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        serPort.WriteLine("L")
    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged
        Command(1) = TextBox2.Text.PadLeft(4, "0")
    End Sub

    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged
        Command(2) = TextBox3.Text.PadLeft(4, "0")
    End Sub

    Private Sub TextBox4_TextChanged(sender As Object, e As EventArgs) Handles TextBox4.TextChanged
        Command(3) = TextBox4.Text.PadLeft(4, "0")
    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        serPort.WriteLine("Z")
    End Sub

    Dim d = New runDlg()

    Private Sub runScript()
        If d.Controls(1).Text IsNot "" Then
            commands = d.Controls(1).Text.Split(vbLf)
            cmdLine = 0
            runNextCmd()
        End If
    End Sub

    Private Sub Button15_Click(sender As Object, e As EventArgs) Handles Button15.Click
        If d.ShowDialog = DialogResult.OK Then
            runScript()
        End If
    End Sub

    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
        runScript()
    End Sub

End Class