using UnityEngine;
using System;
using System.Collections;

public class ProjectileMissile : Projectile
{



    // Use this for initialization
    void Start()
    {
        base.Start();
        spinningVector = Vector3.forward;
    }

    protected override void handleCollisions(Collider collidedWith)
    {

        //check out with what kind of object the missile collided

        if (collidedWith.CompareTag("bound")) //if collided with bound remove missile from the game, since out of bounds
        {
            _eventManager.dispatchEvent(this.gameObject, EventArgs.Empty, EventManager.eventName.OnMissileToBoundCollision);
        }


        if (collidedWith.CompareTag("segment")) //if the missile collided with a (cube-) segment
        {
            _eventManager.dispatchEvent(this.gameObject, EventArgs.Empty, EventManager.eventName.OnMissileToSegmentCollision);

        }


        if (collidedWith.CompareTag("missile")) //remove the missile once it collided with another missile
        {
            _eventManager.dispatchEvent(this.gameObject, EventArgs.Empty, EventManager.eventName.OnMissileToProjectileCollision);
        }

    }




}
