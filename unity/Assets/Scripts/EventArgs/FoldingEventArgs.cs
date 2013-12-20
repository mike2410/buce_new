using UnityEngine;
using System;
using System.Collections;

public class FoldingEventArgs : EventArgs
{

    public Enum currentState;
    public Enum previousState;

    public FoldingEventArgs(Enum currentState, Enum previousState)
    {
        this.currentState = currentState;
        this.previousState = previousState;
    }
}