using UnityEngine;

namespace FuseboxSystem
{
    public class FBInventoryManager : MonoBehaviour
    {
        [Header("Fuses in Inventory")]
        public int inventoryFuses;

        public static FBInventoryManager instance;

        private void Awake()
        {
            if (instance != null) { Destroy(gameObject); }
            else { instance = this; DontDestroyOnLoad(gameObject); }
        }

        private void Update()
        {
            if (Input.GetKeyDown(FBInputManager.instance.inventoryKey))
            {
                FBUIManager.instance.OpenInventory();
            }
        }

        public void UpdateFuseUI()
        {
            inventoryFuses++;
            FBUIManager.instance.UpdateFuseUI(inventoryFuses);
        }

        public void MinusFuseUI()
        {
            inventoryFuses--;
            FBUIManager.instance.UpdateFuseUI(inventoryFuses);
        }
    }
}
