using AITesting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI
{
    static BoardEvaluator BE;

    static short maxDepth = 6;

    public static void init()
    {
        BE = new BoardEvaluator();
    }


    public static Node miniMaxAlgorithm(short depth, Node n, bool maximize, float alpha, float beta)
    {
        depth++;
        Debug.Log(depth);
        if (depth >= maxDepth)
        {
            n.heuristic = BoardState.Instance.pieceSelected[2] * BE.EvaluateConfiguration(n.config);
            n.optimalMove = -1;
            return n;
        }
        Node.fillMoves(n);

        float bestF;

        if (maximize)
        {
            Node max = miniMaxAlgorithm(depth, n.moves[0], false, alpha, beta);
            n.optimalMove = 0;
            for (short passed = 1; passed < n.moves.Length; passed++)
            {
                Node other = miniMaxAlgorithm(depth, n.moves[passed], false, alpha, beta);
                bestF = Math.Max(max.heuristic, other.heuristic);
                if (bestF == other.heuristic)
                {
                    max = other;
                    n.optimalMove = passed;
                }
                alpha = Math.Max(alpha, bestF);
                if (beta <= alpha)
                {
                    n.moves = Node.prune(n.moves, passed + 1);
                    break;
                }
            }
            return max;
        } else
        {
            Node min = miniMaxAlgorithm(depth, n.moves[0], true, alpha, beta);
            for (short passed = 1; passed < n.moves.Length; passed++)
            {
                Node other = miniMaxAlgorithm(depth, n.moves[passed], true, alpha, beta);
                bestF = Math.Min(min.heuristic, other.heuristic);
                if (bestF == other.heuristic)
                {
                    min = other;
                    n.optimalMove = passed;
                }
                beta = Math.Min(beta, bestF);
                if (beta <= alpha)
                {
                    n.moves = Node.prune(n.moves, passed + 1);
                    break;
                }
            }
            return min;
        }
    }
}
