using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MergeMapPanel : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private MergeMapSoData mergeMapSoData;
    [SerializeField] List<Image> images = new();

    private OrderImageController _parentImageController;


    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _parentImageController = transform.GetComponentInParent<OrderImageController>();
        
        _parentImageController.DisplayButtonClickedEvent += (SetEnable);
    }

    private void SetEnable()
    {
        _canvasGroup.alpha = 1;
        CustomerManager.instance.OnMapDisplayed(this);
        AssignMap();
    }

    public void SetDisable()
    {
        _canvasGroup.alpha = 0;
    }

    private void AssignMap()
    {
        FoodData targetData = _parentImageController.GetFoodData();
        List<Sprite> targetSprites = new();
        foreach (MapData t in mergeMapSoData.mapDatas)
        {
            if (targetData.foodType != t.foodType) continue;
            foreach (Sprite sprite in t.foodSprites)
            {
                targetSprites.Add(sprite);
            }

            break;
        }

        for (int i = 0; i < images.Count; i++)
        {
            images[i].sprite = targetSprites[i];
        }
    }
}