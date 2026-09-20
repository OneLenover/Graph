using System.Text;

public static class BipartiteDotVisualizer
{
    public static string ToDot(BipartiteGraph graph, MatchingResult? result = null)
    {
        var chosen = new Dictionary<(int, int), long>();
        if (result is { Exists: true })
        {
            foreach (var pair in result.Pairs)
                chosen[(pair.Left, pair.Right)] = pair.Weight;
        }

        var sb = new StringBuilder();
        sb.AppendLine("graph Bipartite {");
        sb.AppendLine("    rankdir=LR;");
        sb.AppendLine("    node [shape=circle];");

        sb.Append("    { rank=same;");
        for (int i = 0; i < graph.LeftCount; i++) sb.Append($" L{i + 1};");
        sb.AppendLine(" }");

        sb.Append("    { rank=same;");
        for (int j = 0; j < graph.RightCount; j++) sb.Append($" R{j + 1};");
        sb.AppendLine(" }");

        foreach (var (l, r, w) in graph.Edges)
        {
            bool inMatching = chosen.TryGetValue((l, r), out long chosenWeight) && chosenWeight == w;
            string style = inMatching ? ", color=red, penwidth=2.5" : "";
            sb.AppendLine($"    L{l + 1} -- R{r + 1} [label=\"{w}\"{style}];");
        }

        sb.AppendLine("}");
        return sb.ToString();
    }

    public static void SaveDot(BipartiteGraph graph, MatchingResult? result, string outputDotPath)
    {
        File.WriteAllText(outputDotPath, ToDot(graph, result));
        Console.WriteLine($"[Успешно] Файл DOT сохранен: {Path.GetFullPath(outputDotPath)}");
    }
}