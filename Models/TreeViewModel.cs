using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RedBlackTree2.Models
{
    public class TreeViewModel
    {
        public TreeNodeModel Root { get; set; }
        public List<TreeNodeModel> TreeStates { get; set; }

        public string InputValue { get; set; }

        public int Quantity { get; set; }

        public bool isSearchClicked { get; set; }

        public List<InsertStep<string>> InsertSteps { get; set; } = new(); // ✅ Ավելացված քայլերը

        public static TreeViewModel FromRBTree(RedBlackTree<string> tree)
        {
            if (tree == null || tree.Root == null)
            {
                return new TreeViewModel { Root = null };
            }

            return new TreeViewModel
            {
                Root = ConvertNode(tree.Root),
                InsertSteps = tree.InsertSteps, // ✅ Լցնել քայլերը
                InputValue = tree.NewValue,
                Quantity = tree.Quantity,
                isSearchClicked = tree.isSearchClicked
            };
        }

        private static TreeNodeModel ConvertNode(RBTreeNode<string> node)
        {
            if (node == null || node == RedBlackTree<string>.NIL) return null;
            return new TreeNodeModel
            {
                NodeId = node.NodeId,
                Value = node.Value,
                Color = node.Color.ToString(),
                Left = ConvertNode(node.Left),
                Right = ConvertNode(node.Right),
            };
        }
    }

    public class TreeNodeModel
    {
        public string NodeId { get; set; }
        public string Value { get; set; }
        public string Color { get; set; }
        public TreeNodeModel Left { get; set; }
        public TreeNodeModel Right { get; set; }
    }

    
}