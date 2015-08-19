Option Infer On

Imports Rhino

Friend Class CustomConduit
	Inherits Rhino.Display.DisplayConduit

  Protected Overrides Sub DrawForeground(ByVal e As Rhino.Display.DrawEventArgs)
	Dim bounds = e.Viewport.Bounds
	Dim pt = New Rhino.Geometry.Point2d(bounds.Right - 100, bounds.Bottom - 30)
	e.Display.Draw2dText("Hello", System.Drawing.Color.Red, pt, False)
  End Sub
End Class

Partial Friend Class Examples
  Private ReadOnly Shared m_customconduit As New CustomConduit()
  Public Shared Function DrawOverlay(ByVal doc As RhinoDoc) As Rhino.Commands.Result
	' toggle conduit on/off
	m_customconduit.Enabled = Not m_conduit.Enabled

	RhinoApp.WriteLine("Custom conduit enabled = {0}", m_customconduit.Enabled)
	doc.Views.Redraw()
	Return Rhino.Commands.Result.Success
  End Function
End Class