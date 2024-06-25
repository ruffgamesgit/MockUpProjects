using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public event System.Action LevelEndedEvent;
    public event System.Action LevelFailedEvent;
    public event System.Action LevelSucceededEvent;

    [Header("Debug")] public bool isLevelActive;
    
    private void Awake()
    {
        Application.targetFrameRate = Screen.currentResolution.refreshRate;
        instance = this;
        isLevelActive = true;
    }

    public void OnTapRestart()
    {
        LevelEndedEvent?.Invoke();

        isLevelActive = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnTapNext()
    {
        LevelEndedEvent?.Invoke();
        isLevelActive = false;

        #region Cumulative Next Level

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextScene = currentSceneIndex + 1;

        if (nextScene > SceneManager.sceneCountInBuildSettings - 1) nextScene = 0;
        SceneManager.LoadScene(nextScene);

        #endregion
    }

    public void EndGame(bool success)
    {
        if (!isLevelActive) return;
        isLevelActive = false;

        if (!success)
        {
            Taptic.Medium();
            LevelFailedEvent?.Invoke();
        }
        else LevelSucceededEvent?.Invoke();
    }
}