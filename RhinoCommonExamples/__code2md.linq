<Query Kind="Statements">
  <NuGetReference>Rx-Main</NuGetReference>
  <NuGetReference>Rx-Testing</NuGetReference>
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

var output = @"Z:\src_osx\mcneel.com\RhinoDocsMainGhpSite\developer-rhino3d-com\_samples\rhinocommon";
var input = @"Z:\src_osx\mcneel.com\RhinoCommonSamples\RhinoCommonExamples\RhinoCommonExamples";

var front_matter_start = new List<string>{"---", "layout: code-sample", "author:", "platforms: ['Cross-Platform']", "apis: ['RhinoCommon']", "languages: ['C#', 'Python', 'VB.NET']"};
var front_matter_end = new List<string>{"description:", "order: 1", "---"};
            
var files_in_csproj =
  File.ReadAllLines(Path.Combine(input, "RhinoCommonExamples.csproj"))
    .Where (ln => Regex.Match(ln, "(?<=<Compile Include=\"ex_).*(?=\\.cs\" />)").Success)
    .Select (ln => Regex.Match(ln, "(?<=<Compile Include=\"ex_).*(?=\\.cs\" />)").Value);
    
var md_header_cs =
  Directory.GetFiles(input)
    .Where (fn => Path.GetFileName(fn).StartsWith("ex_") && fn.EndsWith(".cs"))
    .Where (fn => files_in_csproj.Select(f => "ex_" + f + ".cs").Contains(Path.GetFileName(fn))) // only files that are in the project work
    .ToDictionary(
      fn => Path.Combine(output, Path.GetFileNameWithoutExtension(fn).Replace("ex_","") + ".md"),
      fn => 
        // front matter
        front_matter_start
        .Concat(File.ReadAllLines(fn).SkipWhile(ln => !ln.Contains("/// <summary>")).Skip(1).TakeWhile(ln => !ln.Contains("/// </summary>")).Select(ln => ln.Replace("/// ", "")/*.Replace("'","")*/))
        .Concat(front_matter_end)
        
        // cs
        .Concat(new List<string>{"", "```cs"})
        .Concat(File.ReadAllLines(fn).SkipWhile(ln => !ln.Contains("/// </summary>")).Skip(1))
        .Concat(new List<string>{"```", "{: #cs .tab-pane .fade .in .active}", ""})
    );
    
var md_vb =
  Directory.GetFiles(input)
    .Where (fn => Path.GetFileName(fn).StartsWith("ex_") && fn.EndsWith(".vb"))
    .Where (fn => files_in_csproj.Select(f => "ex_" + f + ".vb").Contains(Path.GetFileName(fn))) // only files that are in the cs project work
    .ToDictionary(
      fn => Path.Combine(output, Path.GetFileNameWithoutExtension(fn).Replace("ex_","") + ".md"),
      fn => 
        (new List<string>{"", "```vbnet"})
        .Concat(File.ReadAllLines(fn).SkipWhile(ln => !ln.Contains("Partial Friend Class Examples")))
        .Concat(new List<string>{"```", "{: #vb .tab-pane .fade .in}", ""})
    );
    
var md_py =
  Directory.GetFiles(input)
    .Where (fn => Path.GetFileName(fn).StartsWith("ex_") && fn.EndsWith(".py"))
    //.Where (fn => fn.ToLower().Contains("displayconduit"))
    .Where (fn => files_in_csproj.Select(f => "ex_" + f + ".py").Contains(Path.GetFileName(fn))) // only files that are in the cs project work
    .ToDictionary(
      fn => Path.Combine(output, Path.GetFileNameWithoutExtension(fn).Replace("ex_","") + ".md"),
      fn => 
        (new List<string>{"", "```python"})
        .Concat(File.ReadAllLines(fn))
        .Concat(new List<string>{"```", "{: #py .tab-pane .fade .in}", ""})
    )
    //.Dump()
    ;
   
Console.WriteLine();
Console.WriteLine("write C# files ...");
foreach(var f in md_header_cs) {
  Console.WriteLine("write {0}", f.Key);
  File.WriteAllLines(f.Key, f.Value);
}
Console.WriteLine();
Console.WriteLine("write vb files ...");
foreach(var f in md_vb) {
  Console.WriteLine("write {0}", f.Key);
  File.AppendAllLines(f.Key, f.Value);
}
Console.WriteLine();
Console.WriteLine("write py files ...");
foreach(var f in md_py) {
  Console.WriteLine("write {0}", f.Key);
  File.AppendAllLines(f.Key, f.Value);
}

md_header_cs.Count.Dump("cs");
md_vb.Count.Dump("vb");
md_py.Count().Dump("py");