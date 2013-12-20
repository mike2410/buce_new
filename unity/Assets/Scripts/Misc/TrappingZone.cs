using UnityEngine;
using System;

/*
    TrappingZone-Class, checks wheter a missile got trapped, which is the case when missiles enters the GO associated with this class, and all segements are folded up.
*/


public class TrappingZone : MonoBehaviour {

    private GameManager _gameManager;
    private EventManager _eventManager;

	// Use this for initialization
	void Start () {
        _gameManager = GameManager.getInstance();
        _eventManager = EventManager.getInstance();
	}
	

    void OnTriggerStay(Collider c) {
        if (checkForTrapping()) {
            
            _eventManager.dispatchEvent(c.gameObject, EventArgs.Empty, EventManager.eventName.OnProjectileTrapped);
        }
}
    

    private bool checkForTrapping() {
        
        return (_gameManager.segmentsFolded == 4) ?  true :  false;
   
    }
}
