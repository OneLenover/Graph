using System.Text;

public class SvgTreeVisualizer
{
    public static void SaveSvg(TreeNode? root, string outputPath)
    {
        if (root == null) return;

        var positions = new Dictionary<TreeNode, (double X, double Y)>();
        double nextLeafX = 0;
        CalculatePositions(root, 0, positions, ref nextLeafX);

        double spacingX = 60;  // Расстояние между узлами по горизонтали
        double spacingY = 70;  // Расстояние между уровнями по вертикали
        double padding = 50;   // Отступы от краев
        double radius = 20;    // Радиус узла

        // Расчет итогового размера холста
        double width = (positions.Values.Max(p => p.X) + 1) * spacingX + padding * 2;
        double height = (positions.Values.Max(p => p.Y) + 1) * spacingY + padding * 2;

        var sb = new StringBuilder();
        sb.AppendLine($"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"{width}\" height=\"{height}\">");
        
        // Стили
        sb.AppendLine("  <style>");
        sb.AppendLine("    .edge { stroke: #90CAF9; stroke-width: 2px; }");
        sb.AppendLine("    .node { fill: #E3F2FD; stroke: #1E88E5; stroke-width: 2px; }");
        sb.AppendLine("    .text { font-family: Arial, sans-serif; font-size: 14px; text-anchor: middle; dominant-baseline: central; fill: #0D47A1; font-weight: bold; }");
        sb.AppendLine("  </style>");

        DrawEdges(root, positions, sb, padding, spacingX, spacingY);

        DrawNodes(positions, sb, padding, spacingX, spacingY, radius);

        sb.AppendLine("</svg>");

        File.WriteAllText(outputPath, sb.ToString());
        Console.WriteLine($"[Успешно] SVG визуализация сохранена в: {Path.GetFullPath(outputPath)}");
    }

    private static double CalculatePositions(TreeNode node, int depth, Dictionary<TreeNode, (double X, double Y)> positions, ref double nextLeafX)
    {
        if (node.Children.Count == 0)
        {
            double x = nextLeafX;
            nextLeafX += 1.0;
            positions[node] = (x, depth);
            return x;
        }

        double sumX = 0;
        foreach (var child in node.Children)
        {
            sumX += CalculatePositions(child, depth + 1, positions, ref nextLeafX);
        }

        double parentX = sumX / node.Children.Count;
        positions[node] = (parentX, depth);
        return parentX;
    }

    private static void DrawEdges(TreeNode node, Dictionary<TreeNode, (double X, double Y)> positions, StringBuilder sb, double padding, double spacingX, double spacingY)
    {
        var parentPos = positions[node];
        double px = padding + parentPos.X * spacingX;
        double py = padding + parentPos.Y * spacingY;

        foreach (var child in node.Children)
        {
            var childPos = positions[child];
            double cx = padding + childPos.X * spacingX;
            double cy = padding + childPos.Y * spacingY;

            sb.AppendLine($"  <line class=\"edge\" x1=\"{px}\" y1=\"{py}\" x2=\"{cx}\" y2=\"{cy}\" />");
            DrawEdges(child, positions, sb, padding, spacingX, spacingY);
        }
    }

    private static void DrawNodes(Dictionary<TreeNode, (double X, double Y)> positions, StringBuilder sb, double padding, double spacingX, double spacingY, double radius)
    {
        foreach (var kvp in positions)
        {
            TreeNode node = kvp.Key;
            double px = padding + kvp.Value.X * spacingX;
            double py = padding + kvp.Value.Y * spacingY;

            sb.AppendLine($"  <circle class=\"node\" cx=\"{px}\" cy=\"{py}\" r=\"{radius}\" />");
            sb.AppendLine($"  <text class=\"text\" x=\"{px}\" y=\"{py}\">{node.Value}</text>");
        }
    }
}