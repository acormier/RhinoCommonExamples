using Rhino;
using Rhino.DocObjects;
using Rhino.Commands;
using Rhino.Input.Custom;
/// <summary>
/// title: Read Dimension Text
/// keywords: ['read', 'dimension', 'text']
/// categories: ['Other']
/// </summary>
partial class Examples
{
  public static Result ReadDimensionText(RhinoDoc doc)
  {
    var go = new GetObject();
    go.SetCommandPrompt("Select annotation");
    go.GeometryFilter = ObjectType.Annotation;
    go.Get();
    if (go.CommandResult() != Result.Success) 
      return Result.Failure;
    var annotation = go.Object(0).Object() as AnnotationObjectBase;
    if (annotation == null)
      return Result.Failure;

    RhinoApp.WriteLine("Annotation text = {0}", annotation.DisplayText);

    return Result.Success;
  }
}
