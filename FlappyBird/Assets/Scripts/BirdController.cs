using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BirdController : MonoBehaviour
{
    public float flapSpeed = 1000f;
    public float forwardSpeed = 10f;
    public float maxFlapSpeed = 2f;

    private Rigidbody2D rb;
    private Animator animator;

    private bool didFlap;
    private bool isDead;

    private bool gameStarted;

    private Vector2 originalPosition;

    private GameObject startButton;

    private int score = 0;

    public void Start()
    {
        Debug.Log(PlayerPrefs.GetInt("HighScore"));
        this.rb = this.GetComponent<Rigidbody2D>();
        this.animator = this.GetComponent<Animator>();
        this.originalPosition = new Vector2(this.transform.position.x, this.transform.position.y);
        this.startButton = GameObject.Find("StartButton");

        this.rb.gravityScale = 0;
        this.forwardSpeed = 0;
        this.animator.enabled = false;
    }

    // read input, change graphics

    public void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (!isDead)
            {
                if (!this.gameStarted)
                {
                    var renderer = this.startButton.GetComponent<SpriteRenderer>();
                    renderer.enabled = false;
                    this.forwardSpeed = 5;
                    this.rb.gravityScale = 1;
                    this.animator.enabled = true;
                }

                this.didFlap = true;
            }
            else
            {
                SceneManager.LoadScene("Play");
            }
        }
    }

    // apply physics

    public void FixedUpdate()
    {
        var velocity = this.rb.velocity;
        velocity.x = this.forwardSpeed;
        this.rb.velocity = velocity;

        var rotationAngle = (this.rb.velocity.y > 0) ? Math.Min(rb.velocity.y * 8, 30)
            : Math.Max(rb.velocity.y * 8, -90);

        if (!isDead)
        {
            this.rb.MoveRotation(rotationAngle);
        }

        if (didFlap)
        {
            didFlap = false;
            this.rb.AddForce(new Vector2(0, flapSpeed));

            var updatedVelocity = this.rb.velocity;
            if (updatedVelocity.y > this.maxFlapSpeed)
            {
                updatedVelocity.y = this.maxFlapSpeed;
                this.rb.velocity = updatedVelocity;
            }
        }
    }

    public void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.CompareTag("Floor") || collider.gameObject.CompareTag("PipeCollision"))
        {
            this.isDead = true;
            this.animator.SetBool("BirdDead", true);
            this.forwardSpeed = 0;

            var currentHighScore = PlayerPrefs.GetInt("HighScore", 0);
            if (this.score > currentHighScore)
            {
                PlayerPrefs.SetInt("HighScore", this.score);
            }

            var renderer = this.startButton.GetComponent<SpriteRenderer>();
            this.startButton.transform.position = new Vector3(Camera.main.transform.position.x,
                Camera.main.transform.position.y, 0);
            renderer.enabled = true;

            this.gameStarted = false;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pipe"))
        {
            this.score++;
            var text = GameObject.Find("Text");
            text.GetComponent<Text>().text = score.ToString();
        }
    }
}
