using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoSingleton<CustomerManager>
{
    [Header("References")] [SerializeField]
    private List<OrderHandler> orderHandlers;
}