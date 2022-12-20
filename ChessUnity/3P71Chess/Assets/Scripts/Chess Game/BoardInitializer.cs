using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardInitializer : MonoBehaviour
{
    internal bool custom = false;

    [SerializeField]
    private Button customButton;
    [SerializeField]
    private Button backButton;
    [SerializeField]
    private GameObject configMenu;

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


    public void EnableCustom()
    {
        custom = true;
        customButton.gameObject.SetActive(false);
        configMenu.SetActive(true);
    }

    public void DisableCustom()
    {
        custom = false;
        customButton.gameObject.SetActive(true);
        configMenu.SetActive(false);
    }

}
