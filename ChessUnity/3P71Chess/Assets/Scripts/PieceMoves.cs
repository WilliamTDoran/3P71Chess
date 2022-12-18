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
        Position[] right = BoardState.line(b, start, 1, -1, BoardState.BoardLength);
        Position[] left = BoardState.line(b, start, -1, 1, BoardState.BoardLength);
        Position[] up = BoardState.line(b, start, 1, 1, BoardState.BoardLength);
        Position[] down = BoardState.line(b, start, -1, -1, BoardState.BoardLength);
        Position[] ans = new Position[right.Length + left.Length + up.Length + down.Length];
        for (int i = 0; i < right.Length; i++) ans[i] = right[i];
        for (int i = 0; i < left.Length; i++) ans[i + right.Length] = left[i];
        for (int i = 0; i < up.Length; i++) ans[i + left.Length + right.Length] = up[i];
        for (int i = 0; i < down.Length; i++) ans[i + up.Length + left.Length + right.Length] = down[i];
        return ans;
    }

    internal static Position[] KnightMoves(int[][] b, Position start)
    {
        throw new NotImplementedException();
    }

    internal static Position[] RookMoves(int[][] b, Position start)
    {
        Position[] right = BoardState.line(b, start, 1, 0, BoardState.BoardLength);
        Position[] left = BoardState.line(b, start, -1, 0, BoardState.BoardLength);
        Position[] up = BoardState.line(b, start, 0, 1, BoardState.BoardLength);
        Position[] down = BoardState.line(b, start, 0, -1, BoardState.BoardLength);
        Position[] ans = new Position[right.Length + left.Length + up.Length + down.Length];
        for (int i = 0; i < right.Length; i++) ans[i] = right[i];
        for (int i = 0; i < left.Length; i++) ans[i + right.Length] = left[i];
        for (int i = 0; i < up.Length; i++) ans[i + left.Length + right.Length] = up[i];
        for (int i = 0; i < down.Length; i++) ans[i + up.Length + left.Length + right.Length] = down[i];
        return ans;
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
