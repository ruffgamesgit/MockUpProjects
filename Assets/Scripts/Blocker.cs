using UnityEngine;
using UnityEngine.UI;

public class Blocker : MonoBehaviour
{
    [SerializeField] private Button button;
    private MergeMapPanel _activatedPanel;

    private void Start()
    {
        button.onClick.AddListener((OnButtonClicked));
    }

    public void Activate(MergeMapPanel newPanel)
    {
        _activatedPanel = newPanel;
        button.interactable = true;
    }

    private void OnButtonClicked()
    {
        _activatedPanel?.SetDisable();
        _activatedPanel = null;
        gameObject.SetActive(false);
    }
}