using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    public event System.Action LevelEndedEvent;
    public event System.Action LevelFailedEvent;
    public event System.Action LevelSucceededEvent;

    [Header("Debug")] public bool isLevelActive;


    protected override void Awake()
    {
        base.Awake();

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

        if (nextScene > SceneManager.sceneCount) nextScene = 0;
        SceneManager.LoadScene(nextScene);

        #endregion
    }

    public void EndGame(bool success, float delayAsSeconds = 0)
    {
        if (!isLevelActive) return;
        isLevelActive = false;

        if (!success) LevelFailedEvent?.Invoke();
        else LevelSucceededEvent?.Invoke();
    }
}