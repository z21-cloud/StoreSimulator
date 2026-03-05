using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using StoreSimulator.InteractableObjects;
using StoreSimulator.StoreableItems;

namespace StoreSimulator.StoreUI
{
    public class PriceEditUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TMP_Text currentPrice;
        [SerializeField] private TMP_Text basePrice;

        private IPriceStorage _activeStorage;

        public void OpenForStorage(IPriceStorage storage)
        {
            _activeStorage = storage;
            gameObject.SetActive(true);
            currentPrice.text = $"{storage.GetCurrentPrice():F2}$";
            basePrice.text = $"{storage.GetBasePrice():F2}$";
            inputField.text = "";
            Cursor.lockState = CursorLockMode.None;
        }

        public void ConfirmPrice()
        {
            if (float.TryParse(inputField.text, out float newPrice))
            {
                _activeStorage.OnPriceInputChanged(newPrice);
                gameObject.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        public void ClosePanel()
        {
            gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}

