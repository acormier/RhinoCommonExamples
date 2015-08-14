using System;
using System.Runtime.InteropServices;
using Rhino;
using Rhino.Commands;

partial class Examples
{
  [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
  public static extern IntPtr GetParent(IntPtr hWnd);

  [DllImport("user32.dll")]
  static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

  public static Result MaximizeView(RhinoDoc doc)
  {
    IntPtr parent_handle = GetParent(doc.Views.ActiveView.Handle);
    if (parent_handle != IntPtr.Zero)
      ShowWindow(parent_handle, 3);
    return Result.Success;
  }
}
