using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using StoreSimulator.InteractableObjects;
using StoreSimulator.StoreableItems;
using StoreSimulator.PlayerController;
using StoreSimulator.PlayerCamera;

namespace StoreSimulator.StoreUI
{
    public class PriceEditUI : MonoBehaviour
    {
        [Header("Player input script")]
        [SerializeField] private PlayerInputController playerInputController;
        [Header("Player look script")]
        [SerializeField] private PlayerLook playerLook;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TMP_Text currentPrice;
        [SerializeField] private TMP_Text basePrice;
        [SerializeField] private GameObject panel;

        private IPriceStorage _activeStorage;

        public void OpenForStorage(IPriceStorage storage)
        {
            _activeStorage = storage;
            panel.SetActive(true);
            currentPrice.text = $"{storage.GetCurrentPrice():F2}$";
            basePrice.text = $"{storage.GetBasePrice():F2}$";
            inputField.text = "";

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            playerInputController.enabled = false;
            playerLook.enabled = false;
        }

        public void ConfirmPrice()
        {
            if (float.TryParse(inputField.text, out float newPrice))
            {
                _activeStorage.OnPriceInputChanged(newPrice);
                panel.SetActive(false);

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                playerInputController.enabled = true;
                playerLook.enabled = true;
            }
        }

        public void ClosePanel()
        {
            panel.SetActive(false);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            playerInputController.enabled = true;
            playerLook.enabled = true;
        }
    }
}

