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
        else if (piece == PieceCode.Bishop) ans = BishopMoves(b, start, BoardState.BoardLength);
        else if (piece == PieceCode.Knight) ans = KnightMoves(b, start);
        else if (piece == PieceCode.Rook) ans = RookMoves(b, start, BoardState.BoardLength);
        else if (piece == PieceCode.Queen) ans = QueenMoves(b, start, BoardState.BoardLength);
        else if (piece == PieceCode.King) ans = KingMoves(b, start);
        return ans;
    }

    internal static Position[] PawnMoves(int[][] b, Position start)
    {
        Position[] ans;
        int jump = 1;
        if (start.x == 1 && b[start.x][start.y] > 0 || start.x == 6 && b[start.x][start.y] < 0) jump++;
        ans = BoardState.line(b, start.x, start.y, ColourPieces.GetPieceColour(b[start.x][start.y]), 0, jump, false);
        ans = MustCapture(start.x + ColourPieces.GetPieceColour(b[start.x][start.y]), start.y + 1, ans);
        ans = MustCapture(start.x + ColourPieces.GetPieceColour(b[start.x][start.y]), start.y - 1, ans);
        return ans;
    }


    public static Position[] MustCapture(int x, int y, Position[] ans)
    {
        // if not on the board or piece cannot be captured
        if (!BoardState.onBoard(x, y) || BoardState.Instance.getPiece(x, y) * BoardState.Instance.pieceSelected[2] >= 0) return ans;
        else // if piece can be captured
        {
            Position[] ans2 = new Position[ans.Length + 1];
            for (int i=0; i<ans.Length; i++)
            {
                ans2[i] = ans[i];
            }
            ans2[ans.Length] = new Position(x, y);
            ans = ans2;
        }
        return ans;
    }



    internal static Position[] BishopMoves(int[][] b, Position start, int length)
    {
        Position[] right = BoardState.line(b, start.x, start.y, 1, -1, length, true);
        Position[] left = BoardState.line(b, start.x, start.y, -1, 1, length, true);
        Position[] up = BoardState.line(b, start.x, start.y, 1, 1, length, true);
        Position[] down = BoardState.line(b, start.x, start.y, -1, -1, length, true);
        Position[] ans = new Position[right.Length + left.Length + up.Length + down.Length];
        for (int i = 0; i < right.Length; i++) ans[i] = right[i];
        for (int i = 0; i < left.Length; i++) ans[i + right.Length] = left[i];
        for (int i = 0; i < up.Length; i++) ans[i + left.Length + right.Length] = up[i];
        for (int i = 0; i < down.Length; i++) ans[i + up.Length + left.Length + right.Length] = down[i];
        return ans;
    }

    internal static Position[] KnightMoves(int[][] b, Position start)
    {
        Position[] ans = BoardState.line(b, start.x, start.y, 1, 2, 1, true);
        ans = addMoves(ans, b, start, 2, 1);
        ans = addMoves(ans, b, start, -1, 2);
        ans = addMoves(ans, b, start, 2, -1);
        ans = addMoves(ans, b, start, 1, -2);
        ans = addMoves(ans, b, start, -2, 1);
        ans = addMoves(ans, b, start, -1, -2);
        ans = addMoves(ans, b, start, -2, -1);
        return ans;
    }

    internal static Position[] addMoves(Position[] prev, int[][] b, Position start, int x, int y)
    {
        Debug.Log("Adding "+x+", "+y);
        Position[] additional = BoardState.line(b, start.x, start.y, x, y, 1, true);
        Position[] ans = new Position[prev.Length + additional.Length];
        for (int i = 0; i < prev.Length; i++) ans[i] = prev[i];
        for (int i = 0; i < additional.Length; i++) ans[i + prev.Length] = additional[i];
        return ans;
    }

    internal static Position[] RookMoves(int[][] b, Position start, int length)
    {
        Position[] right = BoardState.line(b, start.x, start.y, 1, 0, length, true);
        Position[] left = BoardState.line(b, start.x, start.y, -1, 0, length, true);
        Position[] up = BoardState.line(b, start.x, start.y, 0, 1, length, true);
        Position[] down = BoardState.line(b, start.x, start.y, 0, -1, length, true);
        Position[] ans = new Position[right.Length + left.Length + up.Length + down.Length];
        for (int i = 0; i < right.Length; i++) ans[i] = right[i];
        for (int i = 0; i < left.Length; i++) ans[i + right.Length] = left[i];
        for (int i = 0; i < up.Length; i++) ans[i + left.Length + right.Length] = up[i];
        for (int i = 0; i < down.Length; i++) ans[i + up.Length + left.Length + right.Length] = down[i];
        return ans;
    }

    internal static Position[] QueenMoves(int[][] b, Position start, int length)
    {
        Position[] diagonal = BishopMoves(b, start, length);
        Position[] flat = RookMoves(b, start, length);
        Position[] ans = new Position[diagonal.Length + flat.Length];
        for (int i = 0; i < diagonal.Length; i++) ans[i] = diagonal[i];
        for (int i = 0; i < flat.Length; i++) ans[i + diagonal.Length] = flat[i];
        return ans;

    }

    internal static Position[] KingMoves(int[][] b, Position start)
    {
        Position[] ans = QueenMoves(b, start, 1);
        if (start.y == 4)
        {
            ans = addMoves(ans, b, start, 0, 2);
            ans = addMoves(ans, b, start, 0, -2);
        }
        return ans;
    }
}
