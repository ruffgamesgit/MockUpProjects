using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using ColorUtility = DefaultNamespace.ColorUtility;

public class ConveyorManager : MonoSingleton<ConveyorManager>
{
    public event System.Action<ConveyorBoxController> NewConveyorBoxCameEvent;

    [Header("References")] [SerializeField]
    private ConveyorBoxController conveyorBoxPrefab;

    [Header("Debug")] [SerializeField] private ConveyorBoxController currentBox;
    private bool _sequencePerforming;
    [SerializeField] private List<ConveyorBoxController> spawnedBoxes = new();


    void Start()
    {
        SpawnBox();
    }

    public ConveyorBoxController GetCurrentBox()
    {
        return currentBox;
    }

    private void SpawnBox()
    {
        for (int i = 0; i < 2; i++)
        {
            ColorEnum randomColor = ColorUtility.GetRandomColorEnum();
            Vector3 pos = new(transform.position.x + (i * -4), transform.position.y, transform.position.z);
            ConveyorBoxController cloneBoardBox = Instantiate(conveyorBoxPrefab, pos, Quaternion.identity, transform);
            cloneBoardBox.Initialize(randomColor);
            spawnedBoxes.Add(cloneBoardBox);
        }

        currentBox = spawnedBoxes[0];
    }


    public void RemoveOldBringNew()
    {
        if (!GameManager.instance.isLevelActive) return;

        if (_sequencePerforming) return;
        _sequencePerforming = true;
        if (spawnedBoxes.Count > 2) return;
        spawnedBoxes.RemoveAt(0);

        // Adding New Box
        ColorEnum randomColor = ColorUtility.GetRandomColorEnum();
        if (LayerManager.instance.GetUniqueColorInBoxes().Count > 1)
        {
            while (spawnedBoxes[0].GetColorEnum() == randomColor)
            {
                randomColor = ColorUtility.GetRandomColorEnum();
            }
        }

        Vector3 pos = new(transform.position.x + (-4), transform.position.y, transform.position.z);
        ConveyorBoxController cloneBoardBox = Instantiate(conveyorBoxPrefab, pos, Quaternion.identity, transform);
        cloneBoardBox.transform.DOScale(Vector3.one, .5f).From(Vector3.zero);
        spawnedBoxes.Add(cloneBoardBox);
        cloneBoardBox.Initialize(randomColor);

        currentBox = spawnedBoxes[0];
        MoveBoxes();
    }

    void MoveBoxes()
    {
        for (int i = 0; i < spawnedBoxes.Count; i++)
        {
            Transform box = spawnedBoxes[i].transform;
            Vector3 pos = new(transform.position.x + (i * -4), transform.position.y, transform.position.z);
            int localIndex = i;
            box.DOMove(pos, .15f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                if (localIndex == spawnedBoxes.Count - 1)
                {
                    NewConveyorBoxCameEvent?.Invoke(GetCurrentBox());
                    _sequencePerforming = false;
                }
            });
        }
    }
}