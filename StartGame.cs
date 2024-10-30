using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    // Function to load the game scene
    public void StartTheGame()
    {
        // Replace "GameScene" with the actual name of your game scene
        SceneManager.LoadScene("SampleScene");
    }
}
