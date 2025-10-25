using Microsoft.AspNetCore.Http;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using RedBlackTree2.Models;

namespace RedBlackTree2.Extensions
{
    public static class SessionExtensions
    {
        public static void SetObject(this ISession session, string key, RedBlackTree<string> tree, string newNodeValue)
        {
            if (tree == null)
                throw new ArgumentNullException(nameof(tree));

            if (newNodeValue == null)
                newNodeValue = string.Empty;
            JsonObject treeJson = new JsonObject
            {
                ["Tree"] = SerializeNode(tree.Root),
                ["NewNodeValue"] = JsonValue.Create(newNodeValue),
                ["_nodeIdCounter"] = JsonValue.Create(tree._nodeIdCounter),
                ["InsertSteps"] = JsonSerializer.SerializeToNode(tree.InsertSteps),
                ["Quantity"] = JsonValue.Create(tree.Quantity),
                ["isSearchClicked"] = JsonValue.Create(tree.isSearchClicked),
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = treeJson.ToJsonString(options);
            session.SetString(key, json);
        }

        private static JsonNode SerializeNode<T>(RBTreeNode<T> node) where T : IComparable
        {
            if (node == null || node == RedBlackTree<T>.NIL)
                return null;

            JsonObject obj = new JsonObject
            {
                ["Value"] = JsonValue.Create(node.Value),
                ["NodeId"] = JsonValue.Create(node.NodeId),
                ["Color"] = node.Color.ToString(),
                ["Left"] = SerializeNode(node.Left),
                ["Right"] = SerializeNode(node.Right)
                
            };
            return obj;
        }
        
        public static RedBlackTree<string> GetObject(this ISession session, string key)
        {
            var json = session.GetString(key);
            if (string.IsNullOrEmpty(json))
                return new RedBlackTree<string>();

            JsonNode jsonTree = JsonNode.Parse(json);
            Console.WriteLine("geted");
            var tree = new RedBlackTree<string>
            {
                Root = DeserializeNode<string>(jsonTree["Tree"]),
                NewValue = jsonTree["NewNodeValue"]?.GetValue<string?>(),
                _nodeIdCounter = jsonTree["_nodeIdCounter"].GetValue<int>(),
                Quantity = jsonTree["Quantity"]?.GetValue<int>() ?? 0,
                InsertSteps  = jsonTree["InsertSteps"]?.Deserialize<List<InsertStep<string>>>() ?? new List<InsertStep<string>>(),
                isSearchClicked = jsonTree["isSearchClicked"]?.GetValue<bool>() ?? false
            };

            tree.FixNilReferences();
            tree.FixParentReferences();
            return tree;
        }

        private static RBTreeNode<T> DeserializeNode<T>(JsonNode node) where T : IComparable
        {
            if (node == null)
                return RedBlackTree<T>.NIL;

            T value = node["Value"].GetValue<T>();
             string NodeId  = node["NodeId"].GetValue<string>();
            string colorStr = node["Color"].GetValue<string>();
            NodeColor color = Enum.Parse<NodeColor>(colorStr);

            return new RBTreeNode<T>(value)
            {
                NodeId = NodeId,
                Color = color,
                Left = DeserializeNode<T>(node["Left"]),
                Right = DeserializeNode<T>(node["Right"])
            };
        }
    }
}
