using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float fallSpeed = 2f;           // Speed when falling
    public float horizontalOffset = 0.5f;  // Horizontal offset for alignment
    public float verticalOffset = 0.1f;    // Vertical offset for alignment

    public Sprite frontFacingSprite;       // Sprite at game start
    public Sprite climberLeftHandUp;       // Sprite for left hand raised
    public Sprite climberRightHandUp;      // Sprite for right hand raised
    private SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer

    private bool isFalling = false;        // Track if the player is falling
    private bool hasMoved = false;         // Track if the player has moved

    private RockMovement attachedRock;     // Reference to the attached rock
    private Rigidbody2D rb;                // Reference to the Rigidbody2D
    private RockSpawner rockSpawner;       // Reference to the RockSpawner

    public Text scoreText;                 // Reference to the UI Text for the score
    private int score = 0;                 // Track the player’s score

    public AudioClip correctMoveSound;     // Sound for correct rock movement
    public AudioClip wrongMoveSound;       // Sound for wrong input
    private AudioSource audioSource;       // Reference to the AudioSource

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();  // Get the Rigidbody2D component
        spriteRenderer = GetComponent<SpriteRenderer>();  // Get the SpriteRenderer
        audioSource = GetComponent<AudioSource>();  // Get the AudioSource component

        rb.gravityScale = 0;  // Disable gravity at the start
        rockSpawner = FindObjectOfType<RockSpawner>();  // Find the RockSpawner

        spriteRenderer.sprite = frontFacingSprite;  // Set the initial sprite

        UpdateScoreText();  // Initialize the score display
    }

    void Update()
    {
        if (!isFalling)
        {
            HandleMovementInput();
        }

        // Reset the game if the player falls off the screen
        if (transform.position.y < -Camera.main.orthographicSize - 1f)
        {
            Debug.Log("Player went off-screen! Resetting game.");
            ResetGame();
        }
    }

    void HandleMovementInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (!hasMoved)
            {
                hasMoved = true;  // Track that the player has moved
            }

            bool isLeftInput = Input.GetKeyDown(KeyCode.LeftArrow);

            // Get the next rock from the RockSpawner
            GameObject nextRock = rockSpawner.GetNextRock();

            if (nextRock != null)
            {
                bool isRockOnLeft = nextRock.transform.position.x < transform.position.x;

                // Check if the input matches the side of the next rock
                if (isLeftInput == isRockOnLeft)
                {
                    PlaySound(correctMoveSound);  // Play correct move sound
                    Debug.Log("Correct direction! Snapping to rock.");
                    SnapToRock(nextRock, isLeftInput);
                    rockSpawner.DequeueNextRock();  // Remove the rock from the queue
                    IncreaseScore();  // Increase the player's score
                }
                else
                {
                    PlaySound(wrongMoveSound);  // Play wrong move sound
                    Debug.Log("Wrong direction! Falling.");
                    StartFalling();
                }
            }
            else
            {
                PlaySound(wrongMoveSound);  // Play wrong move sound
                Debug.Log("No rocks available! Falling.");
                StartFalling();
            }
        }
    }

    void SnapToRock(GameObject rock, bool isLeftInput)
    {
        // Calculate the new position with horizontal and vertical offsets
        float offsetX = isLeftInput ? horizontalOffset : -horizontalOffset;
        Vector3 newPosition = new Vector3(
            rock.transform.position.x + offsetX,
            rock.transform.position.y + verticalOffset,
            transform.position.z
        );

        transform.position = newPosition;  // Snap the player to the new position

        // Set the player as a child of the new rock to follow it
        transform.SetParent(rock.transform);

        // Detach from the previous rock, if any
        if (attachedRock != null)
        {
            attachedRock.DetachPlayer();
        }

        attachedRock = rock.GetComponent<RockMovement>();  // Store the new rock reference

        if (attachedRock != null)
        {
            attachedRock.AttachPlayer(this);  // Attach the player to the new rock
        }

        // Stop any movement and disable gravity
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;

        // Switch the sprite based on the input direction
        spriteRenderer.sprite = isLeftInput ? climberLeftHandUp : climberRightHandUp;
    }

    void IncreaseScore()
    {
        score += 1;  // Increase the score
        UpdateScoreText();  // Update the score display
    }

    void UpdateScoreText()
    {
        // Update the UI Text with the current score
        scoreText.text = "Score: " + score;
    }

    public void OnRockDestroyed()
    {
        Debug.Log("Attached rock destroyed! Resetting game.");
        ResetGame();  // Reset the game when the attached rock is destroyed
    }

    void StartFalling()
    {
        isFalling = true;

        if (attachedRock != null)
        {
            attachedRock.DetachPlayer();  // Detach the player from the rock
            attachedRock = null;
        }

        transform.SetParent(null);  // Remove the player from the rock's hierarchy
        StartCoroutine(Fall());  // Start the falling coroutine
    }

    System.Collections.IEnumerator Fall()
    {
        rb.gravityScale = 1;  // Enable gravity

        // Make the player fall smoothly off the screen
        while (transform.position.y > -Camera.main.orthographicSize - 1f)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            yield return null;
        }

        ResetGame();  // Reset the game after falling
    }

    void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  // Reload the scene
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);  // Play the given sound once
        }
    }
}
