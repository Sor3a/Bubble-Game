using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BoomAction : Action
{
    Ball BlowingBall;
    bool didBlow;
    public BoomAction(Ball blowingBall)
    {
        BlowingBall = blowingBall;
        didBlow = false;
    }

    public override void StartAction()
    {
        ObjectsPooler.instance.SpawnParticleFromPool(particleType.Blow, BlowingBall.transform.position, 2f);

        List<Ball> copy = BlowingBall.overlappingColliders.ToList();
        foreach (var ball in copy) // we can't itearte it and we are also modifying on it
        {
            ObjectsPooler.instance.SpawnParticleFromPool(particleType.DestoryBall, BlowingBall.transform.position, ball.getColor());
            if(ball.gameObject.activeSelf==true)
            WorldCreation.ballsPooler.Release(ball);
        }
        BlowingBall.overlappingColliders = copy.ToHashSet();
        if (BlowingBall.gameObject.activeSelf == true)
            WorldCreation.ballsPooler.Release(BlowingBall);
        FinishAction();
    }
    public override void DoAction()
    {
        
    }
    protected override void FinishAction()
    {
        didBlow = true;
    }
    public override bool isFinished()
    {
        if(didBlow)
        {
            didBlow = false;
            return true;
        }
        return false;
        
    }
}
