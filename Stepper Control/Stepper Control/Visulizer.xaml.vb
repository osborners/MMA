Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Threading.Tasks
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Media.Media3D
Imports System.Windows.Navigation
Imports System.Windows.Shapes

Public Class Visulizer

    Class Pellet
        Private g As Model3DGroup
        Shared pelletGeometry As MeshGeometry3D = New MeshGeometry3D With {
            .TriangleIndices = New Int32Collection(New List(Of Integer) From {2, 3, 1, 2, 1, 0, 7, 1, 3, 7, 5, 1, 6, 5, 7, 6, 4, 5, 4, 2, 0, 2, 4, 6, 2, 7, 3, 2, 6, 7, 0, 1, 5, 0, 5, 4}),
            .Positions = New Point3DCollection(New List(Of Point3D) From {
                New Point3D(0, 0, 0),
                New Point3D(0.9, 0, 0),
                New Point3D(0, 0.9, 0),
                New Point3D(0.9, 0.9, 0),
                New Point3D(0, 0, 0.9),
                New Point3D(0.9, 0, 0.9),
                New Point3D(0, 0.9, 0.9),
                New Point3D(0.9, 0.9, 0.9)
            })
        }

        Public Sub New(ByVal pos As Point3D, ByVal offset As Vector3D, ByVal b As Brush, ByVal group As Model3DGroup)
            Brush = b
            Position = pos
            model = New GeometryModel3D(pelletGeometry, New DiffuseMaterial(b))
            model.Transform = New TranslateTransform3D(New Vector3D(pos.X, pos.Y, pos.Z) + offset)
            group.Children.Add(model)
            g = group
        End Sub

        Public Property Brush As Brush
        Public Property Position As Point3D
        Public Property contentsKey As String
        Public ReadOnly Property model As GeometryModel3D

        Public Sub Remove()
            g.Children.Remove(model)
        End Sub

        Protected Overrides Sub Finalize()
        End Sub
    End Class

    Shared brushes As Brush() = {New SolidColorBrush(Color.FromArgb(192, 255, 0, 0)), New SolidColorBrush(Color.FromArgb(192, 0, 255, 0)), New SolidColorBrush(Color.FromArgb(192, 0, 0, 255))}
    Shared WIDTH As Integer = 4
    Shared DEPTH As Integer = 4
    Shared HEIGHT As Integer = 2
        Private Pellets As Dictionary(Of Point3D, Pellet) = New Dictionary(Of Point3D, Pellet)()

    Public Sub Init()
        Pellets.Clear()
        Dim grabber = pelletModels.Children.Item(0)
        pelletModels.Children.Clear()
        pelletModels.Children.Add(grabber)
        grabberTransform.OffsetX = WIDTH / 2 - 1.05
        grabberTransform.OffsetY = -DEPTH / 2 - 0.05
        grabberTransform.OffsetZ = HEIGHT - 1
        Dim offset As Vector3D = New Vector3D(-WIDTH / 2, -DEPTH / 2, 0)

        For x As Integer = 0 To WIDTH - 1

            For y As Integer = 0 To DEPTH - 1

                For z As Integer = 0 To HEIGHT - 1
                    Dim p As Pellet = New Pellet(New Point3D(x, y, z), offset, brushes(z Mod 3), pelletModels)
                    Pellets.Add(New Point3D(x, y, z), p)
                Next
            Next
        Next
    End Sub

    Public Sub moveGrabberX(x As Double)
        grabberTransform.OffsetX = WIDTH / 2 - 1.05 - x
    End Sub

    Public Sub moveGrabberY(y As Double)
        grabberTransform.OffsetY = -DEPTH / 2 - 0.05 + y
    End Sub

    Public Sub moveGrabberZ(z As Double)
        grabberTransform.OffsetZ = HEIGHT - 1 - z
    End Sub

    Private Function lmt(ByVal val As Double, ByVal min As Double, ByVal max As Double) As Double
            If val < min Then Return min
            If val > max Then Return max
            Return val
        End Function

    Private Sub Viewport3D_MouseWheel(ByVal sender As Object, ByVal e As MouseWheelEventArgs) Handles Me.MouseWheel
        Dim v As Double = e.Delta / 50
        camMain.Position = New Point3D(0, -lmt(-2 * v - camMain.Position.Y, 10, Double.MaxValue), lmt(-v + camMain.Position.Z, 5, Double.MaxValue))
    End Sub

    Private down As Point
        Private last As Double

    Private Sub Grid_MouseDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs) Handles Me.MouseRightButtonDown
        down = e.GetPosition(vp)
        last = rot.Angle
    End Sub

    Private Sub Viewport3D_MouseMove(ByVal sender As Object, ByVal e As MouseEventArgs) Handles Me.MouseMove
        If e.RightButton = MouseButtonState.Pressed Then
            rot.Angle = last + ((e.GetPosition(vp).X - down.X) / vp.ActualWidth * 90)
        End If
    End Sub

    Private hitModel As GeometryModel3D

    Public Sub New()
        InitializeComponent()
        Init()
    End Sub

    Private Function HitTestResult(ByVal result As HitTestResult) As HitTestResultBehavior
        Dim rayHTResult As RayMeshGeometry3DHitTestResult = TryCast(result, RayMeshGeometry3DHitTestResult)

        If rayHTResult IsNot Nothing Then
            hitModel = TryCast(rayHTResult.ModelHit, GeometryModel3D)
            Return HitTestResultBehavior.Stop
        End If

        Return HitTestResultBehavior.Continue
    End Function

    Private Sub vp_MouseLeftButtonUp(ByVal sender As Object, ByVal e As MouseButtonEventArgs) Handles Me.MouseLeftButtonUp
        If hitModel Is Nothing Then Return

        For Each item As KeyValuePair(Of Point3D, Pellet) In Pellets

            If (item.Value.model Is hitModel) Then
                item.Value.Remove()
            End If
        Next
    End Sub

    Private Sub vp_MouseLeftButtonDown(ByVal sender As Object, ByVal e As MouseButtonEventArgs) Handles Me.MouseLeftButtonDown
        hitModel = Nothing
        VisualTreeHelper.HitTest(vp, Nothing, AddressOf HitTestResult, New PointHitTestParameters(e.GetPosition(vp)))

        If hitModel IsNot Nothing Then
            hitModel.Material = New DiffuseMaterial(New SolidColorBrush(Colors.White))
        End If
    End Sub


End Class
