using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OrderHandler : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private List<TextMeshProUGUI> texts;

    [Header("Debug")] [SerializeField] private List<FoodData> foodDatas;

    private void Awake()
    {
        for (int i = 0; i < texts.Count; i++)
        {
            FoodData randomData = DataExtensions.GetRandomFoodData();
            foodDatas.Add(randomData);
            texts[i].text =
                randomData.foodType + ", " + randomData.level;
        }
    }
}