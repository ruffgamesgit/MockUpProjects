using System;
using DG.Tweening;
using UnityEngine;

public class CanvasManager :  MonoBehaviour
{
    public static CanvasManager instance;
    
    [Header("References")] [SerializeField]
    CanvasGroup failPanel;

    [SerializeField] CanvasGroup defaultPanel;
    [SerializeField] CanvasGroup winPanel;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        defaultPanel.DOFade(1, .5f);
        GameManager.instance.LevelFailedEvent += OnLevelFailed;
        GameManager.instance.LevelSucceededEvent += OnLevelSucceededEvent;
    }

    private void OnLevelSucceededEvent()
    {
        defaultPanel.gameObject.SetActive(false);
        failPanel.gameObject.SetActive(false);
        winPanel.DOFade(1, .5f);
    }

    private void OnLevelFailed()
    {
        defaultPanel.gameObject.SetActive(false);
        winPanel.gameObject.SetActive(false);

        failPanel.DOFade(1, .5f);
    }

    #region COMPLETED REGION

    public void OnRestart()
    {
        Debug.Log("restart");
        GameManager.instance.OnTapRestart();
    }

    public void OnNext()
    {
        Debug.Log("Next");
        GameManager.instance.OnTapNext();
    }

    #endregion
}