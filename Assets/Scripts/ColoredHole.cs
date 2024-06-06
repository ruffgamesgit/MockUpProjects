using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ColoredHole : BaseHoleClass
{
    [Header("References")] [SerializeField]
    protected ColourEnum colorEnum;

    public ColourEnum GetColorEnum()
    {
        return colorEnum;
    }

    public override void OnBoltArrived()
    {
        if (GetOccupiedPointCount() == placementPoints.Count)
        {
            Debug.LogWarning("Colored hole is full,  and disappearing");
            HoleManager.instance.RemoveOldBringNew();
            Sequence sq = DOTween.Sequence();
            sq.Append(transform.DOMoveX(transform.position.x + 7, .25f));
            sq.Append(transform.DOScale(Vector3.zero, .25f).OnComplete(() => Destroy(gameObject, .5f)));
        }
    }
}