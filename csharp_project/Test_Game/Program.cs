using System;
using System.Threading;
using System.Collections;

namespace mcts
{
    class Program
    {
        static void Main(string[] args)
        {
            bool running = true;
            Console.WriteLine("Hello XiefangChess!");
            XiefangChess xfChess = new XiefangChess();
            xfChess.Reset(null);
            XiefangPlayer curTurn = XiefangPlayer.HUMAN;
            do
            {
                xfChess.Render();
                Thread.Sleep(1000);

                if (curTurn == XiefangPlayer.HUMAN)
                {
                    var command = Console.ReadLine();
                    if (!command.StartsWith('h')) command = "h_" + command;
                    if (xfChess.MakeMove(command))
                    {
                        if (xfChess.CheckWin(curTurn))
                        {
                            break;
                        }
                        curTurn = xfChess.GetOpponent(curTurn);
                    }

                }

                else if (curTurn == XiefangPlayer.MACHINE)
                {
                    var act = xfChess.GetRandomMoveAction(curTurn);
                    if (xfChess.MakeMove(string.Format("m_{0}_{1}", act.Item1, act.Item2)))
                    {
                        if (xfChess.CheckWin(curTurn))
                        {
                            break;
                        }
                        curTurn = xfChess.GetOpponent(curTurn);
                    }
                }

                Console.Clear();


            } while (running);

            Console.WriteLine("胜利者为{0}", curTurn.ToString());

            xfChess.ShowStep();

            Console.ReadKey();
        }
    }
}
