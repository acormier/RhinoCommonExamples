using Rhino.Geometry;
/// <summary>
/// title: Move CPlane
/// keywords: ['move', 'cplane']
/// categories: ['Other']
/// </summary>
partial class Examples
{
  public static Rhino.Commands.Result MoveCPlane(Rhino.RhinoDoc doc)
  {
    Rhino.Display.RhinoView view = doc.Views.ActiveView;
    if (view == null)
      return Rhino.Commands.Result.Failure;

    Rhino.DocObjects.ConstructionPlane cplane = view.ActiveViewport.GetConstructionPlane();
    Point3d origin = cplane.Plane.Origin;

    MoveCPlanePoint gp = new MoveCPlanePoint(cplane);
    gp.SetCommandPrompt("CPlane origin");
    gp.SetBasePoint(origin, true);
    gp.DrawLineFromPoint(origin, true);
    gp.Get();

    if (gp.CommandResult() != Rhino.Commands.Result.Success)
      return gp.CommandResult();

    Point3d point = gp.Point();
    Vector3d v = origin - point;
    if (v.IsTiny())
      return Rhino.Commands.Result.Nothing;

    Plane pl = cplane.Plane;
    pl.Origin = point;
    cplane.Plane = pl;
    view.ActiveViewport.SetConstructionPlane(cplane);
    view.Redraw();
    return Rhino.Commands.Result.Success;
  }
}
