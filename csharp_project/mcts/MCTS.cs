using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace mcts
{
    public class GameNode
    {
        public string move = "";
        public int value = 0;
        public int simulations = 0;

        public GameNode(string move)
        {
            this.move = move;
        }

        public GameNode Copy()
        {
            var new_game_node = new GameNode(move);
            new_game_node.value = value;
            new_game_node.simulations = simulations;
            return new_game_node;
        }
    }

    public class MCTS
    {
        Tree<GameNode> tree;
        XiefangChess gameModel;
        public MCTS(XiefangChess model)
        {
            var root = new Node<GameNode>(new GameNode(XiefangChess.CreateMoveCommand(XiefangPlayer.HUMAN,0,0)));
            tree = new Tree<GameNode>(root);
            gameModel = model;
        }

        public dynamic Select(XiefangChess model)
        {
            var node = tree.GetRoot();

            while (!node.IsLeaf() && IsFullyExplored(node, model))
            {
                node = GetBestChildUCB1(node);
                model.MakeMove(node.data.move);
            }

            return new { node, model };
        }

        public void Expand()
        {

        }

        public void Simulate()
        {

        }

        public void Backpropagate()
        {

        }

        public Node<GameNode> GetBestChildUCB1(Node<GameNode> node)
        {
            var nodeScores = tree.GetChildren(node).Select((f) => Tuple.Create(f, UCB1(f, node))).ToList();

            Tuple<Node<GameNode>, double> max_node = null;

            for (int i = 0 ; i < nodeScores.Count;++i)
            {
                if (max_node == null) max_node = nodeScores[i];
                if (nodeScores[i].Item2 > max_node.Item2)
                {
                    max_node = nodeScores[i];
                }
            }          
            return max_node.Item1;
        }

        public bool IsFullyExplored(Node<GameNode> node, XiefangChess model)
        {
            return GetAvailablePlays(node, model).Count == 0;
        }

        public List<Node<GameNode>> GetAvailablePlays(Node<GameNode> node, XiefangChess model)
        {
            var children = tree.GetChildren(node);

            return model.getLegalPositions().filter((pos) => {
                let explored = children.find((child) => child.data.move.position == pos);
                return !explored;
            });
        }

        public static double UCB1(Node<GameNode> node, Node<GameNode> parent)
        {
            const int c = 2;
            var exploitation = node.data.value / node.data.simulations;
            var exploration = Math.Sqrt(c * Math.Log(parent.data.simulations) / node.data.simulations);
            return exploitation + exploration;
        }
    }
}
