using UnityEngine;
using UnityEngine.UI;

public class OrderImageController : MonoBehaviour
{
    public event System.Action DisplayButtonClickedEvent;
    public event System.Action OrderCompletedEvent;

    [Header("References")] [SerializeField]
    private Button button;
    [SerializeField] private Image image;
    [SerializeField] private GameObject doneImage;

    [Header("Debug")] public bool isCompleted;
    public bool isMarkedToBeCompleted;
    [SerializeField] private FoodData data;


    private void Start()
    {
        button.onClick.AddListener((OnButtonClicked));
    }

    private void OnButtonClicked()
    {
        DisplayButtonClickedEvent?.Invoke();
    }

    public void SetSprite(Sprite sprite, FoodData foodData)
    {
        image.sprite = sprite;
        data = foodData;
    }
    
    public FoodData GetFoodData()
    {
        return data;
    }

    public void SetCompleted()
    {
        isCompleted = true;
        doneImage.SetActive(true);

        SetAlpha(0.5f);
        OrderCompletedEvent?.Invoke();
    }

    public void ResetImage()
    {
        isCompleted = false;
        doneImage.SetActive(false);

        SetAlpha(1);
    }

    private void SetAlpha(float alphaValue)
    {
        Color color = image.color; // Get the current color
        color.a = alphaValue; // Modify the alpha value
        image.color = color;
    }
}