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
  .Where (fn => File.ReadAllLines(fn).Any (ln => ln.ToLower().Contains("rhinoscriptsyntax")))
  .Select (Path.GetFileNameWithoutExtension)
  .Where (fn => files_in_csproj.Contains(fn))
  //.Take(5)
  .Dump()
  ;