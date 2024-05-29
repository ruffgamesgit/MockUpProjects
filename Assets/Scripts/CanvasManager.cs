using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class CanvasManager : MonoSingleton<CanvasManager>
{
    [Header("References")] 
    [SerializeField] CanvasGroup failPanel;
    [FormerlySerializedAs("winPanel")] [SerializeField] CanvasGroup defaultPanel;

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