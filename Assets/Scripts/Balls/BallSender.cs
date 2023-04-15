using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSender : MonoBehaviour
{
    [SerializeField] GameObject BallPrefab; // The BallPrefab to spawn
    [SerializeField] float speed = 10f; // The speed at which the BallPrefab should move

    [SerializeField] BallsManager ballManager;
    int nextNumber, currentNumber;
    [SerializeField] BallToSend nextBall, currentBall,LerpingBall;
    [SerializeField] int BallsToSendToAddLine;
    int ballsSended = 0;
    WorldCreation worldCreator;

    private void Awake()
    {
        worldCreator = FindObjectOfType<WorldCreation>();
        GameDesign.intitliazeDesign += InitialzeLinesNumber;
    }
    void InitialzeLinesNumber(GameDifficulty difficulty)
    {
        BallsToSendToAddLine = difficulty.NumberOfBallsToCreateLine;
    }
    void FollowDirection(Vector2 direction)
    {
        float angle = Vector2.SignedAngle(Vector2.up, direction);
        angle = Mathf.Clamp(angle, -80, 80);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    Vector2 directionToMouse()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // Get the mouse position in world space
        Vector2 direction = (mousePos - (Vector2)transform.position).normalized; // Get the direction to the mouse click
        return direction;
    }
    Vector2 directionToTouch() //with phones
    {
        if (Input.touchCount > 0)
        {
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(Input.touches[0].position); // Get the touch position in world space
            Vector2 direction = (touchPos - (Vector2)transform.position).normalized; // Get the direction to the touch
            return direction;
        }
        return Vector2.zero;
    }
    void CreateBall(Vector2 direction)
    {
        GameObject newBallPrefab = Instantiate(BallPrefab, transform.position, Quaternion.identity); // Instantiate the BallPrefab at the parent object's position
        Ball b = newBallPrefab.GetComponent<Ball>();
        b.UpdateUX(currentNumber);
        UpdateBalls();
        if(++ballsSended>=BallsToSendToAddLine)
        {
            ballsSended = 0;
            worldCreator.CreateLine();
        }
        // Set the velocity of the rigidbody component on the BallPrefab to move it in the direction of the mouse click
        Rigidbody2D rb = newBallPrefab.GetComponent<Rigidbody2D>();
        rb.AddForce(direction * speed, ForceMode2D.Impulse);
        ballManager.AddBall(newBallPrefab.GetComponent<Ball>());
    }

    private void Start()
    {
        NextBall();
        UpdateBalls();
    }
    IEnumerator waitToUpdateCurrentUI()
    {
        
        yield return new WaitWhile(() =>
        {
            while (Vector2.Distance(LerpingBall.transform.position, currentBall.transform.position) > 0.05f)
            {
                LerpingBall.transform.position = Vector2.Lerp(LerpingBall.transform.position, currentBall.transform.position, Time.deltaTime*8f);
                LerpingBall.transform.localScale = Vector2.Lerp(LerpingBall.transform.localScale, currentBall.transform.localScale, Time.deltaTime*8f);
                return true;
            }
            return false;
        }
        );
        currentBall.UpdateUI(currentNumber, Ball.findColor(currentNumber));
        LerpingBall.gameObject.SetActive(false);
    }
    void UpdateBalls()
    {
        NextBall();
        nextBall.UpdateUI(nextNumber, Ball.findColor(nextNumber));
        LerpingBall.gameObject.SetActive(true);
        LerpingBall.UpdateUI(currentNumber, Ball.findColor(currentNumber));
        LerpingBall.transform.position = nextBall.transform.position;
        LerpingBall.transform.localScale = nextBall.transform.localScale;
        StartCoroutine(waitToUpdateCurrentUI());
       //currentBall.UpdateUI(currentNumber, Ball.findColor(currentNumber));
    }
    void NextBall()
    {
        currentNumber = nextNumber;
        int number = 2;
        if (Random.Range(0, 100) < 70)
            number = (int)Mathf.Pow(2f, Random.Range(1, 5));
        else
            number = (int)Mathf.Pow(2f, Random.Range(1, 10));

        nextNumber = number;
    }
    void Update()
    {
        Vector2 direction = directionToMouse();
        FollowDirection(direction);
        //if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && worldCreator.isGameLoaded())
        //    CreateBall(direction);
        if (Input.GetMouseButtonDown(0) && worldCreator.isGameLoaded())
            CreateBall(direction);

    }
}
