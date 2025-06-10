using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fener : MonoBehaviour
{
    [SerializeField] private GameObject Flashlight;
    [SerializeField] private AudioClip flashlightSound;
    [SerializeField] private Animator FenerAnim;
    private bool flashlightOn = true;
    private AudioSource aSource;

    private float gecenzaman = 0;
    private float cooldown = 0.5f;

    private void Start()
    {
        aSource = GetComponent<AudioSource>(); 
        Flashlight.SetActive(flashlightOn);
    }
    private void Update()
    {
        if (Time.time > gecenzaman)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && CharacterEnvanter.Instance.Fener.Count > 0)
            {
                StartCoroutine(FenerAnimSettings());
                aSource.PlayOneShot(flashlightSound);
                flashlightOn = !flashlightOn;
                Flashlight.SetActive(flashlightOn);

                gecenzaman = Time.time + cooldown;
            }
        }
    }

    IEnumerator FenerAnimSettings()
    {
        FenerAnim.SetTrigger("onclose");
        yield return new WaitForSeconds(0.5f);
        FenerAnim.SetTrigger("idle");
    }
}
