using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager
{
    Queue<Action> actions;
    Action currentAction;
    public ActionManager()
    {
        actions = new Queue<Action>();
        currentAction = null;
    }

    public void AddAction(Action action)
    {
        actions.Enqueue(action);
    }
    public void DoingActions()
    {
        if (actions.Count == 0 && currentAction==null) return;
        if(currentAction==null)
        {
            currentAction = actions.Dequeue();
            currentAction.StartAction();
        }
        currentAction.DoAction();
        if (currentAction.isFinished())
            currentAction = null;
    }
    public bool AreActionsFinished()
    {
        if (actions.Count == 0 && currentAction ==null) return true;
        return false;
    }
}
