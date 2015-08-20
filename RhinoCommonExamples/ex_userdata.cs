using System;
using Rhino;
using System.Runtime.InteropServices;
/// <summary>
/// title: Info: RhinoCommon object plug-in user data
/// keywords: ['info:', 'rhinocommon', 'object', 'plug-in', 'user', 'data']
/// categories: ['Adding Objects']
/// </summary>
partial class Examples
{
  public static Rhino.Commands.Result Userdata(RhinoDoc doc)
  {
    Rhino.DocObjects.ObjRef objref;
    var rc = Rhino.Input.RhinoGet.GetOneObject("Select Face", false, Rhino.DocObjects.ObjectType.Surface, out objref);
    if (rc != Rhino.Commands.Result.Success)
      return rc;

    var face = objref.Face();

    // See if user data of my custom type is attached to the geomtry
    // We need to use the underlying surface in order to get the user data
    // to serialize with the file.
    var ud = face.UnderlyingSurface().UserData.Find(typeof(MyCustomData)) as MyCustomData;
    if (ud == null)
    {
      // No user data found; create one and add it
      int i = 0;
      rc = Rhino.Input.RhinoGet.GetInteger("Integer Value", false, ref i);
      if (rc != Rhino.Commands.Result.Success)
        return rc;

      ud = new MyCustomData(i, "This is some text");
      face.UnderlyingSurface().UserData.Add(ud);
    }
    else
    {
      RhinoApp.WriteLine("{0} = {1}", ud.Description, ud);
    }
    return Rhino.Commands.Result.Success;
  }
}
