using System.Collections.Generic;
using StoreSimulator.MoneySystem;
using StoreSimulator.PlayerCamera;
using StoreSimulator.PlayerController;
using UnityEngine;

public class BuildingUIController : MonoBehaviour
{
    [SerializeField] private GameObject buildingUI;
    //[SerializeField] private List<StoreOrder> availableOrders;
    //[SerializeField] private Transform orderList;
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private StoreOrder shelfOrder;

    [SerializeField] private PlayerInputController playerInputController;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private PlayerWallet playerWallet;
    [SerializeField] private BuildingHandler buildingHandler;
    [SerializeField] private BalanceUI balanceUI;

    // store order = delivery order
    // order shelf = orderItem
    public void Open()
    {
        buildingUI.SetActive(true);
        balanceUI.gameObject.SetActive(false);

        Cursor.visible = true;

        Cursor.lockState = CursorLockMode.None;

        playerInputController.enabled = false;
        playerLook.enabled = false;
    }

    /*public void Init()
    {
        foreach(var order in availableOrders)
        {
            OrderShelf storeOrder = Instantiate(shelfOrderPrefab, orderList);
            storeOrder.Init(order);
        }
    }*/

    public void OnBuy()
    {
        if(buildingHandler.Buildable != null) return;

        float price = shelfOrder.Cost;

        if(!playerWallet.CanAfford(price))
        {
            Debug.Log($"[Building UI]: can't buy shelf");
            return;
        }

        GameObject go = Instantiate(shelfOrder.Prefab, spawnPosition);
        buildingHandler.DoInteract(go);

        playerWallet.Spend(price);
        balanceUI.UpdateBalanceUI();
        Close();
    }

    public void Close()
    {
        buildingUI.SetActive(false);
        balanceUI.gameObject.SetActive(true);

        Cursor.visible = false;

        Cursor.lockState = CursorLockMode.Locked;

        playerInputController.enabled = true;
        playerLook.enabled = true;
    }
}
