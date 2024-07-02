using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NumberObjectMeshScriptableObject", order = 1)]
public class NumberObjectMeshSO : ScriptableObject
{
    public List<ColorData> meshColorData;
}
[System.Serializable]
public class ColorData
{
   public Color R_color;
   public Color G_color;
}
