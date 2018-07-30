Imports System.IO.Ports
Imports System.Threading

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

        If up_state = 1 Then
            serPort.WriteLine("U")
            up_state = 0
        Else
            serPort.WriteLine("S")
            up_state = 1
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click

        If up_state = 1 Then
            serPort.WriteLine("D")
            up_state = 0
        Else
            serPort.WriteLine("S")
            up_state = 1
        End If
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
        If state = 1 Then
            serPort.WriteLine("C")
            state = 0
        Else
            serPort.WriteLine("S")
            state = 1
        End If
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        If state2 = 1 Then
            serPort.WriteLine("X")
            state2 = 0
        Else
            serPort.WriteLine("S")
            state2 = 1
        End If

    End Sub

    Private Sub Button2_Click_1(sender As Object, e As EventArgs) Handles Button2.Click, Button14.Click
        If right_state = 1 Then
            serPort.WriteLine("R")
            right_state = 0
        Else
            serPort.WriteLine("S")
            right_state = 1
        End If
    End Sub

    Private Sub SendCommand_Click(sender As Object, e As EventArgs) Handles SendCommand.Click
        Dim FinalCommand As String = String.Join("", Command)
        serPort.WriteLine(FinalCommand)
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        i = ComboBox2.SelectedIndex
        Command(1) = ComboBox2.Items(i)
        For j As Integer = 2 To 4
            Command(j) = "0000"
        Next
    End Sub

    Dim commands As String()
    Dim cmdLine As Integer = -1
    Dim data As String

    Private Sub runNextCmd()
        If cmdLine < commands.Length Then
            Thread.Sleep(200)
            serPort.WriteLine(commands(cmdLine))
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
        If left_state = 1 Then
            serPort.WriteLine("L")
            left_state = 0
        Else
            serPort.WriteLine("S")
            left_state = 1
        End If

    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged
        Command(2) = TextBox2.Text.PadLeft(4, "0")
    End Sub

    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged
        Command(3) = TextBox2.Text.PadLeft(4, "0")
    End Sub

    Private Sub TextBox4_TextChanged(sender As Object, e As EventArgs) Handles TextBox4.TextChanged
        Command(4) = TextBox2.Text.PadLeft(4, "0")
    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        serPort.WriteLine("Z")
    End Sub

    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click
        If openScriptDlg.ShowDialog() = DialogResult.OK Then
            Dim data(openScriptDlg.OpenFile().Length) As Byte
            openScriptDlg.OpenFile().Read(data, 0, openScriptDlg.OpenFile().Length)
            openScriptDlg.OpenFile().Close()
            Dim cmdString = System.Text.Encoding.Default.GetString(data)
            Dim d = New runDlg()
            d.Controls(0).Text = cmdString
            If d.ShowDialog = DialogResult.OK Then
                commands = cmdString.Split(vbLf)
                cmdLine = 0
                runNextCmd()
            End If
        End If
    End Sub
End Class