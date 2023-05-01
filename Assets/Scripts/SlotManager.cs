using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SlotManager : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro[] slots;

    [SerializeField]
    private string[] xn;

    public Dictionary<int, string> XnPairs { private set; get;}

    private void Awake()
    {
        XnPairs = new Dictionary<int, string>();
    
        int index = 0;
        while (xn[index] != null)
        {
            XnPairs.Add(index + 1, xn[index]);
            index++;
        }
    }

    public void EnterInputMiddle(string input)
    {
        string[] symbols = convertSymbols(input);
        if (symbols.Length > 15)
        {
            Debug.LogError("Length exceeded 15! Use other method for longer input");
        }

        for (int i = 15; i < symbols.Length; i++)
        {
            slots[i].text = symbols[i];
        }
    }

    private string[] convertSymbols(string input)
    {
        List<string> convertedSymbols = new List<string>();
        int zeroAmount = 0;
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == '0')
            {
                zeroAmount++;
            }
            else if (input[i] == '1')
            {
                string symbol;
                if (!XnPairs.TryGetValue(zeroAmount, out symbol))
                {
                    Debug.LogError("Symbol was not defined! Cant convert!");
                }
                
                convertedSymbols.Add(symbol);
                zeroAmount = 0;
            }
            else
            {
                Debug.LogError("INVALID Symbol while converting!");
            }
        }
        return convertedSymbols.ToArray();
    }
}
