using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class WorldCreation : MonoBehaviour
{
    [SerializeField] Ball ballPrefab;
    public static ObjectPool<Ball> ballsPooler { private set; get; }
    Vector2 startingPos;
    public delegate void createLine();
    public static event createLine LineCreated;
    bool isLastLineFromTheStart;
    Vector2 offsetBetweenBalls;
    BallsManager ballsManager;

    [SerializeField] int MaxNumberOfLinesCreation; //how much line creation to stop creating
    [SerializeField] GameObject loadingPanel;

    public bool isGameLoaded()
    {
        return !loadingPanel.activeSelf;
    }
    private void Awake()
    {
        ballsManager = FindObjectOfType<BallsManager>();
        ballsPooler = new ObjectPool<Ball>(createBall, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject,true,10,500);
        startingPos = new Vector2(-2.45f, 4.09f);
        isLastLineFromTheStart = false;
        offsetBetweenBalls = new Vector2(0.95f, 0);
        loadingPanel.SetActive(true);
        GameDesign.intitliazeDesign += InitialzeLinesNumber;
    }
    void InitialzeLinesNumber(GameDifficulty difficulty)
    {
        MaxNumberOfLinesCreation = difficulty.LineSpawnedDuringGame;
    }
    IEnumerator CreationSlowly()
    {
        
        for (int i = 0; i < 4; i++)
        {
            yield return new WaitForSeconds(2f);
            CreateLine();
        }
        loadingPanel.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(CreationSlowly());
    }
    Ball createBall()
    {
        Ball ball = Instantiate(ballPrefab,transform);
        return ball;
    }
    private void OnTakeFromPool(Ball obj) => obj.gameObject.SetActive(true);

    private void OnReturnedToPool(Ball obj) => obj.gameObject.SetActive(false);

    private void OnDestroyPoolObject(Ball obj) => Destroy(obj.gameObject);

    public void CreateLine()
    {

        if(MaxNumberOfLinesCreation-->0 && Ceilling.didWin ==false)
        {
            LineCreated?.Invoke();
            CreateLine_();
        }

    }
    void CreateBall(Vector2 position)
    {
        Ball ball = ballsPooler.Get();
        ball.transform.position = position;
        ball.InitializeBall();
        ballsManager.AddBall(ball);
        //Transform ballTransform = ball.transform;

    }
    IEnumerator creationLine()
    {
        yield return new WaitForSeconds(0.1f);
        if (!isLastLineFromTheStart)
        {
            isLastLineFromTheStart = true;
            for (int i = 0; i < 6; i++)
            {
                yield return new WaitForEndOfFrame();
                CreateBall(startingPos + offsetBetweenBalls * i);
            }
        }
        else
        {
            isLastLineFromTheStart = false;
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForEndOfFrame();
                CreateBall(startingPos + offsetBetweenBalls / 2f + offsetBetweenBalls * i);
            }
        }
    }
    void CreateLine_()
    {
        StartCoroutine(creationLine());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            CreateLine();
    }
}
