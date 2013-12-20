using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;


/*
    Inventory-Slot-Class, the visual representation of the inventory. Cares about how feedback is communicated on interaction and fires events the Inventory-Class takes care of.
*/


public class InventorySlot : MonoBehaviour {



     private EventManager _eventManager;
     public Color overColor = new Color(36f / 255, 123f / 255, 235f / 255, 0.7f);
     public Color initialColor = new Color(0.157f, 0.157f, 0.157f, 1.0f);
     public Color clickColor = new Color(40f / 255,190f / 255, 254f / 255, 0.7f);

     public bool isOccupied = false;
     private bool _mouseOver = false;

     private Material initialMaterial;

     private Material hoverstate;
     private Material clickstate;

	// Use this for initialization
	void Start () {
        _eventManager = EventManager.getInstance();
        initialMaterial = this.GetComponentInChildren<Renderer>().material;
        checkIfColorIsSet();
	}

    public void setMaterial(Material m)
    {
        this.GetComponentInChildren<Renderer>().material = m;
    }

    public void resetMaterial()
    {
        this.GetComponentInChildren<Renderer>().material = initialMaterial;
    }


    private void checkIfColorIsSet()
    {
        if (initialColor != null)
        {
            this.GetComponentInChildren<Renderer>().material.color = initialColor;
        }
        else
        {
            initialColor = this.GetComponentInChildren<Renderer>().material.color;
        }
    }
	
    void OnMouseOver()
    {
        if (isOccupied)
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.GetComponentInChildren<Renderer>().material.color = clickColor;
                _eventManager.dispatchEvent(this.gameObject, EventArgs.Empty, EventManager.eventName.OnInventorySlotClicked);
                StartCoroutine(mouseClickEffect(0.15f));
            }

        }

    }

    void OnMouseEnter()
    {

        if (isOccupied) { 
        //change material
        this.GetComponentInChildren<Renderer>().material.color = overColor;
        _eventManager.dispatchEvent(this.gameObject, EventArgs.Empty, EventManager.eventName.OnInventorySlotHover);
        _mouseOver = true;
        }


    }



    void OnMouseExit()
    {
        this.GetComponentInChildren<Renderer>().material.color = initialColor;
        _mouseOver = false;
    }


    private IEnumerator mouseClickEffect(float waitForSeconds)
    {
        
        
        yield return new WaitForSeconds(waitForSeconds);
        if(_mouseOver) this.GetComponentInChildren<Renderer>().material.color = overColor;
    }



}
