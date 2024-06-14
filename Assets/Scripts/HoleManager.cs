using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class HoleManager : MonoBehaviour
{
    public static HoleManager instance;
    public event System.Action<ColoredHole> NewColoredHoleCameEvent;

    [Header("References")] public List<ColoredHole> coloredHoles = new();
    [Header("Debug")] [SerializeField] private ColoredHole currentColoredHole;
    [SerializeField] private bool sequencePerforming;

    void Awake()
    {
        instance = this;
        currentColoredHole = coloredHoles[0];
    }

    public ColoredHole GetCurrentHole()
    {
        return currentColoredHole;
    }

    public void RemoveOldBringNew()
    {
        if (!GameManager.instance.isLevelActive) return;
        if (sequencePerforming) return;
        sequencePerforming = true;

        // Adding New Hole
        coloredHoles.RemoveAt(0);
        if (coloredHoles.Count == 0)
            GameManager.instance.EndGame(true);

        currentColoredHole = coloredHoles[0];
        MoveHoles();
    }

    void MoveHoles()
    {
        if (!GameManager.instance.isLevelActive) return;
        for (int i = 0; i < coloredHoles.Count; i++)
        {
            Transform box = coloredHoles[i].transform;
            Vector3 pos = box.localPosition + (Vector3.right * 3);
            int localIndex = i;
            
            box.DOLocalMove(pos, .15f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                if (localIndex == coloredHoles.Count - 1)
                {
                    NewColoredHoleCameEvent?.Invoke(GetCurrentHole());
                    sequencePerforming = false;
                }
            }).SetDelay(0.2f);
        }
    }

    public ColoredHole GetNextCurrentHole()
    {
        return coloredHoles[1];
    }
}