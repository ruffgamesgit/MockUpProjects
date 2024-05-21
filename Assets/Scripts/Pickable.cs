using UnityEngine;

public class Pickable
{
    [Header("Debug")] public bool IsPicked;

    public void GetPicked()
    {
        IsPicked = true;
    }
}