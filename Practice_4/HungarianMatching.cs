public record MatchingResult(bool Exists, long TotalWeight, List<(int Left, int Right, long Weight)> Pairs);

public static class HungarianMatching
{
    public static MatchingResult Solve(BipartiteGraph graph, bool maximize = false)
    {
        if (graph.LeftCount != graph.RightCount)
            return new MatchingResult(false, 0, new());

        int n = graph.LeftCount;

        long sign = maximize ? -1 : 1;

        var best = new long?[n, n];
        long maxAbs = 1;    
        foreach (var (l, r, w) in graph.Edges)
        {
            long c = sign * w;
            if (!best[l, r].HasValue || c < best[l, r]!.Value)
                best[l, r] = c;
            maxAbs = Math.Max(maxAbs, Math.Abs(w));
        }

        long forbidden = 2L * n * maxAbs + 1;

        var cost = new long[n + 1, n + 1];
        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                cost[i + 1, j + 1] = best[i, j] ?? forbidden;

        int[] rowToCol = Run(cost, n);

        var pairs = new List<(int Left, int Right, long Weight)>();
        long total = 0;
        for (int i = 0; i < n; i++)
        {
            int j = rowToCol[i];
            if (!best[i, j].HasValue)
                return new MatchingResult(false, 0, new());

            long weight = sign * best[i, j]!.Value;
            pairs.Add((i, j, weight));
            total += weight;
        }

        return new MatchingResult(true, total, pairs);
    }


    private static int[] Run(long[,] a, int n)
    {
        const long Inf = long.MaxValue / 4;

        var u = new long[n + 1];
        var v = new long[n + 1];
        var p = new int[n + 1];
        var way = new int[n + 1];

        for (int i = 1; i <= n; i++)
        {
            p[0] = i;
            int j0 = 0;
            var minv = new long[n + 1];
            Array.Fill(minv, Inf);
            var used = new bool[n + 1];

            do
            {
                used[j0] = true;
                int i0 = p[j0];
                int j1 = 0;
                long delta = Inf;

                for (int j = 1; j <= n; j++)
                {
                    if (used[j]) continue;

                    long cur = a[i0, j] - u[i0] - v[j];
                    if (cur < minv[j])
                    {
                        minv[j] = cur;
                        way[j] = j0;
                    }
                    if (minv[j] < delta)
                    {
                        delta = minv[j];
                        j1 = j;
                    }
                }

                for (int j = 0; j <= n; j++)
                {
                    if (used[j])
                    {
                        u[p[j]] += delta;
                        v[j] -= delta;
                    }
                    else
                    {
                        minv[j] -= delta;
                    }
                }

                j0 = j1;
            } while (p[j0] != 0);

            do
            {
                int j1 = way[j0];
                p[j0] = p[j1];
                j0 = j1;
            } while (j0 != 0);
        }

        var rowToCol = new int[n];
        for (int j = 1; j <= n; j++)
            rowToCol[p[j] - 1] = j - 1;

        return rowToCol;
    }
}