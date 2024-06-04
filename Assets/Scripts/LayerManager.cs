using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
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

        int defaultCount = (activeBoxes.Count * 3) / colorWeights.Count;
        int leftOut = (activeBoxes.Count * 3) - (defaultCount * colorWeights.Count);
        colorWeights[0].bottleCount += leftOut;
        for (int i = 0; i < colorWeights.Count; i++)
        {
            colorWeights[i].bottleCount += defaultCount;
        }

        int successfullIterate = 0;
        for (int i = 0; i < 4; i++)
        {
            int attempts = 0;
            for (int cc = 0; cc < colorWeights[i].bottleCount; cc++)
            {
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
                        successfullIterate++;
                        break;
                    }
                    attempts++;
                }

                if (attempts >= 200)
                {
                    Debug.LogError("Failed to find an available point after 200 attempts. Loading the scene again");
                    GameManager.instance.OnTapRestart();
                    return;
                }
            }
        }

        if (successfullIterate != activeBoxes.Count * 3)
        {
            Debug.LogWarning("Sucessfull iterate count is not equal total bottle count, " +
                             "restarting the level");
            GameManager.instance.OnTapRestart();
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

[System.Serializable]
public class ColorData
{
    public ColorEnum ColorEnum;

    [FormerlySerializedAs("colorCount")] [FormerlySerializedAs("count")] [HideInInspector]
    public int bottleCount;
}