using UnityEngine;

public class BasePointClass : MonoBehaviour
{
    [SerializeField] protected PizzaController currentPizza;
    [SerializeField] protected bool isOccupied;

    public Vector3 GetPos()
    {
        return new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z);
    }

    public void SetOccupied(PizzaController pizza)
    {
        isOccupied = true;
        currentPizza = pizza;
    }

    public void SetFree()
    {
        isOccupied = false;
        currentPizza = null;
    }

    public PizzaController GetPizza()
    {
        return currentPizza;
    }

    public bool CheckIfOccupied()
    {
        return isOccupied;
    }
}