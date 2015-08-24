using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
/// <summary>
/// title: Justifying Text Entities
/// keywords: ['justifying', 'text', 'entities']
/// categories: ['Other']
/// </summary>
partial class Examples
{
  public static Result TextJustify(RhinoDoc doc)
  {
    var text_entity = new TextEntity
    {
      Plane = Plane.WorldXY,
      Text = "Hello Rhino!",
      Justification = TextJustification.MiddleCenter,
      FontIndex = doc.Fonts.FindOrCreate("Arial", false, false)
    };

    doc.Objects.AddText(text_entity);
    doc.Views.Redraw();

    return Result.Success;
  }
}
