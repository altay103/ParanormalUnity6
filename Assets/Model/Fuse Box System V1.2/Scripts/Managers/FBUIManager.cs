using UnityEngine;
using UnityEngine.UI;

public class FBUIManager : MonoBehaviour
{
    [Header("Fuse UI")]
    [SerializeField] private Text fuseUI = null;

    [Header("Inventory Canvas GameObject")]
    [SerializeField] private GameObject inventoryCanvas = null;
    private bool isOpen;

    [Header("Crosshair")]
    [SerializeField] private Image crosshair = null;

    public static FBUIManager instance;

    private void Awake()
    {
        if (instance != null) { Destroy(gameObject); }
        else { instance = this; DontDestroyOnLoad(gameObject); }
    }

    public void UpdateFuseUI(int fusesAmount)
    {
        fuseUI.text = fusesAmount.ToString("0");
    }

    public void OpenInventory()
    {
        isOpen = !isOpen;

        inventoryCanvas.SetActive(isOpen);
    }

    public void CrosshairChange(bool on)
    {
        if (on)
        {
            crosshair.color = Color.red;
        }
        else
        {
            crosshair.color = Color.white;
        }
    }
}
