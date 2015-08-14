using Rhino;

partial class Examples
{
  static string m_key = string.Empty;
  public static Rhino.Commands.Result SelByUserText(RhinoDoc doc)
  {
    // You don't have to override RunCommand if you don't need any user input. In
    // this case we want to get a key from the user. If you return something other
    // than Success, the selection is canceled
    return Rhino.Input.RhinoGet.GetString("key", true, ref m_key);
  }

  protected static override bool SelFilter(Rhino.DocObjects.RhinoObject rhObj)
  {
    string s = rhObj.Attributes.GetUserString(m_key);
    return !string.IsNullOrEmpty(s);
  }
}
