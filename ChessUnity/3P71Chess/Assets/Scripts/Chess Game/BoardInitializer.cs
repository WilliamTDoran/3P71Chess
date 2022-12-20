using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField]
    private Transform squaresBasket;
    [SerializeField]
    private BoardState bs;

    short[,] defaultConfiguration = new short[,] { {  -50,  -30,  -32,  -90, -900,  -32,  -30,  -50 },
                                                   {  -10,  -10,  -10,  -10,  -10,  -10,  -10,  -10 },
                                                   {    0,    0,    0,    0,    0,    0,    0,    0 },
                                                   {    0,    0,    0,    0,    0,    0,    0,    0 },
                                                   {    0,    0,    0,    0,    0,    0,    0,    0 },
                                                   {    0,    0,    0,    0,    0,    0,    0,    0 },
                                                   {   10,   10,   10,   10,   10,   10,   10,   10 },
                                                   {   50,   30,   32,   90,  900,   32,   30,   50 } };

    public void Start()
    {
        Transform[] childTransforms = new Transform[32];

        for (int i = 0; i < 32; i++)
        {
            childTransforms[i] = squaresBasket.GetChild(i);
        }

        List<GameObject> inputSquares = new List<GameObject>();

        foreach (Transform child in childTransforms)
        {
            inputSquares.Add(child.gameObject);
        }

        for (int i = 0; i < 32; i++)
        {
            Transform backing = inputSquares[i].transform.GetChild(0);

            for (int j = 0; j < 2; j++)
            {
                GameObject inputSection = backing.GetChild(j + 1).gameObject;
                TMP_InputField thisInput = inputSection.GetComponent<TMP_InputField>();

                TextMeshProUGUI placeholder = inputSection.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();

                thisInput.placeholder = placeholder;
            }
        }
    }

    public short[,] LoadBoard()
    {
        if (!custom)
        {
            return defaultConfiguration;
        }

        short[,] zeroedConfiguration = new short[,] { {    0,    0,    0,    0,    0,    0,    0,    0 },
                                                      {    0,    0,    0,    0,    0,    0,    0,    0 },
                                                      {    0,    0,    0,    0,    0,    0,    0,    0 },
                                                      {    0,    0,    0,    0,    0,    0,    0,    0 },
                                                      {    0,    0,    0,    0,    0,    0,    0,    0 },
                                                      {    0,    0,    0,    0,    0,    0,    0,    0 },
                                                      {    0,    0,    0,    0,    0,    0,    0,    0 },
                                                      {    0,    0,    0,    0,    0,    0,    0,    0 } };

        Transform[] childTransforms = new Transform[32];

        for (int i = 0; i < 32; i++)
        {
            childTransforms[i] = squaresBasket.GetChild(i);
        }

        List<GameObject> inputSquares = new List<GameObject>();

        foreach (Transform child in childTransforms)
        {
            inputSquares.Add(child.gameObject);
        }

        for (int i = 0; i < 32; i ++)
        {
            Transform backing = inputSquares[i].transform.GetChild(0);

            short [] position = new short[2];

            for (int j = 0; j < 2; j++)
            {
                GameObject inputSection = backing.GetChild(j + 1).gameObject;
                TMP_InputField thisInput = inputSection.GetComponent<TMP_InputField>();

                TextMeshProUGUI placeholder = inputSection.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI liveText = inputSection.transform.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();

                string preX = liveText.text.ToString().ToUpper();
                string placeX = placeholder.text.ToString().ToUpper();

                if (preX.Length > 1)
                {
                    char x = preX[0];
                    int y = (int)(x - 48);
                    y = y > 8 ? (y - 17) : (y - 1);

                    position[j] = (short)y;
                }
                else
                {
                    char x = placeX[0];
                    int y = (int)(x - 48);
                    y = y > 8 ? (y - 17) : (y - 1);

                    position[j] = (short)y;
                }
            }

            short[] lookup = new short[] { -50, -30, -32, -90, -900, -32, -30, -50, -10, -10, -10, -10, -10, -10, -10, -10, 
                                            10, 10, 10, 10, 10, 10, 10, 10, 50, 30, 32, 90, 900, 32, 30, 50 };

            zeroedConfiguration[7 - position[1], position[0]] = lookup[i];
        }

        return zeroedConfiguration;
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

    public void UpdateBoard()
    {
        Debug.Log("Setup starting");
        bs.newBoard();
        Debug.Log("Board defined");
        AllPieces.Instance.UpdateBoard();
        Debug.Log("Setup done");
    }

}
