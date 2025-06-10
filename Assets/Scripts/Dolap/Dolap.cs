using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class Dolap : MonoBehaviour
{
    [SerializeField] Animator dolapAnim;
    [SerializeField] AudioSource dolapSound;
    [SerializeField] private AudioClip dolapOpen;
    [SerializeField] private AudioClip dolapClose;
    [SerializeField] private AudioClip dolapTryOpening;
    [SerializeField] private DialogSystem dialog;
    [SerializeField] private GameObject HideObj;

    public bool isOpen;
    
    private bool isAnimating = false;
    
    private bool isLockingAnimPlayed = false;

    public void DolapOpenClose()
    {
            if (isAnimating) return;
        
            if (dolapAnim.GetBool("Open"))
            {
                Close();
            }
            else if (dolapAnim.GetBool("Close"))
            {
                Open();
            }
            isAnimating = true;
            Invoke(nameof(AnimasyonBitti), dolapAnim.GetBool("Close") ? 1.20f : 1f);
    }

    void Open()
    {
        dolapSound.PlayOneShot(dolapOpen);
        dolapAnim.SetBool("Open", true);
        dolapAnim.SetBool("Close", false);
        HideObj.GetComponent<Hide>().SetCovered();
        HideObj.SetActive(false);
        isOpen = true;
    }

    void Close()
    {
        dolapSound.PlayOneShot(dolapClose);
        dolapAnim.SetBool("Close", true);
        dolapAnim.SetBool("Open", false);
        HideObj.SetActive(true);
        isOpen = false;
    }

    public void LayerAyarlaKapali()
    {
        if (gameObject.tag == "Dolap")
        {
            Physics.IgnoreLayerCollision(
                LayerMask.NameToLayer("FPS1"),
                LayerMask.NameToLayer("InteractibleDolap"),
                false
            );
        }
    }
    public void LayerAyarlaAcik()
    {
        if (gameObject.tag == "Dolap")
        {
            Physics.IgnoreLayerCollision(
                LayerMask.NameToLayer("FPS1"),
                LayerMask.NameToLayer("InteractibleDolap"),
                true
            );
        }
    }
    private void AnimasyonBitti()
    {
        isAnimating = false;
    }
}