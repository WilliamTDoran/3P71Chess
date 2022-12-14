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
    internal int AITurnVal = 0;
    internal bool canPlay = true;
    [SerializeField]
    private BoardInitializer init;

    internal Position[] lastMove;


    public void advanceAIState()
    {
        AITurnVal = (AITurnVal + 2) % 3 - 1;
    }


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
        if (AITurnVal == 1)
        {
            AITurn();
        }
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
            if (check(b, colour, (short)(x - changeX), (short)(y - changeY), x, y))
            {
                pos = new Position[pos2.Length + 1];
                for (short i = 0; i < pos2.Length; i++) pos[i] = pos2[i];
                pos[pos2.Length] = new Position(x, y);
            }      
        }
        else // if in the line & space is taken
        {

            if (b[x, y] * Instance.pieceSelected[2] > 0 || !canCapture && b[x, y] * Instance.pieceSelected[2] < 0 || !check(b, colour, (short)(x - changeX), (short)(y - changeY), x, y)) // if same colour
            {
                pos = new Position[0];
            }
            else 
            {
                pos = new Position[1];
                pos[0] = new Position(x, y);
            }
        }
        return pos;
    }

    public static bool check(short[,] b, int colour, short fromX, short fromY, short toX, short toY)
    {
        int kx;
        int ky;
        if (!onBoard(fromX, fromY) || !onBoard(toX, toY)) return false;
        short[,] b2 = moveb(b, fromX, fromY, toX, toY);
        AI.BE.FindKing(out kx, out ky, b2, colour);
        return !ThreatEvaluator.EvaluateThreatened(kx, ky, b2, colour, false, true);// true if threatened
    }

    public static short[,] moveb(short[,] b, short fromX, short fromY, short toX, short toY)
    {
        short[,] ans = new short[b.Length,b.Length];
        for (int i=0; i<BoardLength; i++)
        {
            for (int k=0; k<BoardLength; k++)
            {
                ans[i, k] = b[i, k];
            }
        }
        //Debug.Log(toX + " " + toY + " " + fromX + " " + fromY);
        ans[toX, toY] = ans[fromX, fromY];
        ans[fromX, fromY] = 0;
        return ans;
    }

    internal static bool onBoard(short x, short y)
    {
        bool on = true;
        if (x < 0 || x >= BoardLength) on = false;
        if (y < 0 || y >= BoardLength) on = false;
        return on;
    }

    public void newBoard()
    {
        board = init.LoadBoard();

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


    internal static bool move(ref short[,] b, short oldX, short oldY, short newX, short newY, short colour)
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
                if (p.x == newX && p.y == newY /*&& check(b, ColourPieces.GetPieceColour(b[oldX,oldY]), oldX, oldY, newX, newY)*/)
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
        if (Math.Abs(b[oldX,oldY]) == PieceCode.Pawn && Instance.enPassant(b, new Position(oldX, oldY), 1, colour) && newY == oldY + 1) 
        {
            b[oldX,oldY + 1] = 0;
        }
        if (Math.Abs(b[oldX,oldY]) == PieceCode.Pawn && Instance.enPassant(b, new Position(oldX, oldY), -1, colour) && newY == oldY - 1)
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
        //Debug.Log(Instance.AITurnVal + ", " + Instance.pieceSelected[2]);

        if (AI.BE.EvaluateCheckmate(Instance.board, Instance.pieceSelected[2]))
        {
            Debug.Log("Checkmate!");
            Instance.canPlay = false;
        } else
        {
            if (Instance.AITurnVal == Instance.pieceSelected[2])
            {
                AITurn();
            }
            else Instance.canPlay = true;
        }
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
        short colour = pieceSelected[2];
        short row = (short)(7 - (8 + ColourPieces.GetPieceColour(b[oldX,oldY])) % 9);
        if (colour == -1) playing = AllPieces.Instance.blackPieces;
        else playing = AllPieces.Instance.whitePieces;
        if (Math.Abs(b[oldX,oldY]) == PieceCode.King)
        {
            if (oldX == row && oldY == 4)
            {
                if (newX == row && newY == 6)
                {
                    if (playing.canCastleKing && getPiece(row, 5) == 0 && getPiece(row, 6) == 0 && getPiece(row, 7) == colour * PieceCode.Rook) 
                    {
                        playing.canCastleKing = false;
                        playing.canCastleQueen = false;
                        b[oldX,5] = b[oldX,7];
                        b[oldX,7] = 0;
                        return true;
                    } 
                } else if (newX == row && newY == 2)
                {
                    if (playing.canCastleQueen && getPiece(row, 1) == 0 && getPiece(row, 2) == 0 && getPiece(row, 3) == 0 && getPiece(row, 0) == colour * PieceCode.Rook)
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

    public bool enPassant(short[,] b, Position start, short side, int colour)
    {
        short jump = (short)(-1 * PieceCode.Pawn * colour);
        if (((start.y + side) % BoardLength > 0 || start.y + side == 0) && lastMove[1]!=null && b[start.x,start.y + side] == jump && lastMove[1].x == start.x && lastMove[1].y == start.y + side && lastMove[0].x == start.x - (2 * colour)) return true;

        return false;
    }
}
