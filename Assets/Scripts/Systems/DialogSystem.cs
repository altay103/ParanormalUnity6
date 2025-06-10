using System;
using System.Collections;
using UnityEngine;

public class DialogSystem : MonoBehaviour
{
    [Header("Dialoglar")] 
    public GameObject doorDialog;
    public GameObject dolapDialog;
    private bool isDialogRunning = false;
    public IEnumerator DialogTimer(GameObject dialog_OBJ)
    {
        if (isDialogRunning) yield break;
        
        isDialogRunning = true;
        
        dialog_OBJ.SetActive(true);
        yield return new WaitForSeconds(2);
        dialog_OBJ.SetActive(false);

        isDialogRunning = false;
    }
}