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
    [SerializeField]
    private float waitTime;
    [SerializeField]
    private int statesAmount;

    #region Turing Machine Vars
    private Dictionary<int, State> states;
    private State currentState;
    private bool isTMInitialized = false;
    public static int CurrentSlotIndex { private set; get; } = 14;
    private Vector3 currentTargetPosition;
    #endregion

    
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
        states = new Dictionary<int, State>();
        states.Add(1,new State(1)); //x1 start state
        states.Add(2,new State(2)); //x2 accepting state
        states.Add(3,new State(3)); //x3 state
        states.Add(4,new State(4)); //x4 state
        states.Add(5,new State(5)); //x5 state
        states.Add(6,new State(6)); //x6 state
        states.Add(7,new State(7)); //x7 state
        states.Add(8,new State(8)); //x8 state
        states.Add(9, new State(9)); //x8 state
        states.Add(10, new State(10)); //x8 state
        states.Add(11, new State(11)); //x8 state
        states.Add(12, new State(12)); //x8 state
        states.Add(13, new State(13)); //x8 state
        states.Add(14, new State(14)); //x8 state
        states.Add(15, new State(15)); //x8 state
        states.Add(16, new State(16)); //x8 state
        states.Add(17, new State(17)); //x8 state
        states.Add(18, new State(18)); //x8 state
        states.Add(19, new State(19)); //x8 state
        states.Add(20, new State(20)); //x8 state
        states.Add(21, new State(21)); //x8 state

        states.TryGetValue(1, out currentState);//x1 as start stat
    }

    private void Update()
    {
        if (Vector3.Distance(tmHead.position,currentTargetPosition) > 0.05f)
        {
            tmHead.position = Vector3.Lerp(tmHead.position, currentTargetPosition, Time.deltaTime * 2);
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
                //checks if transition is defined
                if (stateFrom == -1 || read == string.Empty || stateTo == -1 || write == string.Empty || isDirDefiend == false)
                {
                    Debug.LogError("Cant initialize! At least one transition var wasn't defined correctly!");
                    return;
                }

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
                    if (!slotManager.XnPairs.TryGetValue(zeros, out write))
                    {
                        Debug.LogError("Write symbol is not defined!");
                        return;
                    }

                    i += zeros;
                }
                else if (isDirDefiend == false)
                {
                    int zeros = CountZeros(input.Substring(i + 1));
                    if (zeros == 1)
                    {
                        isRigh = false;
                        isDirDefiend = true;
                    }
                    else if (zeros == 2)
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
                return;
            }
            //decide next state if not last
            State nextState;
            if (!states.TryGetValue(nextStateIndex, out nextState))
            {
                Debug.LogError("State does not exist and is not final state!");
                return;
            }
            currentState = nextState;

            //evaluate write tapeSymbol
            string writeTapeSymbol = transitionData.Write;
            if (writeTapeSymbol == string.Empty)
            {
                Debug.Log("Write string was empty!");
                return;
            }
            slotManager.Write(CurrentSlotIndex, writeTapeSymbol);

            //evaluate direction to move
            bool movingRight = transitionData.MoveRight;
            if (movingRight)
            {
                CurrentSlotIndex++;
                currentTargetPosition += new Vector3(-1f, 0, 0);
            }
            else
            {
                tmHead.Translate(1f, 0, 0);
                currentTargetPosition += new Vector3(1f, 0, 0);
            }

            //Changes Color
            if (currentState.StateNr == 2)
            {
                tmHeadRenderer.material = accepted;
                stateText.text = "q" + currentState.StateNr;
            }
            else
            {
                tmHeadRenderer.material = declined;
                stateText.text = "q" + currentState.StateNr;
            }
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

            //evaluate direction to move
            bool movingRight = transitionData.MoveRight;
            if (movingRight)
            {
                CurrentSlotIndex++;
                currentTargetPosition += new Vector3(-1f, 0, 0);
            }
            else
            {
                CurrentSlotIndex--;
                currentTargetPosition += new Vector3(1f, 0, 0);

            }
            //Changes Color
            if (currentState.StateNr == 2)
            {
                tmHeadRenderer.material = accepted;
                stateText.text = "q" + currentState.StateNr;
            }
            else
            {
                tmHeadRenderer.material = declined;
                stateText.text = "q" + currentState.StateNr;
            }

            yield return new WaitForSecondsRealtime(waitTime);
        }
    }
    #endregion
}
