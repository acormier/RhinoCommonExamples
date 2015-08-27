<Query Kind="Statements">
  <Reference>C:\dev\rhino\wip\src4\bin\Debug\RhinoCommon.dll</Reference>
  <NuGetReference>morelinq</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <NuGetReference>Rx-Main</NuGetReference>
  <NuGetReference>Rx-Testing</NuGetReference>
  <Namespace>MoreLinq</Namespace>
  <Namespace>System</Namespace>
  <Namespace>System.IO</Namespace>
  <Namespace>System.Reactive</Namespace>
  <Namespace>System.Reactive.Concurrency</Namespace>
  <Namespace>System.Reactive.Disposables</Namespace>
  <Namespace>System.Reactive.Joins</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>System.Reactive.PlatformServices</Namespace>
  <Namespace>System.Reactive.Subjects</Namespace>
  <Namespace>System.Reactive.Threading.Tasks</Namespace>
</Query>

var input = @"Z:\src_osx\mcneel.com\RhinoCommonSamples\RhinoCommonExamples\RhinoCommonExamples";

var files_in_csproj =
  File.ReadAllLines(Path.Combine(input, "RhinoCommonExamples.csproj"))
    .Where (ln => Regex.Match(ln, "(?<=<Compile Include=\")ex_.*\\.cs(?=\" />)").Success)
    .Select (ln => Regex.Match(ln, "(?<=<Compile Include=\")ex_.*\\.cs(?=\" />)").Value)
    .Select(Path.GetFileNameWithoutExtension)
    //.Take(5).Dump()
    ;
    
var py_files =
  Directory.GetFiles(input)
  .Where (fn => Path.GetFileName(fn).StartsWith("ex_") && fn.EndsWith(".py"))
  .Select (Path.GetFileNameWithoutExtension)
  //.Take(5)
  //.Dump()
  ;

files_in_csproj.Except(py_files).OrderBy(fn => fn)
//.Where (fn => fn.ToLower().Contains("nested"))
//.Take(5)
.Except(new List<string> {"ex_addmeshbox", "ex_addnestedblock", "ex_addtwistedcube" /*!vb*/, "ex_analysismode", "ex_boxshell", "ex_conduitdrawshadedmesh", "ex_createspiral",
    "ex_custompython", "ex_displayconduit", "ex_exportcontrolpoints", "ex_extractrendermesh", "ex_extrudebrepface", "ex_instancearchivefilestatus", "ex_linebetweencurves",
    "ex_movegripobjects", "ex_objectcolor", "ex_showsurfacedirection", "ex_singlecolorbackfaces", "ex_splop" /*!vb*/, "ex_spritedrawing", "ex_tweakcolors", "ex_tweencurve", "ex_userdata"}) // no python equivalent
.Dump("C# file names with no py equivalent");

// whatever is remaining is probably files that are not in the project, i.e, don't work on osx
py_files.Except(files_in_csproj).OrderBy (fn => fn)
//.Where (fn => fn.ToLower().Contains("block"))
.Dump("py fil names with no py equivalent");