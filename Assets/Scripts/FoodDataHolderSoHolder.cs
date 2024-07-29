using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/FoodDataSO", order = 1)]
public class FoodDataHolderSoHolder : ScriptableObject
{
    public List<SoData> generalFoodDatas;
}

[System.Serializable]
public class SoData
{
    public FoodData foodData;
    public Sprite sprite;
    public Mesh mesh;
    public Material[] materials;
}