using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MergeMapSo", order = 2)]
public class MergeMapSoData : ScriptableObject
{
    public List<MapData> mapDatas;
}

[System.Serializable]
public class MapData
{
    public FoodType foodType;
    public List<Sprite> foodSprites;
}