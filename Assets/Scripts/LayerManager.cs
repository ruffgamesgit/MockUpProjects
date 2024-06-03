using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;
using Random = UnityEngine.Random;

public class LayerManager : MonoSingleton<LayerManager>
{
    [SerializeField] private List<BoardBoxController> activeBoxes = new();
    [SerializeField] private List<ColorData> colorWeights;
    public event System.Action LayerDisappearedEvent;

    public void TriggerLayerDisappearEvent()
    {
        LayerDisappearedEvent?.Invoke();
    }

    protected override void Awake()
    {
        base.Awake();

        // Ensure the loop runs until all boxes are spawned or there are no available points.
        for (int i = 0; i < 4; i++)
        {
            int attempts = 0;
            for (int cc = 0; cc < colorWeights[i].count; cc++)
            {
                // Limit the number of attempts to find an available point to avoid infinite loops.
                while (attempts < 100)
                {
                    if (activeBoxes.Count == 0)
                    {
                        Debug.LogError("No active boxes available.");
                        return;
                    }

                    BoardBoxController randomBox = activeBoxes[Random.Range(0, activeBoxes.Count)];
                    if (randomBox.GetAvailablePoint())
                    {
                        randomBox.SpawnBox(colorWeights[i].ColorEnum);
                        break;
                    }
                    else
                    {
                        attempts++;
                    }
                }

                if (attempts >= 100)
                {
                    Debug.LogError("Failed to find an available point after 100 attempts.");
                    return;
                }
            }
        }
    }

    public List<ColorEnum> GetUniqueColorInBoxes()
    {
        List<ColorEnum> colors = new();

        for (int bb = 0; bb < activeBoxes.Count; bb++)
        {
            var box = activeBoxes[bb];
            for (int i = 0; i < box.GetColorInPoints().Count; i++)
            {
                if (!colors.Contains(box.GetColorInPoints()[i]))
                    colors.Add(box.GetColorInPoints()[i]);
            }
        }

        for (int j = 0; j < NeutralBox.instance.GetColorsInPoints().Count; j++)
        {
            if (!colors.Contains(NeutralBox.instance.GetColorsInPoints()[j]))
                colors.Add(NeutralBox.instance.GetColorsInPoints()[j]);
        }

        return colors;
    }

    public void RemoveBoxAndCheck(BoardBoxController box)
    {
        if (activeBoxes.Contains(box))
            activeBoxes.Remove(box);

        if (activeBoxes.Count == 0)
            GameManager.instance.EndGame(true);
    }
}