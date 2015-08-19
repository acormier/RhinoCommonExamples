Option Infer On

Imports Microsoft.VisualBasic
Imports System
Imports Rhino
Imports System.Runtime.InteropServices

' You must define a Guid attribute for your user data derived class
' in order to support serialization. Every custom user data class
' needs a custom Guid
<Guid("7098A105-CD3C-4192-A9AC-0F21017098DC")>
Public Class MyCustomData
	Inherits Rhino.DocObjects.Custom.UserData

  Public Property IntegerData() As Integer
  Public Property StringData() As String

  ' Your UserData class must have a public parameterless constructor
  Public Sub New()
  End Sub

  Public Sub New(ByVal i As Integer, ByVal s As String)
	IntegerData = i
	StringData = s
  End Sub

  Public Overrides ReadOnly Property Description() As String
	Get
		Return "Some Custom Properties"
	End Get
  End Property

  Public Overrides Function ToString() As String
	Return String.Format("integer={0}, string={1}", IntegerData, StringData)
  End Function

  Protected Overrides Sub OnDuplicate(ByVal source As Rhino.DocObjects.Custom.UserData)
	Dim src As MyCustomData = TryCast(source, MyCustomData)
	If src IsNot Nothing Then
	  IntegerData = src.IntegerData
	  StringData = src.StringData
	End If
  End Sub

  ' return true if you have information to save
  Public Overrides ReadOnly Property ShouldWrite() As Boolean
	Get
	  ' make up some rule as to if this should be saved in the 3dm file
	  If IntegerData > 0 AndAlso Not String.IsNullOrEmpty(StringData) Then
		Return True
	  End If
	  Return False
	End Get
  End Property

  Protected Overrides Function Read(ByVal archive As Rhino.FileIO.BinaryArchiveReader) As Boolean
	Dim dict As Rhino.Collections.ArchivableDictionary = archive.ReadDictionary()
	If dict.ContainsKey("IntegerData") AndAlso dict.ContainsKey("StringData") Then
	  IntegerData = CInt(Fix(dict("IntegerData")))
	  StringData = TryCast(dict("StringData"), String)
	End If
	Return True
  End Function
  Protected Overrides Function Write(ByVal archive As Rhino.FileIO.BinaryArchiveWriter) As Boolean
	' you can implement File IO however you want... but the dictionary class makes
	' issues like versioning in the 3dm file a bit easier.  If you didn't want to use
	' the dictionary for writing, your code would look something like.
	'
	'  archive.Write3dmChunkVersion(1, 0);
	'  archive.WriteInt(IntegerData);
	'  archive.WriteString(StringData);
	Dim dict = New Rhino.Collections.ArchivableDictionary(1, "MyCustomData")
	dict.Set("IntegerData", IntegerData)
	dict.Set("StringData", StringData)
	archive.WriteDictionary(dict)
	Return True
  End Function
End Class


Partial Friend Class Examples
  Public Shared Function Userdata(ByVal doc As RhinoDoc) As Rhino.Commands.Result
	Dim objref As Rhino.DocObjects.ObjRef = Nothing
	Dim rc = Rhino.Input.RhinoGet.GetOneObject("Select Face", False, Rhino.DocObjects.ObjectType.Surface, objref)
	If rc IsNot Rhino.Commands.Result.Success Then
	  Return rc
	End If

	Dim face = objref.Face()

	' See if user data of my custom type is attached to the geomtry
	' We need to use the underlying surface in order to get the user data
	' to serialize with the file.
	Dim ud = TryCast(face.UnderlyingSurface().UserData.Find(GetType(MyCustomData)), MyCustomData)
	If ud Is Nothing Then
	  ' No user data found; create one and add it
	  Dim i As Integer = 0
	  rc = Rhino.Input.RhinoGet.GetInteger("Integer Value", False, i)
	  If rc IsNot Rhino.Commands.Result.Success Then
		Return rc
	  End If

	  ud = New MyCustomData(i, "This is some text")
	  face.UnderlyingSurface().UserData.Add(ud)
	Else
	  RhinoApp.WriteLine("{0} = {1}", ud.Description, ud)
	End If
	Return Rhino.Commands.Result.Success
  End Function
End Class