/// <summary>
/// Геометрическая укладка по теореме Татта (барицентрическая укладка):
/// внешняя грань закрепляется на выпуклом многоугольнике (окружности),
/// каждая внутренняя вершина ставится в центр масс своих соседей.
/// Чтобы результат был гарантированно без пересечений и наложений,
/// в каждую внутреннюю грань добавляется вспомогательная вершина (звезда).
/// </summary>
public static class TutteLayout
{
    public static Dictionary<int, (double X, double Y)> Compute(
        HashSet<int> vs, List<int>[] adj, List<List<int>> faces)
    {
        int outer = 0;
        for (int i = 1; i < faces.Count; i++)
            if (faces[i].Count > faces[outer].Count) outer = i;

        var verts = vs.OrderBy(x => x).ToList();
        var idx = new Dictionary<int, int>();
        for (int i = 0; i < verts.Count; i++) idx[verts[i]] = i;

        int N = verts.Count;
        var nb = new List<HashSet<int>>();
        for (int i = 0; i < N; i++) nb.Add(new HashSet<int>());
        foreach (int u in vs)
            foreach (int v in adj[u])
                if (vs.Contains(v)) nb[idx[u]].Add(idx[v]);

        // Закрепляем внешнюю грань на окружности
        var fixedPos = new Dictionary<int, (double X, double Y)>();
        var of = faces[outer];
        for (int k = 0; k < of.Count; k++)
        {
            double ang = 2 * Math.PI * k / of.Count;
            fixedPos[idx[of[k]]] = (Math.Cos(ang), Math.Sin(ang));
        }

        // Вспомогательные вершины во внутренних гранях
        for (int fi = 0; fi < faces.Count; fi++)
        {
            if (fi == outer) continue;
            int d = nb.Count;
            nb.Add(new HashSet<int>());
            foreach (int v in faces[fi])
            {
                nb[d].Add(idx[v]);
                nb[idx[v]].Add(d);
            }
        }

        int total = nb.Count;
        var free = new List<int>();
        var row = new int[total];
        for (int i = 0; i < total; i++)
        {
            row[i] = -1;
            if (!fixedPos.ContainsKey(i)) { row[i] = free.Count; free.Add(i); }
        }

        int f = free.Count;
        var A = new double[f, f];
        var bx = new double[f];
        var by = new double[f];

        foreach (int i in free)
        {
            int r = row[i];
            A[r, r] = nb[i].Count;
            foreach (int j in nb[i])
            {
                if (fixedPos.TryGetValue(j, out var p)) { bx[r] += p.X; by[r] += p.Y; }
                else A[r, row[j]] -= 1;
            }
        }

        var (xs, ys) = f == 0 ? (new double[0], new double[0]) : Solve(A, bx, by, f);

        var result = new Dictionary<int, (double X, double Y)>();
        for (int i = 0; i < N; i++)
            result[verts[i]] = fixedPos.TryGetValue(i, out var p) ? p : (xs[row[i]], ys[row[i]]);
        return result;
    }

    /// <summary>
    /// Метод Гаусса с выбором главного элемента для двух правых частей сразу.
    /// </summary>
    private static (double[], double[]) Solve(double[,] A, double[] bx, double[] by, int n)
    {
        for (int c = 0; c < n; c++)
        {
            int piv = c;
            for (int r = c + 1; r < n; r++)
                if (Math.Abs(A[r, c]) > Math.Abs(A[piv, c])) piv = r;

            if (piv != c)
            {
                for (int k = 0; k < n; k++) (A[c, k], A[piv, k]) = (A[piv, k], A[c, k]);
                (bx[c], bx[piv]) = (bx[piv], bx[c]);
                (by[c], by[piv]) = (by[piv], by[c]);
            }

            for (int r = c + 1; r < n; r++)
            {
                double k = A[r, c] / A[c, c];
                if (k == 0) continue;
                for (int j = c; j < n; j++) A[r, j] -= k * A[c, j];
                bx[r] -= k * bx[c];
                by[r] -= k * by[c];
            }
        }

        var x = new double[n];
        var y = new double[n];
        for (int r = n - 1; r >= 0; r--)
        {
            double sx = bx[r], sy = by[r];
            for (int j = r + 1; j < n; j++) { sx -= A[r, j] * x[j]; sy -= A[r, j] * y[j]; }
            x[r] = sx / A[r, r];
            y[r] = sy / A[r, r];
        }
        return (x, y);
    }
}