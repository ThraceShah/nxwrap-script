using System.Net;
using System.Text.RegularExpressions;
var stopWatch = new Stopwatch();
stopWatch.Start();
var inputDir = @"E:\Code\nx\NXDecompile\NxOpen";
var outputDir = @"E:\Code\nx\nxnativewrap\NXOpen";
var interfaceSet=new HashSet<string>();
foreach(var line in File.ReadAllLines("interface_list.txt"))
{
    interfaceSet.Add(line);
}
if(!Directory.Exists(outputDir))
{
    Directory.CreateDirectory(outputDir);
}
var collectionRegex=new Regex(@"class (?<type>[a-zA-Z0-9_.]+) : TaggedObjectCollection");
var collectionTypeRegex=new Regex(@"public (?<gType>[a-zA-Z0-9_.]+)\[\] ToArray\(\)");
var enumeratorRgex = new Regex(@"IEnumerator enumerator = GetEnumerator\(\);");
var enumeratorRep="var enumerator = GetEnumerator();";
var arrayListRegex = new Regex(@"ArrayList arrayList = new ArrayList\(\);");
var arrayListRep = "";
var enumeratorReturnRegex = new Regex(@"\([a-zA-Z0-9_.]+\[\]\)arrayList.ToArray\(typeof\([a-zA-Z0-9_.]+\)\);");
var enumeratorReturnRep = $"arrayList.ToArray();";

var getRegex = new Regex(@"\((?<type>[a-zA-Z0-9_.]+)\)NXObjectManager\.Get\((?<var>.*)\);$");
var getRep = "NXObjectManager.Get<${type}>(${var});";
var getRep2 = "NXObjectManagerEx.Get<${type}>(${var});";

var constructRegex = new Regex(@"\((?<type>[a-zA-Z0-9_.]+)\)NXObjectManager\.Construct\((?<var>.*)\);$");
var constructRep = "NXObjectManager.Construct<${type}>(${var});";
var protectConstPtrRegex = new Regex(@"protected internal (?<type>[a-zA-Z0-9_.]+)\(IntPtr ptr\)");
var protectConstPtrRep = "public ${type}(IntPtr ptr)";

var protectConstRegex=new Regex(@"protected internal (?<type>[a-zA-Z0-9_.]+)\((?<var>.*)\)");
var protectConstRep="public ${type}(${var})";
var initRegex = new Regex(@"internal new void initialize\(\)");
var initRep = "public override void initialize()";
var sizofRegex = new Regex(@"Marshal.SizeOf\(typeof\((?<type>[a-zA-Z0-9_.]+)\)\)");
var sizofRep = "Marshal.SizeOf<${type}>()";
var offsetofRegex = new Regex(@"Marshal.OffsetOf\(typeof\((?<type>[a-zA-Z0-9_.]+)\), ""(?<field>[a-zA-Z0-9_.]+)""\)");
var offsetofRep = "Marshal.OffsetOf<${type}>(\"${field}\")";
var toarrayObj1Regex = new Regex(@"\((?<type>[a-zA-Z0-9_.]+)\[\]\)JAM\.ToObjectArray\(typeof\([a-zA-Z0-9_.]+\), (?<p1>[a-zA-Z0-9_.]+), (?<p2>\w+)\)");
var toarrayObj1Rep = "JAM.ToObjectArray<${type}>(${p1}, ${p2})";
var toarrayObj1Rep2 = "JAM.ToObjectArray2<${type}>(${p1}, ${p2})";
var ptrToStructRegex = new Regex(@"\((?<type>[a-zA-Z0-9_.]+)\)Marshal.PtrToStructure\((?<p1>\w+), typeof\([a-zA-Z0-9_.]+\)\)");
var ptrToStructRep = "Marshal.PtrToStructure<${type}>(${p1})";
var enumArrayRegex=new Regex(@"\((?<type>[a-zA-Z0-9_.]+)\[\]\)JAM\.ToEnumArray\(typeof\([a-zA-Z0-9_.]+\), (?<p1>[a-zA-Z0-9_.]+), (?<p2>\w+)\)");
var enumArrayRep = "JAM.ToEnumArray<${type}>(${p1}, ${p2})";
var appDomainDirRegex = new Regex(@"AppDomain.CurrentDomain.BaseDirectory");
var appDomainDirRep = "NXOpen.Utilities.NXObjectManagerEx.GetProvider.GetRootPath();";

TraverseAllFiles(inputDir, file => {
    if (file.EndsWith(".cs"))
    {
        using var sr = new StreamReader(file);
        var newFile = file.Replace(inputDir, outputDir);
        using var sw = new StreamWriter(newFile);
        var line = sr.ReadLine();
        bool isCollection=false;
        string gType="";
        while (line != null)
        {
            var collectionMatch=collectionRegex.Match(line);
            if(collectionMatch.Success)
            {
                isCollection=true;
                gType=GetCollectionGType(file);
                if(interfaceSet.Contains(gType))
                {
                    line= $"public class {collectionMatch.Groups["type"].Value}:TaggedObjectCollection2<{gType}>";
                }
                else
                {
                    line = $"public class {collectionMatch.Groups["type"].Value}:TaggedObjectCollection<{gType}>";
                }
                arrayListRep = $"var arrayList = new System.Collections.Generic.List<{gType}>();";
            }
            if(isCollection)
            {
                line=enumeratorRgex.Replace(line,enumeratorRep);
                line=arrayListRegex.Replace(line,arrayListRep);
                line=enumeratorReturnRegex.Replace(line,enumeratorReturnRep);
            }
            var getMatch=getRegex.Match(line);
            if(getMatch.Success)
            {
                if(interfaceSet.Contains(getMatch.Groups["type"].Value))
                {
                    line=getRegex.Replace(line,getRep2);
                }
                else
                {
                    line=getRegex.Replace(line,getRep);
                }
            }
            var constructPtrMatch=protectConstPtrRegex.Match(line);
            if(constructPtrMatch.Success)
            {
                var newLine=$"    public {constructPtrMatch.Groups["type"].Value}(){{}}";
                sw.WriteLine(newLine);
                line=protectConstPtrRegex.Replace(line,protectConstPtrRep);
            }
            line=constructRegex.Replace(line,constructRep);
            line=protectConstRegex.Replace(line,protectConstRep);
            line=initRegex.Replace(line,initRep);
            line=sizofRegex.Replace(line,sizofRep);
            line=offsetofRegex.Replace(line,offsetofRep);
            var toarrayObj1Match=toarrayObj1Regex.Match(line);
            if(toarrayObj1Match.Success)
            {
                if(interfaceSet.Contains(toarrayObj1Match.Groups["type"].Value))
                {
                    line=toarrayObj1Regex.Replace(line,toarrayObj1Rep2);
                }
                else
                {
                    line=toarrayObj1Regex.Replace(line,toarrayObj1Rep);
                }
            }
            line=ptrToStructRegex.Replace(line,ptrToStructRep);
            line=enumArrayRegex.Replace(line,enumArrayRep);
            sw.WriteLine(line);
            line = sr.ReadLine();
        }
        sw.Flush();
    }
}, dir => { 
    var newDir = dir.Replace(inputDir, outputDir);
    if(!Directory.Exists(newDir))
    {
        Directory.CreateDirectory(newDir);
    }
});
var inxobj=$"{outputDir}/NXOpen/INXObject.cs";
if(File.Exists(inxobj))
{
    File.Delete(inxobj);
}
File.Copy("INXObject.cs.data",inxobj);
var session = $"{outputDir}/NXOpen/Session.cs";
if (File.Exists(session))
{
    File.Delete(session);
}
File.Copy("Session.cs.data", session);
stopWatch.Stop();
Console.WriteLine("Time: " + stopWatch.ElapsedMilliseconds + "ms");


string GetCollectionGType(string file)
{
    using var sr=new StreamReader(file);
    var gTypeLine = sr.ReadLine();
    var gType = "";
    while (gTypeLine != null)
    {
        var gTypeMatch = collectionTypeRegex.Match(gTypeLine);
        if (gTypeMatch.Success)
        {
            gType = gTypeMatch.Groups["gType"].Value;
            break;
        }
        gTypeLine = sr.ReadLine();
    }
    return gType;
}


/// <summary>
/// 对文件夹集合下的所有文件执行指定操作
/// </summary>
/// <param name="dirPath">文件夹完整路径</param>
/// <param name="fileAction">对子文件进行的操作</param>
/// <param name="dirAction">对子文件夹进行的操作</param>
public static void TraverseAllFiles(string dirPath, Action<string> fileAction, Action<string> dirAction)
{
    var dirPaths = new List<string> { dirPath };
    while (dirPaths.Count > 0)
    {
        var itemDirsList = new List<string>();
        foreach (var dir in dirPaths)
        {
            var files = Directory.GetFiles(dir);
            foreach (var itemFile in files)
            {
                fileAction?.Invoke(itemFile);
            }
            var dirs = Directory.GetDirectories(dir);
            foreach (var itemDir in dirs)
            {
                dirAction?.Invoke(itemDir);
            }
            itemDirsList = itemDirsList.Union(dirs).ToList();
        }
        dirPaths = itemDirsList;
    }
}
