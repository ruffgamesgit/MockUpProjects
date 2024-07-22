using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrderHandler : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private FoodDataHolder dataSO;

    [SerializeField] private List<Image> images;

    [Header("Debug")] private Dictionary<Image, FoodData> _foodDataDictionary;
    [SerializeField] private int completedOrderCount;

    private void Awake()
    {
        AssignFoodData();
    }

    private void AssignFoodData()
    {
        _foodDataDictionary = new Dictionary<Image, FoodData>();

        foreach (Image image in images)
        {
            FoodData randomFoodData = DataExtensions.GetRandomFoodData(1);
            _foodDataDictionary[image] = randomFoodData;
            image.sprite = GetSpriteFromSO(randomFoodData);
        }
    }

    public void RemoveOrder(FoodData toRemoveData)
    {
        Image imageToRemove = null;

        foreach (var kvp in _foodDataDictionary)
        {
            if (kvp.Value.foodType == toRemoveData.foodType)
            {
                imageToRemove = kvp.Key;
                break;
            }
        }

        if (imageToRemove != null)
        {
            _foodDataDictionary.Remove(imageToRemove);
            imageToRemove.sprite = null;

            FoodData newRandomFoodData = DataExtensions.GetRandomFoodData(1);
            _foodDataDictionary[imageToRemove] = newRandomFoodData;
            imageToRemove.sprite = GetSpriteFromSO(newRandomFoodData);

            CustomerManager.instance.CheckIfDataMatchesForOrders(newRandomFoodData, this);
        }
    }

    public void CompleteOrder(FoodData givenData)
    {
        Image targetImage = null;

        foreach (var kvp in _foodDataDictionary)
        {
            if (kvp.Value.foodType == givenData.foodType)
            {
                if (Mathf.Approximately(kvp.Key.color.a, 1f))
                {
                    targetImage = kvp.Key;
                    break;
                }
            }
        }

        if (targetImage != null)
        {
            completedOrderCount++;

            if (completedOrderCount == 2)
            {
                completedOrderCount = 0;
                AssignFoodData();
                foreach (Image image in images)
                {
                    Debug.Log(2);
                    Color color = image.color; // Get the current color
                    color.a = 1; // Modify the alpha value
                    image.color = color;
                    image.transform.GetChild(0).gameObject.SetActive(false);

                    FoodData newData = _foodDataDictionary[image];
                    CustomerManager.instance.CheckIfDataMatchesForOrders(newData, this);
                }
            }
            else
            {
                Debug.Log(1);
                Color color = targetImage.color; // Get the current color
                color.a = .5f; // Modify the alpha value
                targetImage.color = color;
                targetImage.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    public List<FoodData> GetFoodDataFromDict()
    {
        return new List<FoodData>(_foodDataDictionary.Values);
    }

    public Sprite GetSpriteFromSO(FoodData data)
    {
        foreach (var soData in dataSO.generalFoodDatas)
        {
            if (data.foodType == soData.foodData.foodType &&
                data.level == soData.foodData.level)
            {
                return soData.sprite;
            }
        }

        return null;
    }

    public List<Image> GetEmptyImages()
    {
        List<Image> emptyImages = new List<Image>();

        foreach (var image in images)
        {
            if (!_foodDataDictionary.ContainsKey(image))
            {
                emptyImages.Add(image);
            }
        }

        return emptyImages;
    }
}