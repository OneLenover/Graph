using System.Globalization;

/// dotnet run - встроенный пример (минимум)
/// dotnet run -- input.txt - граф из файла, минимальный вес
/// dotnet run -- input.txt max - граф из файла, максимальный вес
public static class Program
{
    public static void Main(string[] args)
    {
        bool maximize = args.Contains("max", StringComparer.OrdinalIgnoreCase);
        string? path = args.FirstOrDefault(a => !a.Equals("max", StringComparison.OrdinalIgnoreCase));

        BipartiteGraph graph = path != null ? ReadGraph(path) : CreateDemoGraph();

        MatchingResult result = HungarianMatching.Solve(graph, maximize);

        Console.WriteLine(maximize ? "Режим: максимальный вес" : "Режим: минимальный вес");

        if (!result.Exists)
        {
            Console.WriteLine("Полного паросочетания не существует.");
        }
        else
        {
            Console.WriteLine("Паросочетание:");
            foreach (var (l, r, w) in result.Pairs)
                Console.WriteLine($"  L{l + 1} — R{r + 1}  (вес {w})");

            Console.WriteLine($"Оптимальный вес паросочетания: {result.TotalWeight}");
        }

        BipartiteDotVisualizer.SaveDot(graph, result, "bipartite.dot");
    }

    private static BipartiteGraph ReadGraph(string path)
    {
        string[] tokens = File.ReadAllText(path).Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        int pos = 0;
        long Next() => long.Parse(tokens[pos++], CultureInfo.InvariantCulture);

        int n = (int)Next();
        int m = (int)Next();
        int k = (int)Next();

        var graph = new BipartiteGraph(n, m);
        for (int e = 0; e < k; e++)
        {
            int l = (int)Next();
            int r = (int)Next();
            long w = Next();
            graph.AddEdge(l - 1, r - 1, w);
        }

        return graph;
    }

    private static BipartiteGraph CreateDemoGraph()
    {
        int[,] w =
        {
            { 9, 2, 7 },
            { 6, 4, 3 },
            { 5, 8, 1 },
        };

        var graph = new BipartiteGraph(3, 3);
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                graph.AddEdge(i, j, w[i, j]);

        return graph;
    }
}