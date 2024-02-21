
var inputDir = @"E:\Code\nx\NXDecompile\NxOpen";
var output="interface_list.txt";
var set = new HashSet<string>();
TraverseAllFiles(inputDir, file => {
    if (file.EndsWith(".cs"))
    {
        using var sr = new StreamReader(file);
        var line = sr.ReadLine();
        while (line != null)
        {
            if (line.StartsWith("public interface "))
            {
                var newLine = line.Replace("public interface ", "");
                var name = newLine.Split(":")[0].Trim();
                set.Add(name);
            }
            line = sr.ReadLine();
        }
    }
}, dir => { });
using(var sw = new StreamWriter(output))
{
    foreach (var item in set)
    {
        sw.WriteLine(item);
    }
    sw.Flush();
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
    while (dirPaths.Count() > 0)
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
