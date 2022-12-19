using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AITesting;

public class Position
{
    public short x;
    public short y;

    public Position()
    {
        x = 0;
        y = 0;
    }

    public Position(short x, short y)
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
    public static short BoardLength = 8;
    internal short[,] board;
    internal short[] pieceSelected;
    [SerializeField]
    private SpriteRenderer turnUI;
    internal int AITurnVal = -1;
    internal bool canPlay = true;

    internal Position[] lastMove;


    public void Awake()
    {
        instance = this;
        Debug.Log("Setup within Awake");
    }

    private void Start()
    {
        Debug.Log("Setup starting");
        newBoard();
        AllPieces.Instance.UpdateBoard();
        Debug.Log("Setup done");
    }

    internal short getPiece(short x, short y)
    {
        return board[x,y];
    }

    internal short getPiece(Position pos)
    {
        return getPiece(pos.x, pos.y);
    }

    //line function with an short array
    internal static Position[] line(short[,] b, short x, short y, short changeX, short changeY, short maxRemaining, bool canCapture, int colour)
    {
        Position[] pos;
        x += changeX;
        y += changeY;
        if (!onBoard(x, y)) // if not on the board
        {
            pos = new Position[0];
        }
        else if (maxRemaining > 1 && b[x,y] == 0) // if in the line & not space is taken
        {
            Position[] pos2 = line(b, x, y, changeX, changeY, (short)(maxRemaining - 1), canCapture, colour);
            pos = pos2;
            int kx;
            int ky;
            AI.BE.FindKing(out kx, out ky, b, colour);
            if (!ThreatEvaluator.EvaluateThreatened(kx, ky, b, colour, false, true))
            {
                pos = new Position[pos2.Length + 1];
                for (short i = 0; i < pos2.Length; i++) pos[i] = pos2[i];
                pos[pos2.Length] = new Position(x, y);
            }            
        }
        else // if in the line & space is taken
        {
            int kx;
            int ky;
            AI.BE.FindKing(out kx, out ky, b, colour);
            if (b[x,y] * Instance.pieceSelected[2] > 0 || !canCapture && b[x,y] * Instance.pieceSelected[2] < 0) // if same colour
            {
                pos = new Position[0];
            }
            else if(!ThreatEvaluator.EvaluateThreatened(kx, ky, b, colour, false, true))
            {
                pos = new Position[1];
                pos[0] = new Position(x, y);
            } else
            {
                pos = new Position[0];
            }
        }
        return pos;
    }

    internal static bool onBoard(short x, short y)
    {
        bool on = true;
        if (x < 0 || x >= BoardLength) on = false;
        if (y < 0 || y >= BoardLength) on = false;
        return on;
    }

    internal void newBoard()
    {
        board = new short[BoardLength, BoardLength];
        for (short i=0; i<BoardLength; i++)
        {
            board[6,i] = (short)(PieceCode.white * PieceCode.Pawn);
            board[1,i] = (short)(PieceCode.black * PieceCode.Pawn);
        }
        board[7,0] = (short)(PieceCode.white * PieceCode.Rook);
        board[7,1] = (short)(PieceCode.white * PieceCode.Knight);
        board[7,2] = (short)(PieceCode.white * PieceCode.Bishop);
        board[7,3] = (short)(PieceCode.white * PieceCode.Queen);
        board[7,4] = (short)(PieceCode.white * PieceCode.King);
        board[7,5] = (short)(PieceCode.white * PieceCode.Bishop);
        board[7,6] = (short)(PieceCode.white * PieceCode.Knight);
        board[7,7] = (short)(PieceCode.white * PieceCode.Rook);

        board[0,0] = (short)(PieceCode.black * PieceCode.Rook);
        board[0,1] = (short)(PieceCode.black * PieceCode.Knight);
        board[0,2] = (short)(PieceCode.black * PieceCode.Bishop);
        board[0,3] = (short)(PieceCode.black * PieceCode.Queen);
        board[0,4] = (short)(PieceCode.black * PieceCode.King);
        board[0,5] = (short)(PieceCode.black * PieceCode.Bishop);
        board[0,6] = (short)(PieceCode.black * PieceCode.Knight);
        board[0,7] = (short)(PieceCode.black * PieceCode.Rook);

        Debug.Log("Board Created");

        pieceSelected = new short[3];
        pieceSelected[0] = -1;
        pieceSelected[1] = -1;
        pieceSelected[2] = 1;// who's turn
        turnUI.color = new Color(255, 255, 255);

        AllPieces.Instance.whitePieces.canCastleKing = true;
        AllPieces.Instance.whitePieces.canCastleQueen = true;
        AllPieces.Instance.blackPieces.canCastleKing = true;
        AllPieces.Instance.blackPieces.canCastleQueen = true;

        lastMove = new Position[2];

        AI.init();
    }


    internal static bool move(ref short[,] b, short oldX, short oldY, short newX, short newY)
    {
        //check if valid move. Returns false if not
        bool valid = false;
        Position[] moves = PieceMoves.moves(b, new Position(oldX, oldY));
        //Debug.Log("Valid Moves include the following:");
        if (moves != null)
        {
            foreach (Position p in moves)
            {
                //Debug.Log(p.x+", "+p.y);
                if (p.x == newX && p.y == newY)
                {
                    valid = true;
                    break;
                }
                else
                {
                    //Debug.Log(p.x +" "+ newX + " x : y " + p.y + " " + newY);
                }
            }
        }
        
        if (!valid) return false;

        //castling check
        if (!Instance.canCastle(b, oldX, oldY, newX, newY) && Math.Abs(b[oldX,oldY]) == PieceCode.King && oldX == 7-(8 + ColourPieces.GetPieceColour(b[oldX,oldY])) % 9 && oldY == 4 && (newY == 6 || newY == 2))
        {
            return false;
        }

        //Em Passant
        if (Math.Abs(b[oldX,oldY]) == PieceCode.Pawn && Instance.enPassant(b, new Position(oldX, oldY), 1) && newY == oldY + 1) 
        {
            b[oldX,oldY + 1] = 0;
        }
        if (Math.Abs(b[oldX,oldY]) == PieceCode.Pawn && Instance.enPassant(b, new Position(oldX, oldY), -1) && newY == oldY - 1)
        {
            b[oldX,oldY - 1] = 0;
        }

        b[newX,newY] = b[oldX,oldY];
        b[oldX,oldY] = 0;
        if(Instance.AITurnVal != Instance.pieceSelected[2])
        {
            Instance.lastMove[0] = new Position(oldX, oldY);
            Instance.lastMove[1] = new Position(newX, newY);
            swapTurn();
        }
        return true; // if valid move
    }

    private static void swapTurn()
    {
        Instance.pieceSelected[2] *= -1;
        if (Instance.pieceSelected[2] == -1) Instance.turnUI.color = new Color(0, 0, 0);
        else Instance.turnUI.color = new Color(255, 255, 255);
        AllPieces.Instance.UpdateBoard();
        Debug.Log(Instance.AITurnVal + ", " + Instance.pieceSelected[2]);
        if (Instance.AITurnVal == Instance.pieceSelected[2])
        {
            AITurn();
        }
        else Instance.canPlay = true;
    }

    public static void AITurn()
    {
        Instance.canPlay = false;
        Node n = new Node();
        AI.miniMaxAlgorithm(-1, n, true, float.MinValue, float.MaxValue);
        Debug.Log("Optimal move: "+n.optimalMove);
            Instance.board = n.moves[n.optimalMove].config;
        //}
        swapTurn();
        
    }

    private bool canCastle(short[,] b, short oldX, short oldY, short newX, short newY)
    {
        ColourPieces playing;
        short row = (short)(7 - (8 + ColourPieces.GetPieceColour(b[oldX,oldY])) % 9);
        if (pieceSelected[2] == -1) playing = AllPieces.Instance.blackPieces;
        else playing = AllPieces.Instance.whitePieces;
        if (Math.Abs(b[oldX,oldY]) == PieceCode.King)
        {
            if (oldX == row && oldY == 4)
            {
                if (newX == row && newY == 6)
                {
                    if (playing.canCastleKing && getPiece(row, 5) == 0 && getPiece(row, 6) == 0) 
                    {
                        playing.canCastleKing = false;
                        playing.canCastleQueen = false;
                        b[oldX,5] = b[oldX,7];
                        b[oldX,7] = 0;
                        return true;
                    } 
                } else if (newX == row && newY == 2)
                {
                    if (playing.canCastleQueen && getPiece(row, 1) == 0 && getPiece(row, 2) == 0 && getPiece(row, 3) == 0)
                    {
                        playing.canCastleKing = false;
                        playing.canCastleQueen = false;
                        b[oldX,3] = b[oldX,0];
                        b[oldX,0] = 0;
                        return true;
                    }
                } else
                {
                    playing.canCastleKing = false;
                    playing.canCastleQueen = false;
                }
            }
        } else if (Math.Abs(b[oldX,oldY]) == PieceCode.Rook)
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

    public bool enPassant(short[,] b, Position start, short side)
    {
        short jump = (short)(-1 * PieceCode.Pawn * pieceSelected[2]);
        if (((start.y + side) % BoardLength > 0 || start.y + side == 0) && b[start.x,start.y + side] == jump && lastMove[1].x == start.x && lastMove[1].y == start.y + side && lastMove[0].x == start.x - (2 * pieceSelected[2])) return true;

        return false;
    }
}
