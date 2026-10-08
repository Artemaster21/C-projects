using System;
using System.Collections.Generic;
using System.Linq;

namespace DiskTree;

public static class DiskTreeTask
{
    public static List<string> Solve(List<string> input)
    {
        var root = new DirectoryNode(null);

        foreach (var path in input)
        {
            var parts = path.Split('\\');
            root.AddPath(parts, 0);
        }

        var result = new List<string>();
        root.Print(result, "");
        return result;
    }
}

public class DirectoryNode
{
    private readonly Dictionary<string, DirectoryNode> _children = new();
    private readonly string _name;

    public DirectoryNode(string name)
    {
        _name = name;
    }

    public void AddPath(string[] parts, int index)
    {
        if (index >= parts.Length) return;

        var currentPart = parts[index];

        if (!_children.ContainsKey(currentPart))
            _children[currentPart] = new DirectoryNode(currentPart);

        _children[currentPart].AddPath(parts, index + 1);
    }

    public void Print(List<string> result, string indent)
    {
        var sortedKeys = _children.Keys.OrderBy(k => k, StringComparer.Ordinal).ToList();

        foreach (var key in sortedKeys)
        {
            result.Add(indent + key);
            _children[key].Print(result, indent + " ");
        }
    }
}