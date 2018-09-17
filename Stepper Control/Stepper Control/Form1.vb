Imports System.IO
Imports System.IO.Ports
Imports System.Threading
Imports System.Windows.Media.Media3D

'Width = 145
'Depth = 128

Public Class Form1
    Private Delegate Sub UpdateTextboxDelegate(ByVal myText As String, textBox As TextBox)
    Private Delegate Sub UpdateLabelDelegate(ByVal myText As String, label As ToolStripStatusLabel)

    Dim pelletMoves As List(Of PelletMove) = New List(Of PelletMove)

    Dim jogMode As Boolean = False

    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        If Ports.Items.Count < 1 Then Return
        If serPort.IsOpen() Then serPort.Close()
        Try
            serPort.PortName = Ports.SelectedItem.ToString
            serPort.Open()
            serPort.DiscardInBuffer()
            TextBox1.Text = "Port Opened" + vbNewLine
        Catch ex As Exception
            MessageBox.Show("Error opening port " & serPort.PortName, "Error")
        End Try
    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs) Handles Button11.Click
        serPort.Close()
        TextBox2.Clear()
        TextBox3.Clear()
        TextBox4.Clear()
        TextBox1.Text = "Port Closed" + vbNewLine
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
        If (Not jogMode) Then enterJog()
        serPort.WriteLine("U")
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If (Not jogMode) Then enterJog()
        serPort.WriteLine("D")
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        If (Not jogMode) Then enterJog()
        serPort.WriteLine("C")
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        If (Not jogMode) Then enterJog()
        serPort.WriteLine("X")
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If (Not jogMode) Then enterJog()
        serPort.WriteLine("R")
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If (Not jogMode) Then enterJog()
        serPort.WriteLine("L")
    End Sub

    Private Sub Button2_Click_1(sender As Object, e As EventArgs) Handles Button14.Click
        If (Not jogMode) Then enterJog()
        serPort.WriteLine("S")
        cmdLine = -1
    End Sub

    Dim commands As String()
    Dim cmdLine As Integer = -1
    Dim data As String

    Private Sub runNextPelletMove()
        If pelletMoves.Count = 0 Then Return
        commands = pelletMoves(0).getCode
        cmdLine = 0
        runNextCmd()
    End Sub

    Private Sub runNextCmd()
        If commands Is Nothing Then
            If pelletMoves.Count > 0 Then
                vis.move(pelletMoves(0).Position, pelletMoves(0).Position + pelletMoves(0).Offset)
                pelletMoves.RemoveAt(0)
                runNextPelletMove()
            End If
            Return
        End If

        If (jogMode) Then exitJog()

        If cmdLine >= 0 And cmdLine < commands.Length Then
            Thread.Sleep(200)
            Dim s = commands(cmdLine).Replace(vbCr, "").Split(" ")
            If s.Length < 1 Then
                cmdLine = -1
                Return
            End If
            serPort.Write(s(0))

            If pelletMoves.Count > 0 Then
                If (s(0) = "SV" And s(1)(0) = "1") Then
                    vis.grabPellet(pelletMoves(0).Position)
                ElseIf (s(0) = "SV" And s(1)(0) = "0") Then
                    vis.releasePellet()
                End If
            End If

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
                    UpdateStatusLabel("Bridge: " + data.Substring(1), bridgePos)
                ElseIf data(0) = "y" Then
                    UpdateStatusLabel("Track: " + data.Substring(1), trackPos)
                ElseIf data(0) = "z" Then
                    UpdateStatusLabel("Hoist: " + data.Substring(1), hoistPos)
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
            textBox.Text += myText + vbNewLine
            textBox.ScrollToCaret()
        End If
    End Sub


    Private Sub UpdateStatusLabel(ByVal myText As String, label As ToolStripStatusLabel)
        If label.Owner.InvokeRequired Then
            Dim d As New UpdateLabelDelegate(AddressOf UpdateStatusLabel)
            label.Owner.Invoke(d, New Object() {myText, label})
        Else
            label.Text = myText
        End If
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

    Private Sub exitJog()
        serPort.WriteLine("Z")
        jogMode = False
    End Sub

    Private Sub enterJog()
        serPort.WriteLine("JG")
        jogMode = True
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

    Private Sub Button17_Click(sender As Object, e As EventArgs) Handles Button17.Click
        If (rbtnAbsolute.Checked) Then
            Command(0) = "MT"
        Else
            Command(0) = "MB"
        End If
        If (jogMode) Then exitJog()
        serPort.WriteLine(String.Join("", Command))
    End Sub

    Private Sub ReleaseToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ReleaseToolStripMenuItem.Click
        If (jogMode) Then exitJog()
        serPort.WriteLine("SV0000")
    End Sub

    Private Sub EngageToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EngageToolStripMenuItem.Click
        If (jogMode) Then exitJog()
        serPort.WriteLine("SV0001")
    End Sub

    Private Sub AllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AllToolStripMenuItem.Click, ToolStripMenuItem2.Click
        If (jogMode) Then exitJog()
        serPort.WriteLine("HM")
    End Sub

    Private Sub bridgePos_TextChanged(sender As Object, e As EventArgs) Handles bridgePos.TextChanged
        Try
            vis.moveGrabberX(Convert.ToDouble(data.Substring(1)) / Convert.ToDouble(width.Text))
        Catch ex As Exception
        End Try
    End Sub

    Private Sub trackPos_TextChanged(sender As Object, e As EventArgs) Handles trackPos.TextChanged
        Try
            vis.moveGrabberY(Convert.ToDouble(data.Substring(1)) / Convert.ToDouble(depth.Text))
        Catch ex As Exception
        End Try
    End Sub

    Private Sub hoistPos_TextChanged(sender As Object, e As EventArgs) Handles hoistPos.TextChanged
        Try
            vis.moveGrabberZ(Convert.ToDouble(data.Substring(1)) / Convert.ToDouble(height.Text))
        Catch ex As Exception
        End Try
    End Sub

    Class PelletMove
        Shared offsetX As Integer = 0
        Shared offsetY As Integer = 0
        Shared offsetZ As Integer = 0
        Public Property Position As Point3D
        Public Property Offset As Vector3D

        Public Sub New(pellet As Point3D, pelletOffset As Vector3D)
            Position = pellet
            Offset = pelletOffset
        End Sub

        Public Function getCode() As String()
            Dim s(6) As String
            Dim actPos As Point3D = New Point3D(Position.X * Convert.ToInt32(Form1.width.Text) + offsetX, Position.Y * Convert.ToInt32(Form1.depth.Text) + offsetY, (1 - Position.Z) * Convert.ToInt32(Form1.height.Text) + offsetZ)
            Dim actOffset As Point3D = New Point3D(Offset.X * Convert.ToInt32(Form1.width.Text) + offsetX, Offset.Y * Convert.ToInt32(Form1.depth.Text) + offsetY, -Offset.Z * Convert.ToInt32(Form1.height.Text) + offsetZ)
            Dim maxH As Integer = Convert.ToInt32(Form1.height.Text) * 2
            s(0) = "MB 0 0 " + (-maxH).ToString() + vbLf
            s(1) = "MT " + actPos.X.ToString() + " " + actPos.Y.ToString() + " " + actPos.Z.ToString() + vbLf
            s(2) = "SV 1" + vbLf
            s(3) = "MT " + actPos.X.ToString() + " " + actPos.Y.ToString() + " 0" + vbLf
            s(4) = "MT " + (actPos.X + actOffset.X).ToString() + " " + (actPos.Y + actOffset.Y).ToString() + " 0" + vbLf
            s(5) = "MT " + (actPos.X + actOffset.X).ToString() + " " + (actPos.Y + actOffset.Y).ToString() + " " + (actPos.Z + actOffset.Z).ToString() + vbLf
            s(6) = "SV 0" + vbLf
            Return s
        End Function

    End Class

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        pelletMoves.Add(New PelletMove(New Point3D(0, 0, 0), New Vector3D(1, 1, 1)))
        runNextPelletMove()
    End Sub

End Class