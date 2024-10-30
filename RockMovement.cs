using UnityEngine;

public class RockMovement : MonoBehaviour
{
    private InfiniteBackgroundScroller backgroundScroller;  // Reference to the background scroller
    private PlayerMovement attachedPlayer;  // Reference to the player if attached

    void Start()
    {
        // Find the background scroller in the scene
        backgroundScroller = FindObjectOfType<InfiniteBackgroundScroller>();
    }

    void Update()
    {
        // Get the current background scroll speed
        float currentScrollSpeed = backgroundScroller.GetCurrentScrollSpeed();

        // Move the rock down along the Y-axis using the same speed as the background
        transform.position += Vector3.down * currentScrollSpeed * Time.deltaTime;

        // Destroy the rock if it moves off the bottom of the screen
        if (transform.position.y < -Camera.main.orthographicSize - 1f)
        {
            NotifyPlayerOfDestruction();  // Notify the player if attached
            Destroy(gameObject);
        }
    }

    public void AttachPlayer(PlayerMovement player)
    {
        // Set the attached player reference
        attachedPlayer = player;
    }

    public void DetachPlayer()
    {
        // Clear the attached player reference
        attachedPlayer = null;
    }

    private void NotifyPlayerOfDestruction()
    {
        // Notify the player only if they are attached to this rock
        if (attachedPlayer != null)
        {
            attachedPlayer.OnRockDestroyed();  // Notify the player
        }
    }
}