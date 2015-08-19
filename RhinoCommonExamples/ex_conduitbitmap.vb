Option Infer On

Imports System.Drawing
Imports Rhino
Imports Rhino.Commands
Imports Rhino.Display

Public Class DrawBitmapConduit
	Inherits Rhino.Display.DisplayConduit

  Private ReadOnly m_display_bitmap As DisplayBitmap

  Public Sub New()
	Dim flag = New System.Drawing.Bitmap(100, 100)
	For x As Integer = 0 To flag.Height - 1
		For y As Integer = 0 To flag.Width - 1
			flag.SetPixel(x, y, Color.White)
		Next y
	Next x

	Dim g = Graphics.FromImage(flag)
	g.FillEllipse(Brushes.Blue, 25, 25, 50, 50)
	m_display_bitmap = New DisplayBitmap(flag)
  End Sub

  Protected Overrides Sub DrawForeground(ByVal e As Rhino.Display.DrawEventArgs)
	e.Display.DrawBitmap(m_display_bitmap, 50, 50, Color.White)
  End Sub
End Class

Partial Friend Class Examples
  Private Shared ReadOnly m_conduit As New DrawBitmapConduit()

  Public Shared Function ConduitBitmap(ByVal doc As RhinoDoc) As Result
	' toggle conduit on/off
	m_conduit.Enabled = Not m_conduit.Enabled

	RhinoApp.WriteLine("Custom conduit enabled = {0}", m_conduit.Enabled)
	doc.Views.Redraw()
	Return Result.Success
  End Function
End Class