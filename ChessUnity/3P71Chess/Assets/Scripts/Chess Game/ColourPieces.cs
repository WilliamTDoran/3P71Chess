using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceCode
{
    public static short Pawn = 10;
    public static short Knight = 30;
    public static short Bishop = 32;
    public static short Rook = 50;
    public static short Queen = 90;
    public static short King = 900;

    public static short black = -1;
    public static short white = 1;
}

[System.Serializable]
public class PieceArrays
{
    [SerializeField]
    private GameObject[] Pieces;
    public short length { get { return (short)Pieces.Length; } }
    public GameObject get(short i)
    {
        return Pieces[i];
    }
}


[System.Serializable]
public class ColourPieces
{
    [SerializeField]
    [Tooltip("Top array length: 6. Piece order: Pawns, Bishops, Knights, Rooks, Queens")]
    public PieceArrays[] Pieces;
    public bool canCastleQueen;
    public bool canCastleKing;

    public short arrayLocation(short piece)
    {
        piece = Math.Abs(piece);
        if (piece == 0)
        {
            Debug.Log("Blank piece.");
            throw new Exception();
        }
        if (piece == PieceCode.Pawn) return 0;
        if (piece == PieceCode.Bishop) return 1;
        if (piece == PieceCode.Knight) return 2;
        if (piece == PieceCode.Rook) return 3;
        if (piece == PieceCode.Queen) return 4;
        if (piece == PieceCode.King) return 5;
        Debug.Log("Invalid piece code.");
        throw new Exception();
    }

    internal void UpdateBoard(short thisColour)
    {
        //disable all pieces
        for (short i = 0; i < Pieces.Length; i++)
        {
            for (short j = 0; j < Pieces[i].length; j++)
            {
                Pieces[i].get(j).SetActive(false);
            }
        }

        //For every spot on the board
        for (short i = 0; i < BoardState.BoardLength; i++)
        {
            for (short j = 0; j < BoardState.BoardLength; j++)
            {
                //If it is a piece & matches the colour
                short p = BoardState.Instance.getPiece(new Position(i, j));
                if (GetPieceColour(p) == thisColour)
                {
                    //Move next disabled piece of that type to it's position & set to active
                    GameObject placing;
                    for (short k=0; k<Pieces[arrayLocation(p)].length; k++)
                    {
                        placing = Pieces[arrayLocation(p)].get(k);
                        if (!placing.activeInHierarchy)
                        {
                            placing.transform.position = new Vector2(j, BoardState.BoardLength-i-1);

                            placing.SetActive(true);
                            break;
                        }
                    }
                }
            }
        }
    }

    // returns 0 if no piece, and +-1 depending on the colour
    internal static short GetPieceColour(short p)
    {
        if (p == 0) return 0;
        return (short)(p / Math.Abs(p));
    }
}
