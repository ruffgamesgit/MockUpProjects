using DG.Tweening;
using UnityEngine;

public class ColoredHole : BaseHoleClass
{
    [Header("References")] [SerializeField]
    protected ColourEnum colorEnum;

    private bool _isDisappearing;
    public bool willBeDisappeared;

    public ColourEnum GetColorEnum()
    {
        return colorEnum;
    }

    public override void OnBoltArrived()
    {
        if (_isDisappearing) return;

        if (GetOccupiedPointCount() == placementPoints.Count)
        {
            _isDisappearing = true; 
            HoleManager.instance.RemoveOldBringNew();
            transform.DOScale(Vector3.zero, .25f).OnComplete(() => Destroy(gameObject));
        }
    }

    public void CheckDisappearingSequence()
    {
        if (GetOccupiedPointCount() == placementPoints.Count)
            willBeDisappeared = true;
    }
}