using System;
using Rhino;
using Rhino.Geometry;

namespace examples_cs
{
  public class csTransformCommand : Rhino.Commands.TransformCommand
  {
    public override string EnglishName { get { return "examples_csTransform"; } }

    double m_scale = 1;
    bool m_copy;
    protected override Rhino.Commands.Result RunCommand(RhinoDoc doc, Rhino.Commands.RunMode mode)
    {
      // Select objects to scale
      var list = new Rhino.Collections.TransformObjectList();
      var rc = SelectObjects("Select objects to scale", list);
      if (rc != Rhino.Commands.Result.Success)
        return rc;

      Point3d anchor;
      Point3d _ref;
      Plane plane;

      // Origin point
      var gp = new Rhino.Input.Custom.GetPoint();
      gp.SetCommandPrompt("Origin point");
      var copy = new Rhino.Input.Custom.OptionToggle(m_copy,"No", "Yes");
      gp.AddOptionToggle("Copy", ref copy);
      for (; ; )
      {
        var res = gp.Get();
        if (res == Rhino.Input.GetResult.Point)
        {
          var view = gp.View();
          if (view == null)
            return Rhino.Commands.Result.Failure;
          plane = view.ActiveViewport.ConstructionPlane();
          anchor = gp.Point();
          break;
        }
        if (res == Rhino.Input.GetResult.Option)
          continue;

        return Rhino.Commands.Result.Cancel;
      }

      bool bAcceptDefault = true;

      // Scale factor or first reference point
      gp.SetCommandPrompt("Scale factor or first reference point");
      gp.SetBasePoint(anchor, true);
      gp.DrawLineFromPoint(anchor, true);
      gp.ConstrainToConstructionPlane(true);
      for (; ; )
      {
        if (bAcceptDefault)
        {
          gp.AcceptNumber(true, false);
          gp.SetDefaultNumber(m_scale);
        }
        else
        {
          gp.AcceptNothing(true);
          gp.ClearDefault();
        }

        var res = gp.Get();
        if (res == Rhino.Input.GetResult.Point)
        {
          _ref = gp.Point();
          break;
        }
        if (res == Rhino.Input.GetResult.Number)
        {
          double scale = Math.Abs(gp.Number());
          const double EPS_DIVIDE = 0.000001;
          if (scale < EPS_DIVIDE)
            continue;
          m_scale = scale;
          plane.Origin = anchor;

          var xform = Transform.Scale(plane, m_scale, m_scale, m_scale);
          TransformObjects(list, xform, copy.CurrentValue, copy.CurrentValue);
          m_copy = copy.CurrentValue;
          if (m_copy)
          {
            bAcceptDefault = false;
            continue;
          }
          doc.Views.Redraw();
          return Rhino.Commands.Result.Success;
        }

        if (res == Rhino.Input.GetResult.Nothing)
        {
          if (bAcceptDefault == false)
            return Rhino.Commands.Result.Success;

          plane.Origin = anchor;
          var xform = Transform.Scale(plane, m_scale, m_scale, m_scale);
          TransformObjects(list, xform, copy.CurrentValue, copy.CurrentValue);

          m_copy = copy.CurrentValue;
          if (m_copy)
          {
            bAcceptDefault = false;
            continue;
          }
          doc.Views.Redraw();
          return Rhino.Commands.Result.Success;
        }
        if (res == Rhino.Input.GetResult.Option)
          continue;
        return Rhino.Commands.Result.Cancel;
      }

      // Second reference point
      var gx = new GetScaleXform();
      gx.SetCommandPrompt("Second reference point");
      gx.AddOptionToggle("copy", ref copy);
      gx.AddTransformObjects(list);
      gx.SetBasePoint(anchor, true);
      gx.DrawLineFromPoint(anchor, true);
      gx.ConstrainToConstructionPlane(true);
      gx.Plane = plane;
      gx.RefPoint = _ref;
      gx.AcceptNothing(true);
      gx.AcceptNumber(true, false);

      rc = Rhino.Commands.Result.Cancel;
      for (; ; )
      {
        var res = gx.GetXform();
        if (res == Rhino.Input.GetResult.Point)
        {
          var view = gx.View();
          if (view == null)
          {
            rc = Rhino.Commands.Result.Failure;
            break;
          }
          var xform = gx.CalculateTransform(view.ActiveViewport, gx.Point());
          if (xform.IsValid && xform != Transform.Identity)
          {
            TransformObjects(list, xform, copy.CurrentValue, copy.CurrentValue);
            rc = Rhino.Commands.Result.Success;
            m_scale = gx.Scale;
          }
          m_copy = copy.CurrentValue;
          if (m_copy)
            continue;

          break;
        }

        if (res == Rhino.Input.GetResult.Number)
        {
          var view = gx.View();
          if (view == null)
          {
            rc = Rhino.Commands.Result.Failure;
            break;
          }

          var xform = gx.CalculateTransform(view.ActiveViewport, gx.Number());
          if (xform.IsValid && xform != Transform.Identity)
          {
            TransformObjects(list, xform, copy.CurrentValue, copy.CurrentValue);
            rc = Rhino.Commands.Result.Success;
            m_scale = gx.Scale;
          }
          m_copy = copy.CurrentValue;
          if (m_copy)
            continue;
          break;
        }

        if (res == Rhino.Input.GetResult.Option)
          continue;

        break;
      }

      doc.Views.Redraw();
      return rc;
    }
  }
}

