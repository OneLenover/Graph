using System.Diagnostics;

internal class Program
{
    static void Main(string[] args)
    {
        string code = "00101011001101010101"; 
        TreeNode? root = TreeRestorer.RestoreFromBinaryCode(code);

        GraphvizVisualizer.SaveDot(root, "tree.dot");

        SvgTreeVisualizer.SaveSvg(root, "tree_custom.svg");
    }
}