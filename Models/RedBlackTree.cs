using System;
using System.Text.Json.Serialization;
namespace RedBlackTree2.Models
{
 
    public class RedBlackTree<T> where T : IComparable
    {
        public List<InsertStep<T>> InsertSteps { get; set; } = new List<InsertStep<T>>();
        public RBTreeNode<T> Root { get;  set; }
        public string NewValue { get; set; } 
       
        public int Quantity { get;  set; }
        public bool isSearchClicked { get; set; }
        [JsonIgnore]
        public static readonly RBTreeNode<T> NIL = new RBTreeNode<T>(default(T)) { Color = NodeColor.Black };

        public int _nodeIdCounter { get; set; } 

        public RedBlackTree()
        {
            _nodeIdCounter = 0;
            Root = NIL;
            isSearchClicked = false;
           
        }
        private string GenerateNodeId()
        {
            Console.WriteLine(_nodeIdCounter + "id");
            return $"node_{_nodeIdCounter++}";
          
        }
        int CompareWords(string word1, string word2)
        {
            if (double.TryParse(word1, out double num1) && double.TryParse(word2, out double num2))
            {
                return num1.CompareTo(num2);
            }
            return string.Compare(word1, word2, StringComparison.Ordinal);
        }
        
        public void Insert(T value)
        {
            isSearchClicked = false;
            Quantity++;
            InsertSteps.Clear();
            
            if (Quantity > 14)
            {
                NewValue = value.ToString();
            }
            else
            {
                NewValue = "";
            }
    
            var newNode = new RBTreeNode<T>(value)
            {
                Left = NIL,
                Right = NIL,
                Parent = NIL,
                NodeId = GenerateNodeId()
            };
    
            RBTreeNode<T> y = NIL;
            RBTreeNode<T> x = Root;
    
            while (x != NIL)
            {
                y = x;
                if (CompareWords(value.ToString(), x.Value.ToString()) < 0)
                    x = x.Left;
                else
                    x = x.Right;
            }
    
            newNode.Parent = y;
    
            if (y == NIL)
            {
                Root = newNode;
            }
            else if (CompareWords(value.ToString(), y.Value.ToString()) < 0)
                y.Left = newNode;
            else
                y.Right = newNode;
           
            newNode.Color = NodeColor.Red;
            InsertSteps.Add(new InsertStep<T>
            {
                Action = "AddNode",
                NodeValue = value,
                NodeId = newNode.NodeId,
                Color = "Red",
                TreeState = GetCurrentTreeState()
            });
            FixInsert(newNode);
        }

        
        private void RotateLeft(RBTreeNode<T> node)
        {
          
            InsertSteps.Add(new InsertStep<T>
            {
                NodeId = node.NodeId,
                Action = "BeforeRotateLeft",
                NodeValue = node.Value , 
                TreeState = GetCurrentTreeState()
            });
            RBTreeNode<T> rightChild = node.Right;
            node.Right = rightChild.Left;
            if (rightChild.Left != NIL)
                rightChild.Left.Parent = node;
            rightChild.Parent = node.Parent;
            if (node.Parent == NIL)
                Root = rightChild;
            else if (node == node.Parent.Left)
                node.Parent.Left = rightChild;
            else
                node.Parent.Right = rightChild;
            rightChild.Left = node;
            node.Parent = rightChild;
            InsertSteps.Add(new InsertStep<T>
            {
                NodeId = node.NodeId,
                Action = "AfterRotateLeft",
                NodeValue = node.Value , 
                TreeState = GetCurrentTreeState()
            });
        }

        private void RotateRight(RBTreeNode<T> node)
        {
            InsertSteps.Add(new InsertStep<T>
            {
                NodeId = node.NodeId,
                Action = "BeforeRotateRight",
                NodeValue = node.Value,
                TreeState = GetCurrentTreeState()
            });
            RBTreeNode<T> leftChild = node.Left;
            node.Left = leftChild.Right;
            if (leftChild.Right != NIL)
                leftChild.Right.Parent = node;
            leftChild.Parent = node.Parent;
            if (node.Parent == NIL)
                Root = leftChild;
            else if (node == node.Parent.Right)
                node.Parent.Right = leftChild;
            else
                node.Parent.Left = leftChild;
            leftChild.Right = node;
            node.Parent = leftChild;
            InsertSteps.Add(new InsertStep<T>
            {
                NodeId = node.NodeId,
                Action = "AfterRotateRight",
                NodeValue = node.Value,
                TreeState = GetCurrentTreeState()
            });
        }
        
        private void FixInsert(RBTreeNode<T> node)
        {
            while (node != NIL && node.Parent != null && node.Parent.Color == NodeColor.Red)
            {
                var parent = node.Parent;
                var grandparent = parent.Parent;

                if (grandparent == null || grandparent == NIL)
                    break;

                bool isLeft = (parent == grandparent.Left);
                var uncle = isLeft ? grandparent.Right : grandparent.Left;

                if (uncle == null) uncle = NIL;

                if (uncle.Color == NodeColor.Red)
                {
                    parent.Color = NodeColor.Black;
                    LogColorChange(parent, NodeColor.Black);
            
                    uncle.Color = NodeColor.Black;
                    LogColorChange(uncle, NodeColor.Black);
            
                    grandparent.Color = NodeColor.Red;
                    LogColorChange(grandparent, NodeColor.Red);
            
                    node = grandparent;
                }
                else
                {
                    if (isLeft && node == parent.Right)
                    {
                        node = parent;
                        RotateLeft(node);
                    }
                    else if (!isLeft && node == parent.Left)
                    {
                        node = parent;
                        RotateRight(node);
                    }

                    parent = node.Parent;
                    grandparent = parent?.Parent;

                    if (parent != null)
                    {
                        parent.Color = NodeColor.Black;
                        LogColorChange(parent, NodeColor.Black);
                    }
            
                    if (grandparent != null)
                    {
                        grandparent.Color = NodeColor.Red;
                        LogColorChange(grandparent, NodeColor.Red);
                    }

                    if (isLeft && grandparent != null)
                        RotateRight(grandparent);
                    else if (grandparent != null)
                        RotateLeft(grandparent);
                }

                if (node == Root)
                    break;
            }

            if ( Root.Color!= NodeColor.Black)
            {
                Root.Color = NodeColor.Black;
                LogColorChange(Root, NodeColor.Black);
            }
            
           
        }

        
        public bool Delete(T value)
        {
            InsertSteps.Clear();
            if (Quantity < 14)
            {
                NewValue = "";
            }

          
            InsertSteps.Add(new InsertStep<T>
            {
                Action = "BeginDelete",
                NodeValue = value,
                NodeId = Root.NodeId,
                TreeState = GetCurrentTreeState(),
            });
         RBTreeNode<T> node = Search(value, isExternalCall: false);

            if (node == NIL|| node == null)
            {
                return false;
            }
            
            DeleteNode(node);
            Quantity--;
            isSearchClicked = false;
            InsertSteps.Add(new InsertStep<T>
            {
                Action = "DeleteComplete",
                NodeValue = value,
                TreeState = GetCurrentTreeState()
            });
            
            return true;
        }
        
        private void DeleteNode(RBTreeNode<T> z)
        {

            RBTreeNode<T> y = z;
            NodeColor yOriginalColor = y.Color;
            RBTreeNode<T> x;
           
            if (z.Left == NIL)
            {
                x = z.Right;
                InsertSteps.Add(new InsertStep<T>
                {
                    Action = "DeleteCaseLeftNil",
                    NodeId = z.NodeId,
                    NodeValue = z.Value,
                    TreeState = GetCurrentTreeState()
                });
                Transplant(z, z.Right);
            }
            else if (z.Right == NIL)
            {
                x = z.Left;
                InsertSteps.Add(new InsertStep<T>
                {
                    Action = "DeleteCaseRightNil",
                    NodeId = z.NodeId,
                    NodeValue = z.Value,
                    TreeState = GetCurrentTreeState()
                });
                Transplant(z, z.Left);
            }
            else
            {
                y = Minimum(z.Right);
                InsertSteps.Add(new InsertStep<T>
                {
                    Action = "FindSuccessor",
                    NodeId = y.NodeId,
                    NodeValue = y.Value,
                    TreeState = GetCurrentTreeState()
                });
                
                yOriginalColor = y.Color;
                x = y.Right;
                if (y.Parent == z)
                {
                    x.Parent = y;
                    InsertSteps.Add(new InsertStep<T>
                    {
                        Action = "SuccessorIsDirectChild",
                        NodeId = y.NodeId,
                        NodeValue = y.Value,
                        TreeState = GetCurrentTreeState()
                    });
                }
                else
                {
                    InsertSteps.Add(new InsertStep<T>
                    {
                        Action = "BeforeTransplantSuccessor",
                        NodeId = y.NodeId,
                        NodeValue = y.Value,
                        TreeState = GetCurrentTreeState()
                    });
                    
                    Transplant(y, y.Right);
                    y.Right = z.Right;
                    y.Right.Parent = y;
                    
                    InsertSteps.Add(new InsertStep<T>
                    {
                        Action = "AfterTransplantSuccessor",
                        NodeId = y.NodeId,
                        NodeValue = y.Value,
                        TreeState = GetCurrentTreeState()
                    });
                }
                
                InsertSteps.Add(new InsertStep<T>
                {
                    Action = "BeforeReplaceWithSuccessor",
                    NodeId = z.NodeId,
                    NodeValue = z.Value,
                    TreeState = GetCurrentTreeState()
                });
                
                Transplant(z, y);
                y.Left = z.Left;
                y.Left.Parent = y;
                y.Color = z.Color;
                
                InsertSteps.Add(new InsertStep<T>
                {
                    Action = "AfterReplaceWithSuccessor",
                    NodeId = y.NodeId,
                    NodeValue = y.Value,
                    TreeState = GetCurrentTreeState()
                });
            }
            
            if (yOriginalColor == NodeColor.Black)
            {
                InsertSteps.Add(new InsertStep<T>
                {
                    Action = "BeginFixDelete",
                    NodeValue = x != NIL ? x.Value : default(T),
                    NodeId = x != NIL ? x.NodeId : "NIL",
                    TreeState = GetCurrentTreeState()
                });
                
                FixDelete(x);
            }
        }
        
        private void Transplant(RBTreeNode<T> u, RBTreeNode<T> v)
        {
            InsertSteps.Add(new InsertStep<T>
            {
                Action = "BeforeTransplant",
                NodeId = u.NodeId,
                NodeValue = u.Value,
                TreeState = GetCurrentTreeState()
            });
            
            if (u.Parent == NIL)
                Root = v;
            else if (u == u.Parent.Left)
                u.Parent.Left = v;
            else
                u.Parent.Right = v;
            v.Parent = u.Parent;
            
            InsertSteps.Add(new InsertStep<T>
            {
                Action = "AfterTransplant",
                NodeId = v != NIL ? v.NodeId : "NIL",
                NodeValue = v != NIL ? v.Value : default(T),
                TreeState = GetCurrentTreeState()
            });
        }
        
        private void FixDelete(RBTreeNode<T> x)
        {
            while (x != Root && x.Color == NodeColor.Black)
            {
                
                if (x == x.Parent.Left)
                {
                    RBTreeNode<T> w = x.Parent.Right;
                    if (w.Color == NodeColor.Red)
                    {
                        InsertSteps.Add(new InsertStep<T>
                        {
                            Action = "FixDeleteCase1",
                            NodeId = w.NodeId,
                            NodeValue = w.Value,
                            TreeState = GetCurrentTreeState()
                        });
                        
                        w.Color = NodeColor.Black;
                        LogColorChange(w, NodeColor.Black);
                        
                        x.Parent.Color = NodeColor.Red;
                        LogColorChange(x.Parent, NodeColor.Red);
                        
                        RotateLeft(x.Parent);
                        w = x.Parent.Right;
                    }
                    if (w.Left.Color == NodeColor.Black && w.Right.Color == NodeColor.Black)
                    {
                        InsertSteps.Add(new InsertStep<T>
                        {
                            Action = "FixDeleteCase2",
                            NodeId = w.NodeId,
                            NodeValue = w.Value,
                            TreeState = GetCurrentTreeState()
                        });
                        
                        w.Color = NodeColor.Red;
                        LogColorChange(w, NodeColor.Red);
                        
                        x = x.Parent;
                    }
                    else
                    {
                        if (w.Right.Color == NodeColor.Black)
                        {
                            InsertSteps.Add(new InsertStep<T>
                            {
                                Action = "FixDeleteCase3",
                                NodeId = w.NodeId,
                                NodeValue = w.Value,
                                TreeState = GetCurrentTreeState()
                            });
                            
                            w.Left.Color = NodeColor.Black;
                            LogColorChange(w.Left, NodeColor.Black);
                            
                            w.Color = NodeColor.Red;
                            LogColorChange(w, NodeColor.Red);
                            
                            RotateRight(w);
                            w = x.Parent.Right;
                        }
                        
                        InsertSteps.Add(new InsertStep<T>
                        {
                            Action = "FixDeleteCase4",
                            NodeId = w.NodeId,
                            NodeValue = w.Value,
                            TreeState = GetCurrentTreeState()
                        });
                        
                        w.Color = x.Parent.Color;
                        LogColorChange(w, w.Color);
                        
                        x.Parent.Color = NodeColor.Black;
                        LogColorChange(x.Parent, NodeColor.Black);
                        
                        w.Right.Color = NodeColor.Black;
                        LogColorChange(w.Right, NodeColor.Black);
                        
                        RotateLeft(x.Parent);
                        x = Root;
                    }
                }
                else
                {
                    RBTreeNode<T> w = x.Parent.Left;
                    if (w.Color == NodeColor.Red)
                    {
                        InsertSteps.Add(new InsertStep<T>
                        {
                            Action = "FixDeleteCase5",
                            NodeId = w.NodeId,
                            NodeValue = w.Value,
                            TreeState = GetCurrentTreeState()
                        });
                        
                        w.Color = NodeColor.Black;
                        LogColorChange(w, NodeColor.Black);
                        
                        x.Parent.Color = NodeColor.Red;
                        LogColorChange(x.Parent, NodeColor.Red);
                        
                        RotateRight(x.Parent);
                        w = x.Parent.Left;
                    }
                    if (w.Right.Color == NodeColor.Black && w.Left.Color == NodeColor.Black)
                    {
                        InsertSteps.Add(new InsertStep<T>
                        {
                            Action = "FixDeleteCase6",
                            NodeId = w.NodeId,
                            NodeValue = w.Value,
                            TreeState = GetCurrentTreeState()
                        });
                        
                        w.Color = NodeColor.Red;
                        LogColorChange(w, NodeColor.Red);
                        
                        x = x.Parent;
                    }
                    else
                    {
                        if (w.Left.Color == NodeColor.Black)
                        {
                            InsertSteps.Add(new InsertStep<T>
                            {
                                Action = "FixDeleteCase7",
                                NodeId = w.NodeId,
                                NodeValue = w.Value,
                                TreeState = GetCurrentTreeState()
                            });
                            
                            w.Right.Color = NodeColor.Black;
                            LogColorChange(w.Right, NodeColor.Black);
                            
                            w.Color = NodeColor.Red;
                            LogColorChange(w, NodeColor.Red);
                            
                            RotateLeft(w);
                            w = x.Parent.Left;
                        }
                        
                        InsertSteps.Add(new InsertStep<T>
                        {
                            Action = "FixDeleteCase8",
                            NodeId = w.NodeId,
                            NodeValue = w.Value,
                            TreeState = GetCurrentTreeState()
                        });
                        
                        w.Color = x.Parent.Color;
                        LogColorChange(w, w.Color);
                        
                        x.Parent.Color = NodeColor.Black;
                        LogColorChange(x.Parent, NodeColor.Black);
                        
                        w.Left.Color = NodeColor.Black;
                        LogColorChange(w.Left, NodeColor.Black);
                        
                        RotateRight(x.Parent);
                        x = Root;
                    }
                }
            }
            x.Color = NodeColor.Black;
            LogColorChange(x, NodeColor.Black);
            
        }

        // The original Search method that clears InsertSteps
        public RBTreeNode<T> Search(T value)
        {
            InsertSteps.Clear();
            isSearchClicked = true;
            
            InsertSteps.Add(new InsertStep<T>
            {
                Action = "BeginSearch",
                NodeValue = value,
                TreeState = GetCurrentTreeState()
            });
            
            return PerformSearch(value);
        }
        
        private RBTreeNode<T> Search(T value, bool isExternalCall)
        {
            if (isExternalCall)
            {
                InsertSteps.Clear();
                isSearchClicked = true;

                InsertSteps.Add(new InsertStep<T>
                {
                    Action = "BeginSearch",
                    NodeValue = value,
                    TreeState = GetCurrentTreeState()
                });
            }
            

            
            return PerformSearch(value);
        }
        
        private RBTreeNode<T> PerformSearch(T value)
        {
            RBTreeNode<T> current = Root;
            int steps = 0;
            
            while (current != NIL)
            {
                steps++;
                
                InsertSteps.Add(new InsertStep<T>
                {
                    Action = "SearchStep",
                    NodeId = current.NodeId,
                    NodeValue = current.Value,
                    TreeState = GetCurrentTreeState()
                });
                int cmp = CompareWords(value.ToString(), current.Value.ToString());
                if (cmp == 0)
                {
                    NewValue = current.Value.ToString();
                    
                    InsertSteps.Add(new InsertStep<T>
                    {
                        Action = "SearchFound",
                        NodeId = current.NodeId,
                        NodeValue = current.Value,
                        TreeState = GetCurrentTreeState()
                    });
                    
                    return current;
                }
                  
                if (cmp < 0)
                {
                    InsertSteps.Add(new InsertStep<T>
                    {
                        Action = "SearchGoLeft",
                        NodeId = current.NodeId,
                        NodeValue = current.Value,
                        TreeState = GetCurrentTreeState()
                    });
                    
                    current = current.Left;
                }
                else
                {
                    InsertSteps.Add(new InsertStep<T>
                    {
                        Action = "SearchGoRight",
                        NodeId = current.NodeId,
                        NodeValue = current.Value,
                        TreeState = GetCurrentTreeState()
                    });
                    
                    current = current.Right;
                }
            }
            
            InsertSteps.Add(new InsertStep<T>
            {
                Action = "SearchNotFound",
                NodeValue = value,
                TreeState = GetCurrentTreeState()
            });
            Console.WriteLine("Search result: " + (current == NIL ? "NIL" : current.Value.ToString()));

            return NIL;
        }
        
        public int GetHeight(RBTreeNode<T> node)
        {
            if (node == NIL)
                return 0;
            int leftHeight = GetHeight(node.Left);
            int rightHeight = GetHeight(node.Right);
        
            return 1 + Math.Max(leftHeight, rightHeight);
        }
        
        public RBTreeNode<T> Minimum(RBTreeNode<T> node)
        {
            InsertSteps.Add(new InsertStep<T>
            {
                Action = "FindMinimumStart",
                NodeId = node != NIL ? node.NodeId : "NIL",
                NodeValue = node != NIL ? node.Value : default(T),
                TreeState = GetCurrentTreeState()
            });
            
            RBTreeNode<T> current = node;
            while (current.Left != NIL)
            {
                InsertSteps.Add(new InsertStep<T>
                {
                    Action = "FindMinimumStep",
                    NodeId = current.NodeId,
                    NodeValue = current.Value,
                    TreeState = GetCurrentTreeState()
                });
                
                current = current.Left;
            }
            
            
            return current;
        }
        
        public RBTreeNode<T> Maximum(RBTreeNode<T> node)
        {
            InsertSteps.Add(new InsertStep<T>
            {
                Action = "FindMaximumStart",
                NodeId = node != NIL ? node.NodeId : "NIL",
                NodeValue = node != NIL ? node.Value : default(T),
                TreeState = GetCurrentTreeState()
            });
            
            RBTreeNode<T> current = node;
            while (current.Right != NIL)
            {
                InsertSteps.Add(new InsertStep<T>
                {
                    Action = "FindMaximumStep",
                    NodeId = current.NodeId,
                    NodeValue = current.Value,
                    TreeState = GetCurrentTreeState()
                });
                
                current = current.Right;
            }
            
            InsertSteps.Add(new InsertStep<T>
            {
                Action = "FindMaximumComplete",
                NodeId = current.NodeId,
                NodeValue = current.Value,
                TreeState = GetCurrentTreeState()
            });
            
            return current;
        }
        
        public void FixNilReferences()
        {
            FixNilReferencesHelper(Root);
        }
        
        public void FixParentReferences()
        {
            FixParentReferencesHelper(Root, NIL);
        }
        
        private void FixNilReferencesHelper(RBTreeNode<T> node)
        {
            if (node == null || node == NIL)
                return;
            
            if (node.Left == null)
                node.Left = NIL;
            
            if (node.Right == null)
                node.Right = NIL;
            
            FixNilReferencesHelper(node.Left);
            FixNilReferencesHelper(node.Right);
        }
        
        private void FixParentReferencesHelper(RBTreeNode<T> node, RBTreeNode<T> parent)
        {
            if (node == null || node == NIL)
                return;
            node.Parent = parent;
            FixParentReferencesHelper(node.Left, node);
            FixParentReferencesHelper(node.Right, node);
        }
        private void LogColorChange(RBTreeNode<T> node, NodeColor newColor)
        {
            InsertSteps.Add(new InsertStep<T>
            {
                NodeId = node.NodeId,
                Action = "ColorChange",
                NodeValue = node.Value,
                Color = newColor.ToString(),
                TreeState = GetCurrentTreeState()
            });
        }
        private RBTreeNode<T> CloneTree(RBTreeNode<T> node)
        {
            if (node == null)
                return null;
    
            if (ReferenceEquals(node, NIL))
            {
                return new RBTreeNode<T>(default(T)) { Color = NodeColor.Black };
            }
    
            
            var clone = new RBTreeNode<T>(node.Value)
            {
                NodeId = node.NodeId,
                Color = node.Color,
                Left = CloneTree(node.Left),
                Right = CloneTree(node.Right)
            };
    
            return clone;
        }

        private TreeNodeModel GetCurrentTreeState()
        {
            return ConvertNode(Root);
        }

        private TreeNodeModel ConvertNode(RBTreeNode<T> node)
        {
            if (node == null) return null;
            if (node == NIL ) return null;
    
            return new TreeNodeModel
            {
                NodeId = node.NodeId,
                Value = node.Value?.ToString(),
                Color = node.Color.ToString(),
                Left = ConvertNode(node.Left),
                Right = ConvertNode(node.Right)
            };
        }
    }
}