public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Использование: dotnet run -- graph.txt [out.svg]");
            return;
        }

        var g = SimpleGraph.FromFile(args[0]);
        string svgPath = args.Length > 1 ? args[1] : "planar.svg";

        Console.WriteLine($"Вершин: {g.N}, рёбер: {g.EdgeCount}");

        if (!PlanarityChecker.IsPlanar(g, out var bad))
        {
            Console.WriteLine("Граф НЕ планарен.");
            Console.WriteLine("Непланарный блок (двусвязная часть): " + string.Join(", ", bad!.Select(v => v + 1)));
            return;
        }

        Console.WriteLine("Граф планарен.");

        var blocks = PlanarityChecker.Blocks(g);
        var vs = new HashSet<int>(Enumerable.Range(0, g.N));
        bool biconnected = g.N >= 3 && blocks.Count == 1 && blocks[0].Count == g.EdgeCount;

        if (!biconnected)
        {
            Console.WriteLine("Граф не двусвязен (несвязен, есть мост или точка сочленения): рисунок не строится.");
            return;
        }

        var faces = PlanarityChecker.Embed(vs, PlanarityChecker.BlockAdjacency(g.N, blocks[0]))!;

        Console.WriteLine($"Граней: {faces.Count} (формула Эйлера: {g.N} - {g.EdgeCount} + {faces.Count} = {g.N - g.EdgeCount + faces.Count})");
        for (int i = 0; i < faces.Count; i++)
            Console.WriteLine($"  Грань {i + 1}: " + string.Join(" - ", faces[i].Select(v => v + 1)));

        var pos = TutteLayout.Compute(vs, g.Adj, faces);
        SvgGraphWriter.Save(g, pos, svgPath);
    }
}