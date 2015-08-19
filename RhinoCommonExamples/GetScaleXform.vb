Imports System
Imports Rhino.Geometry

Namespace examples_cs
  Friend Class GetScaleXform
	  Inherits Rhino.Input.Custom.GetTransform

	Public Property Plane() As Plane
	Public Property RefPoint() As Point3d
	Public Property Scale() As Double

	Public Sub New()
	  Plane = Plane.WorldXY
	  RefPoint = Point3d.Origin
	  Scale = 1
	End Sub

	Protected Overrides Sub OnDynamicDraw(ByVal e As Rhino.Input.Custom.GetPointDrawEventArgs)
	  Dim basePoint As Point3d = Nothing
	  If TryGetBasePoint(basePoint) Then
		e.Display.DrawLine(basePoint, RefPoint, System.Drawing.Color.Black)
		e.Display.DrawPoint(RefPoint, System.Drawing.Color.Black)
		MyBase.OnDynamicDraw(e)
	  End If
	End Sub

	Public Function CalculateTransform(ByVal viewport As Rhino.Display.RhinoViewport, ByVal d As Double) As Transform
	  Dim basePoint As Point3d = Nothing
	  If Not TryGetBasePoint(basePoint) Then
		Return Transform.Identity
	  End If
'INSTANT VB NOTE: The variable plane was renamed since Visual Basic does not handle local variables named the same as class members well:
	  Dim plane_Renamed As Plane = viewport.ConstructionPlane()
	  plane_Renamed.Origin = basePoint
	  Dim v As Vector3d = RefPoint - basePoint

	  Dim len1 As Double = v.Length
	  If Math.Abs(len1) < 0.000001 Then
		Return Transform.Identity
	  End If

	  v.Unitize()
	  v = v * d
	  Dim len2 As Double = v.Length
	  If Math.Abs(len2) < 0.000001 Then
		Return Transform.Identity
	  End If

	  Scale = len2 / len1
	  Return Transform.Scale(plane_Renamed, Scale, Scale, Scale)
	End Function
	Public Overrides Function CalculateTransform(ByVal viewport As Rhino.Display.RhinoViewport, ByVal point As Point3d) As Transform
	  Dim basePoint As Point3d = Nothing
	  If Not TryGetBasePoint(basePoint) Then
		Return Transform.Identity
	  End If
	  Dim len2 As Double = (point - basePoint).Length
	  Dim len1 As Double = (RefPoint - basePoint).Length
	  If Math.Abs(len1) < 0.000001 OrElse Math.Abs(len2) < 0.000001 Then
		Return Transform.Identity
	  End If

	  Scale = len2 / len1

'INSTANT VB NOTE: The variable plane was renamed since Visual Basic does not handle local variables named the same as class members well:
	  Dim plane_Renamed As Plane = viewport.ConstructionPlane()
	  plane_Renamed.Origin = basePoint

	  Return Transform.Scale(plane_Renamed, Scale, Scale, Scale)
	End Function
  End Class
End Namespace