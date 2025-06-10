using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class CharacterRaycast : MonoBehaviour
{
    [SerializeField] private Transform cameraOBJ;
    [SerializeField] private float distance;
    [SerializeField] private LayerMask Interaction;
    [SerializeField] private LayerMask InteractionDoor;
    [SerializeField] private LayerMask InteractionDolap;
    [SerializeField] private GameObject InteractionCursor;
    [SerializeField] CharacterEnvanter envanter;
    [SerializeField] private AudioClip pickUpClip;
    
    [SerializeField] private GameObject gameplayVCamObj;
    [SerializeField] private GameObject hedefVCamObj;
    [SerializeField] private CinemachineCamera gameplayVCam;
    [SerializeField] private CinemachineCamera hedefVCam;

    private Vector3 gameplayCamStartPos;
    private Quaternion gameplayCamStartRot;


    private AudioSource aSource;
    private RaycastHit hit;
    private Highlighters.Highlighter lastHighlighter = null;

    private void Start()
    {
        aSource = GetComponent<AudioSource>();
        
        gameplayVCamObj.SetActive(false);
        hedefVCamObj.SetActive(false);
        
        gameplayCamStartPos = gameplayVCam.transform.position;
        gameplayCamStartRot = gameplayVCam.transform.rotation;
    }

    private void Update()
    {
        Debug.DrawRay(cameraOBJ.position, cameraOBJ.forward * distance, Color.red);

        if (Physics.Raycast(cameraOBJ.position, cameraOBJ.forward, out hit, distance))
        {
            if (((1 << hit.transform.gameObject.layer) & (Interaction | InteractionDoor | InteractionDolap)) != 0)
            {
                InteractionCursor.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (hit.transform.CompareTag("Door"))
                    {
                        hit.transform.GetComponentInParent<Door>().DoorOpenClose();
                        Debug.Log("Kapı Açıldı");
                    }
                    else if (hit.transform.CompareTag("Dolap"))
                    {
                        hit.transform.GetComponent<Dolap>().DolapOpenClose();
                        Debug.Log("Dolap Açıldı");
                    }
                    else if (hit.transform.CompareTag("SebekeKutusu"))
                    {
                        var sebekeScript = hit.transform.GetComponent<Sebeke>();
                        if (sebekeScript != null)
                        {
                            sebekeScript.SebekeTak();
                            Debug.Log("Şebeke Konmaya Çalışıldı");
                        }
                        else
                        {
                            Debug.LogWarning("Şebeke scripti bulunamadı!");
                        }
                    }
                }
                else if (Input.GetKeyDown(KeyCode.F))
                {
                    if (hit.transform.CompareTag("Sebeke") ||
                        hit.transform.CompareTag("Not") ||
                        hit.transform.CompareTag("Fener") ||
                        hit.transform.CompareTag("Key") ||
                        hit.transform.CompareTag("Cekic") ||
                        hit.transform.CompareTag("Benzin"))
                    {
                        bool alindiMi = envanter.EnvantereEkle(hit.transform.gameObject);
                        if (alindiMi)
                        {
                            hit.transform.gameObject.SetActive(false);
                            ObjeAlmaSound();
                            Debug.Log(hit.transform.tag + " Alındı");
                        }
                        else
                        {
                            Debug.LogWarning("Obje alınamadı: Envanter dolu olabilir!");
                        }
                    }
                    else if (hit.transform.CompareTag("Araba"))
                    {
                        int index = int.Parse(PcUIManager.instance.GidilicekMap);
                        SceneManager.LoadScene(index);
                    }
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    if (hit.transform.CompareTag("KeypadButton"))
                    {
                        string digit = hit.transform.name.Replace("Button", "");
                        var kasa = hit.transform.GetComponentInParent<KasaSystem>();
                        if (kasa != null)
                        {
                            kasa.PressButton(digit);
                        }
                    }
                    else if (hit.transform.CompareTag("OyunBaslatma"))
                    {
                        PcUIManager.instance.menuIsOpened = true;
                        PcUIManager.instance.PlayerInteractionToggle();

                        gameplayVCamObj.SetActive(true);
                        hedefVCamObj.SetActive(true);

                        gameplayVCam.Follow = hedefVCam.transform;
                        gameplayVCam.LookAt = hedefVCam.transform;
                        gameplayVCam.Priority = 10;
                    }

                }

                var highlighter = hit.transform.GetComponent<Highlighters.Highlighter>();
                if (highlighter != null)
                {
                    if (lastHighlighter != null && lastHighlighter != highlighter)
                        lastHighlighter.enabled = false;

                    highlighter.enabled = true;
                    lastHighlighter = highlighter;
                }
                else if (lastHighlighter != null)
                {
                    lastHighlighter.enabled = false;
                    lastHighlighter = null;
                }
            }
            else
            {
                InteractionCursor.SetActive(false);

                if (lastHighlighter != null)
                {
                    lastHighlighter.enabled = false;
                    lastHighlighter = null;
                }
            }
        }
        else
        {
            InteractionCursor.SetActive(false);

            if (lastHighlighter != null)
            {
                lastHighlighter.enabled = false;
                lastHighlighter = null;
            }
        }
    }


    void ObjeAlmaSound()
    {
        aSource.PlayOneShot(pickUpClip);
    }


    public void ReturnToGameplayCamera()
    {
        StartCoroutine(ResetGameplayCameraSmooth());
    }
    private IEnumerator ResetGameplayCameraSmooth()
    {
        Transform camTransform = gameplayVCam.transform;

        Vector3 startPos = camTransform.position;
        Quaternion startRot = camTransform.rotation;

        float duration = 1.5f;
        float time = 0f;

        gameplayVCam.Follow = null;
        gameplayVCam.LookAt = null;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            camTransform.position = Vector3.Lerp(startPos, gameplayCamStartPos, t);
            camTransform.rotation = Quaternion.Slerp(startRot, gameplayCamStartRot, t);

            yield return null;
        }

        camTransform.position = gameplayCamStartPos;
        camTransform.rotation = gameplayCamStartRot;

        gameplayVCamObj.SetActive(false);
        hedefVCamObj.SetActive(false);
    }

}