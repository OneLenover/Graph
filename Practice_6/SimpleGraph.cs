/// <summary>
/// Простой неориентированный граф (без петель и кратных рёбер). Вершины нумеруются с 0.
/// </summary>
public class SimpleGraph
{
    public int N { get; }
    public List<int>[] Adj { get; }

    public SimpleGraph(int n)
    {
        N = n;
        Adj = new List<int>[n];
        for (int i = 0; i < n; i++) Adj[i] = new List<int>();
    }

    public void AddEdge(int u, int v)
    {
        if (u == v || Adj[u].Contains(v)) return;
        Adj[u].Add(v);
        Adj[v].Add(u);
    }

    public int EdgeCount => Adj.Sum(a => a.Count) / 2;

    public static SimpleGraph FromFile(string path)
    {
        var t = File.ReadAllText(path)
            .Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(int.Parse).ToArray();

        var g = new SimpleGraph(t[0]);
        for (int i = 0; i < t[1]; i++)
            g.AddEdge(t[2 + 2 * i] - 1, t[3 + 2 * i] - 1);
        return g;
    }
}