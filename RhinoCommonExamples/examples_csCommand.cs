using System;
using Rhino;
using Rhino.Geometry;
using System.Runtime.InteropServices;

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

  [System.Runtime.InteropServices.Guid("96fdf4ac-5457-439a-95ed-4085977065df")]
  public class examples_csCommand : Rhino.Commands.Command
  {
    public override string EnglishName { get { return "examples_cs"; } }

    void Test(Func<RhinoDoc, Rhino.Commands.Result> func, RhinoDoc doc)
    {
      RhinoApp.WriteLine("[TEST START] - " + func.Method.ToString());
      Rhino.Commands.Result rc = func(doc);
      RhinoApp.WriteLine("[TEST DONE] - result = " + rc.ToString());
    }

    protected override Rhino.Commands.Result RunCommand(RhinoDoc doc, Rhino.Commands.RunMode mode)
    {
      //RhinoCommonExamples.RhinoCommonExamplesPlugin.Instance.IncrementRunCommandCount();
      /*Test(Examples.ActiveViewport, doc);
      Test(Examples.AddBrepBox, doc);
      Test(Examples.AddChildLayer, doc);
      Test(Examples.AddCircle, doc);
      Test(Examples.AddCone, doc);
      Test(Examples.AddCylinder, doc);

      //Test(Examples.AddBackgroundBitmap, doc); // doesn't work.  Silently fails

      Test(Examples.AddClippingPlane, doc);
      Test(Examples.AddLayer, doc);
      Test(Examples.AddLayout, doc);
      Test(Examples.AddLine, doc);
      Test(Examples.AddLinearDimension, doc);
      Test(Examples.AddLinearDimension2, doc);
      Test(Examples.AddMaterial, doc);
      Test(Examples.AddMesh, doc);
      Test(Examples.AddMeshBox, doc);
      Test(Examples.AddTwistedCube, doc);
      Test(Examples.BoxShell, doc);
      Test(Examples.CircleCenter, doc);
      Test(Examples.ConduitDrawShadedMesh, doc);
      Test(Examples.CreateSpiral, doc);
      Test(Examples.ExportControlPoints, doc);
      Test(Examples.ExtractRenderMesh, doc);
      Test(Examples.ExtrudeBrepFace, doc);
      Test(Examples.GetMultipleWithOptions, doc);
      Test(Examples.InstanceArchiveFileStatus, doc);
      Test(Examples.LineBetweenCurves, doc);
      Test(Examples.MoveGripObjects, doc);

      Test(Examples.ObjectColor, doc); // doesn't work.  SelColor not implemented yet.

      Test(Examples.TransformBrep, doc);
      Test(Examples.TweakColors, doc);
      Test(Examples.TweenCurve, doc);
      
      //Test(Examples.AddNamedView, doc); // doesn't work.  Need to have a look

      Test(Examples.AddNurbsCircle, doc);
      Test(Examples.AddNurbsCurve, doc);
      Test(Examples.AddObjectsToGroup, doc);
      Test(Examples.AddSphere, doc);
      Test(Examples.AddAnnotationText, doc);
      Test(Examples.AddTorus, doc);
      Test(Examples.AddTruncatedCone, doc);
      Test(Examples.AdvancedDisplay, doc);
      Test(Examples.ArcLengthPoint, doc);
      Test(Examples.BooleanDifference, doc);
      Test(Examples.BlockInsertionPoint, doc);
      Test(Examples.CommandLineOptions, doc);
      Test(Examples.ConstrainedCopy, doc);
      Test(Examples.CreateBlock, doc);
      Test(Examples.CurveBoundingBox, doc);
      Test(Examples.DivideByLengthPoints, doc);
      Test(Examples.DetermineObjectLayer, doc);
      Test(Examples.DupBorder, doc);
      Test(Examples.EditText, doc);
      Test(Examples.FindObjectsByName, doc);
      Test(Examples.HatchCurve, doc);
      Test(Examples.IntersectCurves, doc);
      Test(Examples.InsertKnot, doc);
      Test(Examples.IntersectLines, doc);
      Test(Examples.InstanceDefinitionObjects, doc);
      Test(Examples.IsBrepBox, doc);
      Test(Examples.IsocurveDensity, doc);
      Test(Examples.MoveCPlane, doc);
      Test(Examples.AddNestedBlock, doc);
      Test(Examples.ObjectDecoration, doc);
      Test(Examples.ObjectDisplayMode, doc);
      Test(Examples.OrientOnSrf, doc);
      Test(Examples.SelLayer, doc);
      Test(Examples.ShowSurfaceDirection, doc);
      Test(Examples.Sweep1, doc);
      Test(Examples.UnrollSurface, doc);
      Test(Examples.UnrollSurface2, doc);
      Test(Examples.ZoomToObject, doc);
      Test(Examples.ExplodeHatch, doc);
      
      //Test(Examples.Splop, doc); // doesn't work
      
      Test(Examples.AddTexture, doc);

      //// the rest were converted from stand alone commands
      Test (Examples.AddRadialDimension, doc);
      Test (Examples.AnalysisMode_on, doc);
      Test (Examples.AnalysisMode_off, doc);
      Test (Examples.ArrayByDistance, doc);
      Test (Examples.RTreeClosestPoint, doc);
      Test (Examples.ConduitArrowHeads, doc);*/

      //Test (Examples.ConduitBitmap, doc); // doesn't work.  Had to remove ex_conduitbitmap.cs from project as it crashes Rhino

      /*Test (Examples.CopyGroups, doc);
      Test (Examples.CreateMeshFromBrep, doc);
      Test (Examples.CreateSurfaceFromPointsAndKnots, doc);
      Test (Examples.CrvDeviation, doc);
      Test (Examples.BrepFromCurveBBox, doc);
      Test (Examples.FindCurveParameterAtPoint, doc);
      Test (Examples.ReverseCurve, doc);
      Test (Examples.CurveSurfaceIntersect, doc);
      Test (Examples.CustomGeometryFilter, doc);*/

      //Test (Examples.CustomPython, doc); // doesn't work.

      /*Test (Examples.CustomUndo, doc);
      Test (Examples.DeleteBlock, doc);
      Test (Examples.DetermineCurrentLanguage, doc);
      Test (Examples.ChangeDimensionStyle, doc);
      Test (Examples.DisplayConduit, doc);
      Test (Examples.DisplayOrder, doc);
      Test (Examples.DisplayPrecision, doc);
      Test (Examples.DivideCurveBySegments, doc);
      Test (Examples.DivideCurveStraight, doc);
      Test (Examples.MovePointObjects, doc);
      Test (Examples.MovePointObjectsNonUniform, doc);*/

      //Test (Examples.DrawOverlay, doc); // doesn't work.

      /*Test (Examples.DrawString, doc);
      Test (Examples.DuplicateObject, doc);
      Test (Examples.DupMeshBoundary, doc);
      Test (Examples.EdgeSrf, doc);
      Test (Examples.FurthestZOnSurfaceGivenXY, doc);
      Test (Examples.DetermineNormalDirectionOfBrepFace, doc);
      Test (Examples.ExtendCurve, doc);
      Test (Examples.ExtendSurface, doc);
      Test (Examples.ExtractIsoCurve, doc);
      Test (Examples.FilletCurves, doc);
      Test (Examples.GetAngle, doc);
      Test (Examples.GetPointDynamicDraw, doc);*/

      //Test (Examples.ReadDimensionText, doc); // doesn't work : bug - annotation.DisplayText returns an empty string

      /*Test (Examples.GetUUID, doc);*/

      // Test (Examples.Gumball, doc); // doesn't work : remove ex_gumball.cs from project or it won't compile.
      // Test (Examples.IntersectLineCircle, doc); // doesn't work : GetLine is not implemented on the mac

      /*Test (Examples.IsPlanarSurfaceInPlane, doc);
      Test (Examples.Leader, doc);
      Test (Examples.LockLayer, doc);
      Test (Examples.Loft, doc);
      Test (Examples.ContourCurves, doc);*/

      //Test(Examples.MaximizeView, doc); // donesn't work.  Has win32 references so ex_maximizeview.cs is not added to project.

      /*Test (Examples.DrawMesh, doc);
      Test (Examples.MeshVolume, doc);*/

      //Test (Examples.ModifyLightColor, doc); // doesn't work. SelLight not implemented.
      //Test (Examples.ModifyObjectColor, doc); // doesn't work. SelColor not implemented.

      /*Test (Examples.MoveObjectsToCurrentLayer, doc);
      Test (Examples.NurbsCurveIncreaseDegree, doc);
      Test (Examples.NurbsSurfaceIncreaseDegree, doc);
      Test (Examples.ObjectIterator, doc);
      Test (Examples.RenameObject, doc);
      Test (Examples.OffsetCurve, doc);
      Test (Examples.Ortho, doc);
      Test (Examples.PickPoints, doc);
      Test (Examples.PickPoint, doc);
      Test (Examples.PlanarSurface, doc);*/

      //Test (Examples.PointAtCursor, doc); // doesn't work.  Has win32 references so ex_pointatcursor.cs is not added to project.

      /*Test (Examples.PrePostPick, doc);
      Test (Examples.PrincipalCurvature, doc);
      Test (Examples.InstanceDefinitionNames, doc);
      Test (Examples.InstanceDefinitionTree, doc);
      Test (Examples.ProjectPointsToBreps, doc);
      Test (Examples.ProjectPointsToMeshesEx, doc);
      Test (Examples.RenameBlock, doc);
      Test (Examples.RenameLayer, doc);
      Test (Examples.ReparameterizeCurve, doc);*/

      //Test (Examples.ReplaceColorDialog, doc); // doesn't work.  investigate

      /*Test (Examples.ReplaceHatchPattern, doc);
      Test (Examples.SetRhinoPageViewWidthAndHeight, doc);*/

      //Test (Examples.SelByUserText, doc); // doesn't work.  Has to derive from SelCommand.  Removed ex_selbyusertext.cs from project
      //Test (Examples.SelectObjectsInObjectGroups, doc); // doesn't work.  exception on line 26.

      /*Test (Examples.SetActiveView, doc);
      Test (Examples.SetViewName, doc);*/

      //Test (Examples.SingleColorBackfaces, doc); // doesn't work.  look into

      /*Test (Examples.SplitBrepsWithPlane, doc);*/

      //Test (Examples.SpriteDrawing, doc); // doesn't work.  crash

      /*Test (Examples.SurfaceFromCorners, doc);
      Test (Examples.TextJustify, doc);
      Test (Examples.TightBoundingBox, doc);
      Test (Examples.Userdata, doc);
      Test (Examples.ViewportResolution, doc);*/
      return Rhino.Commands.Result.Success;
    }
  }
}


