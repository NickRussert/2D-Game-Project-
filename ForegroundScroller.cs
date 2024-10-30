using UnityEngine;

public class ForegroundScroller : MonoBehaviour
{
    public float scrollSpeed = 2f;  // Speed at which the foreground scrolls
    private bool isScrolling = false;  // Track if the scrolling has started
    private float objectHeight;  // Height of the foreground object

    void Start()
    {
        // Calculate the height of the foreground sprite
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            objectHeight = spriteRenderer.bounds.size.y;
        }
    }

    void Update()
    {
        // Start scrolling on the first player input
        if (!isScrolling && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            isScrolling = true;
        }

        // Scroll the foreground if the game has started
        if (isScrolling)
        {
            ScrollForeground();
            CheckIfFullyOffScreen();
        }
    }

    void ScrollForeground()
    {
        // Move the foreground downward
        transform.position += Vector3.down * scrollSpeed * Time.deltaTime;
    }

    void CheckIfFullyOffScreen()
    {
        // Calculate the position at which the object is fully off the screen
        float offScreenPositionY = -Camera.main.orthographicSize - (objectHeight / 2);

        // Destroy the object only after it is fully off the screen
        if (transform.position.y < offScreenPositionY)
        {
            Destroy(gameObject);
        }
    }
}
