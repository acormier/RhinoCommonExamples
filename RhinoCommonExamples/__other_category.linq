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

var examples_dir = @"Z:\src_osx\mcneel.com\RhinoCommonSamples\RhinoCommonExamples\RhinoCommonExamples";

var files_with_no_categories = 
  Directory.GetFiles(examples_dir)
    .Where (fn => Path.GetFileName(fn).StartsWith("ex_") && fn.EndsWith(".cs"))
    .Where (fn => !File.ReadAllLines(fn).Any (ln => ln.StartsWith("/// categories: ['")))
    .ToDictionary(
      fn => fn,
      fn => File.ReadAllLines(fn).TakeWhile(ln => !ln.Contains("/// </summary>"))
            .Concat("/// categories: ['Other']")
            .Concat(File.ReadAllLines(fn).SkipWhile(ln2 => !ln2.Contains("/// </summary>")))
    );
    //.Dump();
  
foreach( var f in files_with_no_categories) {
  Console.WriteLine("write {0} ...", f.Key);
  File.WriteAllLines(f.Key, f.Value);
}