using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Action 
{
    public abstract void DoAction();
    public abstract bool isFinished();
    public virtual void StartAction() { }
    protected virtual void FinishAction() { }
    public virtual void Print() { }
}
