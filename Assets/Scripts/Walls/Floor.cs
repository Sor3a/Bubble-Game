using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="Ball")
        {
            Ball ball = collision.GetComponent<Ball>();
            ObjectsPooler.instance.SpawnParticleFromPool(particleType.DestoryBall, ball.transform.position, ball.getColor(), 1f);
            if (ball != null && ball.gameObject.activeSelf == true)
                WorldCreation.ballsPooler.Release(ball);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "newBall")
        {
            Ball ball = collision.gameObject.GetComponent<Ball>();
            ObjectsPooler.instance.SpawnParticleFromPool(particleType.DestoryBall, ball.transform.position, ball.getColor(), 1f);
            if(ball!=null && ball.gameObject.activeSelf==true)
            WorldCreation.ballsPooler.Release(ball);
        }
    }
}
