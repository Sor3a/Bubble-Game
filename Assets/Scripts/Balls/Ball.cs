using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
public class Ball : MonoBehaviour
{
    public int number;
    public HashSet<Ball> overlappingColliders;
    public int Length { private set; get; }
    public Ball LongestChild{ private set; get; }
    public Ball bestMergeBall { private set; get; }
    Rigidbody2D rb;

    [HideInInspector] public CircleCollider2D CircleCollider { private set; get; }
    public bool attachedCeilling = false;
    BallsManager manager;
    //bool visited=false;
    public delegate void DestoryBall(Ball destroyed);
    public static event DestoryBall ballDestroyed;

    [SerializeField] TextMeshProUGUI numberText;
    public AudioSource source { private set; get; }
    public Ball MergerdDestination { private set; get; }
    [SerializeField] float speed;
    SpriteRenderer sprite;
    public int getNumber() { return number; }


    static Dictionary<int, Color> numbersColor;
    public static Color findColor(int number)
    {
        if (numbersColor == null)
            InitilaizeColors();
        return numbersColor[number];
    }
    public Color getColor()
    {
        return sprite.color;
    }
    private void OnDestroy()
    {
        
        ballDestroyed -= this.OnballDestoryed;
        WorldCreation.LineCreated -= LineCreated;
    }
    private void OnDisable()
    {
        ballDestroyed(this);
        WorldCreation.LineCreated -= LineCreated;
    }
    private void Awake()
    {
        if (numbersColor == null)
            InitilaizeColors();
    }
    static void InitilaizeColors()
    {
        numbersColor = new Dictionary<int, Color>();
        Color[] shinyColors = new Color[] {
    new Color(1f, 0.76f, 0.03f), // shiny yellow
    new Color(1f, 0.34f, 0.20f), // shiny orange
    new Color(1f, 0.75f, 0.80f), // shiny pink
    new Color(0f, 1f, 1f),      // shiny cyan
    new Color(0.50f, 1f, 0.83f), // shiny aqua
    new Color(0f, 1f, 0.50f),    // shiny green
    new Color(0.85f, 0.44f, 0.84f), // shiny purple
    new Color(1f, 0.41f, 0.71f), // shiny hot pink
    new Color(1f, 0.84f, 0f),    // shiny gold
    new Color(0.94f, 0.90f, 0.55f) // shiny khaki
};

        for (int i = 1; i <= 10; i++)
        {
            int number = (int)Mathf.Pow(2, i);
            int colorIndex = i - 1; // shinyColors array is 0-based
            Color color = shinyColors[colorIndex];
            numbersColor.Add(number, color);
        }

    }
    private void OnEnable()
    {
        if(manager==null)
        {
            source = GetComponent<AudioSource>();
            manager = FindObjectOfType<BallsManager>();
            overlappingColliders = new HashSet<Ball>();
            rb = GetComponent<Rigidbody2D>();
            CircleCollider = GetComponent<CircleCollider2D>();
            LongestChild = null;
            ballDestroyed += OnballDestoryed;
            sprite = GetComponent<SpriteRenderer>();
        }
        WorldCreation.LineCreated += LineCreated;

    }
    Vector2 newPosition;
    bool startMoving;
    private void Update()
    {
        if(startMoving)
        {
            transform.position = Vector2.Lerp(transform.position, newPosition, Time.deltaTime * 20f);
            if (Vector2.Distance(newPosition, transform.position) < 0.1f)
                startMoving = false;
        }
        
    }
    void LineCreated()
    {
        if(gameObject.tag == "Ball")
        {
            newPosition = transform.position + new Vector3(0, -0.9f, 0);
            startMoving = true;
        }

    }
    void OnballDestoryed(Ball ball)
    {
        //if (MergerdDestination == ball) MergerdDestination = ball.MergerdDestination;
        if (overlappingColliders.Contains(ball))
        {
            overlappingColliders.Remove(ball);
        }
    }
    void InitializeColliders()
    {
        overlappingColliders.Clear();
        List<Collider2D> colliders = Physics2D.OverlapCircleAll(transform.position, CircleCollider.radius+0.05f).ToList();
        colliders.Remove(CircleCollider);
        foreach (var item in colliders)
        {
            if (item.TryGetComponent(out Ball ball))
            {
                overlappingColliders.Add(ball);
            }
        }
    }

    public void UpdateUX(int number)
    {
        if (numbersColor.TryGetValue(number, out Color color))
        {
            sprite.color = color;
        }
        else
        {
            var newColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            numbersColor.Add(number, newColor);
            sprite.color = newColor;
        }
        this.number = number;
        numberText.text = number.ToString();
    }
    public void InitializeBall()
    {
        int number = 2; 
       if(Random.Range(0,100)<70)
            number = (int)Mathf.Pow(2f, Random.Range(1, 5));
       else
            number = (int)Mathf.Pow(2f, Random.Range(1, 10));

        attachedCeilling = true;
        rb.bodyType = RigidbodyType2D.Kinematic;
        InitializeColliders();
        foreach (var item in overlappingColliders)
        {
            item.overlappingColliders.Add(this);
        }
        UpdateUX(number);
    }
    public void InitializeBall(int number)
    {
        this.number = number;

            
        UpdateUX(number);
    }
    public void UpdateRb()
    {
        if (!attachedCeilling && rb)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            CircleCollider.isTrigger = true;
        }
    }
    public void CeillingAttachment(HashSet<Ball> visited) //for checking all the ceilling for an object
    {
        if (visited.Contains(this))
            return;
        visited.Add(this);
        if (overlappingColliders.Count == 0) InitializeColliders();
        foreach (var ball in overlappingColliders)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            CircleCollider.isTrigger = false;
            ball.attachedCeilling = attachedCeilling;
            ball.CeillingAttachment(visited);
        }
    }
    int GetLength(HashSet<Ball> visited, int total, int number)
    {
        // Check if this ball has already been visited to avoid infinite recursion

        if (visited.Contains(this))
        {
            return -999;
        }
        Length = 0;
        LongestChild = null;
        visited.Add(this);

        if (overlappingColliders.Count == 0) InitializeColliders();


        // Loop through all the colliders that are overlapping with the collider of the current object
        foreach (Ball ball in overlappingColliders)
        {
            if (ball.getNumber() == total)
            {
                int length_ = 1 + ball.GetLength(visited, total * 2, number);
                if (length_ > Length)
                {
                    Length = length_;
                    LongestChild = ball;
                    //bestMergeBall = ball;
                }
            }
            else if (number == ball.getNumber())
            {
                int length_ = ball.GetLength(visited, total, number);
                if (length_ > Length)
                {
                    Length = length_;
                    LongestChild = ball;
                    //bestMergeBall = ball;
                }
            }

        }

        return Length;
    }
    Ball FindMergingBall(HashSet<Ball> balls)
    {
        foreach (var ball in balls)
        {
            if (ball.LongestChild != null)
            {
                if (ball.number != ball.LongestChild.number)
                {
                    return ball;
                }
            }
        }
        return null;
    }
    int calculateTotalNumber(HashSet<Ball> visited, ref HashSet<Ball> sameBalls) //calculate the total for finding the best place to merge into
    {
        if (visited.Contains(this))
            return 0;
        visited.Add(this);

        int number_ = number;
        foreach (Ball ball in overlappingColliders)
        {
            if (ball.getNumber() == number)
            {
                sameBalls.Add(ball);
                number_ += ball.calculateTotalNumber(visited, ref sameBalls);
            }
        }
        return number_;
    }

    void mergeBalls( HashSet<Ball> merged, int total,HashSet<Ball> visited)
    {
        if (visited.Contains(this))
        {
            return;
        }
           
        visited.Add(this);
        foreach (var item in merged)
        {   
            if (item.number == number)
            {
                if (visited.Contains(item)) continue;
                item.mergeBalls(item.overlappingColliders, total, visited);
                //item.startMerging(this);
                manager.AddAction(new MoveAction(item, this, speed));
                item.MergerdDestination = this;
            }
        }

        
    }
   
    void StartMerging(Ball head,int total,bool isMultipleMerge) //head is the head that we gonna merge into , total is the total of numbers
    {                                                           // is multiple merge is when we gonna not only merge with the same number we have (2,2,2,2 and 8) 

        if (head != null)
        {
            head.mergeBalls(head.overlappingColliders, total, new HashSet<Ball>());
            while (head.LongestChild != null && isMultipleMerge)
            {
                manager.AddAction(new MoveAction(head, head.LongestChild, speed));
                head = head.LongestChild;
            }
            manager.StartDoingActions();
        }
    }
    public void UpdateNumber(int added)
    {
        number += added;
        if (number >= 2048)
        {
            manager.AddAction(new BoomAction(this));
            manager.StartDoingActions();
        }
        UpdateUX(number);
    }


    //void startMerging(Ball MergerdDestination)
    //{
    //    manager.AddAction(new MoveAction(this, MergerdDestination, speed));
    //    this.MergerdDestination = MergerdDestination;
    //}
    IEnumerator nullVelocity(Vector2 direction) //some times the ball stop a little bit too far from the other ball so we will make it a little bit close
    {                          //for detection
        yield return new WaitForEndOfFrame();
        rb.velocity = Vector2.zero;
        transform.Translate(direction * 0.1f);
    }
    void BallCollides(Ball touched)
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        StartCoroutine(nullVelocity((touched.transform.position-transform.position).normalized));
        gameObject.tag = "Ball";
        InitializeColliders();
        foreach (var ball in overlappingColliders)
        {
            ball.overlappingColliders.Add(this);
        }
        attachedCeilling = touched.attachedCeilling;
        HashSet<Ball> sameBallsType = new HashSet<Ball>();
        int total_ = calculateTotalNumber(new HashSet<Ball>(), ref sameBallsType); 
        Length = GetLength(new HashSet<Ball>(), total_, number);
        Ball mergingNode = FindMergingBall(sameBallsType);

        if (mergingNode != null)
            StartMerging(mergingNode, total_,true);
        else if(sameBallsType.Count>0)
            StartMerging(sameBallsType.First(),total_,false);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "newBall")
        {
            if (collision.gameObject.TryGetComponent(out Ball ball))
            {
                ball.BallCollides(this);
            }
        }
    }

}
