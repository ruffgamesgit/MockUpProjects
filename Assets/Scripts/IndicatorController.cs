using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class IndicatorController : MonoBehaviour
{
    [SerializeField] private GameObject indicatorObj;

    public void EnableIndicator()
    {
        indicatorObj.SetActive(true);
    }

    public void DisableIndicator()
    {
        indicatorObj.SetActive(false);
    }
}