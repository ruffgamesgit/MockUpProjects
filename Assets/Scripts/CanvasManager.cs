using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasManager :  MonoBehaviour
{
    public static CanvasManager instance;
    
    [Header("References")] [SerializeField]
    CanvasGroup failPanel;

    [SerializeField] CanvasGroup defaultPanel;
    [SerializeField] CanvasGroup winPanel;
    public TextMeshProUGUI levelName;
    
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        defaultPanel.DOFade(1, .5f);
        GameManager.instance.LevelFailedEvent += OnLevelFailed;
        GameManager.instance.LevelSucceededEvent += OnLevelSucceededEvent;
        levelName.text = SceneManager.GetActiveScene().name;
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
        GameManager.instance.OnTapRestart();
    }

    public void OnNext()
    {
        GameManager.instance.OnTapNext();
    }

    #endregion
}