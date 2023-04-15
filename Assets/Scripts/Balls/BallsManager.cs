using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallsManager : MonoBehaviour
{
    List<Ball> balls;
    ActionManager actions;
    bool startActions;
    public delegate void FinishActions();
    public event FinishActions finishActions;
    private void Awake()
    {
        balls = new List<Ball>();
        actions = new ActionManager();
        Ball.ballDestroyed += ballRemoved;
    }
    public void AddAction(Action action)
    {
        //action.Print();
        actions.AddAction(action);
    }
    public void StartDoingActions()
    {
        startActions = true;
    }
    public void AddBall(Ball ball)
    {
        balls.Add(ball);
    }
    void ballRemoved(Ball ball)
    {
        balls.Remove(ball);
    }
    private void OnDestroy()
    {
        Ball.ballDestroyed -= ballRemoved;
    }
    public void RemoveCeilling()
    {
        foreach (var ball in balls)
        {
            ball.attachedCeilling = false;
        }
    }
    public void UpdateRigidBody()
    {
        foreach (Ball ball in balls)
        {
            ball.UpdateRb();
        }
    }
  
    private void Update()
    {
        if (startActions)
            actions.DoingActions();
        if (actions.AreActionsFinished() && startActions)
        {
            finishActions?.Invoke();
            startActions = false;
        }
            
    }

}
