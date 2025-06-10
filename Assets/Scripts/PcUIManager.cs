using System;
using UnityEngine;
public class PcUIManager : MonoBehaviour
{
    public static PcUIManager instance;
    [SerializeField] private GameObject[] MenuPanels; 
    [SerializeField] private GameObject ButtonPanels;
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject PlayerCamera;

    [Header("Map Index")] 
    [SerializeField] private int Hastane = 1;
    
    public string GidilicekMap;
    
    public bool menuIsOpened = false;
    private bool mapPanelIsOpened = false;

    private void Awake()
    {
        instance = this;
        GidilicekMap = "";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePc();
        }
    }

    void ClosePc()
    {
        if (menuIsOpened)
        {
            menuIsOpened = false;
            PlayerInteractionToggle();
            
            CharacterRaycast characterRay = Player.GetComponent<CharacterRaycast>();
            if (characterRay != null)
            {
                characterRay.ReturnToGameplayCamera(); 
            }
        }
    }

    public void OpenMapPanel()
    {
        if(mapPanelIsOpened)
        {
            mapPanelIsOpened = false;
            MenuPanels[0].SetActive(false);
        }
        else
        {
            mapPanelIsOpened = true;
            MenuPanels[0].SetActive(true);
        }
    }

    public void MapSec(string mapName)
    {
        if (mapName == "Hastane")
        {
            GidilicekMap = Hastane.ToString();
        }
    }
    
    public bool IsPcMenuActive()
    {
        return menuIsOpened || mapPanelIsOpened;
    }
    
    public void PlayerInteractionToggle()
    {
        bool enablePlayer = !menuIsOpened;

        Player.GetComponent<physicWalk_MouseLook>().enabled = enablePlayer;
        PlayerCamera.GetComponent<physicWalk_MouseLook>().enabled = enablePlayer;

        Cursor.lockState = enablePlayer ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !enablePlayer;
    }

}