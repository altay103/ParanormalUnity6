using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sebeke : MonoBehaviour
{
    [SerializeField] int istenilenSebeke = 4;
    [SerializeField] bool elektirikAktif = false;
    [SerializeField] CharacterEnvanter envanter;
    [SerializeField] List<GameObject> gucPoint = new();
    [SerializeField] List<Renderer> gucLight = new();
    
    [SerializeField] private AudioClip sebekeSound;
    private AudioSource aSource;
    
    private float gecenzaman = 0;
    private float kalanzaman = 0.8f;

    private void Start()
    {
        aSource = GetComponent<AudioSource>();
    }

    public void SebekeTak()
    {
        if (istenilenSebeke <= 0 || elektirikAktif || envanter.Sebekeler.Count == 0)
        {
            Debug.Log("Sebeke takılamaz.");
            return;
        }

        if (Time.time < gecenzaman)
        {
            Debug.Log("Cooldown: Şebeke takmak için bekle");
            return;
        }

        for (int i = 0; i < gucPoint.Count; i++)
        {
            if (gucPoint[i].transform.childCount == 0)
            {
                gecenzaman = Time.time + kalanzaman;

                GameObject yeniSebeke = envanter.Sebekeler[0];
                
                envanter.ItemCikar(yeniSebeke);
                envanter.Sebekeler.Remove(yeniSebeke);
                
                envanter.StartCoroutine(envanter.SwitchToItemSilently(null));

                yeniSebeke.transform.SetParent(gucPoint[i].transform);
                yeniSebeke.transform.localPosition = Vector3.zero;
                yeniSebeke.transform.localRotation = Quaternion.Euler(180f, 180f, 0f);
                yeniSebeke.transform.localScale = new Vector3(3.60509f, 2.635479f, 65.48545f);
                yeniSebeke.layer = LayerMask.NameToLayer("Interactible");
                yeniSebeke.tag = "PlacedSebeke";

                BoxCollider col = yeniSebeke.GetComponent<BoxCollider>();
                if (col != null) col.enabled = false;

                if (i < gucLight.Count && gucLight[i])
                {
                    gucLight[i].material.color = Color.green;
                    Light l = gucLight[i].GetComponentInChildren<Light>();
                    if (l) l.color = Color.green;
                }

                aSource.PlayOneShot(sebekeSound);
                istenilenSebeke--;

                yeniSebeke.SetActive(true);
                envanter.StartCoroutine(envanter.OtomatikGeciseZorla());
                return;
            }
        }

        Debug.Log("Boş slot bulunamadı, şebeke geri kondu.");
    }

    private void Update()
    {
        if (istenilenSebeke == 0)
            elektirikAktif = true;
    }
}