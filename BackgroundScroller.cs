using UnityEngine;

public class InfiniteBackgroundScroller : MonoBehaviour
{
    public float initialScrollSpeed = 1f;    // Starting scroll speed
    public float maxScrollSpeed = 10f;        // Cap the maximum scroll speed
    public float gradualIncreaseRate = 10f; // Rate at which the speed increases

    private float currentScrollSpeed;  // Store the current scroll speed
    private float offset;              // Texture offset
    private Material mat;              // Reference to the material
    private bool isScrolling = false;  // Track if scrolling has started
    private float elapsedTime = 0f;    // Track elapsed time for speed scaling

    void Start()
    {
        // Get the material from the Renderer
        mat = GetComponent<Renderer>().material;

        // Initialize the scroll speed
        currentScrollSpeed = initialScrollSpeed;
    }

    void Update()
    {
        // Start scrolling when the player presses an arrow key
        if (!isScrolling && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)))
        {
            isScrolling = true;
        }

        // If scrolling, increase the speed and scroll the background
        if (isScrolling)
        {
            IncreaseScrollSpeedGradually();
            ScrollBackground();
        }
    }

    void IncreaseScrollSpeedGradually()
    {
        // Increment elapsed time
        elapsedTime += Time.deltaTime;

        // Gradually increase the speed, capped at maxScrollSpeed
        currentScrollSpeed = Mathf.Min(
            maxScrollSpeed,
            initialScrollSpeed + gradualIncreaseRate * elapsedTime
        );
    }

    void ScrollBackground()
    {
        // Update the texture offset to scroll the background vertically
        offset += (Time.deltaTime * currentScrollSpeed) / 10f;
        mat.SetTextureOffset("_MainTex", new Vector2(0, offset));
    }

    // Public method to provide the current scroll speed
    public float GetCurrentScrollSpeed()
    {
        return currentScrollSpeed;
    }
}
