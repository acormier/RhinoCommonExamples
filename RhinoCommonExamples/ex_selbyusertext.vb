Imports Rhino

Partial Friend Class Examples
  Private Shared m_key As String = String.Empty
  Public Shared Function SelByUserText(ByVal doc As RhinoDoc) As Rhino.Commands.Result
	' You don't have to override RunCommand if you don't need any user input. In
	' this case we want to get a key from the user. If you return something other
	' than Success, the selection is canceled
	Return Rhino.Input.RhinoGet.GetString("key", True, m_key)
  End Function

  Protected Shared Overrides Function SelFilter(ByVal rhObj As Rhino.DocObjects.RhinoObject) As Boolean
	Dim s As String = rhObj.Attributes.GetUserString(m_key)
	Return Not String.IsNullOrEmpty(s)
  End Function
End Class