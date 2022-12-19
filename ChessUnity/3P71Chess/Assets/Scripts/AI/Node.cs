using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    internal short[,] config;
    internal Node[] moves;
    internal float heuristic;
    internal short optimalMove;

    public Node(short[,] configuration, Position from, Position to)
    {
        config = configuration;
        BoardState.move(ref config, from.x, from.y, to.x, to.y);
    }

    public Node()
    {
        config = BoardState.Instance.board;
    }

    internal static void fillMoves(Node n)
    {
        n.moves = new Node[0];
        for (short i=0; i<BoardState.BoardLength; i++)
        {
            for (short j=0; j< BoardState.BoardLength; j++)
            {
                if (n.config[i,j]==0) continue;
                Position start = new Position(i, j);
                Position[] movesPos = PieceMoves.moves(n.config, start);
                for (short k = 0; k < movesPos.Length; k++)
                {
                    n.moves = addMove(n.moves, new Node(n.config, start, movesPos[k]));
                }
            }
        }
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
