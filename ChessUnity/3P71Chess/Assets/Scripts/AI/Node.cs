using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    internal short[,] config;
    internal Node[] moves;
    internal float heuristic;
    internal short optimalMove;

    public Node(short[,] configuration, Position from, Position to, short colour)
    {

        config = new short[BoardState.BoardLength,BoardState.BoardLength];
        for (int i=0; i< BoardState.BoardLength; i++)
        {
            for (int j = 0; j < BoardState.BoardLength; j++)
            {
                config[i, j] = configuration[i, j];
            }
        }
        BoardState.move(ref config, from.x, from.y, to.x, to.y, colour);
    }

    public Node()
    {
        config = BoardState.Instance.board;
    }

    internal static void fillMoves(Node n, int turn)
    {
        //string pos = "";
        n.moves = new Node[0];
        for (short i=0; i<BoardState.BoardLength; i++)
        {
            for (short j=0; j< BoardState.BoardLength; j++)
            {
                if (n.config[i,j]*turn <= 0) continue;
                //pos = pos + ", " + i + "." + j;
                Position start = new Position(i, j);
                Position[] movesPos = PieceMoves.moves(n.config, start);
                //n.heuristic = BoardState.Instance.pieceSelected[2] * AI.BE.EvaluateConfiguration(n.config);
                //pos = pos + "." +movesPos.Length+":"+n.heuristic+"c:";
                for (short k = 0; k < movesPos.Length; k++)
                {
                    n.moves = addMove(n.moves, new Node(n.config, start, movesPos[k], (short)(turn *-1)));
                    n.moves[k].heuristic = BoardState.Instance.pieceSelected[2] * AI.BE.EvaluateConfiguration(n.moves[k].config);

                    //pos = pos + n.moves[k].heuristic+"/";
                }
            }
        }
        //Debug.Log(pos);
    }


    static Node[] addMove(Node[] moves, Node adding)
    {
        Node[] n = new Node[moves.Length + 1];
        for (int i = 0; i < moves.Length; i++) n[i] = moves[i];
        n[moves.Length] = adding;
        return n;
    }

    public static Node[] prune(Node[] moves, int length)
    {
        Node[] shortN = new Node[length];
        for (int i=0; i<length; i++)
        {
            shortN[i] = moves[i];
        }
        return shortN;
    }
}
