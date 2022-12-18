using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMoves
{
    internal static Position[] moves(int[][] b, Position start)
    {
        int piece = Math.Abs(b[start.x][start.y]);
        Position[] ans = null;
        if (piece == PieceCode.Pawn) ans = PawnMoves(b, start);
        else if (piece == PieceCode.Bishop) ans = BishopMoves(b, start);
        else if (piece == PieceCode.Knight) ans = KnightMoves(b, start);
        else if (piece == PieceCode.Rook) ans = RookMoves(b, start);
        else if (piece == PieceCode.Queen) ans = QueenMoves(b, start);
        else if (piece == PieceCode.King) ans = KingMoves(b, start);
        return ans;
    }

    internal static Position[] PawnMoves(int[][] b, Position start)
    {
        throw new NotImplementedException();
    }

    internal static Position[] BishopMoves(int[][] b, Position start)
    {
        throw new NotImplementedException();
    }

    internal static Position[] KnightMoves(int[][] b, Position start)
    {
        throw new NotImplementedException();
    }

    internal static Position[] RookMoves(int[][] b, Position start)
    {
        throw new NotImplementedException();
    }

    internal static Position[] QueenMoves(int[][] b, Position start)
    {
        throw new NotImplementedException();
    }

    internal static Position[] KingMoves(int[][] b, Position start)
    {
        throw new NotImplementedException();
    }
}
