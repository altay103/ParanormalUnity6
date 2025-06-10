using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Start()
    {
        Physics.IgnoreLayerCollision(
            LayerMask.NameToLayer("FPS1"),
            LayerMask.NameToLayer("Interactible"),
            true
        );
    }
}
