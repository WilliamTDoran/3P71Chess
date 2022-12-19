using AITesting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI
{
    static BoardEvaluator BE;

    public static void init()
    {
        BE = new BoardEvaluator();
    }


    static Node miniMaxAlgorithm(short depth, Node n, bool maximize, short maxDepth, float alpha, float beta)
    {
        if (depth == maxDepth)
        {
            n.heuristic = BE.EvaluateConfiguration(n.config);
            n.optimalMove = -1;
            return n;
        }
        Node.fillMoves(n);

        float bestF;

        if (maximize)
        {
            Node max = miniMaxAlgorithm((short)(depth + 1), n.moves[0], false, maxDepth, alpha, beta);
            n.optimalMove = 0;
            for (short passed = 1; passed < n.moves.Length; passed++)
            {
                Node other = miniMaxAlgorithm((short)(depth + 1), n.moves[passed], false, maxDepth, alpha, beta);
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
            Node min = miniMaxAlgorithm((short)(depth + 1), n.moves[0], true, maxDepth, alpha, beta);
            for (short passed = 1; passed < n.moves.Length; passed++)
            {
                Node other = miniMaxAlgorithm((short)(depth + 1), n.moves[passed], true, maxDepth, alpha, beta);
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
