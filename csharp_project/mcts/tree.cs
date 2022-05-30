using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace mcts
{
    /*
      This Tree class is re-written by the https://github.com/vgarciasc/mcts-viz       
    */

    public class Node<T>
    {
        public int id;//
        public int pid;//parent
        public List<int> cid;//children
        public T data;
        public Node(T data, int id = -1, int parent_id = -1)
        {
            this.data = data;
            this.id = id;
            this.cid = new List<int>();
            this.pid = parent_id;
        }

        public Node<T> Copy()
        {
            return new Node<T>(data, id, pid);
        }

        public bool HasNChildren(int n)
        {
            return cid.Count == n;
        }

        public bool IsLeaf()
        {
            return HasNChildren(0);
        }

        public bool IsRoot()
        {
            return id == 0;
        }
    }
    public class Tree<T>
    {
        public List<Node<T>> nodes;
        public Tree(Node<T> root)
        {
            root.id = 0;
            nodes = new List<Node<T>>();
            nodes.Add(root);
        }

        public Node<T> Get(int id)
        {
            return nodes[id];
        }

        public void Insert(Node<T> node, Node<T> parent)
        {
            node.id = nodes.Count;
            node.pid = parent.id;
            nodes.Add(node);
            nodes[node.pid].cid.Add(node.id);
        }

        public void Remove(Node<T> node)
        {
            var removed_ids = Remove_rec(node);

            nodes = nodes.Where((n, index) => n != null).ToList();

            for (var i = 0; i < nodes.Count; i++)
            {
                var removed_before_id = Removed_before(nodes[i].id, removed_ids);
                nodes[i].id -= removed_before_id;

                var removed_before_parent_id = Removed_before(nodes[i].pid, removed_ids);
                nodes[i].pid -= removed_before_parent_id;

                for (var j = 0; j < nodes[i].cid.Count; j++)
                {
                    var removed_before_children_id = Removed_before(nodes[i].cid[j], removed_ids);
                    nodes[i].cid[j] -= removed_before_children_id;
                }
            }
        }

        public List<int> Remove_rec(Node<T> node)
        {
            var removed = new List<int>();

            if (node.IsRoot()) return removed;

            var children = GetChildren(node);   //?
            for (var i = 0; i < children.Count; i++)
            {
                if (children[i] != null)
                {
                    removed.AddRange(Remove_rec(children[i]));
                }
            }

            var parent_children = GetParent(node).cid;

            var index_of_in_parent = parent_children.IndexOf(node.id);

            if (index_of_in_parent == -1) return null;

            GetParent(node).cid.RemoveAt(index_of_in_parent);

            nodes[node.id] = null;

            removed.Add(node.id);

            return removed;
        }

        public int Removed_before(int id, List<int> removed_id)
        {
            var num = 0;
            for (var i = 0; i < removed_id.Count; i++)
            {
                if (removed_id[i] < id)
                {
                    num += 1;
                }
            }
            return num;
        }

        public void Update(Node<T> node, T new_data)
        {
            nodes[node.id].data = new_data;
        }

        public Node<T> GetParent(Node<T> node)
        {
            return nodes[node.pid];
        }

        public List<Node<T>> GetChildren(Node<T> node)
        {
            if (node == null) return new List<Node<T>>();
            var arr = new List<Node<T>>();
            for (var i = 0; i < node.cid.Count; i++)
            {
                arr.Add(nodes[node.cid[i]]);
            }
            return arr;
        }

        public List<Node<T>> GetSiblings(Node<T> node)
        {
            return GetChildren(GetParent(node));
        }

        public Node<T> GetRoot()
        {
            return Get(0);
        }

        public Tree<T> copy()
        {
            var arr = new List<Node<T>>();
            for (var i = 0; i < nodes.Count; i++)
            {
                arr.Add(nodes[i].Copy());
            }
            var new_tree = new Tree<T>(arr[0]);
            new_tree.nodes = arr.GetRange(0,arr.Count);
            return new_tree;
        }

        //可视化输出
        public static void PostorderRender(Tree<T> tree , Node<T> node , int level = 1)
        {
            var children = tree.GetChildren(node);

            for (var i = 0; i < children.Count; i++)
            {
                var child = children[i];
                PostorderRender(tree, child , level + 1);
            }
            //Console.SetCursorPosition(node.id, level);
            Console.Write(node.data + " ");
        }


    }
}

