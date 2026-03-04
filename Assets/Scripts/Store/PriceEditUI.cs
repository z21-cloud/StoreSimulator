using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using StoreSimulator.InteractableObjects;

public class PriceEditUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    private IStorage _activeStorage;

    public void OpenForStorage(IStorage storage)
    {
        _activeStorage = storage;
        gameObject.SetActive(true);
        inputField.text = "";
        Cursor.lockState = CursorLockMode.None;
    }

    public void ConfirmPrice()
    {
        if(float.TryParse(inputField.text, out float newPrice))
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
