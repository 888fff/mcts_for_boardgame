using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace mcts
{
    public class XiefangChess
    {
        static char[] SEP = new char[] { '_', ' ', ',' };

        Random rand = new Random(6666);
        public XiefangBoard board = new XiefangBoard();
        public List<string> memory = new List<string>();
        public void Reset(List<int> boardState = null)
        {
            board.Reset(boardState);
        }
        //这个只是为了mcts用
        public XiefangChess Copy()
        {
            XiefangChess c = new XiefangChess();
            c.Reset(board.boardNodes);
            return c;
        }
        //这里缺少一个 重复棋的判断
        public bool HumanMakeMove(int from, int to)
        {
            bool r = false;
            if (board.boardNodes[from] == (int)XiefangPlayer.HUMAN)
            {
                r = board.MovePiece(from, to);
                if (r) memory.Add(CreateMoveCommand(XiefangPlayer.HUMAN, from, to));
            }
            return r;
        }
        public bool MachineMakeMove(int from, int to)
        {
            bool r = false;

            if (board.boardNodes[from] == (int)XiefangPlayer.MACHINE)
            {
                r = board.MovePiece(from, to);
                if (r) memory.Add(CreateMoveCommand(XiefangPlayer.MACHINE, from, to));
            }
            return r;

        }
        public bool MakeMove(string command)   /* h_0_1 */
        {
            var items = command.Split(SEP);
            bool ret = false;
            switch (items[0])
            {
                case "h":
                    { ret = HumanMakeMove(Convert.ToInt32(items[1]), Convert.ToInt32(items[2])); break; }
                case "m":
                    { ret = MachineMakeMove(Convert.ToInt32(items[1]), Convert.ToInt32(items[2])); break; }
                case "r":
                    { Reset(null); ret = true; break; }
                default:
                    break;
            }
            return ret;
        }

        public XiefangPlayer GetOpponent(XiefangPlayer p)
        {
            return p == XiefangPlayer.HUMAN ? XiefangPlayer.MACHINE : XiefangPlayer.HUMAN;
        }

        //检查当前这个玩家是否成功，意味着对手没有可移动的点,如果没有可移动的点，若对手失败了，则被检查的玩家成功
        public bool CheckWin(XiefangPlayer p)
        {
            var nodes = board.FindPlayerNodes(GetOpponent(p));
            foreach (var nodeIdx in nodes)
            {
                var freeNodes = board.FindDestNodes(nodeIdx);
                if (freeNodes.Count != 0) return false;
            }
            return true;
        }

        public void Render()
        {
            Console.WriteLine("{0} - {1} - {2}", board.boardNodes[0], board.boardNodes[1], board.boardNodes[2]);
            Console.WriteLine("| {0} ^ {1} |", board.boardNodes[3], board.boardNodes[4]);
            Console.WriteLine("{0}<  x  >{1}", board.boardNodes[5], board.boardNodes[6]);
            Console.WriteLine("| {0} v {1} |", board.boardNodes[7], board.boardNodes[8]);
            Console.WriteLine("{0} - {1} - {2}", board.boardNodes[9], board.boardNodes[10], board.boardNodes[11]);
        }

        public void ShowStep()
        {
            foreach (var step in memory)
            {
                Console.WriteLine(step);
            }
        }

        ///
        public int GetRandomPieceIdx(XiefangPlayer p)
        {
            var nodes = board.FindPlayerNodes(p);
            return nodes[rand.Next(0, nodes.Count)];
        }

        public int GetRandomDestIdx(int nodeIdx)
        {
            var nodes = board.FindDestNodes(nodeIdx);
            if (nodes.Count == 0) return -1;
            return nodes[rand.Next(0, nodes.Count)];
        }

        public List<int> GetCanArrivedNodes(XiefangPlayer p)
        {
            List<int> arrivedNodes = new List<int>();
            var nodes = board.FindPlayerNodes(p);
            foreach (var idx in nodes)
            {
                arrivedNodes.AddRange(board.FindDestNodes(idx));
            }
            return arrivedNodes;
        }

        public Tuple<int, int> GetRandomMoveAction(XiefangPlayer p)
        {
            Tuple<int, int> act = null;
            int from = -1;
            var nodes = GetCanArrivedNodes(p);
            List<int> sp = new List<int>();
            foreach (var idx in nodes)
            {
                var edges = board.boardNodesLinks[idx];
                foreach (var edge in edges)
                {
                    if (board.boardNodes[edge.Item2] == (int)p)
                    { sp.Add(edge.Item2); from = idx; }
                }
            }
            if (sp.Count > 0) { act = new Tuple<int, int>(sp[rand.Next(0, sp.Count)], from); }
            return act;
        }
        //
        public static string CreateMoveCommand(XiefangPlayer player, int from, int to)
        {
            var p = player == XiefangPlayer.HUMAN ? "h" : "m";
            return string.Format("{0}_{1}_{2}", p, from, to);
        }
    }

    public enum XiefangPlayer
    {
        HUMAN = 1,
        MACHINE = 2,
    }

    public class XiefangBoard
    {
        public List<int> boardNodes;    //斜方棋 node，索引为node编号，值为 棋子情况
        public List<List<Tuple<int, int>>> boardNodesLinks;    //tuple--from-to，为nodes索引值

        /*
            0 - 1 - 2
              3 ^ 4
            5<  X  >6
              7 v 8
            9 - 10 -11        
        */
        public XiefangBoard()
        {
            boardNodes = new List<int>(12);
            boardNodesLinks = new List<List<Tuple<int, int>>>(12);
            //12个落子点
            for (var i = 0; i < 12; ++i) boardNodes.Add(0);
            //路径初始化,双向图
            boardNodesLinks.Add(new List<Tuple<int, int>> {
                new Tuple<int, int>(0, 1),
                new Tuple<int, int>(0, 5),
            });
            boardNodesLinks.Add(new List<Tuple<int, int>> {
                new Tuple<int, int>(1, 0),
                new Tuple<int, int>(1, 2),
                new Tuple<int, int>(1, 3),
                new Tuple<int, int>(1, 4)
            });
            boardNodesLinks.Add(new List<Tuple<int, int>> {
                new Tuple<int, int>(2, 6),
                new Tuple<int, int>(2, 1),
            });
            boardNodesLinks.Add(new List<Tuple<int, int>> {
                new Tuple<int, int>(3, 1),
                new Tuple<int, int>(3, 5),
                new Tuple<int, int>(3, 8),
            });
            boardNodesLinks.Add(new List<Tuple<int, int>> {
                new Tuple<int, int>(4, 1),
                new Tuple<int, int>(4, 6),
                new Tuple<int, int>(4, 7),
            });
            boardNodesLinks.Add(new List<Tuple<int, int>> {
                new Tuple<int, int>(5, 0),
                new Tuple<int, int>(5, 3),
                new Tuple<int, int>(5, 7),
                new Tuple<int, int>(5, 9),
            });
            boardNodesLinks.Add(new List<Tuple<int, int>> {
                new Tuple<int, int>(6, 2),
                new Tuple<int, int>(6, 4),
                new Tuple<int, int>(6, 8),
                new Tuple<int, int>(6, 11),
            });
            boardNodesLinks.Add(new List<Tuple<int, int>> {
                new Tuple<int, int>(7, 5),
                new Tuple<int, int>(7, 4),
                new Tuple<int, int>(7, 10),
            });
            boardNodesLinks.Add(new List<Tuple<int, int>> {
                new Tuple<int, int>(8, 6),
                new Tuple<int, int>(8, 3),
                new Tuple<int, int>(8, 10),
            });
            boardNodesLinks.Add(new List<Tuple<int, int>> {
                new Tuple<int, int>(9, 10),
                new Tuple<int, int>(9, 5),
            });
            boardNodesLinks.Add(new List<Tuple<int, int>> {
                new Tuple<int, int>(10, 7),
                new Tuple<int, int>(10, 8),
                new Tuple<int, int>(10, 9),
                new Tuple<int, int>(10, 11),
            });
            boardNodesLinks.Add(new List<Tuple<int, int>> {
                new Tuple<int, int>(11, 10),
                new Tuple<int, int>(11, 6),
            });
        }
        //
        public void Reset(List<int> boardState)
        {
            boardNodes[0] = (int)XiefangPlayer.MACHINE;
            boardNodes[1] = (int)XiefangPlayer.MACHINE;
            boardNodes[2] = (int)XiefangPlayer.MACHINE;
            boardNodes[3] = (int)XiefangPlayer.MACHINE;
            boardNodes[4] = (int)XiefangPlayer.MACHINE;
            boardNodes[5] = 0;
            boardNodes[6] = 0;
            boardNodes[7] = (int)XiefangPlayer.HUMAN;
            boardNodes[8] = (int)XiefangPlayer.HUMAN;
            boardNodes[9] = (int)XiefangPlayer.HUMAN;
            boardNodes[10] = (int)XiefangPlayer.HUMAN;
            boardNodes[11] = (int)XiefangPlayer.HUMAN;
            //
            if (boardState != null)
            {
                boardNodes.Clear();
                boardNodes = new List<int>(boardState);
            }
        }
        //
        public List<int> FindPlayerNodes(XiefangPlayer player)
        {
            List<int> found = new List<int>(5);
            for (int i = 0; i < boardNodes.Count; ++i)
            {
                if (boardNodes[i] == (int)player) found.Add(i);
            }
            return found;
        }
        //
        public List<int> FindDestNodes(int nodeIdx)
        {
            List<int> nodes = new List<int>(4);
            var edges = boardNodesLinks[nodeIdx];
            foreach (var edge in edges)
            {
                if (boardNodes[edge.Item2] == 0)
                {
                    nodes.Add(edge.Item2);
                }
            }
            return nodes;
        }

        public bool MovePiece(int from, int to)
        {
            var piece = boardNodes[from];
            if (boardNodes[to] == 0 && piece != 0 && IsAdjacent(from, to))
            {
                boardNodes[to] = piece;
                boardNodes[from] = 0;
                return true;
            }
            return false;
        }

        public bool IsAdjacent(int a, int b)
        {
            var edges = boardNodesLinks[a];
            foreach (var edge in edges)
            {
                if (edge.Item2 == b) return true;
            }
            return false;
        }

    }
}
