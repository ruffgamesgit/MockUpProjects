using System.Collections.Generic;
using UnityEngine;

public class LotHolder : MonoSingleton<LotHolder>
{
    public List<Lot> Lots = new List<Lot>();
    [SerializeField] private int minMatchCount;

    public Lot GetAvailableLot()
    {
        for (int i = 0; i < Lots.Count; i++)
        {
            if (!Lots[i].CheckIfOccupied())
                return Lots[i];
        }

        return null;
    }

    public bool HasAvailableLot()
    {
        bool has = false;

        for (int i = 0; i < Lots.Count; i++)
        {
            if (!Lots[i].CheckIfOccupied())
                has = true;
        }

        return has;
    }

    public void CheckForPossibleMatches()
    {
        List<PizzaController> pizzas = GetAllPizzasOnLots();
        if (pizzas.Count < minMatchCount) return;
        Dictionary<PizzaType, List<PizzaController>> separatedPizzas = SeparatePizzaControllersByType(pizzas);
        List<PizzaController> matchablePizzas = new();
        foreach (var kvp in separatedPizzas)
        {
            if (kvp.Value.Count >= minMatchCount)
            {
                matchablePizzas = kvp.Value;
                break;
            }
        }

        int iterate = minMatchCount;

        if (matchablePizzas.Count != 0)
        {
            for (int i = 0; i < matchablePizzas.Count; i++)
            {
                if (iterate == 0) break;
                
                matchablePizzas[i].DisapperaFromLot();
                iterate--;
            }
        }
    }

    List<PizzaController> GetAllPizzasOnLots()
    {
        List<PizzaController> pizzas = new();

        for (int i = 0; i < Lots.Count; i++)
        {
            if (Lots[i].GetPizza())
                pizzas.Add(Lots[i].GetPizza());
        }

        return pizzas;
    }

    Dictionary<PizzaType, List<PizzaController>> SeparatePizzaControllersByType(List<PizzaController> controllers)
    {
        Dictionary<PizzaType, List<PizzaController>> separatedControllers =
            new Dictionary<PizzaType, List<PizzaController>>();

        // Initialize the dictionary with empty lists for each PizzaType
        foreach (PizzaType type in System.Enum.GetValues(typeof(PizzaType)))
        {
            separatedControllers[type] = new List<PizzaController>();
        }

        // Populate the dictionary
        foreach (PizzaController controller in controllers)
        {
            if (separatedControllers.ContainsKey(controller.GetPizzaData().pizzaType))
            {
                separatedControllers[controller.GetPizzaData().pizzaType].Add(controller);
            }
        }

        return separatedControllers;
    }
}