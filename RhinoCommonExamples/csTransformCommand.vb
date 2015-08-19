Option Infer On

Imports System
Imports Rhino
Imports Rhino.Geometry

Namespace examples_cs
  Public Class csTransformCommand
	  Inherits Rhino.Commands.TransformCommand

	Public Overrides ReadOnly Property EnglishName() As String
		Get
			Return "examples_csTransform"
		End Get
	End Property

	Private m_scale As Double = 1
	Private m_copy As Boolean
	Protected Overrides Function RunCommand(ByVal doc As RhinoDoc, ByVal mode As Rhino.Commands.RunMode) As Rhino.Commands.Result
	  ' Select objects to scale
	  Dim list = New Rhino.Collections.TransformObjectList()
	  Dim rc = SelectObjects("Select objects to scale", list)
	  If rc IsNot Rhino.Commands.Result.Success Then
		Return rc
	  End If

	  Dim anchor As Point3d
	  Dim _ref As Point3d
	  Dim plane As Plane

	  ' Origin point
	  Dim gp = New Rhino.Input.Custom.GetPoint()
	  gp.SetCommandPrompt("Origin point")
	  Dim copy = New Rhino.Input.Custom.OptionToggle(m_copy,"No", "Yes")
	  gp.AddOptionToggle("Copy", copy)
	  Do
		Dim res = gp.Get()
		If res Is Rhino.Input.GetResult.Point Then
		  Dim view = gp.View()
		  If view Is Nothing Then
			Return Rhino.Commands.Result.Failure
		  End If
		  plane = view.ActiveViewport.ConstructionPlane()
		  anchor = gp.Point()
		  Exit Do
		End If
		If res Is Rhino.Input.GetResult.Option Then
		  Continue Do
		End If

		Return Rhino.Commands.Result.Cancel
	  Loop

	  Dim bAcceptDefault As Boolean = True

	  ' Scale factor or first reference point
	  gp.SetCommandPrompt("Scale factor or first reference point")
	  gp.SetBasePoint(anchor, True)
	  gp.DrawLineFromPoint(anchor, True)
	  gp.ConstrainToConstructionPlane(True)
	  Do
		If bAcceptDefault Then
		  gp.AcceptNumber(True, False)
		  gp.SetDefaultNumber(m_scale)
		Else
		  gp.AcceptNothing(True)
		  gp.ClearDefault()
		End If

		Dim res = gp.Get()
		If res Is Rhino.Input.GetResult.Point Then
		  _ref = gp.Point()
		  Exit Do
		End If
		If res Is Rhino.Input.GetResult.Number Then
		  Dim scale As Double = Math.Abs(gp.Number())
		  Const EPS_DIVIDE As Double = 0.000001
		  If scale < EPS_DIVIDE Then
			Continue Do
		  End If
		  m_scale = scale
		  plane.Origin = anchor

		  Dim xform = Transform.Scale(plane, m_scale, m_scale, m_scale)
		  TransformObjects(list, xform, copy.CurrentValue, copy.CurrentValue)
		  m_copy = copy.CurrentValue
		  If m_copy Then
			bAcceptDefault = False
			Continue Do
		  End If
		  doc.Views.Redraw()
		  Return Rhino.Commands.Result.Success
		End If

		If res Is Rhino.Input.GetResult.Nothing Then
		  If bAcceptDefault = False Then
			Return Rhino.Commands.Result.Success
		  End If

		  plane.Origin = anchor
		  Dim xform = Transform.Scale(plane, m_scale, m_scale, m_scale)
		  TransformObjects(list, xform, copy.CurrentValue, copy.CurrentValue)

		  m_copy = copy.CurrentValue
		  If m_copy Then
			bAcceptDefault = False
			Continue Do
		  End If
		  doc.Views.Redraw()
		  Return Rhino.Commands.Result.Success
		End If
		If res Is Rhino.Input.GetResult.Option Then
		  Continue Do
		End If
		Return Rhino.Commands.Result.Cancel
	  Loop

	  ' Second reference point
	  Dim gx = New GetScaleXform()
	  gx.SetCommandPrompt("Second reference point")
	  gx.AddOptionToggle("copy", copy)
	  gx.AddTransformObjects(list)
	  gx.SetBasePoint(anchor, True)
	  gx.DrawLineFromPoint(anchor, True)
	  gx.ConstrainToConstructionPlane(True)
	  gx.Plane = plane
	  gx.RefPoint = _ref
	  gx.AcceptNothing(True)
	  gx.AcceptNumber(True, False)

	  rc = Rhino.Commands.Result.Cancel
	  Do
		Dim res = gx.GetXform()
		If res Is Rhino.Input.GetResult.Point Then
		  Dim view = gx.View()
		  If view Is Nothing Then
			rc = Rhino.Commands.Result.Failure
			Exit Do
		  End If
		  Dim xform = gx.CalculateTransform(view.ActiveViewport, gx.Point())
		  If xform.IsValid AndAlso xform IsNot Transform.Identity Then
			TransformObjects(list, xform, copy.CurrentValue, copy.CurrentValue)
			rc = Rhino.Commands.Result.Success
			m_scale = gx.Scale
		  End If
		  m_copy = copy.CurrentValue
		  If m_copy Then
			Continue Do
		  End If

		  Exit Do
		End If

		If res Is Rhino.Input.GetResult.Number Then
		  Dim view = gx.View()
		  If view Is Nothing Then
			rc = Rhino.Commands.Result.Failure
			Exit Do
		  End If

		  Dim xform = gx.CalculateTransform(view.ActiveViewport, gx.Number())
		  If xform.IsValid AndAlso xform IsNot Transform.Identity Then
			TransformObjects(list, xform, copy.CurrentValue, copy.CurrentValue)
			rc = Rhino.Commands.Result.Success
			m_scale = gx.Scale
		  End If
		  m_copy = copy.CurrentValue
		  If m_copy Then
			Continue Do
		  End If
		  Exit Do
		End If

		If res Is Rhino.Input.GetResult.Option Then
		  Continue Do
		End If

		Exit Do
	  Loop

	  doc.Views.Redraw()
	  Return rc
	End Function
  End Class
End Namespace