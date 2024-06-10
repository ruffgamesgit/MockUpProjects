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
            Debug.LogWarning("Colored hole is full, and disappearing");
            HoleManager.instance.RemoveOldBringNew();
            Sequence sq = DOTween.Sequence();
            sq.Append(transform.DOMoveX(transform.position.x + 7, .25f));
            sq.Append(transform.DOScale(Vector3.zero, .25f).OnComplete(() => Destroy(gameObject, .5f)));
        }
    }

    public void CheckDisappearingSequence()
    {
        if (GetOccupiedPointCount() == placementPoints.Count)
            willBeDisappeared = true;
    }
}