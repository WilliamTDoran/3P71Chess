using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal abstract class Piece : MonoBehaviour
{
    internal Position pos; // [x, y]

    internal abstract Position[] availablePos(); // list of available positions to move to


}
