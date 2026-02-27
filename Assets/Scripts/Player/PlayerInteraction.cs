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

        public IInteractable GetCurrentInteractable()
        {
            _currentInteractable.TryGetComponent<IInteractable>(out var interactable);
            {
                return interactable;
            }
        }
    }
}

