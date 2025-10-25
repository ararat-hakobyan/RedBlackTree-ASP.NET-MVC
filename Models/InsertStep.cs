using RedBlackTree2.Models;

public class InsertStep<T> where T : IComparable
{
    public string Action { get; set; } = string.Empty;
    public T NodeValue { get; set; }
    public string NodeId { get; set; } 
    public TreeNodeModel TreeState { get; set; }
    public string Color { get; set; }
}