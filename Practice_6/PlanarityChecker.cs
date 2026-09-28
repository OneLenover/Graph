/// <summary>
/// Проверка планарности и построение комбинаторной укладки
/// алгоритмом Демукрона - Мальгранжа - Пуазена (гамма-алгоритм).
/// </summary>
public static class PlanarityChecker
{
    /// <summary>
    /// Блоки (компоненты двусвязности) графа; 
    /// каждый блок — список рёбер.
    /// </summary>
    public static List<List<(int, int)>> Blocks(SimpleGraph g)
    {
        int n = g.N, timer = 0;
        var disc = new int[n];
        var low = new int[n];
        Array.Fill(disc, -1);
        var stack = new Stack<(int, int)>();
        var result = new List<List<(int, int)>>();

        void Dfs(int u, int parent)
        {
            disc[u] = low[u] = timer++;
            foreach (int v in g.Adj[u])
            {
                if (v == parent) continue;
                if (disc[v] == -1)
                {
                    stack.Push((u, v));
                    Dfs(v, u);
                    low[u] = Math.Min(low[u], low[v]);
                    if (low[v] >= disc[u])
                    {
                        var comp = new List<(int, int)>();
                        (int, int) e;
                        do { e = stack.Pop(); comp.Add(e); } while (e != (u, v));
                        result.Add(comp);
                    }
                }
                else if (disc[v] < disc[u])
                {
                    stack.Push((u, v));
                    low[u] = Math.Min(low[u], disc[v]);
                }
            }
        }

        for (int s = 0; s < n; s++)
            if (disc[s] == -1) Dfs(s, -1);
        return result;
    }

    /// <summary>
    /// Планарен ли граф. Граф планарен тогда и только тогда, когда планарен каждый его блок.
    /// В failedBlock возвращается вершинный набор первого непланарного блока.
    /// </summary>
    public static bool IsPlanar(SimpleGraph g, out List<int>? failedBlock)
    {
        failedBlock = null;
        foreach (var comp in Blocks(g))
        {
            var vs = new HashSet<int>();
            foreach (var (a, b) in comp) { vs.Add(a); vs.Add(b); }
            if (vs.Count < 3) continue; // мост — всегда планарен

            var badj = BlockAdjacency(g.N, comp);
            if (Embed(vs, badj) == null)
            {
                failedBlock = vs.OrderBy(x => x).ToList();
                return false;
            }
        }
        return true;
    }

    public static List<int>[] BlockAdjacency(int n, List<(int, int)> edges)
    {
        var adj = new List<int>[n];
        for (int i = 0; i < n; i++) adj[i] = new List<int>();
        foreach (var (a, b) in edges) { adj[a].Add(b); adj[b].Add(a); }
        return adj;
    }

    private static List<int>? FindCycle(HashSet<int> vs, List<int>[] adj)
    {
        var parent = new Dictionary<int, int>();
        int start = vs.First();
        parent[start] = -1;

        List<int>? Dfs(int u)
        {
            foreach (int v in adj[u])
            {
                if (!vs.Contains(v) || v == parent[u]) continue;
                if (parent.ContainsKey(v))
                {
                    int x = u;
                    var path = new List<int> { u };
                    while (x != v && x != -1) { x = parent[x]; if (x != -1) path.Add(x); }
                    if (x == v) return path;
                }
                else
                {
                    parent[v] = u;
                    var r = Dfs(v);
                    if (r != null) return r;
                }
            }
            return null;
        }

        return Dfs(start);
    }

    private class Segment
    {
        public bool IsChord;
        public List<int> Chord = new();
        public HashSet<int> Comp = new();
        public HashSet<int> Attach = new();
    }

    /// <summary>
    /// Комбинаторная укладка двусвязного графа
    /// или null, если граф не планарен.
    /// </summary>
    public static List<List<int>>? Embed(HashSet<int> vs, List<int>[] adj)
    {
        var cycle = FindCycle(vs, adj);
        if (cycle == null) return null; // для двусвязного графа не бывает

        var inH = new HashSet<int>(cycle);
        var hEdges = new HashSet<(int, int)>();
        void AddEdge(int a, int b) => hEdges.Add((Math.Min(a, b), Math.Max(a, b)));

        for (int i = 0; i < cycle.Count; i++)
            AddEdge(cycle[i], cycle[(i + 1) % cycle.Count]);

        var faces = new List<List<int>>
        {
            new List<int>(cycle),
            cycle.AsEnumerable().Reverse().ToList()
        };

        int totalEdges = vs.Sum(u => adj[u].Count(v => vs.Contains(v))) / 2;

        while (hEdges.Count < totalEdges)
        {
            var segments = new List<Segment>();

            foreach (int u in inH)
                foreach (int v in adj[u])
                    if (vs.Contains(v) && inH.Contains(v) && u < v && !hEdges.Contains((u, v)))
                        segments.Add(new Segment { IsChord = true, Chord = new List<int> { u, v }, Attach = new HashSet<int> { u, v } });

            var seen = new HashSet<int>();
            foreach (int s in vs)
            {
                if (inH.Contains(s) || seen.Contains(s)) continue;
                var comp = new HashSet<int> { s };
                var q = new Queue<int>();
                q.Enqueue(s); seen.Add(s);
                while (q.Count > 0)
                {
                    int x = q.Dequeue();
                    foreach (int y in adj[x])
                        if (vs.Contains(y) && !inH.Contains(y) && comp.Add(y)) { seen.Add(y); q.Enqueue(y); }
                }
                var att = new HashSet<int>();
                foreach (int x in comp)
                    foreach (int y in adj[x])
                        if (inH.Contains(y)) att.Add(y);
                segments.Add(new Segment { Comp = comp, Attach = att });
            }

            Segment? bestSeg = null;
            List<int>? bestAdm = null;
            foreach (var sg in segments)
            {
                var adm = new List<int>();
                for (int i = 0; i < faces.Count; i++)
                    if (sg.Attach.IsSubsetOf(faces[i])) adm.Add(i);

                if (adm.Count == 0) return null;

                if (bestAdm == null || adm.Count < bestAdm.Count) { bestSeg = sg; bestAdm = adm; }
                if (adm.Count == 1) break;
            }

            var seg = bestSeg!;
            int faceIdx = bestAdm![0];

            List<int> path = seg.IsChord ? seg.Chord : FindPath(seg, adj, inH);
            int a = path[0], b = path[^1];
            var mid = path.GetRange(1, path.Count - 2);

            var f = faces[faceIdx];
            int m = f.Count, ia = f.IndexOf(a), ib = f.IndexOf(b);

            var w1 = new List<int>();
            for (int k = 0; k <= ((ib - ia) % m + m) % m; k++) w1.Add(f[(ia + k) % m]);
            for (int k = mid.Count - 1; k >= 0; k--) w1.Add(mid[k]);

            var w2 = new List<int>();
            for (int k = 0; k <= ((ia - ib) % m + m) % m; k++) w2.Add(f[(ib + k) % m]);
            w2.AddRange(mid);

            faces[faceIdx] = w1;
            faces.Add(w2);

            for (int i = 0; i + 1 < path.Count; i++) AddEdge(path[i], path[i + 1]);
            foreach (int v in mid) inH.Add(v);
        }

        return faces;
    }

    private static List<int> FindPath(Segment seg, List<int>[] adj, HashSet<int> inH)
    {
        int a = seg.Attach.First();
        foreach (int u in adj[a])
        {
            if (!seg.Comp.Contains(u)) continue;

            var prev = new Dictionary<int, int> { [u] = -1 };
            var q = new Queue<int>();
            q.Enqueue(u);

            while (q.Count > 0)
            {
                int w = q.Dequeue();

                foreach (int b in adj[w])
                {
                    if (inH.Contains(b) && b != a)
                    {
                        var mid = new List<int>();
                        for (int x = w; x != -1; x = prev[x]) mid.Add(x);
                        mid.Reverse();

                        var path = new List<int> { a };
                        path.AddRange(mid);
                        path.Add(b);
                        return path;
                    }
                }

                foreach (int y in adj[w])
                    if (seg.Comp.Contains(y) && !prev.ContainsKey(y)) { prev[y] = w; q.Enqueue(y); }
            }
        }
        throw new InvalidOperationException("Не найдена цепь сегмента (граф должен быть двусвязным).");
    }
}