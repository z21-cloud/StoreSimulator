using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
    public enum PlayerMode { Default, Building }
    public class PlayerInteraction : MonoBehaviour
    {
        [Header("Interaction detector")]
        [SerializeField] private InteractionDetector interactionDetector;
        [Header("Interaction detector")]
        [SerializeField] private HoldingHandler holdingHandler;
        [Header("Interaction detector")]
        [SerializeField] private PlayerThrowHandler throwHandler;
        [Header("Interaction detector")]
        [SerializeField] private PackingHandler packingHandler;
        [Header("Building ")]
        [SerializeField] private BuildingHandler buildingHandler;
        [Header("Building UI")]
        [SerializeField] private GameObject buildingUI;

        private PlayerMode _currentMode = PlayerMode.Default;


        private GameObject _currentInteractable;

        private void Update()
        {
            _currentInteractable = interactionDetector.CurrentInteractable;
            Debug.Log(_currentMode);
            Debug.Log(_currentInteractable);
        }

        public void ToggleBuildingMode()
        {
            _currentMode = _currentMode == PlayerMode.Default
                        ? PlayerMode.Building
                        : PlayerMode.Default;
            
            if(_currentMode == PlayerMode.Default) buildingHandler.CancelHolding();
            buildingUI.SetActive(_currentMode == PlayerMode.Building);
        }
        // Interaction with gameobjects
        public void DoInteract()
        {
            if(_currentMode == PlayerMode.Building)
                buildingHandler.DoInteract(_currentInteractable);
            
            else
                holdingHandler.DoInteract(_currentInteractable);
        }

        public void DoPlace()
        {
            if(_currentMode != PlayerMode.Building) return;
            buildingHandler.DoPlace();
        }

        public void DoThrow()
        {
            bool value = throwHandler.DoThrow(holdingHandler.HeldObject);
            if (value) holdingHandler.ClearHeldObject();
        }

        public void DoPack()
        {
            bool value = packingHandler.DoPackItem(holdingHandler.HeldObject, _currentInteractable);
            Debug.Log($"PlayerInteraction: {value}");
            if (value) holdingHandler.ClearHeldObject();
        }

        public IInteractable GetCurrentInteractable()
        {
            _currentInteractable.TryGetComponent<IInteractable>(out var interactable);
            {
                return interactable;
            }
        }
    }
}

