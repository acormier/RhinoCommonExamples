Imports System
Imports Rhino
Imports Rhino.Commands
Imports Rhino.Geometry
Imports Rhino.Input
Imports Rhino.Input.Custom
Imports Rhino.UI
Imports Eto.Drawing
Imports Eto.Forms

Namespace RhinoCommonExamples
  Public Class RhinoCommonExamplesCommand
	  Inherits Rhino.Commands.Command

	Public Overrides ReadOnly Property EnglishName() As String
	  Get
		  Return "RhinoCommonExamplesCommand"
	  End Get
	End Property

	Protected Overrides Function RunCommand(ByVal doc As Rhino.RhinoDoc, ByVal mode As RunMode) As Result
	  ' TODO: start here modifying the behaviour of your command.
	  ' ---
	  RhinoApp.WriteLine("The {0} command will add a line right now.", EnglishName)

	  Dim pt0 As Point3d
	  Using getPointAction As New GetPoint()
		getPointAction.SetCommandPrompt("Please select the start point")
		If getPointAction.Get() <> GetResult.Point Then
		  RhinoApp.WriteLine("No start point was selected.")
		  Return getPointAction.CommandResult()
		End If
		pt0 = getPointAction.Point()
	  End Using

	  Dim pt1 As Point3d
	  Using getPointAction As New GetPoint()
		getPointAction.SetCommandPrompt("Please select the end point")
		getPointAction.SetBasePoint(pt0, True)
		getPointAction.DrawLineFromPoint(pt0, True)
		If getPointAction.Get() <> GetResult.Point Then
		  RhinoApp.WriteLine("No end point was selected.")
		  Return getPointAction.CommandResult()
		End If
		pt1 = getPointAction.Point()
	  End Using

	  doc.Objects.AddLine(pt0, pt1)
	  doc.Views.Redraw()
	  RhinoApp.WriteLine("The {0} command added one line to the document.", EnglishName)


	  Return Result.Success
	End Function
  End Class
End Namespace