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
var wiki_files = @"Z:\src_osx\mcneel.com\wikidata\pages\developer\rhinocommonsamples"; // still need the wiki files until I redo the python samples

var front_matter_start = new List<string>{"---", "layout: code-sample", "author:", "platforms: ['Cross-Platform']", "apis: ['RhinoCommon']", "languages: ['C#', 'Python', 'VB.NET']"};
var front_matter_end = new List<string>{"description:", "order: 1", "---"};
            
var md_header_cs =
  Directory.GetFiles(input)
    .Where (fn => Path.GetFileName(fn).StartsWith("ex_") && fn.EndsWith(".cs"))
    .ToDictionary(
      fn => Path.Combine(output, Path.GetFileNameWithoutExtension(fn).Replace("ex_","") + ".md"),
      fn => 
        // front matter
        front_matter_start
        .Concat(File.ReadAllLines(fn).SkipWhile(ln => !ln.Contains("/// <summary>")).Skip(1).TakeWhile(ln => !ln.Contains("/// </summary>")).Select(ln => ln.Replace("/// ", "").Replace("'","")))
        .Concat(front_matter_end)
        
        // cs
        .Concat(new List<string>{"", "```cs"})
        .Concat(File.ReadAllLines(fn).SkipWhile(ln => !ln.Contains("/// </summary>")).Skip(1))
        .Concat(new List<string>{"```", "{: #cs .tab-pane .fade .in .active}", ""})
    );
    
var md_vb =
  Directory.GetFiles(input)
    .Where (fn => Path.GetFileName(fn).StartsWith("ex_") && fn.EndsWith(".vb"))
    .ToDictionary(
      fn => Path.Combine(output, Path.GetFileNameWithoutExtension(fn).Replace("ex_","") + ".md"),
      fn => 
        (new List<string>{"", "```vbnet"})
        .Concat(File.ReadAllLines(fn).SkipWhile(ln => !ln.Contains("Partial Friend Class Examples")))
        .Concat(new List<string>{"```", "{: #vb .tab-pane .fade .in .active}", ""})
    );

// python sample still from the wiki data because it hasn't been revised yet.
var md_py =
  Directory.GetFiles(wiki_files)
    .Where (fn => fn.EndsWith(".txt"))
    .ToDictionary(
      fn => Path.Combine(output, Path.GetFileNameWithoutExtension(fn) + ".md"),
      fn => 
        (new List<string>{"", "```python"})
        .Concat(File.ReadAllLines(fn).SkipWhile(ln => !ln.Contains("<code python>")).Skip(1).TakeWhile(ln => !ln.Contains("</code>")))
        .Concat(new List<string>{"```", "{: #py .tab-pane .fade .in .active}", ""})
    );
    
foreach(var f in md_header_cs) {
  Console.WriteLine("write {0}", f.Key);
  File.WriteAllLines(f.Key, f.Value);
}
foreach(var f in md_vb) {
  Console.WriteLine("write {0}", f.Key);
  File.AppendAllLines(f.Key, f.Value);
}
foreach(var f in md_py) {
  Console.WriteLine("write {0}", f.Key);
  File.AppendAllLines(f.Key, f.Value);
}

