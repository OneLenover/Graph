public class BipartiteGraph
{
    public int LeftCount { get; }
    public int RightCount { get; }
    public List<(int Left, int Right, long Weight)> Edges { get; } = new();

    public BipartiteGraph(int leftCount, int rightCount)
    {
        if (leftCount <= 0 || rightCount <= 0)
            throw new ArgumentException("Размеры долей должны быть положительными.");

        LeftCount = leftCount;
        RightCount = rightCount;
    }

    public void AddEdge(int left, int right, long weight)
    {
        if (left < 0 || left >= LeftCount)
            throw new ArgumentOutOfRangeException(nameof(left), "Вершина левой доли вне диапазона.");
        if (right < 0 || right >= RightCount)
            throw new ArgumentOutOfRangeException(nameof(right), "Вершина правой доли вне диапазона.");

        Edges.Add((left, right, weight));
    }
}