using DG.Tweening;
using UnityEngine;

public class NumberObject : MonoBehaviour
{
    [Header("Config")] public int levelValue;

    [Header("References")] [SerializeField]
    private NumberObjectMeshSO meshDataSo;

    [Header("Debug")] [SerializeField] private GridCell occupiedCell;
    [SerializeField] private PlacementPoint currentPoint;
    private GameObject _currentModel;

    private void Awake()
    {
        levelValue = Random.Range(1, 7);
        occupiedCell = transform.GetComponentInParent<GridCell>();
        occupiedCell.SetNumberObject(this);
        SetMesh();
    }

    public void SetMesh()
    {
        if (_currentModel) Destroy(_currentModel);

        _currentModel = Instantiate(meshDataSo.meshes[levelValue - 1], transform.position + (Vector3.up / 4),
            Quaternion.identity,
            transform);
    }

    public void OnCellPicked()
    {
        if (PointManager.instance.GetOccupiedPointCount() == 7)
        {
            Debug.LogError("NO EMPTY POINT LEFT");
            return;
        }

        MoveToPoint(PointManager.instance.GetAvailablePoint(), true);
    }

    private void MoveToPoint(PlacementPoint targetPoint, bool informCell = false)
    {
        if (informCell)
        {
            occupiedCell.OnNumberObjectLeft();
            occupiedCell.SetNumberObject(null);
        }

        currentPoint?.SetFree();
        currentPoint = targetPoint;
        currentPoint.SetOccupied(this);

        transform.DOMove(targetPoint.transform.position, 0.5f).OnComplete(PointManager.instance.OnNewNumberArrived);
    }

    public void Merge(Vector3 targetPos)
    {
        currentPoint?.SetFree();
        transform.DOMoveX(targetPos.x, .25f).OnComplete((() => gameObject.SetActive(false)));
    }

    public void UpgradeSelf()
    {
        levelValue++;
        SetMesh();
        PointManager.instance.OnNewNumberArrived();
    }
}

/*
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

                matchablePizzas[i].DisapearFromLot();
                pizzas.Remove(matchablePizzas[i]);
                iterate--;
            }
        }
        else
        {
            if (pizzas.Count == Lots.Count)
            {
                GameManager.instance.EndGame(false);
                Debug.LogError("No more available lot, LOT is FULL");
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

    */