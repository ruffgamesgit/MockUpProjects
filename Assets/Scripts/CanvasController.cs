using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private Button restartButton;

    private void Awake()
    {
        restartButton.onClick.AddListener((() => SceneManager.LoadScene(0)));
    }
}