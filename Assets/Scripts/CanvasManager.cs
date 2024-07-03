using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CanvasManager : MonoSingleton<CanvasManager>
{
    [Header("References")] [SerializeField]
    CanvasGroup failPanel;

    [FormerlySerializedAs("winPanel")] [SerializeField]
    CanvasGroup defaultPanel;
    private int _currentScore;

    void Start()
    {
        defaultPanel.DOFade(1, .5f);
        GameManager.instance.LevelFailedEvent += OnLevelFailed;
        GameManager.instance.LevelSucceededEvent += OnLevelSucceededEvent;
    }

    private void OnLevelSucceededEvent()
    {
        defaultPanel.DOFade(1, .5f);
    }

    private void OnLevelFailed()
    {
        failPanel.DOFade(1, .5f);
        defaultPanel.gameObject.SetActive(false);
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