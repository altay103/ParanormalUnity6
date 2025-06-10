using System.Collections.Generic;
using UnityEngine;

public class GameUIManager : MonoBehaviour
{
    [SerializeField] private GameObject[] MenuPanels;  
    [SerializeField] private GameObject ButtonPanels;
    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject PlayerCamera;

    private bool menuIsOpened = false;
    private bool settingsIsOpened = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !PcUIManager.instance.IsPcMenuActive())
        {
            PauseGameControl();
        }
    }
    public void ContinueGame()
    {
        Debug.Log("Continue Game");

        menuIsOpened = false;

        MenuPanels[0].SetActive(false);      
        MenuPanels[1].SetActive(false);      
        OpenCloseAllButtons(false);
        settingsIsOpened = false;

        PlayerInteractionToggle();
    }

    public void PauseGameControl()
    {
        menuIsOpened = !menuIsOpened;

        if (menuIsOpened)
        {
            Debug.Log("Pause Game");
            MenuPanels[0].SetActive(true);      
            OpenCloseAllButtons(true);
        }
        else
        {
            Debug.Log("Menu Closed");
            MenuPanels[0].SetActive(false);     
            MenuPanels[1].SetActive(false);    
            OpenCloseAllButtons(false);
            settingsIsOpened = false;
        }

        PlayerInteractionToggle();
    }

    public void OpenCloseSettings(bool durum)
    {
        settingsIsOpened = durum;
        MenuPanels[1].SetActive(durum);        
        OpenCloseAllButtons(!durum);            

        Debug.Log(durum ? "Settings Opened" : "Settings Closed");
    }

    private void OpenCloseAllButtons(bool durum)
    {
        ButtonPanels.SetActive(durum);
    }

    private void PlayerInteractionToggle()
    {
        bool enablePlayer = !menuIsOpened;

        Player.GetComponent<physicWalk_MouseLook>().enabled = enablePlayer;
        PlayerCamera.GetComponent<physicWalk_MouseLook>().enabled = enablePlayer;

        Cursor.lockState = enablePlayer ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !enablePlayer;
    }
}
