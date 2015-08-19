Option Infer On

Imports Rhino
Imports Rhino.Geometry

Partial Friend Class Examples
  Public Shared Function Gumball(ByVal doc As RhinoDoc) As Rhino.Commands.Result
	' Select objects to scale
	Dim list = New Rhino.Collections.TransformObjectList()
	Dim rc = SelectObjects("Select objects to gumball", list)
	If rc IsNot Rhino.Commands.Result.Success Then
	  Return rc
	End If

	Dim bbox = list.GetBoundingBox(True, True)
	If Not bbox.IsValid Then
	  Return Rhino.Commands.Result.Failure
	End If

	Dim cmdrc As Rhino.Commands.Result

	Dim base_gumball = New Rhino.UI.Gumball.GumballObject()
	base_gumball.SetFromBoundingBox(bbox)
	Dim dc = New Rhino.UI.Gumball.GumballDisplayConduit()
	Dim appearance = New Rhino.UI.Gumball.GumballAppearanceSettings()

	' turn off some of the scale appearance settings to have a slightly different gumball
	appearance.ScaleXEnabled = False
	appearance.ScaleYEnabled = False
	appearance.ScaleZEnabled = False

	Dim bCopy As Boolean = False
	Do
	  dc.SetBaseGumball(base_gumball, appearance)
	  dc.Enabled = True
	  doc.Views.Redraw()

	  Dim gp As New GetGumballXform(dc)
	  Dim copy_optindx As Integer = gp.AddOption("Copy")
	  If dc.PreTransform = Transform.Identity Then
		gp.SetCommandPrompt("Drag gumball")
	  Else
		gp.AcceptNothing(True)
		gp.SetCommandPrompt("Drag gumball. Press Enter when done")
	  End If
	  gp.AddTransformObjects(list)
	  gp.MoveGumball()
	  dc.Enabled = False
	  cmdrc = gp.CommandResult()
	  If cmdrc IsNot Rhino.Commands.Result.Success Then
		Exit Do
	  End If

	  Dim getpoint_result = gp.Result()
	  If getpoint_result Is Rhino.Input.GetResult.Point Then
		If Not dc.InRelocate Then
		  Dim xform As Transform = dc.TotalTransform
		  dc.PreTransform = xform
		End If
		' update location of base gumball
		Dim gbframe = dc.Gumball.Frame
		Dim baseFrame = base_gumball.Frame
		baseFrame.Plane = gbframe.Plane
		baseFrame.ScaleGripDistance = gbframe.ScaleGripDistance
		base_gumball.Frame = baseFrame
		Continue Do
	  End If
	  If getpoint_result Is Rhino.Input.GetResult.Option Then
		If gp.OptionIndex() = copy_optindx Then
		  bCopy = True
		End If
		Continue Do
	  End If

	  Exit Do
	Loop

	dc.Enabled = False
	If dc.PreTransform <> Transform.Identity Then
	  Dim xform As Transform = dc.PreTransform
	  TransformObjects(list, xform, bCopy, bCopy)
	End If
	doc.Views.Redraw()
	Return cmdrc
  End Function
End Class


Friend Class GetGumballXform
	Inherits Rhino.Input.Custom.GetTransform

  Private ReadOnly m_dc As Rhino.UI.Gumball.GumballDisplayConduit
  Public Sub New(ByVal dc As Rhino.UI.Gumball.GumballDisplayConduit)
	m_dc = dc
  End Sub

  Public Overrides Function CalculateTransform(ByVal viewport As Rhino.Display.RhinoViewport, ByVal point As Point3d) As Transform
	If m_dc.InRelocate Then
	  ' don't move objects while relocating gumball
	  Return m_dc.PreTransform
	End If

	Return m_dc.TotalTransform
  End Function

  Protected Overrides Sub OnMouseDown(ByVal e As Rhino.Input.Custom.GetPointMouseEventArgs)
	If m_dc.PickResult.Mode <> Rhino.UI.Gumball.GumballMode.None Then
	  Return
	End If
	m_dc.PickResult.SetToDefault()

	Dim pick_context As New Rhino.Input.Custom.PickContext()
	pick_context.View = e.Viewport.ParentView
	pick_context.PickStyle = Rhino.Input.Custom.PickStyle.PointPick
	Dim xform = e.Viewport.GetPickTransform(e.WindowPoint)
	pick_context.SetPickTransform(xform)
	Dim pick_line As Rhino.Geometry.Line = Nothing
	e.Viewport.GetFrustumLine(e.WindowPoint.X, e.WindowPoint.Y, pick_line)
	pick_context.PickLine = pick_line
	pick_context.UpdateClippingPlanes()
	' pick gumball and, if hit, set getpoint dragging constraints.
	m_dc.PickGumball(pick_context, Me)
  End Sub

  Protected Overrides Sub OnMouseMove(ByVal e As Rhino.Input.Custom.GetPointMouseEventArgs)
	If m_dc.PickResult.Mode = Rhino.UI.Gumball.GumballMode.None Then
	  Return
	End If

	m_dc.CheckShiftAndControlKeys()
	Dim world_line As Rhino.Geometry.Line = Nothing
	If Not e.Viewport.GetFrustumLine(e.WindowPoint.X, e.WindowPoint.Y, world_line) Then
	  world_line = Rhino.Geometry.Line.Unset
	End If

	Dim rc As Boolean = m_dc.UpdateGumball(e.Point, world_line)
	If rc Then
	  MyBase.OnMouseMove(e)
	End If
  End Sub

  Protected Overrides Sub OnDynamicDraw(ByVal e As Rhino.Input.Custom.GetPointDrawEventArgs)
	' Disable default GetTransform drawing by not calling the base class
	' implementation. All aspects of gumball display are handled by 
	' GumballDisplayConduit
	'base.OnDynamicDraw(e);
  End Sub

  ' lets user drag m_gumball around.
  Public Function MoveGumball() As Rhino.Input.GetResult
	' Get point on a MouseUp event
	If m_dc.PreTransform <> Transform.Identity Then
	  HaveTransform = True
	  Transform = m_dc.PreTransform
	End If
	SetBasePoint(m_dc.BaseGumball.Frame.Plane.Origin, False)

	' V5 uses a display conduit to provide display feedback
	' so shaded objects move
	ObjectList.DisplayFeedbackEnabled = True
	If Transform <> Transform.Identity Then
	  ObjectList.UpdateDisplayFeedbackTransform(Transform)
	End If

	' Call Get with mouseUp set to true
	Dim rc = Me.Get(True)

	' V5 uses a display conduit to provide display feedback
	' so shaded objects move
	ObjectList.DisplayFeedbackEnabled = False
	Return rc
  End Function
End Class