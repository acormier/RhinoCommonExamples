Imports System
Imports System.Runtime.InteropServices
Imports Rhino
Imports Rhino.Commands

Partial Friend Class Examples
  <DllImport("user32.dll", ExactSpelling := True, CharSet := CharSet.Auto)>
  Public Shared Function GetParent(ByVal hWnd As IntPtr) As IntPtr
  End Function

  <DllImport("user32.dll")>
  Shared Function ShowWindow(ByVal hWnd As IntPtr, ByVal nCmdShow As Integer) As Boolean
  End Function

  Public Shared Function MaximizeView(ByVal doc As RhinoDoc) As Result
	Dim parent_handle As IntPtr = GetParent(doc.Views.ActiveView.Handle)
	If parent_handle <> IntPtr.Zero Then
	  ShowWindow(parent_handle, 3)
	End If
	Return Result.Success
  End Function
End Class