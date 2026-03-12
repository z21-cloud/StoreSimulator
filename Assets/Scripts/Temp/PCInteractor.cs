using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.InteractableObjects;
using StoreSimulator.StoreableItems;
using StoreSimulator.Boxes;
using StoreSimulator.PlayerController;
using StoreSimulator.PlayerCamera;

public class PCInteractor : MonoBehaviour, IInteractable
{
    //[SerializeField] private DeliveryOrder waterOrder;
    //[SerializeField] private GameObject boxPrefab;
    //[SerializeField] private Transform spawnPoint;

    [SerializeField] private GameObject screenRoot;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private DeliveryPanel deliveryPanel;
    [SerializeField] private PlayerInputController playerInputController;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private PlayerWallet playerWallet;
    
    public DeliveryPanel DeliveryPanel => deliveryPanel;  

    private void Start()
    {
        // to be sure that GO is inactive at start
        screenRoot.SetActive(false);
    }

    public void Open()
    {
        screenRoot.SetActive(true);
        mainMenu.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        playerInputController.enabled = false;
        playerLook.enabled = false;
    }

    public void Close()
    {
        screenRoot.SetActive(false);
        deliveryPanel.gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerInputController.enabled = true;
        playerLook.enabled = true;
    }

    public void OpenDelivery()
    {
        if(playerWallet == null)
        {
            Debug.Log($"[PCInteractor]: wallet is null");
            return;
        }
        
        deliveryPanel.gameObject.SetActive(true);
        deliveryPanel.Init(playerWallet);
    }

    public string GetDescription()
    {
        throw new System.NotImplementedException();
    }
}
