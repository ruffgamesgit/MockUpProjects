using System.Collections;
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
        //   if (!GameManager.instance.isLevelActive) return;
        // if (_sequencePerforming) return;
        // _sequencePerforming = true;

        // Adding New Box
        coloredHoles.RemoveAt(0);
        currentColoredHole = coloredHoles[0];
        MoveHoles();
    }

    void MoveHoles()
    {
        for (int i = 0; i < coloredHoles.Count; i++)
        {
            Transform box = coloredHoles[i].transform;
            Vector3 pos = new(transform.position.x + (i * -4), transform.position.y, transform.position.z);
            int localIndex = i;
            box.DOMove(pos, .15f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                if (localIndex == coloredHoles.Count - 1)
                {
                    NewColoredHoleCameEvent?.Invoke(GetCurrentHole());
                    //   _sequencePerforming = false;
                }
            });
        }
    }
}