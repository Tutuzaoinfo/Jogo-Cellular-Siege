using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // Add this for TextMeshPro support

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public int totalCollectables = 6;
    public string endSceneName = "Fim"; // Name of your end scene
    
    [Header("UI References")]
    public Text collectablesText; // For Legacy UI Text
    public TextMeshProUGUI collectablesTextTMP; // For TextMeshPro UI
    public GameObject endGameUI; // Optional: In-scene end game UI instead of scene switch
    
    private int collectablesFound = 0;
    
    void Start()
    {
        UpdateUI();
        
        // If endGameUI is assigned, make sure it's hidden at start
        if (endGameUI != null)
        {
            endGameUI.SetActive(false);
        }
    }
    
    public void CollectItem()
    {
        collectablesFound++;
        UpdateUI();
        
        Debug.Log($"Collected item! Total: {collectablesFound}/{totalCollectables}");
        
        // Check if all collectables are found
        if (collectablesFound >= totalCollectables)
        {
            EndGame();
        }
    }
    
    void UpdateUI()
    {
        string displayText = $"Collectables: {collectablesFound}/{totalCollectables}";
        
        // Update Legacy UI Text
        if (collectablesText != null)
        {
            collectablesText.text = displayText;
        }
        
        // Update TextMeshPro Text
        if (collectablesTextTMP != null)
        {
            collectablesTextTMP.text = displayText;
        }
    }
    
    void EndGame()
    {
        Debug.Log("All collectables found! Game ending...");
        
        // Option 1: Switch to end scene
        if (!string.IsNullOrEmpty(endSceneName))
        {
            SceneManager.LoadScene(endSceneName);
        }
        // Option 2: Show in-scene end game UI
        else if (endGameUI != null)
        {
            endGameUI.SetActive(true);
            Time.timeScale = 0f; // Pause the game
        }
    }
    
    // Call this method to restart the game
    public void RestartGame()
    {
        Time.timeScale = 1f; // Resume time
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    // Call this method to quit the game
    public void QuitGame()
    {
        Application.Quit();
    }
}