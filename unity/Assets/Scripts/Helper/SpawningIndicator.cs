using UnityEngine;
using System.Collections;
using System;

/*
SpawningIndicator-Class, plays an animation when a projectile spawns at a specific location
*/

public class SpawningIndicator : MonoBehaviour
{


    EventManager _eventManager;
    public int spawningPositionThisIndicatorSitsAt; //TODO: bessere lösung, evtl. im game manager als list?

    public AnimationClip blinkAnimation;

    public int amountOfBlinkLoops = 2;
    public bool shouldBlink = false;

    public int currentLoopPosition = 0;


    // Use this for initialization
    void Start()
    {

        _eventManager = EventManager.getInstance();

        _eventManager.addListener(SpawningIndicator_OnMissileSpawned, EventManager.eventName.OnMissileSpawned);
        _eventManager.addListener(SpawningIndicator_OnMissileSpawned, EventManager.eventName.OnPowerupSpawned);
        _eventManager.addListener(SpawningIndicator_OnRestart, EventManager.eventName.OnRestart);

    }

    private void SpawningIndicator_OnRestart(GameObject g, EventArgs e)
    {
        stopBlinking();
    }

    private void stopBlinking()
    {

        this.gameObject.animation.Rewind();
        shouldBlink = false;
        currentLoopPosition = 0;
    }

    private void SpawningIndicator_OnMissileSpawned(GameObject g, EventArgs e)
    {
        SpawningEventArgs se = (SpawningEventArgs)e;


        if (se.spawningPosition == spawningPositionThisIndicatorSitsAt && !shouldBlink)
        {
            shouldBlink = true;
            this.gameObject.animation["anim_indicator_blink"].speed = se.projectileSpeed * se.projectileSpeed/2;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (shouldBlink)
        {

            if (!gameObject.animation.isPlaying)
            {

                if (currentLoopPosition < amountOfBlinkLoops)
                {
                    //executed when blinking for first time
                    gameObject.animation.Play();
                    currentLoopPosition++;
                }
                else
                {
                    stopBlinking();
                }
            }




        }
 
    }
}
