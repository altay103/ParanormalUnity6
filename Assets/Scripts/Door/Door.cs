using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{
    public static Door Instance;

    [SerializeField] Animator doorAnim;
    [SerializeField] AudioSource doorSound;
    [SerializeField] private AudioClip doorOpen;
    [SerializeField] private AudioClip doorClose;
    [SerializeField] private AudioClip doorTryOpening;
    [SerializeField] private DialogSystem dialog;
    [SerializeField] private CharacterEnvanter envanter;

    [Header("Kapıyı açacak anahtar ID")] public string requiredKeyID;

    private bool isAnimating = false;
    private bool isLockingAnimPlayed = false;
    public bool KapiKilit = true;
    public bool isDoorOpen;

    private void Awake()
    {
        Instance = this;

        ObstacleCheck();
    }

    public void DoorOpenClose()
    {
        ;
        if (isAnimating) return;

        if (!KapiKilit)
        {
            ToggleDoor();
            return;
        }

        if (envanter.OwnedKeyIDs.Contains(requiredKeyID))
        {
            var currentKey = envanter.CurrentItem?.GetComponent<KeySystem>();

            if (currentKey != null && currentKey.keyID == requiredKeyID)
            {
                Debug.Log("Doğru anahtar bulundu, kapı açıldı: " + requiredKeyID);

                envanter.ItemCikar(envanter.CurrentItem);

                KapiKilit = false;
                ToggleDoor();
            }
            else
            {
                StartCoroutine(LockAnimTimer());
                StartCoroutine(dialog.DialogTimer(dialog.doorDialog));
                Debug.Log("Doğru anahtar çantada var ama elinde değil: " + requiredKeyID);
            }
        }
        else
        {
            StartCoroutine(LockAnimTimer());
            StartCoroutine(dialog.DialogTimer(dialog.doorDialog));
            Debug.Log("Kapı kilitli, doğru anahtar yok: " + requiredKeyID);
        }


        ObstacleCheck();
    }

    private void ToggleDoor()
    {
        if (doorAnim.GetBool("Open"))
        {
            doorSound.PlayOneShot(doorClose);
            doorAnim.SetBool("Close", true);
            doorAnim.SetBool("Open", false);
        }
        else
        {
            doorSound.PlayOneShot(doorOpen);
            doorAnim.SetBool("Open", true);
            doorAnim.SetBool("Close", false);
        }

        isAnimating = true;
        Invoke(nameof(AnimasyonBitti), doorAnim.GetBool("Close") ? 1.20f : 1f);
    }

    private void LayerAyarlaKapali()
    {
        if (gameObject.tag != "Door")
        {
            Physics.IgnoreLayerCollision(
                LayerMask.NameToLayer("FPS1"),
                LayerMask.NameToLayer("InteractibleDoor"),
                false
            );
        }
    }
    
    private void LayerAyarlaAcik()
    {
        if (gameObject.tag != "Door")
        {
            Physics.IgnoreLayerCollision(
                LayerMask.NameToLayer("FPS1"),
                LayerMask.NameToLayer("InteractibleDoor"),
                true
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && !isDoorOpen && !KapiKilit)
        {
            OpenDoor();
            isDoorOpen = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && isDoorOpen && !KapiKilit)
        {
            CloseDoor();
            isDoorOpen = false;
        }
    }

    public void OpenDoor()
    {
        doorSound.PlayOneShot(doorOpen);
        doorAnim.SetBool("Open", true);
        doorAnim.SetBool("Close", false);

        Physics.IgnoreLayerCollision(
            LayerMask.NameToLayer("FPS1"),
            LayerMask.NameToLayer("InteractibleDoor"),
            true
        );
    }

    public void CloseDoor()
    {
        doorSound.PlayOneShot(doorClose);
        doorAnim.SetBool("Close", true);
        doorAnim.SetBool("Open", false);

        Physics.IgnoreLayerCollision(
            LayerMask.NameToLayer("FPS1"),
            LayerMask.NameToLayer("InteractibleDoor"),
            false
        );
    }

    private void AnimasyonBitti()
    {
        isAnimating = false;
    }

    IEnumerator LockAnimTimer()
    {
        if (isLockingAnimPlayed) yield break;

        doorSound.PlayOneShot(doorTryOpening);
        isLockingAnimPlayed = true;
        doorAnim.SetTrigger("Lock");
        yield return new WaitForSeconds(1.5f);
        doorAnim.SetTrigger("Idle");
        isLockingAnimPlayed = false;
    }

    void ObstacleCheck()
    {
        if (KapiKilit)
        {
            gameObject.transform.GetChild(0).GetComponent<NavMeshObstacle>().carving = true;
        }
        else
        {
            gameObject.transform.GetChild(0).GetComponent<NavMeshObstacle>().carving = false;
        }
    }
}