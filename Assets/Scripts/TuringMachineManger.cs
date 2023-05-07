using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TuringMachineManger : MonoBehaviour
{
    #region Object References
    [Header("References")]
    [SerializeField]
    private TapeManager slotManager;

    [SerializeField]
    private Transform tmHead;

    [SerializeField]
    private Animation tmhHeadAnim;
    #endregion

    #region User Interface
    [Space(2)]
    [Header("UI")]
    [SerializeField]
    private Button loadTMButton;

    [SerializeField]
    private Button calculateSBSButton;

    [SerializeField]
    private Button calculateFastButton;

    [SerializeField]
    private TMP_InputField inputField;

    [SerializeField]
    private Material accepted, declined;

    [SerializeField]
    private MeshRenderer tmHeadRenderer;

    [SerializeField]
    private TextMeshPro stateText;
    #endregion

    #region Settings
    [Space(2)]
    [Header("Settings")]
    [Range(1f,5f)]
    [SerializeField]
    private float waitTime;
    [SerializeField]
    private int statesAmount;
    [SerializeField]
    private float tmHeadSpeed = 2;
    #endregion

    #region Turing Machine Vars
    private Dictionary<int, State> states;
    private State currentState;
    private State acceptedState;
    private bool isTMInitialized = false;
    public int CurrentSlotIndex { private set; get; } = 14;
    private Vector3 currentTargetPosition;
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

        //Initialize Position
        currentTargetPosition = tmHead.position;

        //Initialize States
        InitEmptyStates();

        states.TryGetValue(1, out currentState); //x1 as start state
        states.TryGetValue(2, out acceptedState); //x2 as accepted state
    }

    private void Update()
    {
        if (Vector3.Distance(tmHead.position,currentTargetPosition) > 0.05f)
        {
            tmHead.position = Vector3.Lerp(tmHead.position, currentTargetPosition, Time.deltaTime * tmHeadSpeed);
        }
    }

    private void OnDestroy()
    {
        loadTMButton.onClick.RemoveListener(LoadTM);
        calculateSBSButton.onClick.RemoveListener(CalculateSBS);
        calculateFastButton.onClick.RemoveListener(CalculateFast);
    }
    #endregion

    #region Method Behaviour
    private void InitEmptyStates()
    {
        states = new Dictionary<int, State>();
        for (int i = 0; i < statesAmount; i++)
        {
            states.Add(i + 1, new State(i + 1));
        }
    }

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
        bool isDirDefined = false; // helper var, cant tell otherwise


        for (int i = 0; i < input.Length; i++)
        {
            if (input.Substring(i,3) == "111")
            {
                //checks if transition is defined
                if (!CheckTransitionDefined(stateFrom, read, stateTo, write, isDirDefined)) return;

                //return finished transition if defined
                states[stateFrom].AddTransition(read, new Transition(stateTo, write, isRigh));

                //gives the rest input to the slot manager
                slotManager.EnterInputMiddle(input.Substring(i+3)); 
                //defines TM as initialized for calculation
                isTMInitialized = true;
                return;
            }
            else if (input.Substring(i, 2) == "11") //sets calculated transition to state
            {
                //checks if transition is defined
                if (!CheckTransitionDefined(stateFrom, read, stateTo, write, isDirDefined)) return;

                //return finished transition if defined
                states[stateFrom].AddTransition(read, new Transition(stateTo, write, isRigh));

                //prepares next transition
                stateFrom = -1; //qn
                read = string.Empty; //xm
                stateTo = -1; // qj
                write = string.Empty; //xj
                isRigh = false; //dk
                isDirDefined = false;
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
                    if (!slotManager.XnPairs.TryGetValue(zeros, out write))
                    {
                        Debug.LogError("Write symbol is not defined!");
                        return;
                    }

                    i += zeros;
                }
                else if (isDirDefined == false)
                {
                    int zeros = CountZeros(input.Substring(i + 1));
                    if (zeros == 1)
                    {
                        isRigh = false;
                        isDirDefined = true;
                    }
                    else if (zeros == 2)
                    {
                        isRigh = true;
                        isDirDefined = true;
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

    private bool CheckTransitionDefined(int stateFrom, string read, int stateTo, string write, bool isDirDefined)
    {
        //checks if transition is defined
        if (stateFrom == -1 || read == string.Empty || stateTo == -1 || write == string.Empty || isDirDefined == false)
        {
            Debug.LogErrorFormat($"Cant initialize! At least one transition parameter wasn't defined correctly!\n stateFrom: {0}\n read: {1}\n stateTo: {2}\n write: {3}\n isDirDefined: {4}\n", stateFrom, read, stateTo, write, isDirDefined);
            return false;
        }
        return true;
    }

    private int CountZeros(string input)
    {
        if (input.Length < 1)
        {
            Debug.LogError("input string is too small!! can't count zeros");
            return -1;
        }

        if (input[0] != '0')
        {
            Debug.LogError("Input starts with invalid char! Can't count zeros!");
            return -1;
        }

        int zeros = 0;
        while (input[zeros] != '1')
        {
            if (input[zeros] != '0')
            {
                Debug.LogErrorFormat($"Illegal char found in TM Code: {0}\n Must be 0 or 1", input[zeros]);
            }
            zeros++;
        }

        return zeros;
    }

    private void CalculateSBS()
    {
        if (!isTMInitialized)
        {
            Debug.LogWarning("TM not initialized");
            return;
        }

        StartCoroutine(StepByStep(waitTime));
    }

    private void CalculateFast()
    {
        if (!isTMInitialized)
        {
            Debug.LogWarning("TM not initialized");
            return;
        }

        string readTapeSymbol;
        Transition transitionData;

        while (true)
        {
            //Read from tape
            readTapeSymbol = slotManager.Read(CurrentSlotIndex);

            //Try to find next state
            transitionData = currentState.GetNextMove(readTapeSymbol);
            int nextStateIndex = transitionData.State;

            //evaluate if it is the last move
            if (nextStateIndex == -1)
            {
                Debug.Log("StateMachine Ended!");
                break;
            }

            //decide next state if not last
            State nextState;
            if (!states.TryGetValue(nextStateIndex, out nextState))
            {
                Debug.LogError("State does not exist and is not final state!");
                break;
            }
            currentState = nextState;

            //evaluate write tapeSymbol
            string writeTapeSymbol = transitionData.Write;
            if (writeTapeSymbol == string.Empty)
            {
                Debug.Log("Write string was empty!");
                break;
            }
            slotManager.Write(CurrentSlotIndex, writeTapeSymbol);

            UpdatePosition(transitionData.MoveRight);

            UpdateVisuals();
        }
    }

    private IEnumerator StepByStep(float waitTime)
    {
        string readTapeSymbol;
        Transition transitionData;

        while (true)
        {
            //Read from tape
            readTapeSymbol = slotManager.Read(CurrentSlotIndex);

            //Try to find next state
            transitionData = currentState.GetNextMove(readTapeSymbol);
            int nextStateIndex = transitionData.State;

            //evaluate if it is the last move
            if (nextStateIndex == -1)
            {
                Debug.Log("StateMachine Ended!");
                break;
            }

            //decide next state if not last
            State nextState;
            if (!states.TryGetValue(nextStateIndex, out nextState))
            {
                Debug.LogError("State does not exist and is not final state!");
                break;
            }
            currentState = nextState;

            //evaluate write tapeSymbol
            string writeTapeSymbol = transitionData.Write;
            if (writeTapeSymbol == string.Empty)
            {
                Debug.Log("Write string was empty!");
                break;
            }
            slotManager.Write(CurrentSlotIndex, writeTapeSymbol);
            
            tmhHeadAnim.Play();

            UpdatePosition(transitionData.MoveRight);

            UpdateVisuals();

            yield return new WaitForSecondsRealtime(waitTime);
        }
    }

    private void UpdatePosition(bool movingRight)
    {
        if (movingRight)
        {
            CurrentSlotIndex++;
            currentTargetPosition += new Vector3(-1f, 0, 0);
            return;
        }

        CurrentSlotIndex--;
        currentTargetPosition += new Vector3(1f, 0, 0);
    }

    private void UpdateVisuals()
    {
        if (currentState.StateNr == acceptedState.StateNr)
        {
            tmHeadRenderer.material = accepted;
            stateText.text = "q" + currentState.StateNr;
            return;
        }

        tmHeadRenderer.material = declined;
        stateText.text = "q" + currentState.StateNr;
    }
    #endregion
}
