using System;
using Rhino.Geometry;

namespace examples_cs
{
  class GetScaleXform : Rhino.Input.Custom.GetTransform
  {
    public Plane Plane { get; set; }
    public Point3d RefPoint { get; set; }
    public double Scale { get; set; }

    public GetScaleXform()
    {
      Plane = Plane.WorldXY;
      RefPoint = Point3d.Origin;
      Scale = 1;
    }

    protected override void OnDynamicDraw(Rhino.Input.Custom.GetPointDrawEventArgs e)
    {
      Point3d basePoint;
      if (TryGetBasePoint(out basePoint))
      {
        e.Display.DrawLine(basePoint, RefPoint, System.Drawing.Color.Black);
        e.Display.DrawPoint(RefPoint, System.Drawing.Color.Black);
        base.OnDynamicDraw(e);
      }
    }

    public Transform CalculateTransform( Rhino.Display.RhinoViewport viewport, double d )
    {
      Point3d basePoint;
      if (!TryGetBasePoint(out basePoint))
        return Transform.Identity;
      Plane plane = viewport.ConstructionPlane();
      plane.Origin = basePoint;
      Vector3d v = RefPoint - basePoint;

      double len1 = v.Length;
      if (Math.Abs(len1) < 0.000001)
        return Transform.Identity;

      v.Unitize();
      v = v * d;
      double len2 = v.Length;
      if (Math.Abs(len2) < 0.000001)
        return Transform.Identity;

      Scale = len2 / len1;
      return Transform.Scale(plane, Scale, Scale, Scale);
    }
    public override Transform CalculateTransform(Rhino.Display.RhinoViewport viewport, Point3d point)
    {
      Point3d basePoint;
      if (!TryGetBasePoint(out basePoint))
        return Transform.Identity;
      double len2 = (point - basePoint).Length;
      double len1 = (RefPoint - basePoint).Length;
      if (Math.Abs(len1) < 0.000001 || Math.Abs(len2) < 0.000001)
        return Transform.Identity;

      Scale = len2 / len1;

      Plane plane = viewport.ConstructionPlane();
      plane.Origin = basePoint;

      return Transform.Scale(plane, Scale, Scale, Scale);
    }
  }
}

