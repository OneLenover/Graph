public class TreeNode
{
    public int Value;
    public List<TreeNode> Children { get; } = new List<TreeNode>();

    public TreeNode(int value)
    {
        Value = value;
    }
}

public class TreeRestorer
{
    public static TreeNode? RestoreFromBinaryCode(string code)
    {
        if (string.IsNullOrEmpty(code))
            return null;

        int valueCounter = 1;
        TreeNode root = new TreeNode(valueCounter++);

        Stack<TreeNode> stack = new Stack<TreeNode>();
        stack.Push(root);

        foreach (char bit in code)
        {
            if (bit == '0')
            {
                if (stack.Count == 0) break;

                TreeNode current = stack.Peek();
                TreeNode child = new TreeNode(valueCounter++);

                current.Children.Add(child);
                stack.Push(child);
            }
            else if (bit == '1')
            {
                if (stack.Count > 0)
                {
                    stack.Pop();
                }
            }
        }

        return root;
    }
}