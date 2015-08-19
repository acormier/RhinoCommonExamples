Option Infer On

Imports Rhino
Imports Rhino.Commands
Imports Rhino.DocObjects
Imports System

Partial Friend Class Examples
  <System.Runtime.InteropServices.DllImport("user32.dll")>
  Public Shared Function GetCursorPos(<System.Runtime.InteropServices.Out()> ByRef point As System.Drawing.Point) As Boolean
  End Function

  <System.Runtime.InteropServices.DllImport("user32.dll")>
  Public Shared Function ScreenToClient(ByVal hWnd As IntPtr, ByRef point As System.Drawing.Point) As Boolean
  End Function

  Public Shared Function PointAtCursor(ByVal doc As RhinoDoc) As Result
	Dim result = Result.Failure
	Dim view = doc.Views.ActiveView
	If view Is Nothing Then
		Return result
	End If

	Dim windows_drawing_point As System.Drawing.Point = Nothing
	If Not GetCursorPos(windows_drawing_point) OrElse Not ScreenToClient(view.Handle, windows_drawing_point) Then
	  Return result
	End If

	Dim xform = view.ActiveViewport.GetTransform(CoordinateSystem.Screen, CoordinateSystem.World)
	Dim point = New Rhino.Geometry.Point3d(windows_drawing_point.X, windows_drawing_point.Y, 0.0)
	RhinoApp.WriteLine("screen point: ({0})", point)
	point.Transform(xform)
	RhinoApp.WriteLine("world point: ({0})", point)
	result = Result.Success
	Return result
  End Function
End Class