using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    [SerializeField] private string itemDisplayName;

    public string GetItemName()
    {
        return itemDisplayName;
    }
}

