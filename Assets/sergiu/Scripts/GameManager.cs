using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Stats")]
    public int lives = 3;
    public int score = 0;
    public int currentLevel = 1;

    // Numele scenelor in ordine
    public string[] sceneOrder = new string[]
    {
        "MainScene",
        "Room1",
        "Minigame1",
        "Room2",
        "Minigame2"
    };

    private int currentSceneIndex = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddScore(int points)
    {
        score += points;
        Debug.Log($"Scor: {score}");
    }

    public void LoseLife()
    {
        lives--;
        Debug.Log($"Vieti ramase: {lives}");

        if (lives <= 0)
            GameOver();
    }

    public void GoToNextScene()
    {
        currentSceneIndex++;

        if (currentSceneIndex >= sceneOrder.Length)
        {
            Debug.Log("Joc terminat!");
            // Aici poti incarca o scena de victorie
            return;
        }

        SceneLoader.Instance.LoadScene(sceneOrder[currentSceneIndex]);
    }

    public void GoToScene(string sceneName)
    {
        SceneLoader.Instance.LoadScene(sceneName);
    }

    void GameOver()
    {
        Debug.Log("Game Over!");
        // SceneLoader.Instance.LoadScene("GameOver");
    }

    public void ResetGame()
    {
        lives = 3;
        score = 0;
        currentLevel = 1;
        currentSceneIndex = 0;
    }
}