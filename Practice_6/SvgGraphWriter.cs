using System.Globalization;
using System.Text;

/// <summary>
/// Сохранение укладки в SVG
/// </summary>
public static class SvgGraphWriter
{
    public static void Save(SimpleGraph g, Dictionary<int, (double X, double Y)> pos, string path)
    {
        const double size = 640, padding = 50, radius = 18;

        double minX = pos.Values.Min(p => p.X), maxX = pos.Values.Max(p => p.X);
        double minY = pos.Values.Min(p => p.Y), maxY = pos.Values.Max(p => p.Y);
        double span = Math.Max(Math.Max(maxX - minX, maxY - minY), 1e-9);
        double scale = size / span;

        string F(double v) => v.ToString("0.##", CultureInfo.InvariantCulture);
        double Px(int v) => padding + (pos[v].X - minX) * scale;
        double Py(int v) => padding + (pos[v].Y - minY) * scale;

        double canvas = size + 2 * padding;
        var sb = new StringBuilder();
        sb.AppendLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{F(canvas)}\" height=\"{F(canvas)}\">");
        sb.AppendLine("  <style>");
        sb.AppendLine("    .edge { stroke: #90CAF9; stroke-width: 2px; }");
        sb.AppendLine("    .node { fill: #E3F2FD; stroke: #1E88E5; stroke-width: 2px; }");
        sb.AppendLine("    .text { font-family: Arial, sans-serif; font-size: 14px; text-anchor: middle; dominant-baseline: central; fill: #0D47A1; font-weight: bold; }");
        sb.AppendLine("  </style>");

        for (int u = 0; u < g.N; u++)
            foreach (int v in g.Adj[u])
                if (u < v && pos.ContainsKey(u) && pos.ContainsKey(v))
                    sb.AppendLine($"  <line class=\"edge\" x1=\"{F(Px(u))}\" y1=\"{F(Py(u))}\" x2=\"{F(Px(v))}\" y2=\"{F(Py(v))}\" />");

        foreach (int v in pos.Keys)
        {
            sb.AppendLine($"  <circle class=\"node\" cx=\"{F(Px(v))}\" cy=\"{F(Py(v))}\" r=\"{radius}\" />");
            sb.AppendLine($"  <text class=\"text\" x=\"{F(Px(v))}\" y=\"{F(Py(v))}\">{v + 1}</text>");
        }

        sb.AppendLine("</svg>");
        File.WriteAllText(path, sb.ToString());
        Console.WriteLine($"[Успешно] SVG сохранён: {Path.GetFullPath(path)}");
    }
}