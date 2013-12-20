using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/*
    Inventory-Class, keeps track of which powerups are currently collected and could thusly be activated
*/

public class Inventory : MonoBehaviour {
    
    public static int inventorySize = 3;
    
    public List<GameObject> guiSlots = new List<GameObject>(inventorySize);
    public List<GameObject> freeGuiSlots = new List<GameObject>(inventorySize);

    public Dictionary<GameObject, PowerUp> slotPowerupDict = new Dictionary<GameObject,PowerUp>();
    
    private EventManager _eventManager;

	// Use this for initialization
	void Start () {
        _eventManager = EventManager.getInstance();

        setupInventorySlots();
        
        _eventManager.addListener(inventory_onInventorySlotClicked, EventManager.eventName.OnInventorySlotClicked);
        _eventManager.addListener(Inventory_OnProjectilePowerupToSegmentCollision, EventManager.eventName.OnProjectilePowerupPickedUp);
        _eventManager.addListener(Inventory_OnRestartLoss, EventManager.eventName.OnRestartLoss);
	}

    private void Inventory_OnRestartLoss(GameObject g, EventArgs e)
    {

        freeGuiSlots.Clear();

        foreach (GameObject guiSlot in guiSlots)
        {
            guiSlot.GetComponent<InventorySlot>().resetMaterial();
            freeGuiSlots.Add(guiSlot);
            guiSlot.GetComponent<InventorySlot>().isOccupied = false;
        }
        slotPowerupDict.Clear();

    }



    private void Inventory_OnProjectilePowerupToSegmentCollision(GameObject g, EventArgs e)
    { 
        addPowerUp(g.GetComponent<PowerUp>());
    }



    private void setupInventorySlots()
    {
        foreach (GameObject slot in guiSlots)
        {
            freeGuiSlots.Add(slot);
            slot.AddComponent<InventorySlot>();
            slot.AddComponent<BoxCollider>().isTrigger = true;
        }
    }
	
    private void inventory_onInventorySlotClicked(GameObject go, EventArgs ea)
    {
        activatePowerup(go);
        removePowerUp(go);
    }

    private void activatePowerup(GameObject go)
    {
        PowerUp foundPowerup;
        if (slotPowerupDict.TryGetValue(go, out foundPowerup) && freeGuiSlots.Count < inventorySize)
        {
            go.GetComponent<InventorySlot>().resetMaterial();
            foundPowerup.activate();
        }
    }

    public void addPowerUp(PowerUp powerUp)
    {

        foreach (GameObject slotGO in guiSlots)
        {
            
            InventorySlot inventorySlot = slotGO.GetComponent<InventorySlot>();

            if (!inventorySlot.isOccupied) {
                slotPowerupDict.Add(slotGO, powerUp);
                slotGO.GetComponent<InventorySlot>().setMaterial(powerUp.powerupGUIMaterial);
                freeGuiSlots.Remove(slotGO);
                inventorySlot.isOccupied = true;
                break;
            }
        }


    }

    public void removePowerUp(GameObject slot)
    {
        if (freeGuiSlots.Count < inventorySize)
        {
            InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();
            
            slotPowerupDict.Remove(slot);
            freeGuiSlots.Add(slot);
            inventorySlot.isOccupied = false;
        }
    }



}
