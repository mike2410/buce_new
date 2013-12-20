using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/*
    EventManager-Class, that is used by other classes via retrieving an instance as singleton.
    Defines all possible Events which other classes can dispatch or subscribe to.

*/
public delegate void GameEvent(GameObject g, EventArgs e);

public class EventManager
{

    private static EventManager Instance;


    public enum eventName
    {
        OnSegmentHover,
        OnFoldingStarted,
        OnFoldingDone,
        OnFolded,
        OnFoldedOut,
        OnFoldedInward,
        

        OnMissileSpawned,
        OnPowerupSpawned,

        OnProjectileCollision,
        OnMissileToBoundCollision,
        OnMissileToSegmentCollision,
        OnMissileToProjectileCollision,
        OnProjectileTrapped,

        OnProjectilePowerupPickedUp,
        OnProjectilePowerupToBoundCollision,
        OnProjectilePowerupToProjectileCollision,


        OnRestart,
        OnRestartLoss,
        OnRestartWin,
        OnPowerUpMultiplicatorActivated,
        OnPowerUpMultiplicatorDepleted,
        OnPowerUpSlowMissilesActivated,
        OnPowerUpSlowMissilesDeactivated,
        OnPowerUpTransmute,
        OnInventorySlotClicked,
        OnInventorySlotHover
    }

    private Dictionary<eventName, GameEvent> eventDictionary = new Dictionary<eventName, GameEvent>();




    private EventManager()
    {

        var tmpAllEvents = Enum.GetValues(typeof(eventName));

        foreach (eventName evt in tmpAllEvents)
        {
            eventDictionary.Add(evt, null);
        }

    }




    public static EventManager getInstance()
    {
        if (Instance == null)
        {
            Instance = new EventManager();
        }
        return Instance;
    }

    public void addListener(GameEvent gameEvt, eventName evtName)
    {
        eventDictionary[evtName] += gameEvt;
    }

    public void dispatchEvent(GameObject obj, EventArgs e, eventName evtName)
    {

        if (eventDictionary[evtName] != null)
        {
            eventDictionary[evtName].Invoke(obj, e);
        }
    }


}
