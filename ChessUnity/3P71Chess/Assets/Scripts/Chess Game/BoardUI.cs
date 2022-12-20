using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardUI : MonoBehaviour
{
    [SerializeField]
    private short x;
    [SerializeField]
    private short y;

    public void clicked()
    {
        if (BoardState.Instance.canPlay)
        {
            short[] pieceSelected = BoardState.Instance.pieceSelected;

            Debug.Log("Selected Piece " + x + "," + y + " is " + BoardState.Instance.getPiece(x, y));
            // no piece selected                                        Piece is player's who's turn it is
            if ((pieceSelected[0] == -1 || pieceSelected[1] == -1) && BoardState.Instance.getPiece(x, y) * pieceSelected[2] > 0)
            {
                pieceSelected[0] = x;
                pieceSelected[1] = y;
                BoardState.Instance.pieceSelected = pieceSelected;
            }
            else if ((pieceSelected[0] != -1 && pieceSelected[1] != -1)) // Piece has been selected
            {
                if (BoardState.move(ref BoardState.Instance.board, pieceSelected[0], pieceSelected[1], x, y, BoardState.Instance.pieceSelected[2]))
                {
                    //Debug.Log("Piece " + pieceSelected[0] + "," + pieceSelected[1] + " moved to " + x + "," + y);
                    pieceSelected[0] = -1;
                    pieceSelected[1] = -1;
                }
                else
                {
                    //Debug.Log("Not valid move");
                    pieceSelected[0] = -1;
                    pieceSelected[1] = -1;
                }
            }
            else
            {
                //Debug.Log("Input on square "+x+","+y);
            }
        }
    }
}
