using Rhino;
using Rhino.Commands;
using Rhino.DocObjects;
using Rhino.Geometry;
using System.Drawing;
using Rhino.Input;
/// <summary>
/// title: Determine the Deviation between two Curves
/// </summary>
partial class Examples
{
  public static Result CrvDeviation(RhinoDoc doc)
  {
    doc.Objects.UnselectAll();

    ObjRef obj_ref1;
    var rc1 = RhinoGet.GetOneObject("first curve", true, ObjectType.Curve, out obj_ref1);
    if (rc1 != Result.Success)
      return rc1;
    Curve curve_a = null;
    if (obj_ref1 != null)
      curve_a = obj_ref1.Curve();
    if (curve_a == null)
      return Result.Failure;

    // Since you already selected a curve if you don't unselect it
    // the next GetOneObject won't stop as it considers that curve 
    // input, i.e., curveA and curveB will point to the same curve.
    // Another option would be to use an instance of Rhino.Input.Custom.GetObject
    // instead of Rhino.Input.RhinoGet as GetObject has a DisablePreSelect() method.
    doc.Objects.UnselectAll();

    ObjRef obj_ref2;
    var rc2 = RhinoGet.GetOneObject("second curve", true, ObjectType.Curve, out obj_ref2);
    if (rc2 != Result.Success)
      return rc2;
    Curve curve_b = null;
    if (obj_ref2 != null)
      curve_b = obj_ref2.Curve();
    if (curve_b == null)
      return Result.Failure;

    var tolerance = doc.ModelAbsoluteTolerance;

    double max_distance;
    double max_distance_parameter_a;
    double max_distance_parameter_b;
    double min_distance;
    double min_distance_parameter_a;
    double min_distance_parameter_b;

    DeviationConduit conduit;
    if (!Curve.GetDistancesBetweenCurves(curve_a, curve_b, tolerance, out max_distance, 
              out max_distance_parameter_a, out max_distance_parameter_b,
              out min_distance, out min_distance_parameter_a, out min_distance_parameter_b))
    {
      RhinoApp.WriteLine("Unable to find overlap intervals.");
      return Result.Success;
    }
    else
    {
      if (min_distance <= RhinoMath.ZeroTolerance)
        min_distance = 0.0;
      var max_dist_pt_a = curve_a.PointAt(max_distance_parameter_a);
      var max_dist_pt_b = curve_b.PointAt(max_distance_parameter_b);
      var min_dist_pt_a = curve_a.PointAt(min_distance_parameter_a);
      var min_dist_pt_b = curve_b.PointAt(min_distance_parameter_b);

      conduit = new DeviationConduit(curve_a, curve_b, min_dist_pt_a, min_dist_pt_b, max_dist_pt_a, max_dist_pt_b) {Enabled = true};
      doc.Views.Redraw();

      RhinoApp.WriteLine("Minimum deviation = {0}   pointA({1}), pointB({2})", min_distance, min_dist_pt_a, min_dist_pt_b);
      RhinoApp.WriteLine("Maximum deviation = {0}   pointA({1}), pointB({2})", max_distance, max_dist_pt_a, max_dist_pt_b);
    }

    var str = "";
    RhinoGet.GetString("Press Enter when done", true, ref str);
    conduit.Enabled = false;

    return Result.Success;
  }
}
