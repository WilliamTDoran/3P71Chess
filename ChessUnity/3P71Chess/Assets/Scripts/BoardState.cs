using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position
{
    public int x;
    public int y;

    public Position()
    {
        x = 0;
        y = 0;
    }

    public Position(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

//Internal can't be seen by outside code; better for security. Inspector can't see subclasses.


internal class BoardState : MonoBehaviour
{
    private static BoardState instance;
    public static BoardState Instance { get { return instance; } }
    [HideInInspector]
    public static int BoardLength = 8;
    int[][] board;

    public void Awake()
    {
        instance = this;
        Debug.Log("Code waking up.");
    }

    private void Start()
    {
        Debug.Log("Code starting.");
        newBoard();
        AllPieces.Instance.UpdateBoard();
        Debug.Log("Code done.");
    }

    internal int getPiece(Position pos)
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
    /*public static Position[] line(BoardState b, Position p, int x, int y, int maxRemaining)
    {
        Position[] pos;
        p.x+=x;
        p.y+=y;
        if (!onBoard(p)) // if not on the board
        {
            pos = new Position[0];
        } else if (maxRemaining > 1 && b.getPiece(p) == 0) // if in the line & not space is taken
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
    }*/

    //line function with an int array
    public static Position[] line(int[][] b, Position p, int x, int y, int maxRemaining)
    {
        Position[] pos;
        p.x += x;
        p.y += y;
        if (!onBoard(p)) // if not on the board
        {
            pos = new Position[0];
        }
        else if (maxRemaining > 1 && b[p.x][p.y] == 0) // if in the line & not space is taken
        {
            Position[] pos2 = line(b, p, x, y, maxRemaining - 1);
            pos = new Position[pos2.Length + 1];
            for (int i = 0; i < pos2.Length; i++) pos[i] = pos2[i];
            pos[pos2.Length] = p;
        }
        else // if in the line & space is taken
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

    public void newBoard()
    {
        board = new int[BoardLength][];
        for (int i=0; i<board.Length; i++)
        {
            board[i] = new int[BoardLength];
        }
        for (int i=0; i<BoardLength; i++)
        {
            board[6][i] = PieceCode.black * PieceCode.Pawn;
            board[1][i] = PieceCode.white * PieceCode.Pawn;
        }
        board[7][0] = PieceCode.black * PieceCode.Rook;
        board[7][1] = PieceCode.black * PieceCode.Knight;
        board[7][2] = PieceCode.black * PieceCode.Bishop;
        board[7][3] = PieceCode.black * PieceCode.Queen;
        board[7][4] = PieceCode.black * PieceCode.King;
        board[7][5] = PieceCode.black * PieceCode.Bishop;
        board[7][6] = PieceCode.black * PieceCode.Knight;
        board[7][7] = PieceCode.black * PieceCode.Rook;

        board[0][0] = PieceCode.white * PieceCode.Rook;
        board[0][1] = PieceCode.white * PieceCode.Knight;
        board[0][2] = PieceCode.white * PieceCode.Bishop;
        board[0][3] = PieceCode.white * PieceCode.Queen;
        board[0][4] = PieceCode.white * PieceCode.King;
        board[0][5] = PieceCode.white * PieceCode.Bishop;
        board[0][6] = PieceCode.white * PieceCode.Knight;
        board[0][7] = PieceCode.white * PieceCode.Rook;

        Debug.Log("Board Created");
    }
}
