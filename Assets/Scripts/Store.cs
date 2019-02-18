using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Purchasing;
using System.Linq;
using UnityEngine.UI;

[RequireComponent(typeof(GameController))]
public class Store : MonoBehaviour
{
    public GameObject ShowStoreButton;
    public GameObject StorePanel;
    public GameController GameController;
    public GameObject InfoWindow;
    public Text SalvageText;

    [HideInInspector]
    public bool IsOpen;

    
    // Use this for initialization
    void Start ()
    {
        IsOpen = false;
        HideInfoWindow();
    }
	
	// Update is called once per frame
	void Update () {
        SalvageText.text = GameController.GetPlayerSalvagePartsTotal().ToString();
	}

    public void ShowStore()
    {
        if (ShowStoreButton && StorePanel)
        {
            IsOpen = true;
            GameController.PauseGame(true);

            GameController.HideHeaderPanel();
            ShowStoreButton.SetActive(false);

            var canvasGroup = StorePanel.GetComponent<CanvasGroup>();
            canvasGroup.alpha = .8f;
            canvasGroup.interactable = true;
        }
    }

    public void HideStore()
    {
        if (ShowStoreButton && StorePanel)
        {
            IsOpen = false;
            GameController.ShowHeaderPanel();
            HideInfoWindow();
            //ShowStoreButton.SetActive(true);

            var canvasGroup = StorePanel.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;

            GameController.PauseGame(false);
        }
    }

    public void HideInfoWindow()
    {
        var canvasGroup = InfoWindow.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void ShowInfoWindow(string message)
    {
        var messageComponent = InfoWindow.GetComponentsInChildren<Text>().FirstOrDefault(t => t.name == "Contents");
        messageComponent.text = message;

        var canvasGroup = InfoWindow.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void HandlePurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        var message = string.Format("Captain, we were unable to purchase the {0} because {1}!",
            product.metadata.localizedTitle.ToUpper(), reason);

        ShowInfoWindow(message);
        Debug.Log(message);
    }

    public void HandlePurchaseSucceeded(Product product)
    {
        var message = string.Format("Captain, we have successfully purchased the {0}!",
            product.metadata.localizedTitle.ToUpper());

        ShowInfoWindow(message);
        Debug.Log(message);

        var partsPurchased = 0;
        switch(product.definition.id)
        {
            case "SalvageParts.100c":
                partsPurchased = 100;
                break;
            case "SalvageParts.550c":
                partsPurchased = 550;
                break;
            case "SalvageParts.1200c":
                partsPurchased = 1200;
                break;
            case "SalvageParts.2500c":
                partsPurchased = 2500;
                break;
            case "SalvageParts.6000c":
                partsPurchased = 6000;
                break;
            case "SalvageParts.15000c":
                partsPurchased = 15000;
                break;
        }
        
        GameController.AddScore(partsPurchased);
    }

    public void PurchaseShip(string shipName)
    {
        var message = "";

        //if the player already owns this ship, then select it as ship-in-use
        if (GameController.IsShipAlreadyOwned(shipName))
        {
            GameController.SetNewShip(shipName);

            message = string.Format("Captain, we have successfully transported to the {0}!", shipName.ToUpper());
            ShowInfoWindow(message);
            Debug.Log(message);

            return;
        }

        //Make sure player has enough salvage parts to afford ship
        var shipCost = GetShipCost(shipName);
        if (shipCost > GameController.GetPlayerSalvagePartsTotal())
        {
            //notify player that the ship is unaffordable
            message = string.Format("Captain, we require more salvage parts to upgrade to the {0}!", shipName.ToUpper());
            ShowInfoWindow(message);
            Debug.Log(message);

            return;
        }

        //TODO: Ensure player wants to purchase this ship
        //Debug.Log("Do you really want to buy this ship?");

        //Ship purchase successful so remove cost in salvage parts,
        // and set the new ship as the ship-in-use
        GameController.RemoveScore(shipCost);
        GameController.SetShipAsOwned(shipName);
        GameController.SetNewShip(shipName);
        //TODO: In the store, create a particle effect around the newly selected ship

        message = string.Format("Captain, we have completed the upgrade to the {0}!", shipName.ToUpper());
        ShowInfoWindow(message);
        Debug.Log(message);
    }

    public int GetShipCost(string shipName)
    {
        var ship = GameController.StoreShips
            .FirstOrDefault(s => s.name.ToLowerInvariant() == shipName.ToLowerInvariant());
        if (ship == null)
        {
            return 99999999;
        }

        var storeInfo = ship.GetComponentInChildren<ShipStoreInfo>();
        if (storeInfo == null)
        {
            return 99999999;
        }

        return storeInfo.Cost;
    }
}
