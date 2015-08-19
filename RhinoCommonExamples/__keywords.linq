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

var categories = new List<string> {"Adding Objects", "Curves", "Layers", "Picking and Selection", "Viewports and Views", "Blocks", "Draw", "Drafting"};

Func<string, string> cleanTitle = s => s.ToLower().Replace("'","").Replace("(","").Replace(")","");

Func<string, IEnumerable<string>> keywordList =
       s => cleanTitle(s).Split(' ')
              .Except(new List<string>{"from", "this"}) // excude some meaningless words
              .Where(w => (new List<string> {"add"}).Contains(w) || w.Length > 3); // exclude short words unless they're in the list
              
Func<IEnumerable<string>, string> coll2StrLiteral = e => string.Format("['{0}']", e.Aggregate((current, next) => current + "', '" + next));
                                                         
var file_keywords = 
  Directory.GetFiles(examples_dir)
    .Where(fn => Path.GetFileName(fn).StartsWith("ex_") && Path.GetFileName(fn).EndsWith(".cs"))
    .ToDictionary (
      fn => fn,
      fn => File.ReadLines(fn)
              .Where (ln => ln.StartsWith("/// title:"))
              .Select (ln => ln.Replace("/// title:", ""))
              .SingleOrDefault()
    )
    .ToDictionary(
      d => d.Key,
      d => Tuple.Create<string, IEnumerable<string>>(d.Value, keywordList(d.Value))
    )
    //.SelectMany (kvp => kvp.Value.Item2).Distinct().OrderBy(i => i) // unique keywords
    .ToDictionary (
      kvp => kvp.Key,
      kvp => File.ReadAllLines(kvp.Key).TakeWhile(ln => !ln.Contains("/// </summary>"))
             .Concat(string.Format("/// keywords: {0}", coll2StrLiteral(kvp.Value.Item2)))
             .Concat(File.ReadAllLines(kvp.Key).SkipWhile(ln2 => !ln2.Contains("/// </summary>")))
    )
    //.Dump()
    ;
    
foreach( var f in file_keywords) {
  Console.WriteLine("write {0} ...", f.Key);
  File.WriteAllLines(f.Key, f.Value);
}