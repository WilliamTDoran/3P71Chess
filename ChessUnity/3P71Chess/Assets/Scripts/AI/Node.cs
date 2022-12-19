using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    protected short[,] config;
    protected Node[] moves;
    protected float rating;

    Node(short[,] configuration, Position from, Position to)
    {
        config = configuration;
        BoardState.move(ref config, from.x, from.y, to.x, to.y);
    }

    Node()
    {

    }

    static void fillMoves(Node n)
    {
        n.moves = new Node[0];
        for (short i=0; i<n.config.Length; i++)
        {
            for (short j=0; j<n.config.Length; j++)
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
}
