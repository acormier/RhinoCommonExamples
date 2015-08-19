using System;

class __rndP {
  public Rhino.Geometry.Point3d P { get;}

  public __rndP(Rhino.Geometry.Point3d p) {
      P = p;
    }
}

partial class Examples
{
  public static Rhino.Commands.Result __rnd(Rhino.RhinoDoc doc)
  {
    var p = new __rndP (Rhino.Geometry.Point3d.Origin);
    //p.P.X = 3.0;
    var p2 = p.P;
    p2.X = 3.0;
    Rhino.RhinoApp.WriteLine (p.P.ToString());
    Rhino.RhinoApp.WriteLine (p2.ToString());
    return Rhino.Commands.Result.Success;
  }
}
