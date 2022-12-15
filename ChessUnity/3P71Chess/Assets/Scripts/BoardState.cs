using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position
{
    public int x;
    public int y;
}

//Internal can't be seen by outside code; better for security. Inspector can't see subclasses.
public enum PieceType
{
    None,
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King
}

internal class BoardState
{
    public static int BoardLength = 8;
    PieceType[][] board;

    internal BoardState() // creates a initial board state for game start
    {

    }

    internal BoardState(BoardState prev) // Boardstate + piece moved
    {

    }

    internal PieceType getPiece(Position pos)
    {
        return board[pos.x][pos.y];
    }

    /// <summary>
    /// Recursive function that returns a list of positions in a line
    /// </summary>
    /// <param name="b">The current board state.</param>
    /// <param name="p">The previous position within the line</param>
    /// <param name="x">the change along the x (-1, 0, or 1 usually)</param>
    /// <param name="y">the change along the y (-1, 0, or 1 usually)</param>
    /// <param name="maxRemaining">The max number of positions checked within the function.</param>
    /// <returns>List of positions within the line</returns>
    public static Position[] line(BoardState b, Position p, int x, int y, int maxRemaining)
    {
        Position[] pos;
        p.x+=x;
        p.y+=y;
        if (!onBoard(p)) // if not on the board
        {
            pos = new Position[0];
        } else if (maxRemaining > 1 && b.getPiece(p) == PieceType.None) // if in the line & not space is taken
        {
            Position[] pos2 = line(b, p, x, y, maxRemaining - 1);
            pos = new Position[pos2.Length+1];
            for (int i=0; i<pos2.Length; i++) pos[i] = pos2[i];
            pos[pos2.Length] = p;
        } else // if in the line & space is taken
        {
            pos = new Position[1];
            pos[0] = p;
        }
        return pos;
    }

    /** 
     * Example ways to use the line function
     * 
     * Position[] horizontalLineRight = BoardState.line(board, pos, 1, 0, BoardState.BoardLength);
     * Position[] DiagonalLineUpRight = BoardState.line(board, pos, 1, 1, BoardState.BoardLength);
     * Position[] VerticalLineUp = BoardState.line(board, pos, 0, 1, BoardState.BoardLength);
     * Position[] KnightMovement2Up1Right = BoardState.line(board, pos, 1, 2, 1);
     **/

    public static bool onBoard(Position pos)
    {
        bool on = true;
        if (pos.x < 0 || pos.x >= BoardLength) on = false;
        if (pos.y < 0 || pos.y >= BoardLength) on = false;
        return on;
    }
}
