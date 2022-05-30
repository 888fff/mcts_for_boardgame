using System;
using System.Collections.Generic;

namespace mcts
{
    class Program
    {
        static void Main(string[] args)
        {
            var tree = new Tree<int>(new Node<int>(888,0));

            tree.Insert(new Node<int>(1), tree.GetRoot());
            tree.Insert(new Node<int>(2), tree.GetRoot());
            tree.Insert(new Node<int>(3), tree.GetRoot());

            tree.Insert(new Node<int>(4), tree.Get(1));
            tree.Insert(new Node<int>(5), tree.Get(3));
            tree.Insert(new Node<int>(6), tree.Get(3));

            Tree<int>.PostorderRender(tree, tree.GetRoot());
            Console.WriteLine("");

            tree.Remove(tree.Get(6));

            Tree<int>.PostorderRender(tree, tree.GetRoot());
            Console.WriteLine("");

            tree.Remove(tree.Get(1));

            Tree<int>.PostorderRender(tree, tree.GetRoot());
            Console.WriteLine("");


            Console.ReadKey();

        }
    }
}
