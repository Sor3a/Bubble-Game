using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : Action
{
    Ball start;
    Ball destination;
    float speed;
    Vector2 offset;
    public override void Print()
    {
        Debug.Log(start.name + " going to " + destination.name);
    }
    public MoveAction(Ball start,Ball destination,float speed)
    {
        this.start = start;
        this.destination = destination;
        this.speed = speed;
        Ball.ballDestroyed += OnDestinationDestroyed;
        offset = new Vector2(0.1f, -0.8f);
    }
    void OnDestinationDestroyed(Ball destination)
    {
        if(destination == this.destination)
        {
            this.destination = destination.MergerdDestination;
        }
    }
    public override void StartAction()
    {
        start.source.Play();
        start.CircleCollider.isTrigger = true;
    }
    public override void DoAction()
    {
        if (start && destination)
        {
            Vector2 direction = (destination.transform.position - start.transform.position).normalized;
            start.transform.Translate(direction * Time.deltaTime * speed);
            if (Vector2.Distance(destination.transform.position, start.transform.position) < 0.1f && !start.source.isPlaying)
            {
                FinishAction();
            }
        }
        else
            FinishAction();
  
    }

    protected override void FinishAction()
    {
        if(destination && start)
        destination.UpdateNumber(start.number);
        //MonoBehaviour.Destroy(start.gameObject);
        if(start.gameObject.activeSelf==true)
        {
           // Debug.Log("a");
            
            Announcer.Instance.DoCombot();
            ObjectsPooler.instance.SpawnParticleFromPool(particleType.DestoryBall, start.transform.position, start.getColor(), 1f);
            ObjectsPooler.instance.SpawnTextFromPool(particleType.ScoreText, (Vector2)start.transform.position+ 
                new Vector2(Random.Range(-1f,1f), Random.Range(-1f, 1f)), start.getNumber().ToString(), 0.8f);
            WorldCreation.ballsPooler.Release(start);
        }

        start = null;
    }
    public override bool isFinished()
    {
        if (start == null)
            return true;
        return false;
    }
}
