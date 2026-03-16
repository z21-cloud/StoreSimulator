using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using StoreSimulator.InteractableObjects;
using StoreSimulator.PlayerController;
using StoreSimulator.PlayerCamera;
using StoreSimulator.Delivery;
using StoreSimulator.MoneySystem;

namespace StoreSimulator.Delivery
{
    public class PCInteractor : MonoBehaviour, IInteractable
    {
        [Header("PC Screen")]
        [SerializeField] private GameObject screenRoot;
        [Header("PC Screen main menu")]
        [SerializeField] private GameObject mainMenu;
        [Header("Delivery panel script")]
        [SerializeField] private DeliveryPanel deliveryPanel;
        [Header("Player input script")]
        [SerializeField] private PlayerInputController playerInputController;
        [Header("Player look script")]
        [SerializeField] private PlayerLook playerLook;
        [Header("Player wallet script")]
        [SerializeField] private PlayerWallet playerWallet;

        [Header("Player UI Balance")]
        [SerializeField] private BalanceUI balanceUI;

        public DeliveryPanel DeliveryPanel => deliveryPanel;

        private void Start()
        {
            // to be sure that GO is inactive at start
            screenRoot.SetActive(false);
        }

        public void Open()
        {
            // disable player banalce ui
            balanceUI.BalanceUISetActive(false);
            
            // enable PC screen
            screenRoot.SetActive(true);
            mainMenu.SetActive(true);
            
            // controls player's input
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            playerInputController.enabled = false;
            playerLook.enabled = false;
        }

        public void Close()
        {
            // enable and update player banalce ui
            balanceUI.UpdateBalanceUI();
            balanceUI.BalanceUISetActive(true);

            // disable PC screen
            screenRoot.SetActive(false);
            deliveryPanel.gameObject.SetActive(false);

            // controls player's input
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            playerInputController.enabled = true;
            playerLook.enabled = true;
        }

        public void OpenDelivery()
        {
            if (playerWallet == null)
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
}

