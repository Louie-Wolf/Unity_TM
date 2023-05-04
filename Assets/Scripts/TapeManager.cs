using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TapeManager : MonoBehaviour
{
    #region Settings and References
    [Header("Settings and References")]
    [SerializeField]
    private TuringMachineManger tmManager;

    [SerializeField]
    private TextMeshPro[] slots;

    [SerializeField]
    private string[] xn;

    [SerializeField]
    private int tapeLength;
    #endregion

    public Dictionary<int, string> XnPairs { private set; get; }

    private void Awake()
    {
        //tape empty default
        foreach (TextMeshPro slot in slots)
        {
            slot.text = "_";
        }

        //converts amout of zeros to symbol for every element in array
        XnPairs = new Dictionary<int, string>();
        for (int i = 0; i < xn.Length; i++)
        {
            XnPairs.Add(i + 1, xn[i]);
        }
    }

    /// <summary>
    /// Enters the tape input in the middle of the tape.
    /// </summary>
    /// <param name="input"> is limited to the tapesize</param>
    public void EnterInputMiddle(string input)
    {
        int currentSlotIndex = tmManager.CurrentSlotIndex;
        Debug.LogFormat($"Starting Index on TapeManger is: {0}", currentSlotIndex);

        string[] symbols = ConvertSymbols(input);
        
        if (symbols.Length > tapeLength - currentSlotIndex)
        {
            Debug.LogError("Length exceeded the maximum space! Use other method for longer input");
        }
        int index = currentSlotIndex;
        foreach (string symbol in symbols)
        {
            slots[index].text = symbol;
            index++;
        }
    }

    private string[] ConvertSymbols(string input)
    {
        List<string> convertedSymbols = new ();

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

    public string Read(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length)
        {
            Debug.LogError("Tried to Read from non exisiting slot!");
            return string.Empty;
        }

        return slots[slotIndex].text;
    }

    public void Write(int slotIndex, string write)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length)
        {
            Debug.LogError("Tried to Read from non exisiting slot!");
            return;
        }

        slots[slotIndex].text = write;
    }
}
