using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Ceilling : MonoBehaviour
{

    BallsManager ballsManger;
    private void Awake()
    {
        ballsManger = FindObjectOfType<BallsManager>();
        


    }
    private void Start()
    {
        //reworkCeilling();
        ballsManger.finishActions += reworkCeilling;
    }
    private void OnDestroy()
    {
        //Ball.ballDestroyed -= reworkCeilling;
    }
    public static bool didWin = false;
    void updateCeilling_()
    {
        List<Collider2D> colliders = Physics2D.OverlapBoxAll(transform.position, transform.localScale*1.2f, 0).ToList();
        colliders.Remove(GetComponent<BoxCollider2D>());
        var visited = new HashSet<Ball>();
        int number = 0;
        foreach (var item in colliders)
        {
            if (item.TryGetComponent(out Ball ball))
            {
                number++;
                ball.attachedCeilling = true;
                ball.CeillingAttachment(visited);
            }
        }
        if (number == 0 && didWin ==false)
        {
            didWin = true;
            Announcer.Instance.Announce("Perfect! YOU WON", 3f);
        }
            
    }
    IEnumerator updateEverything()
    {
        yield return new WaitForEndOfFrame();
        ballsManger.UpdateRigidBody();
    }
    void reworkCeilling()
    {
        ballsManger.RemoveCeilling();
        updateCeilling_();
        StartCoroutine(updateEverything());
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U)) reworkCeilling();
    }
}
