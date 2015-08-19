using System.Drawing;
using Rhino;
using Rhino.Commands;
using Rhino.Display;
/// <summary>
/// title: draw a non-square bitmap in a display conduit
/// </summary>
partial class Examples
{
  static readonly DrawBitmapConduit m_conduit = new DrawBitmapConduit();

  public static Result ConduitBitmap(RhinoDoc doc)
  {
    // toggle conduit on/off
    m_conduit.Enabled = !m_conduit.Enabled;
    
    RhinoApp.WriteLine("Custom conduit enabled = {0}", m_conduit.Enabled);
    doc.Views.Redraw();
    return Result.Success;
  }
}
