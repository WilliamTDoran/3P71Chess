using System;
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
    internal int[] pieceSelected;
    [SerializeField]
    private SpriteRenderer turnUI;

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

    internal int getPiece(int x, int y)
    {
        return board[x][y];
    }

    internal int getPiece(Position pos)
    {
        return getPiece(pos.x, pos.y);
    }

    /// <summary>
    /// Recursive function that returns a list of positions in a line
    /// </summary>
    /// <param name="b">The current board state.</param>
    /// <param name="p">The previous position within the line</param>
    /// <param name="changeX">the change along the x (-1, 0, or 1 usually)</param>
    /// <param name="changeY">the change along the y (-1, 0, or 1 usually)</param>
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
    public static Position[] line(int[][] b, int x, int y, int changeX, int changeY, int maxRemaining, bool canCapture)
    {
        Position[] pos;
        x += changeX;
        y += changeY;
        if (!onBoard(x, y)) // if not on the board
        {
            pos = new Position[0];
        }
        else if (maxRemaining > 1 && b[x][y] == 0) // if in the line & not space is taken
        {
            Position[] pos2 = line(b, x, y, changeX, changeY, maxRemaining - 1, canCapture);
            pos = new Position[pos2.Length + 1];
            for (int i = 0; i < pos2.Length; i++) pos[i] = pos2[i];
            pos[pos2.Length] = new Position(x, y);
        }
        else // if in the line & space is taken
        {
            if (Instance.getPiece(x, y) * Instance.pieceSelected[2] > 0 || !canCapture && Instance.getPiece(x, y) * Instance.pieceSelected[2] < 0) // if same colour
            {
                pos = new Position[0];
            } else
            {
                pos = new Position[1];
                pos[0] = new Position(x, y);
            }
        }
        return pos;
    }

    public static bool onBoard(int x, int y)
    {
        bool on = true;
        if (x < 0 || x >= BoardLength) on = false;
        if (y < 0 || y >= BoardLength) on = false;
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

        pieceSelected = new int[3];
        pieceSelected[0] = -1;
        pieceSelected[1] = -1;
        pieceSelected[2] = 1;// who's turn
        turnUI.color = new Color(255, 255, 255);

        AllPieces.Instance.whitePieces.canCastleKing = true;
        AllPieces.Instance.whitePieces.canCastleQueen = true;
        AllPieces.Instance.blackPieces.canCastleKing = true;
        AllPieces.Instance.blackPieces.canCastleQueen = true;
    }


    public bool move(int oldX, int oldY, int newX, int newY)
    {
        //check if valid move. Returns false if not
        bool valid = false;
        Position[] moves = PieceMoves.moves(board, new Position(oldX, oldY));
        Debug.Log("Valid Moves include the following:");
        foreach (Position p in moves)
        {
            //Debug.Log(p.x+", "+p.y);
            if (p.x == newX && p.y == newY)
            {
                valid = true;
                break;
            } else
            {
                Debug.Log(p.x +" "+ newX + " x : y " + p.y + " " + newY);
            }
        }
        if (!valid) return false;

        //castling check
        if (canCastle(oldX, oldY, newX, newY))
        {
            //move rook
        }
        else if (Math.Abs(board[oldX][oldY]) == PieceCode.King && oldX == (8 + ColourPieces.GetPieceColour(board[oldX][oldY])) % 9 && oldY == 4 && (newY == 6 || newY == 2))
        {
            return false;
        }


        board[newX][newY] = board[oldX][oldY];
        board[oldX][oldY] = 0;
        pieceSelected[2] *= -1;
        if (pieceSelected[2] == -1) turnUI.color = new Color(0, 0, 0);
        else turnUI.color = new Color(255, 255, 255);
        AllPieces.Instance.UpdateBoard();
        return true; // if valid move
    }

    private bool canCastle(int oldX, int oldY, int newX, int newY)
    {
        ColourPieces playing;
        int row = (8 + ColourPieces.GetPieceColour(board[oldX][oldY])) % 9;
        if (pieceSelected[2] == -1) playing = AllPieces.Instance.blackPieces;
        else playing = AllPieces.Instance.whitePieces;
        if (Math.Abs(board[oldX][oldY]) == PieceCode.King)
        {
            if (oldX == row && oldY == 4)
            {
                if (newX == row && newY == 6)
                {
                    if (playing.canCastleKing && getPiece(row, 5) == 0 && getPiece(row, 6) == 0) 
                    {
                        playing.canCastleKing = false;
                        playing.canCastleQueen = false;
                        board[oldX][5] = board[oldX][7];
                        board[oldX][7] = 0;
                        return true;
                    } 
                } else if (newX == row && newY == 2)
                {
                    if (playing.canCastleQueen && getPiece(row, 1) == 0 && getPiece(row, 2) == 0 && getPiece(row, 3) == 0)
                    {
                        playing.canCastleKing = false;
                        playing.canCastleQueen = false;
                        board[oldX][3] = board[oldX][0];
                        board[oldX][0] = 0;
                        return true;
                    }
                } else
                {
                    playing.canCastleKing = false;
                    playing.canCastleQueen = false;
                }
            }
        } else if (Math.Abs(board[oldX][oldY]) == PieceCode.Rook)
        {
            if (oldX == row && oldY == 0) // if rook in original position Left
            {
                playing.canCastleQueen = false;
            } else if (oldX == row && oldY == 7)// if rook in original position right
            {
                playing.canCastleKing = false;
            }
        }
        return false;
    }
}
