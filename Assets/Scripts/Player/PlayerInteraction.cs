using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StoreSimulator.InteractableObjects
{
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

        // components
        private GameObject _currentInteractable;
        
        private void Update()
        {
            _currentInteractable = interactionDetector.CurrentInteractable;
        }

        // Interaction with gameobjects
        public void DoInteract()
        {
            holdingHandler.DoInteract(_currentInteractable);
        }

        public void DoThrow()
        {
            bool value = throwHandler.DoThrow(holdingHandler.HeldObject);
            if(value) holdingHandler.ClearHeldObject();
        }

        public void DoPack()
        {
            bool value = packingHandler.DoPackItem(holdingHandler.HeldObject, _currentInteractable);
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

