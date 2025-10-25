using System;
using Newtonsoft.Json;

namespace RedBlackTree2.Models
{
    public enum NodeColor
    {
        Red,
        Black
    }

    public class RBTreeNode<T> where T : IComparable
    {
        public T Value { get; set; }
        public NodeColor Color { get; set; }
        public RBTreeNode<T> Left { get; set; }
        public RBTreeNode<T> Right { get; set; }
        public string NodeId { get; set; } 

        [JsonIgnore]
        public RBTreeNode<T> Parent { get; set; }
        

        public RBTreeNode(T value)
        {
            Value = value;
            Color = NodeColor.Red;
        }
    }
}