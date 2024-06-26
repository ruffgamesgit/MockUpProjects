using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NumberObjectMeshScriptableObject", order = 1)]
public class NumberObjectMeshSO : ScriptableObject
{
    public List<GameObject> meshes;
}
