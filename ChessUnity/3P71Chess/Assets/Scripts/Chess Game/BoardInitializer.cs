using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardInitializer : MonoBehaviour
{
    internal bool custom = false;

    short[,] defaultConfiguration = new short[,] { {  -50,  -30,  -32,  -90, -900,  -32,  -30,  -50 },
                                                   {  -10,  -10,  -10,  -10,  -10,  -10,  -10,  -10 },
                                                   {    0,    0,    0,    0,    0,    0,    0,    0 },
                                                   {    0,    0,    0,    0,    0,    0,    0,    0 },
                                                   {    0,    0,    0,    0,    0,    0,    0,    0 },
                                                   {    0,    0,    0,    0,    0,    0,    0,    0 },
                                                   {   10,   10,   10,   10,   10,   10,   10,   10 },
                                                   {   50,   30,   32,   90,  900,   32,   30,   50 } };

    public short[,] LoadBoard()
    {
        if (!custom)
        {
            return defaultConfiguration;
        }

        return null;
    }
}
