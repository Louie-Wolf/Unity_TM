using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TuringMachineManger : MonoBehaviour
{
    #region Object References
    [SerializeField]
    private SlotManager slotManager;

    [SerializeField]
    private Transform tmHead;
    #endregion

    #region User Interface
    [SerializeField]
    private Button loadTMButton;

    [SerializeField]
    private Button calculateSBSButton;

    [SerializeField]
    private Button calculateFastButton;

    [SerializeField]
    private InputField inputField;

    [SerializeField]
    private Material accepted, declined;

    [SerializeField]
    private MeshRenderer tmHeadRenderer;
    #endregion

    #region Turing Machine Vars
    private List<State> states;
    private State currentState;
    private bool isTMInitialized = false;
    #endregion

    #region Unity Callables
    private void Awake()
    {
        //Initialize Buttons
        loadTMButton.onClick.AddListener(LoadTM);
        calculateSBSButton.onClick.AddListener(CalculateSBS);
        calculateFastButton.onClick.AddListener(CalculateFast);

        //Initialize Graphics
        tmHeadRenderer.material = declined;

        //Initialize States
        states = new List<State>();
        states.Add(new State(1)); //x1 start state
        states.Add(new State(2)); //x2 accepting state
        states.Add(new State(3)); //x3 state
        states.Add(new State(4)); //x4 state
        states.Add(new State(5)); //x5 state
        states.Add(new State(6)); //x6 state
        states.Add(new State(7)); //x7 state
        states.Add(new State(8)); //x8 state

        currentState = states[0]; //x1 as start state

    }

    private void OnDestroy()
    {
        loadTMButton.onClick.RemoveListener(LoadTM);
        calculateSBSButton.onClick.RemoveListener(CalculateSBS);
        calculateFastButton.onClick.RemoveListener(CalculateFast);
    }
    #endregion
    
    private void LoadTM()
    {
        string input = inputField.text;

        if (input[0] != '1')
        {
            Debug.LogError("Invalid Input!!! Has to start with 1");
            return;
        }

        //(qn, xm) -> (qj, xj, dk)
        int stateFrom = -1; //qn
        string read = string.Empty; //xm
        int stateTo = -1; // qj
        string write = string.Empty; //xj
        bool isRigh = false; //dk
        bool isDirDefiend = false; // helper var, cant tell otherwise


        for (int i = 0; i < input.Length; i++)
        {
            if (input.Substring(i,3) == "111")
            {
                //gives the rest input to the slot manager
                slotManager.EnterInputMiddle(input.Substring(i+3)); 
                //defines TM as initialized for calculation
                isTMInitialized = true;
                return;
            }
            else if (input.Substring(i, 2) == "11") //sets calculated transition to state
            {
                //checks if transition is defined
                if (stateFrom == -1 || read == string.Empty || stateTo == -1 || write == string.Empty || isDirDefiend == false)
                {
                    Debug.LogError("Cant initialize! At least one transition var wasn't defined correctly!");
                    return;
                }

                //return finished transition if defined
                states[stateFrom].AddTransition(read, new Transition(stateTo, write, isRigh));

                //prepares next transition
                stateFrom = -1; //qn
                read = string.Empty; //xm
                stateTo = -1; // qj
                write = string.Empty; //xj
                isRigh = false; //dk
                isDirDefiend = false;
            }
            else if (input[i] == '1') //reads next transition var
            {
                
                if (stateFrom == -1)
                {
                    int zeros = CountZeros(input.Substring(i + 1));
                    if (zeros == -1)
                    {
                        Debug.LogError("StateFrom symbol is not defined!");
                        return;
                    }

                    stateFrom = zeros;

                    i += zeros;
                }
                else if (read == string.Empty)
                {
                    int zeros = CountZeros(input.Substring(i + 1));
                    if (!slotManager.XnPairs.TryGetValue(zeros, out read))
                    {
                        Debug.LogError("Read symbol is not defined!");
                        return;
                    }
                    
                    i += zeros;
                }
                else if (stateTo == -1)
                {
                    int zeros = CountZeros(input.Substring(i + 1));
                    if (zeros == -1)
                    {
                        Debug.LogError("StateTo symbol is not defined!");
                        return;
                    }
                    stateTo = zeros;

                    i += zeros;
                }
                else if (write == string.Empty)
                {
                    int zeros = CountZeros(input.Substring(i + 1));
                    if (!slotManager.XnPairs.TryGetValue(zeros, out read))
                    {
                        Debug.LogError("Write symbol is not defined!");
                        return;
                    }

                    i += zeros;
                }
                else if (isDirDefiend == false)
                {
                    int zeros = CountZeros(input.Substring(i + 1));
                    if (zeros == 0)
                    {
                        isRigh = false;
                        isDirDefiend = true;
                    }
                    else if (zeros == 1)
                    {
                        isRigh = true;
                        isDirDefiend = true;
                    }
                    else
                    {
                        Debug.LogError("Direction symbol is not defined!");
                        return;
                    }

                    i += zeros;
                }
            }
        }
    }

    private int CountZeros(string input)
    {
        if (input.Length < 1)
        {
            Debug.LogError("input string is too small!! can't count zeros");
            return -1;
        }

        if (input[0] == '1')
        {
            Debug.LogError("Input starts with 1! Can't count zeros!");
            return -1;
        }

        int zeros = 0;
        while (input[zeros] != '1')
        {
            zeros++;
        }

        return zeros;
    }

    private void CalculateSBS()
    {

    }

    private void CalculateFast()
    {

    }
}
