using Rhino;
using Rhino.DocObjects;
using Rhino.Commands;
using Rhino.Input;
using Rhino.Geometry;
/// <summary>
/// title: Divide Curve by Segments
/// keywords: ['divide', 'curve', 'segments']
/// categories: ['Curves']
/// </summary>
partial class Examples
{
  public static Result DivideCurveBySegments(RhinoDoc doc)
  {
    const ObjectType filter = ObjectType.Curve; 
    ObjRef objref;
    var rc = RhinoGet.GetOneObject("Select curve to divide", false, filter, out objref);
    if (rc != Result.Success || objref == null)
      return rc;

    var curve = objref.Curve();
    if (curve == null || curve.IsShort(RhinoMath.ZeroTolerance))
      return Result.Failure;

    var segment_count = 2;
    rc = RhinoGet.GetInteger("Divide curve into how many segments?", false, ref segment_count);
    if (rc != Result.Success)
      return rc;

    Point3d[] points;
    curve.DivideByCount(segment_count, true, out points);
    if (points == null)
      return Result.Failure;

    foreach (var point in points)
      doc.Objects.AddPoint(point);

    doc.Views.Redraw();
    return Result.Success;
  }
}
