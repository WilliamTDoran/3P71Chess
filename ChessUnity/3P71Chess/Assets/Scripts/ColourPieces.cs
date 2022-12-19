using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceCode
{
    public static int Pawn = 10;
    public static int Knight = 30;
    public static int Bishop = 32;
    public static int Rook = 50;
    public static int Queen = 90;
    public static int King = 900;

    public static int black = -1;
    public static int white = 1;
}

[System.Serializable]
public class PieceArrays
{
    [SerializeField]
    private GameObject[] Pieces;
    public int length { get { return Pieces.Length; } }
    public GameObject get(int i)
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

    public int arrayLocation(int piece)
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

    internal void UpdateBoard(int thisColour)
    {
        //disable all pieces
        for (int i = 0; i < Pieces.Length; i++)
        {
            for (int j = 0; j < Pieces[i].length; j++)
            {
                Pieces[i].get(j).SetActive(false);
            }
        }

        //For every spot on the board
        for (int i = 0; i < BoardState.BoardLength; i++)
        {
            for (int j = 0; j < BoardState.BoardLength; j++)
            {
                //If it is a piece & matches the colour
                int p = BoardState.Instance.getPiece(new Position(i, j));
                if (GetPieceColour(p) == thisColour)
                {
                    //Move next disabled piece of that type to it's position & set to active
                    GameObject placing;
                    for (int k=0; k<Pieces[arrayLocation(p)].length; k++)
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
    internal static int GetPieceColour(int p)
    {
        if (p == 0) return 0;
        return p / Math.Abs(p);
    }
}
