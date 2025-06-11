var file = File.ReadAllLines("FileList.txt");
var tree = FsEntry.BuildFileTree(file);
FsEntry.PrintTree(tree);


Console.ReadKey();

public class FsEntry
{
    // name of a directory or a file
    public string Name { get; set; }

    // full path to the directory of file with a trailing slash
    public string Path { get; set; }

    // return type of the entry depending on the Path and Children
    public bool IsDirectory => Children.Count > 0;
    public bool IsFile => !IsDirectory && !IsFile;
    public bool IsShortcut => Name.EndsWith(".lnk");

    // subdirectories or files in directory
    public List<FsEntry> Children { get; set; }


    public static List<FsEntry> BuildFileTree(IEnumerable<string> paths)
    {
        var pathParts = paths
            .Select(path => path.Split('\\'))
            .ToList();

        return BuildTree(pathParts, "");
    }

    private static List<FsEntry> BuildTree(List<string[]> partsList, 
        string basePath)
    {
        return partsList.Where(p => p.Length > 0)
                        .GroupBy(p => p[0])
                        .Select(group =>
                        {
                            var name = group.Key;
                            var fullPath = basePath + name + "\\";

                            var childrenParts = group.Where(g => g.Length > 1)
                            .Select(g => g.Skip(1).ToArray()).ToList();

                            var children = BuildTree(childrenParts, fullPath);

                            return new FsEntry
                            {
                                Name = name,
                                Children = children,
                                Path = fullPath
                            };
                        }).ToList();
    }

    public static void PrintTree(List<FsEntry> entries, int indent = 0)
    {
        foreach (var entry in entries)
        {
            Console.WriteLine($"{new string(' ', indent)}- {entry.Name} " +
                $"{(entry.IsDirectory ? "[DIR]" : entry.IsShortcut ? "[LNK]" : "[FILE]")}");
            if (entry.Children != null && entry.Children.Count > 0)
                PrintTree(entry.Children, indent + 2);
        }
    }

}
