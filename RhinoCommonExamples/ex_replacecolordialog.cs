using Rhino;
using Rhino.Commands;
using Rhino.UI;
using Eto.Forms;
/// <summary>
/// title: Replace Rhino's Color Picking Dialog
/// keywords: ['replace', 'rhinos', 'color', 'picking', 'dialog']
/// categories: ['Picking and Selection']
/// </summary>
partial class Examples
{
  private static ColorDialog m_dlg = null;

  public static Result ReplaceColorDialog(RhinoDoc doc)
  {
    Dialogs.SetCustomColorDialog(OnSetCustomColorDialog);
    return Result.Success;
  }

  static void OnSetCustomColorDialog(object sender, GetColorEventArgs e)
  {
    m_dlg = new ColorDialog();
    if (m_dlg.ShowDialog(null) == DialogResult.Ok)
    {
      var c = m_dlg.Color;
      e.SelectedColor = System.Drawing.Color.FromArgb (c.Ab, c.Rb, c.Gb, c.Bb);
    }
  }
}
