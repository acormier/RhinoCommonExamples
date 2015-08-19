using Rhino;
/// <summary>
/// title: Display conduit to draw overlay text
/// keywords: ['display', 'conduit', 'draw', 'overlay', 'text']
/// </summary>
partial class Examples
{
  readonly static CustomConduit m_customconduit = new CustomConduit();
  public static Rhino.Commands.Result DrawOverlay(RhinoDoc doc)
  {
    // toggle conduit on/off
    m_customconduit.Enabled = !m_conduit.Enabled;
    
    RhinoApp.WriteLine("Custom conduit enabled = {0}", m_customconduit.Enabled);
    doc.Views.Redraw();
    return Rhino.Commands.Result.Success;
  }
}
