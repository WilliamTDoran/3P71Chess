using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class AllPieces : MonoBehaviour
{
    private static AllPieces instance;
    public static AllPieces Instance { get { return instance; } }


    public void Awake()
    {
        instance = this;
    }

    [SerializeField]
    public ColourPieces blackPieces;
    [SerializeField]
    public ColourPieces whitePieces;

    internal void UpdateBoard()
    {
        blackPieces.UpdateBoard(PieceCode.black);
        //Debug.Log("Black Pieces drawn");
        whitePieces.UpdateBoard(PieceCode.white);
        //Debug.Log("White Pieces drawn");
    }
}
