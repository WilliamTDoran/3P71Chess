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

        short[,] zeroedConfiguration = new short[,] { {    0,    0,    0,    0,    0,    0,    0,    0 },
                                                      {    0,    0,    0,    0,    0,    0,    0,    0 },
                                                      {    0,    0,    0,    0,    0,    0,    0,    0 },
                                                      {    0,    0,    0,    0,    0,    0,    0,    0 },
                                                      {    0,    0,    0,    0,    0,    0,    0,    0 },
                                                      {    0,    0,    0,    0,    0,    0,    0,    0 },
                                                      {    0,    0,    0,    0,    0,    0,    0,    0 },
                                                      {    0,    0,    0,    0,    0,    0,    0,    0 } };

        Transform[] childTransforms = GetComponentsInChildren<Transform>();
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
                GameObject inputSection = backing.GetChild(i + 1).gameObject;
                InputField thisInput = inputSection.GetComponent<InputField>();

                TextMeshPro placeholder = inputSection.transform.GetChild(0).GetChild(0).GetComponent<TextMeshPro>();
                TextMeshPro liveText = inputSection.transform.GetChild(0).GetChild(1).GetComponent<TextMeshPro>();

                thisInput.placeholder = placeholder;

                if (liveText.text.Length == 1)
                {
                    char x = char.Parse(liveText.text);
                    int y = x > 8 ? x - 66 : x - 1;

                    position[j] = (short)y;
                }
            }

            short[] lookup = new short[] { -50, -30, -32, -90, -900, -32, -30, -50, -10, -10, -10, -10, -10, -10, -10, -10, 
                                            10, 10, 10, 10, 10, 10, 10, 10, 50, 30, 32, 90, 900, 32, 30, 50 };

            zeroedConfiguration[position[0], position[1]] = lookup[i];
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
