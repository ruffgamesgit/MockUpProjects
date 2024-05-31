using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using ColorUtility = DefaultNamespace.ColorUtility;

public class ConveyorManager : MonoSingleton<ConveyorManager>
{
    public event System.Action<ConveyorBoxController> NewConveyorBoxCameEvent;

    [Header("References")] [SerializeField]
    private ConveyorBoxController conveyorBoxPrefab;

    [FormerlySerializedAs("currentBox")] [Header("Debug")] [SerializeField]
    private ConveyorBoxController currentBox;

    [SerializeField] private List<ConveyorBoxController> spawnedBoxes = new List<ConveyorBoxController>();

    public ConveyorBoxController GetCurrentBox()
    {
        return spawnedBoxes[0];
    }

    protected override void Awake()
    {
        base.Awake();

        SpawnBox();
    }

    private void SpawnBox()
    {
        for (int i = 0; i < 3; i++)
        {
            ColorEnum randomColor = ColorUtility.GetRandomColorEnum();
            Vector3 pos = new(transform.position.x + (i * -4), transform.position.y, transform.position.z);
            ConveyorBoxController cloneBoardBox = Instantiate(conveyorBoxPrefab, pos, Quaternion.identity, transform);
            cloneBoardBox.Initialize(randomColor);
            spawnedBoxes.Add(cloneBoardBox);
        }
    }

    public void BringNewBox()
    {
        spawnedBoxes.RemoveAt(0);
        ColorEnum randomColor = ColorUtility.GetRandomColorEnum();
        Vector3 pos = new(transform.position.x + (-8), transform.position.y, transform.position.z);
        ConveyorBoxController cloneBoardBox = Instantiate(conveyorBoxPrefab, pos, Quaternion.identity, transform);
        spawnedBoxes.Add(cloneBoardBox);
        cloneBoardBox.Initialize(randomColor);
        MoveBoxes();
    }

    void MoveBoxes()
    {
        for (int i = 0; i < spawnedBoxes.Count; i++)
        {
            Transform box = spawnedBoxes[i].transform;
            Vector3 pos = new(transform.position.x + (i * -4), transform.position.y, transform.position.z);
            int localIndex = i;
            box.DOMove(pos, .25f).SetEase(Ease.OutQuad).OnComplete(() =>
            {
                if (localIndex == spawnedBoxes.Count - 1)
                {
                    NewConveyorBoxCameEvent?.Invoke(GetCurrentBox());
                }
            });
        }
    }
}