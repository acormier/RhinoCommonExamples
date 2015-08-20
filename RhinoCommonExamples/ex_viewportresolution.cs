using Rhino;
using Rhino.Commands;
/// <summary>
/// title: Print Active Viewport Resolution
/// keywords: ['print', 'active', 'viewport', 'resolution']
/// categories: ['Viewports and Views']
/// </summary>
partial class Examples
{
  public static Result ViewportResolution(RhinoDoc doc)
  {
    var active_viewport = doc.Views.ActiveView.ActiveViewport;
    RhinoApp.WriteLine("Name = {0}: Width = {1}, Height = {2}", 
      active_viewport.Name, active_viewport.Size.Width, active_viewport.Size.Height);
    return Result.Success;
  }
}
