using System.Text;

public class GraphvizVisualizer
{
    public static string ToDot(TreeNode? root)
    {
        if (root == null) return "digraph G {}";

        var sb = new StringBuilder();
        sb.AppendLine("digraph Tree {");

        void BuildDot(TreeNode node)
        {
            foreach (var child in node.Children)
            {
                sb.AppendLine($"    {node.Value} -> {child.Value};");
                BuildDot(child);
            }
        }

        BuildDot(root);
        sb.AppendLine("}");
        return sb.ToString();
    }

    public static void SaveDot(TreeNode? root, string outputDotPath)
    {
        string dotContent = ToDot(root);
        File.WriteAllText(outputDotPath, dotContent);
        Console.WriteLine($"[Успешно] Файл DOT сохранен: {Path.GetFullPath(outputDotPath)}");
    }
}