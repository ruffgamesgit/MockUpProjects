using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoSingleton<CanvasManager>
{
    [Header("References")] public Button RestartButton;
    public Button NextButton;
    [SerializeField] CanvasGroup failPanel;
    [SerializeField] CanvasGroup winPanel;

    void Start()
    {
        GameManager.instance.LevelFailedEvent += OnLevelFailed;
        GameManager.instance.LevelSucceededEvent += OnLevelSucceededEvent;
    }

    private void OnLevelSucceededEvent()
    {
        winPanel.DOFade(1, .5f);
    }

    private void OnLevelFailed()
    {
        failPanel.DOFade(1, .5f);
    }

    #region COMPLETED REGION

    public void OnRestart()
    {
        GameManager.instance.OnTapRestart();
    }

    public void OnNext()
    {
        GameManager.instance.OnTapNext();
    }

    #endregion
}