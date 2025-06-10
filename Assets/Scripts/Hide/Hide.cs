using System;
using UnityEngine;

public class Hide : MonoBehaviour
{
    private bool isPlayerInside;
    public bool IsPlayerInside => isPlayerInside;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerInside = true;
            Debug.Log("Saklanma Alanına Girildi");
        }
    }

    public void SetCovered()
    {
        isPlayerInside = false;
    }
}