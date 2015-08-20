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

Func<string, IEnumerable<string>> titleWords =
       s => cleanTitle(s).Split(' ').Where (w => !string.IsNullOrWhiteSpace(w));
       
Func<Dictionary<string, IEnumerable<string>>, IEnumerable<Tuple<string, string>>> dictToTupleList = d => d.SelectMany (kvp => kvp.Value.Select (v => Tuple.Create<string, string>(kvp.Key, v)));
              
Func<IEnumerable<string>, string> coll2StrLiteral = e => string.Format("['{0}']", e.Aggregate((current, next) => current + "', '" + next));
                                                         
var file_catogories = 
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
      d => titleWords(d.Value)
    )
    //.Dump()
    ;
    
var q = (from tw in dictToTupleList(file_catogories)
        from cat in categories
        where tw.Item2.Length >= 3 && tw.Item2 != "and" && cat.ToLower().Contains(tw.Item2.ToLower()) // || tw.Item1.Contains("addannotationtext.cs")
        select new {fn=tw.Item1, cat}).Distinct().Dump();

/*foreach( var f in file_keywords) {
  Console.WriteLine("write {0} ...", f.Key);
  File.WriteAllLines(f.Key, f.Value);
}*/