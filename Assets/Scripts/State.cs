using System.Collections.Generic;
using UnityEngine;

public class State
{
    private Dictionary<string, Transition> transitions = new Dictionary<string, Transition>();
    public int StateNr { private set; get; }

    public State(int stateNr)
    {
        this.StateNr = stateNr;
    }

    public void AddTransition(string input, Transition transition)
    {
        transitions.Add(input, transition);
    }

    public Transition GetNextMove(string input)
    {
        Transition transition;

        if (!transitions.TryGetValue(input, out transition))
        {
            Debug.Log("Wasn't able to get a Transition -> end?");
            return new Transition(-1, string.Empty, false);
        }

        return transition;
    }
}
